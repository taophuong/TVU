using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TVUStartup.Web.Data;
using TVUStartup.Web.Helpers;
using TVUStartup.Web.Models.Entities;
using TVUStartup.Web.Models.ViewModels;

namespace TVUStartup.Web.Controllers;

/// <summary>Danh sách, chi tiết dự án và các tương tác: bình chọn, bình luận, liên hệ hợp tác</summary>
public class ProjectsController : Controller
{
    private readonly AppDbContext _db;

    public ProjectsController(AppDbContext db)
    {
        _db = db;
    }

    // GET /du-an?q=&category=&faculty=&stage=&sort=&page=
    [HttpGet("du-an")]
    public async Task<IActionResult> Index(string? q, string? category, int? faculty, ProjectStage? stage, string sort = "newest", int page = 1)
    {
        var query = _db.Projects.Published();

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Search(q);
        if (!string.IsNullOrEmpty(category))
            query = query.Where(p => p.Category.Slug == category);
        if (faculty.HasValue)
            query = query.Where(p => p.FacultyId == faculty);
        if (stage.HasValue)
            query = query.Where(p => p.Stage == stage);

        query = sort switch
        {
            "views" => query.OrderByDescending(p => p.ViewCount),
            "votes" => query.OrderByDescending(p => p.Votes.Count),
            "name" => query.OrderBy(p => p.ProjectName),
            _ => query.OrderByDescending(p => p.ApprovedAt)
        };

        var total = await query.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)ProjectListViewModel.PageSize));
        page = Math.Clamp(page, 1, totalPages);

        var model = new ProjectListViewModel
        {
            Q = q,
            Category = category,
            Faculty = faculty,
            Stage = stage,
            Sort = sort,
            Page = page,
            TotalCount = total,
            Projects = await query
                .Skip((page - 1) * ProjectListViewModel.PageSize)
                .Take(ProjectListViewModel.PageSize)
                .ToCards()
                .ToListAsync(),
            Categories = await _db.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .Select(c => new CategoryCountViewModel
                {
                    CategoryName = c.CategoryName,
                    Slug = c.Slug,
                    Icon = c.Icon,
                    ProjectCount = c.Projects.Count(p => p.Status == ProjectStatus.DaDuyet)
                })
                .ToListAsync(),
            Faculties = await _db.Faculties.OrderBy(f => f.DisplayOrder).ToListAsync()
        };

        return View(model);
    }

    // GET /du-an/{slug}
    [HttpGet("du-an/{slug}")]
    public async Task<IActionResult> Detail(string slug)
    {
        var project = await _db.Projects
            .Include(p => p.Category)
            .Include(p => p.Faculty)
            .Include(p => p.Owner)
            .Include(p => p.Features.OrderBy(f => f.DisplayOrder))
            .Include(p => p.Images.OrderBy(i => i.DisplayOrder))
            .Include(p => p.TeamMembers.OrderBy(m => m.DisplayOrder))
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.Slug == slug);
        if (project == null) return NotFound();

        var accountId = User.GetAccountId();
        var isOwner = accountId == project.OwnerId;
        var isAdmin = User.IsAdmin();
        var isPublished = project.Status == ProjectStatus.DaDuyet;

        // Dự án chưa duyệt / bị ẩn: chỉ chủ dự án và Admin được xem trước
        if (!isPublished && !isOwner && !isAdmin) return NotFound();

        if (isPublished)
        {
            // Tăng lượt xem trực tiếp trong CSDL (tránh ghi đè khi nhiều người xem cùng lúc)
            await _db.Projects
                .Where(p => p.ProjectId == project.ProjectId)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.ViewCount, p => p.ViewCount + 1));
            project.ViewCount++;
        }

        var versions = await _db.ProjectVersions
            .Where(v => v.ProjectId == project.ProjectId && (v.IsPublic || isOwner || isAdmin))
            .OrderByDescending(v => v.IsPublic)
            .ThenByDescending(v => v.ReleaseDate.HasValue)
            .ThenByDescending(v => v.ReleaseDate)
            .ThenBy(v => v.VersionId)
            .ToListAsync();

        // Bình luận gốc (kèm câu trả lời) chưa bị ẩn
        var comments = await _db.Comments
            .Where(c => c.ProjectId == project.ProjectId && c.ParentId == null && !c.IsHidden)
            .Include(c => c.Account)
            .Include(c => c.Replies.Where(r => !r.IsHidden).OrderBy(r => r.CreatedAt))
                .ThenInclude(r => r.Account)
            .OrderByDescending(c => c.CreatedAt)
            .AsSplitQuery()
            .ToListAsync();

        var ratings = comments.Where(c => c.Rating.HasValue).Select(c => (int)c.Rating!.Value).ToList();

        var model = new ProjectDetailViewModel
        {
            Project = project,
            Versions = versions,
            Comments = comments,
            VoteCount = await _db.Votes.CountAsync(v => v.ProjectId == project.ProjectId),
            AverageRating = ratings.Count > 0 ? ratings.Average() : null,
            RatingCount = ratings.Count,
            VideoEmbedUrl = DisplayHelper.ToYouTubeEmbedUrl(project.VideoUrl),
            IsPreview = !isPublished,
            IsOwner = isOwner,
            IsAdmin = isAdmin,
            CurrentAccountId = accountId,
            HasVoted = accountId.HasValue && await _db.Votes.AnyAsync(v => v.ProjectId == project.ProjectId && v.AccountId == accountId),
            HasRated = comments.Any(c => c.AccountId == accountId && c.Rating.HasValue),
            ContactForm = new ContactFormModel
            {
                FullName = User.Identity?.Name ?? "",
                Email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? ""
            },
            RelatedProjects = await _db.Projects
                .Published()
                .Where(p => p.CategoryId == project.CategoryId && p.ProjectId != project.ProjectId)
                .OrderByDescending(p => p.ViewCount)
                .Take(3)
                .ToCards()
                .ToListAsync()
        };

        return View(model);
    }

    // POST /du-an/{id}/binh-chon – bật / tắt yêu thích
    [Authorize]
    [HttpPost("du-an/{id:int}/binh-chon")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Vote(int id)
    {
        var accountId = User.GetAccountId()!.Value;
        var project = await _db.Projects.Published().FirstOrDefaultAsync(p => p.ProjectId == id);
        if (project == null) return NotFound();

        bool voted;
        if (project.OwnerId == accountId)
        {
            return IsAjax()
                ? BadRequest(new { message = "Bạn không thể bình chọn cho dự án của chính mình." })
                : RedirectWithError(project.Slug, "Bạn không thể bình chọn cho dự án của chính mình.");
        }

        var vote = await _db.Votes.FindAsync(id, accountId);
        if (vote == null)
        {
            _db.Votes.Add(new Vote { ProjectId = id, AccountId = accountId, CreatedAt = DateTime.Now });
            voted = true;
        }
        else
        {
            _db.Votes.Remove(vote);
            voted = false;
        }
        await _db.SaveChangesAsync();

        // Gọi bằng AJAX: trả về JSON để cập nhật nút mà không tải lại trang
        if (IsAjax())
        {
            var count = await _db.Votes.CountAsync(v => v.ProjectId == id);
            return Json(new { voted, count });
        }
        return RedirectToAction(nameof(Detail), new { slug = project.Slug });
    }

    // POST /du-an/{id}/binh-luan
    [Authorize]
    [HttpPost("du-an/{id:int}/binh-luan")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Comment(int id, CommentFormModel form)
    {
        var accountId = User.GetAccountId()!.Value;
        var project = await _db.Projects.Published().FirstOrDefaultAsync(p => p.ProjectId == id);
        if (project == null) return NotFound();

        if (!ModelState.IsValid)
            return RedirectWithError(project.Slug, FirstError(), "comments");

        // Chủ dự án không tự chấm sao; mỗi tài khoản chỉ chấm sao một lần cho mỗi dự án
        byte? rating = form.Rating;
        if (rating.HasValue)
        {
            var alreadyRated = await _db.Comments.AnyAsync(c => c.ProjectId == id && c.AccountId == accountId && c.Rating != null);
            if (project.OwnerId == accountId || alreadyRated) rating = null;
        }

        _db.Comments.Add(new Comment
        {
            ProjectId = id,
            AccountId = accountId,
            Content = form.Content.Trim(),
            Rating = rating,
            CreatedAt = DateTime.Now
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Đã gửi bình luận.";
        return Redirect(Url.Action(nameof(Detail), new { slug = project.Slug }) + "#comments");
    }

    // POST /du-an/binh-luan/{commentId}/tra-loi – chủ dự án hoặc Admin trả lời bình luận
    [Authorize]
    [HttpPost("du-an/binh-luan/{commentId:int}/tra-loi")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reply(int commentId, [Bind(Prefix = "reply")] CommentFormModel form)
    {
        var accountId = User.GetAccountId()!.Value;
        var parent = await _db.Comments.Include(c => c.Project).FirstOrDefaultAsync(c => c.CommentId == commentId && c.ParentId == null);
        if (parent == null) return NotFound();
        if (parent.Project.OwnerId != accountId && !User.IsAdmin()) return Forbid();

        if (!ModelState.IsValid)
            return RedirectWithError(parent.Project.Slug, FirstError(), "comments");

        _db.Comments.Add(new Comment
        {
            ProjectId = parent.ProjectId,
            AccountId = accountId,
            ParentId = parent.CommentId,
            Content = form.Content.Trim(),
            CreatedAt = DateTime.Now
        });
        await _db.SaveChangesAsync();
        return Redirect(Url.Action(nameof(Detail), new { slug = parent.Project.Slug }) + $"#comment-{commentId}");
    }

    // POST /du-an/binh-luan/{commentId}/xoa – người viết hoặc Admin xóa bình luận
    [Authorize]
    [HttpPost("du-an/binh-luan/{commentId:int}/xoa")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteComment(int commentId)
    {
        var comment = await _db.Comments
            .Include(c => c.Project)
            .Include(c => c.Replies) // nạp câu trả lời để EF xóa kèm (ClientCascade)
            .FirstOrDefaultAsync(c => c.CommentId == commentId);
        if (comment == null) return NotFound();
        if (comment.AccountId != User.GetAccountId() && !User.IsAdmin()) return Forbid();

        _db.Comments.Remove(comment);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Đã xóa bình luận.";
        return Redirect(Url.Action(nameof(Detail), new { slug = comment.Project.Slug }) + "#comments");
    }

    // POST /du-an/{id}/lien-he – khách gửi yêu cầu hợp tác / đầu tư / sử dụng sản phẩm
    [HttpPost("du-an/{id:int}/lien-he")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Contact(int id, [Bind(Prefix = "ContactForm")] ContactFormModel form)
    {
        var project = await _db.Projects.Published().FirstOrDefaultAsync(p => p.ProjectId == id);
        if (project == null) return NotFound();

        if (!ModelState.IsValid)
            return RedirectWithError(project.Slug, FirstError());

        _db.ContactRequests.Add(new ContactRequest
        {
            ProjectId = id,
            FullName = form.FullName.Trim(),
            Email = form.Email.Trim(),
            Phone = form.Phone,
            Organization = form.Organization,
            RequestType = form.RequestType,
            Message = form.Message.Trim(),
            Status = ContactStatus.Moi,
            CreatedAt = DateTime.Now
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Đã gửi yêu cầu liên hệ tới nhóm dự án {project.ProjectName}. Nhóm sẽ phản hồi qua email của bạn.";
        return RedirectToAction(nameof(Detail), new { slug = project.Slug });
    }

    // ===== Hàm hỗ trợ =====
    private bool IsAjax() => Request.Headers.XRequestedWith == "XMLHttpRequest";

    private string FirstError() =>
        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).FirstOrDefault() ?? "Dữ liệu không hợp lệ.";

    private IActionResult RedirectWithError(string slug, string message, string? anchor = null)
    {
        TempData["Error"] = message;
        var url = Url.Action(nameof(Detail), new { slug });
        return Redirect(anchor == null ? url! : $"{url}#{anchor}");
    }
}
