using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TVUStartup.Web.Data;
using TVUStartup.Web.Helpers;
using TVUStartup.Web.Models.Entities;
using TVUStartup.Web.Models.ViewModels;
using TVUStartup.Web.Services;

namespace TVUStartup.Web.Controllers;

/// <summary>Sinh viên quản lý dự án của mình: thêm, sửa, xóa, gửi duyệt, quản lý ảnh / tính năng / phiên bản / thành viên</summary>
[Authorize(Roles = Role.SinhVien)]
[Route("du-an-cua-toi")]
public class MyProjectsController : Controller
{
    private const int MaxImages = 10;

    private readonly AppDbContext _db;
    private readonly IFileStorageService _files;
    private readonly IHtmlSanitizer _sanitizer;

    public MyProjectsController(AppDbContext db, IFileStorageService files, IHtmlSanitizer sanitizer)
    {
        _db = db;
        _files = files;
        _sanitizer = sanitizer;
    }

    private int CurrentId => User.GetAccountId()!.Value;

    // GET /du-an-cua-toi
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var items = await _db.Projects
            .Where(p => p.OwnerId == CurrentId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new MyProjectItemViewModel
            {
                ProjectId = p.ProjectId,
                ProjectName = p.ProjectName,
                Slug = p.Slug,
                Logo = p.Logo,
                CategoryName = p.Category.CategoryName,
                Status = p.Status,
                RejectReason = p.RejectReason,
                ViewCount = p.ViewCount,
                VoteCount = p.Votes.Count,
                CommentCount = p.Comments.Count,
                NewContactCount = p.ContactRequests.Count(c => c.Status == ContactStatus.Moi),
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .ToListAsync();
        return View(items);
    }

    // ===== Thêm dự án =====
    [HttpGet("tao-moi")]
    public async Task<IActionResult> Create()
    {
        var me = await _db.Accounts.FindAsync(CurrentId);
        var form = new ProjectFormModel { FacultyId = me?.FacultyId };
        await FillOptionsAsync(form);
        return View(form);
    }

    [HttpPost("tao-moi")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProjectFormModel form)
    {
        await ValidateFormAsync(form);
        if (!ModelState.IsValid)
        {
            await FillOptionsAsync(form);
            return View(form);
        }

        var me = await _db.Accounts.FindAsync(CurrentId);
        var project = new Project
        {
            OwnerId = CurrentId,
            Slug = await UniqueSlugAsync(form.ProjectName),
            Status = ProjectStatus.Nhap,
            CreatedAt = DateTime.Now,
            // Người tạo tự động là thành viên đầu tiên của đội ngũ
            TeamMembers =
            {
                new TeamMember
                {
                    AccountId = CurrentId, FullName = me!.FullName, Position = "Trưởng nhóm",
                    StudentCode = me.StudentCode, ClassName = me.ClassName, Email = me.Email, DisplayOrder = 1
                }
            }
        };
        await ApplyFormAsync(project, form);
        _db.Projects.Add(project);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Đã tạo dự án ở trạng thái Nháp. Hãy bổ sung tính năng, hình ảnh rồi gửi duyệt.";
        return RedirectToAction(nameof(Edit), new { id = project.ProjectId, tab = "features" });
    }

    // ===== Sửa dự án =====
    [HttpGet("{id:int}/sua")]
    public async Task<IActionResult> Edit(int id, string tab = "info")
    {
        var project = await LoadOwnAsync(id, includeChildren: true);
        if (project == null) return NotFound();
        if (!ProjectRules.CanEdit(project.Status))
        {
            TempData["Error"] = $"Dự án đang ở trạng thái \"{project.Status.ToLabel()}\" nên không thể chỉnh sửa.";
            return RedirectToAction(nameof(Index));
        }

        var model = await BuildEditModelAsync(project, ToForm(project), tab);
        return View(model);
    }

    [HttpPost("{id:int}/sua")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind(Prefix = "Form")] ProjectFormModel form)
    {
        var project = await LoadOwnAsync(id, includeChildren: true);
        if (project == null) return NotFound();
        if (!ProjectRules.CanEdit(project.Status)) return RedirectToAction(nameof(Index));

        await ValidateFormAsync(form);
        if (!ModelState.IsValid)
        {
            form.Logo = project.Logo;
            form.CoverImage = project.CoverImage;
            return View(await BuildEditModelAsync(project, form, "info"));
        }

        await ApplyFormAsync(project, form);
        project.UpdatedAt = DateTime.Now;
        await _db.SaveChangesAsync();

        TempData["Success"] = "Đã lưu thông tin dự án.";
        return RedirectToAction(nameof(Edit), new { id });
    }

    // ===== Xóa / gửi duyệt / rút lại =====
    [HttpPost("{id:int}/xoa")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await LoadOwnAsync(id, includeChildren: true);
        if (project == null) return NotFound();
        if (!ProjectRules.CanDelete(project.Status))
        {
            TempData["Error"] = "Chỉ được xóa dự án ở trạng thái Nháp hoặc Bị từ chối.";
            return RedirectToAction(nameof(Index));
        }

        // Xóa tệp ảnh đã tải lên trước khi xóa dữ liệu (ảnh, tính năng... xóa theo cascade)
        _files.Delete(project.Logo);
        _files.Delete(project.CoverImage);
        foreach (var img in project.Images) _files.Delete(img.ImageUrl);

        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();
        TempData["Success"] = $"Đã xóa dự án \"{project.ProjectName}\".";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("{id:int}/gui-duyet")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(int id)
    {
        var project = await LoadOwnAsync(id, includeChildren: true);
        if (project == null) return NotFound();
        if (!ProjectRules.CanSubmit(project.Status)) return RedirectToAction(nameof(Index));

        // Điều kiện tối thiểu trước khi gửi duyệt
        var missing = new List<string>();
        if (string.IsNullOrEmpty(project.CoverImage)) missing.Add("ảnh bìa");
        if (project.Features.Count == 0) missing.Add("ít nhất 1 tính năng nổi bật");
        if (missing.Count > 0)
        {
            TempData["Error"] = "Chưa thể gửi duyệt, dự án còn thiếu: " + string.Join(", ", missing) + ".";
            return RedirectToAction(nameof(Edit), new { id });
        }

        project.Status = ProjectStatus.ChoDuyet;
        project.RejectReason = null;
        project.UpdatedAt = DateTime.Now;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Đã gửi dự án tới Ban quản trị. Bạn sẽ thấy kết quả duyệt tại trang này.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("{id:int}/rut-lai")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Withdraw(int id)
    {
        var project = await LoadOwnAsync(id);
        if (project == null) return NotFound();
        if (ProjectRules.CanWithdraw(project.Status))
        {
            project.Status = ProjectStatus.Nhap;
            project.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Đã rút lại yêu cầu duyệt, dự án trở về trạng thái Nháp.";
        }
        return RedirectToAction(nameof(Index));
    }

    // ===== Hình ảnh =====
    [HttpPost("{id:int}/hinh-anh")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddImages(int id, List<IFormFile> files, string? caption)
    {
        var project = await LoadOwnAsync(id, includeChildren: true);
        if (project == null) return NotFound();
        if (!ProjectRules.CanEdit(project.Status)) return RedirectToAction(nameof(Index));

        if (files.Count == 0)
            TempData["Error"] = "Vui lòng chọn ít nhất một ảnh.";
        else if (project.Images.Count + files.Count > MaxImages)
            TempData["Error"] = $"Mỗi dự án tối đa {MaxImages} ảnh.";
        else
        {
            var errors = files.Select(f => _files.Validate(f)).Where(e => e != null).ToList();
            if (errors.Count > 0)
                TempData["Error"] = errors[0];
            else
            {
                var order = project.Images.Select(i => i.DisplayOrder).DefaultIfEmpty(0).Max();
                foreach (var file in files)
                {
                    project.Images.Add(new ProjectImage
                    {
                        ImageUrl = await _files.SaveImageAsync(file, $"projects/{project.ProjectId}"),
                        Caption = string.IsNullOrWhiteSpace(caption) ? null : caption.Trim(),
                        DisplayOrder = ++order
                    });
                }
                await TouchAsync(project);
                TempData["Success"] = $"Đã thêm {files.Count} ảnh.";
            }
        }
        return RedirectToAction(nameof(Edit), new { id, tab = "images" });
    }

    [HttpPost("hinh-anh/{imageId:int}/xoa")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteImage(int imageId)
    {
        var image = await _db.ProjectImages.Include(i => i.Project)
            .FirstOrDefaultAsync(i => i.ImageId == imageId && i.Project.OwnerId == CurrentId);
        if (image == null) return NotFound();
        if (!ProjectRules.CanEdit(image.Project.Status)) return RedirectToAction(nameof(Index));

        _files.Delete(image.ImageUrl);
        _db.ProjectImages.Remove(image);
        await TouchAsync(image.Project);
        return RedirectToAction(nameof(Edit), new { id = image.ProjectId, tab = "images" });
    }

    // ===== Tính năng =====
    [HttpPost("{id:int}/tinh-nang")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddFeature(int id, [Bind(Prefix = "NewFeature")] FeatureFormModel form)
    {
        var project = await LoadOwnAsync(id, includeChildren: true);
        if (project == null) return NotFound();
        if (!ProjectRules.CanEdit(project.Status)) return RedirectToAction(nameof(Index));

        if (!ModelState.IsValid)
            TempData["Error"] = FirstError();
        else
        {
            var icon = ProjectRules.FeatureIcons.Any(i => i.Icon == form.Icon) ? form.Icon : "bi-check2-circle";
            project.Features.Add(new ProjectFeature
            {
                Title = form.Title.Trim(),
                Description = form.Description?.Trim(),
                Icon = icon,
                DisplayOrder = project.Features.Select(f => f.DisplayOrder).DefaultIfEmpty(0).Max() + 1
            });
            await TouchAsync(project);
        }
        return RedirectToAction(nameof(Edit), new { id, tab = "features" });
    }

    [HttpPost("tinh-nang/{featureId:int}/xoa")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteFeature(int featureId)
    {
        var feature = await _db.ProjectFeatures.Include(f => f.Project)
            .FirstOrDefaultAsync(f => f.FeatureId == featureId && f.Project.OwnerId == CurrentId);
        if (feature == null) return NotFound();
        if (!ProjectRules.CanEdit(feature.Project.Status)) return RedirectToAction(nameof(Index));

        _db.ProjectFeatures.Remove(feature);
        await TouchAsync(feature.Project);
        return RedirectToAction(nameof(Edit), new { id = feature.ProjectId, tab = "features" });
    }

    // ===== Phiên bản =====
    [HttpPost("{id:int}/phien-ban")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddVersion(int id, [Bind(Prefix = "NewVersion")] VersionFormModel form)
    {
        var project = await LoadOwnAsync(id, includeChildren: true);
        if (project == null) return NotFound();
        if (!ProjectRules.CanEdit(project.Status)) return RedirectToAction(nameof(Index));

        if (!ModelState.IsValid)
            TempData["Error"] = FirstError();
        else
        {
            project.Versions.Add(new ProjectVersion
            {
                VersionName = form.VersionName.Trim(),
                Platform = form.Platform?.Trim(),
                ReleaseDate = form.ReleaseDate,
                DownloadUrl = form.DownloadUrl,
                ChangeLog = form.ChangeLog?.Trim(),
                IsPublic = form.IsPublic
            });
            await TouchAsync(project);
        }
        return RedirectToAction(nameof(Edit), new { id, tab = "versions" });
    }

    [HttpPost("phien-ban/{versionId:int}/xoa")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteVersion(int versionId)
    {
        var version = await _db.ProjectVersions.Include(v => v.Project)
            .FirstOrDefaultAsync(v => v.VersionId == versionId && v.Project.OwnerId == CurrentId);
        if (version == null) return NotFound();
        if (!ProjectRules.CanEdit(version.Project.Status)) return RedirectToAction(nameof(Index));

        _db.ProjectVersions.Remove(version);
        await TouchAsync(version.Project);
        return RedirectToAction(nameof(Edit), new { id = version.ProjectId, tab = "versions" });
    }

    // ===== Thành viên =====
    [HttpPost("{id:int}/thanh-vien")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddMember(int id, [Bind(Prefix = "NewMember")] MemberFormModel form)
    {
        var project = await LoadOwnAsync(id, includeChildren: true);
        if (project == null) return NotFound();
        if (!ProjectRules.CanEdit(project.Status)) return RedirectToAction(nameof(Index));

        if (!ModelState.IsValid)
            TempData["Error"] = FirstError();
        else
        {
            // Nếu email trùng tài khoản trên hệ thống thì liên kết thành viên với tài khoản đó
            var email = form.Email?.Trim().ToLowerInvariant();
            var linked = email == null ? null : await _db.Accounts.FirstOrDefaultAsync(a => a.Email == email);
            project.TeamMembers.Add(new TeamMember
            {
                AccountId = linked?.AccountId,
                FullName = form.FullName.Trim(),
                Position = form.Position?.Trim(),
                StudentCode = form.StudentCode?.Trim(),
                ClassName = form.ClassName?.Trim(),
                Email = email,
                Avatar = linked?.Avatar,
                DisplayOrder = project.TeamMembers.Select(m => m.DisplayOrder).DefaultIfEmpty(0).Max() + 1
            });
            await TouchAsync(project);
        }
        return RedirectToAction(nameof(Edit), new { id, tab = "members" });
    }

    [HttpPost("thanh-vien/{memberId:int}/xoa")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMember(int memberId)
    {
        var member = await _db.TeamMembers.Include(m => m.Project)
            .FirstOrDefaultAsync(m => m.MemberId == memberId && m.Project.OwnerId == CurrentId);
        if (member == null) return NotFound();
        if (!ProjectRules.CanEdit(member.Project.Status)) return RedirectToAction(nameof(Index));

        _db.TeamMembers.Remove(member);
        await TouchAsync(member.Project);
        return RedirectToAction(nameof(Edit), new { id = member.ProjectId, tab = "members" });
    }

    // ===== Liên hệ hợp tác gửi tới dự án của tôi =====
    [HttpGet("lien-he")]
    public async Task<IActionResult> Contacts()
    {
        var items = await _db.ContactRequests
            .Where(c => c.Project != null && c.Project.OwnerId == CurrentId)
            .OrderBy(c => c.Status)
            .ThenByDescending(c => c.CreatedAt)
            .Select(c => new MyContactViewModel { Request = c, ProjectName = c.Project!.ProjectName, ProjectSlug = c.Project.Slug })
            .ToListAsync();
        return View(items);
    }

    [HttpPost("lien-he/{contactId:int}/trang-thai")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateContactStatus(int contactId, ContactStatus status)
    {
        var contact = await _db.ContactRequests
            .FirstOrDefaultAsync(c => c.ContactId == contactId && c.Project != null && c.Project.OwnerId == CurrentId);
        if (contact == null) return NotFound();
        if (!Enum.IsDefined(status)) return BadRequest();

        contact.Status = status;
        contact.HandledBy = CurrentId;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Đã cập nhật trạng thái liên hệ.";
        return RedirectToAction(nameof(Contacts));
    }

    // ===== Hàm hỗ trợ =====

    /// <summary>Lấy dự án theo id, chỉ khi thuộc về sinh viên đang đăng nhập (chống sửa dự án của người khác)</summary>
    private async Task<Project?> LoadOwnAsync(int id, bool includeChildren = false)
    {
        var query = _db.Projects.Where(p => p.ProjectId == id && p.OwnerId == CurrentId);
        if (includeChildren)
        {
            query = query
                .Include(p => p.Images.OrderBy(i => i.DisplayOrder))
                .Include(p => p.Features.OrderBy(f => f.DisplayOrder))
                .Include(p => p.Versions)
                .Include(p => p.TeamMembers.OrderBy(m => m.DisplayOrder))
                .AsSplitQuery();
        }
        return await query.FirstOrDefaultAsync();
    }

    private async Task TouchAsync(Project project)
    {
        project.UpdatedAt = DateTime.Now;
        await _db.SaveChangesAsync();
    }

    /// <summary>Kiểm tra thêm các ràng buộc không khai báo được bằng DataAnnotations</summary>
    private async Task ValidateFormAsync(ProjectFormModel form)
    {
        if (form.CategoryId.HasValue && !await _db.Categories.AnyAsync(c => c.CategoryId == form.CategoryId && c.IsActive))
            ModelState.AddModelError(nameof(form.CategoryId), "Lĩnh vực không hợp lệ.");
        if (form.FacultyId.HasValue && !await _db.Faculties.AnyAsync(f => f.FacultyId == form.FacultyId))
            ModelState.AddModelError(nameof(form.FacultyId), "Khoa không hợp lệ.");
        if (!Enum.IsDefined(form.Stage))
            ModelState.AddModelError(nameof(form.Stage), "Giai đoạn không hợp lệ.");
        if (form.LogoFile != null && _files.Validate(form.LogoFile) is { } logoError)
            ModelState.AddModelError(nameof(form.LogoFile), logoError);
        if (form.CoverFile != null && _files.Validate(form.CoverFile) is { } coverError)
            ModelState.AddModelError(nameof(form.CoverFile), coverError);
    }

    /// <summary>Chép dữ liệu từ form vào entity; lọc HTML phần giới thiệu và lưu ảnh mới (nếu có)</summary>
    private async Task ApplyFormAsync(Project project, ProjectFormModel form)
    {
        project.ProjectName = form.ProjectName.Trim();
        project.Slogan = form.Slogan?.Trim();
        project.ShortDescription = form.ShortDescription.Trim();
        project.Content = string.IsNullOrWhiteSpace(form.Content) ? null : _sanitizer.Sanitize(form.Content);
        project.CategoryId = form.CategoryId!.Value;
        project.FacultyId = form.FacultyId!.Value;
        project.Stage = form.Stage;
        project.FoundedDate = form.FoundedDate;
        project.WebsiteUrl = form.WebsiteUrl;
        project.CommunityUrl = form.CommunityUrl;
        project.VideoUrl = form.VideoUrl;

        // Ảnh lưu theo thư mục riêng của người dùng; dự án mới chưa có Id nên dùng thư mục "projects/new"
        var folder = project.ProjectId > 0 ? $"projects/{project.ProjectId}" : "projects/new";
        if (form.LogoFile != null)
        {
            _files.Delete(project.Logo);
            project.Logo = await _files.SaveImageAsync(form.LogoFile, folder);
        }
        if (form.CoverFile != null)
        {
            _files.Delete(project.CoverImage);
            project.CoverImage = await _files.SaveImageAsync(form.CoverFile, folder);
        }
    }

    private static ProjectFormModel ToForm(Project p) => new()
    {
        ProjectId = p.ProjectId,
        ProjectName = p.ProjectName,
        Slogan = p.Slogan,
        ShortDescription = p.ShortDescription,
        Content = p.Content,
        CategoryId = p.CategoryId,
        FacultyId = p.FacultyId,
        Stage = p.Stage,
        FoundedDate = p.FoundedDate,
        WebsiteUrl = p.WebsiteUrl,
        CommunityUrl = p.CommunityUrl,
        VideoUrl = p.VideoUrl,
        Logo = p.Logo,
        CoverImage = p.CoverImage
    };

    private async Task<ProjectEditViewModel> BuildEditModelAsync(Project project, ProjectFormModel form, string tab)
    {
        form.ProjectId = project.ProjectId;
        await FillOptionsAsync(form);
        return new ProjectEditViewModel
        {
            Form = form,
            Slug = project.Slug,
            Status = project.Status,
            RejectReason = project.RejectReason,
            Tab = tab,
            Images = project.Images.ToList(),
            Features = project.Features.ToList(),
            Versions = project.Versions.OrderByDescending(v => v.ReleaseDate).ToList(),
            Members = project.TeamMembers.ToList()
        };
    }

    private async Task FillOptionsAsync(ProjectFormModel form)
    {
        form.Categories = await _db.CategoryOptionsAsync();
        form.Faculties = await _db.FacultyOptionsAsync();
    }

    /// <summary>Tạo slug không trùng: "nong-san-sach", "nong-san-sach-2", ...</summary>
    private async Task<string> UniqueSlugAsync(string name)
    {
        var baseSlug = SlugHelper.ToSlug(name);
        if (string.IsNullOrEmpty(baseSlug)) baseSlug = "du-an";
        var slug = baseSlug;
        for (var i = 2; await _db.Projects.AnyAsync(p => p.Slug == slug); i++)
            slug = $"{baseSlug}-{i}";
        return slug;
    }

    private string FirstError() =>
        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).FirstOrDefault() ?? "Dữ liệu không hợp lệ.";
}
