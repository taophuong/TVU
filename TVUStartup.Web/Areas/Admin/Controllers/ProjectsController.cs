using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TVUStartup.Web.Areas.Admin.Models;
using TVUStartup.Web.Data;
using TVUStartup.Web.Models.Entities;
using TVUStartup.Web.Services;

namespace TVUStartup.Web.Areas.Admin.Controllers;

/// <summary>Duyệt, từ chối, ẩn, đánh dấu nổi bật và xóa dự án</summary>
public class ProjectsController : AdminBaseController
{
    private readonly AppDbContext _db;
    private readonly IFileStorageService _files;

    public ProjectsController(AppDbContext db, IFileStorageService files)
    {
        _db = db;
        _files = files;
    }

    // GET /quan-tri/Projects?status=&q=
    public async Task<IActionResult> Index(ProjectStatus? status, string? q)
    {
        // Mặc định hiển thị các dự án đang chờ duyệt nếu có
        var counts = await _db.Projects.GroupBy(p => p.Status)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count);

        var query = _db.Projects.Include(p => p.Owner).Include(p => p.Category).Include(p => p.Faculty).AsQueryable();
        if (status.HasValue) query = query.Where(p => p.Status == status);
        if (!string.IsNullOrWhiteSpace(q)) query = query.Search(q);

        var model = new AdminProjectListViewModel
        {
            Status = status,
            Q = q,
            StatusCounts = counts,
            Items = await query
                .OrderBy(p => p.Status == ProjectStatus.ChoDuyet ? 0 : 1) // chờ duyệt lên đầu
                .ThenByDescending(p => p.UpdatedAt ?? p.CreatedAt)
                .AsNoTracking()
                .ToListAsync()
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Approve(int id, string? returnUrl)
    {
        var project = await _db.Projects.FindAsync(id);
        if (project == null) return NotFound();
        if (project.Status != ProjectStatus.ChoDuyet)
        {
            Error("Chỉ duyệt được dự án đang ở trạng thái Chờ duyệt.");
            return Back(returnUrl);
        }

        project.Status = ProjectStatus.DaDuyet;
        project.RejectReason = null;
        project.ApprovedAt = DateTime.Now;
        project.ApprovedBy = CurrentId;
        await _db.SaveChangesAsync();
        Success($"Đã duyệt dự án \"{project.ProjectName}\". Dự án đã hiển thị công khai.");
        return Back(returnUrl);
    }

    [HttpPost]
    public async Task<IActionResult> Reject(int id, string? reason, string? returnUrl)
    {
        var project = await _db.Projects.FindAsync(id);
        if (project == null) return NotFound();
        if (project.Status != ProjectStatus.ChoDuyet)
        {
            Error("Chỉ từ chối được dự án đang ở trạng thái Chờ duyệt.");
            return Back(returnUrl);
        }
        if (string.IsNullOrWhiteSpace(reason) || reason.Length > 500)
        {
            Error("Vui lòng nhập lý do từ chối (tối đa 500 ký tự) để sinh viên biết cần chỉnh sửa gì.");
            return Back(returnUrl);
        }

        project.Status = ProjectStatus.TuChoi;
        project.RejectReason = reason.Trim();
        await _db.SaveChangesAsync();
        Success($"Đã từ chối dự án \"{project.ProjectName}\".");
        return Back(returnUrl);
    }

    /// <summary>Ẩn dự án đang công khai (vi phạm, sai sự thật...) hoặc hiện lại dự án đã ẩn</summary>
    [HttpPost]
    public async Task<IActionResult> ToggleHidden(int id, string? returnUrl)
    {
        var project = await _db.Projects.FindAsync(id);
        if (project == null) return NotFound();

        if (project.Status == ProjectStatus.DaDuyet)
        {
            project.Status = ProjectStatus.An;
            project.IsFeatured = false;
            Success($"Đã ẩn dự án \"{project.ProjectName}\".");
        }
        else if (project.Status == ProjectStatus.An)
        {
            project.Status = ProjectStatus.DaDuyet;
            Success($"Đã hiển thị lại dự án \"{project.ProjectName}\".");
        }
        await _db.SaveChangesAsync();
        return Back(returnUrl);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleFeatured(int id, string? returnUrl)
    {
        var project = await _db.Projects.FindAsync(id);
        if (project == null) return NotFound();
        if (project.Status != ProjectStatus.DaDuyet)
        {
            Error("Chỉ dự án đã duyệt mới được đánh dấu nổi bật.");
            return Back(returnUrl);
        }

        project.IsFeatured = !project.IsFeatured;
        await _db.SaveChangesAsync();
        Success(project.IsFeatured ? $"Đã đưa \"{project.ProjectName}\" vào dự án nổi bật." : $"Đã bỏ nổi bật \"{project.ProjectName}\".");
        return Back(returnUrl);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id, string? returnUrl)
    {
        var project = await _db.Projects
            .Include(p => p.Images)
            .Include(p => p.Comments) // bình luận có quan hệ cha – con nên nạp để EF xóa đúng thứ tự
            .FirstOrDefaultAsync(p => p.ProjectId == id);
        if (project == null) return NotFound();

        _files.Delete(project.Logo);
        _files.Delete(project.CoverImage);
        foreach (var img in project.Images) _files.Delete(img.ImageUrl);

        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();
        Success($"Đã xóa dự án \"{project.ProjectName}\".");
        return Back(returnUrl);
    }

    private IActionResult Back(string? returnUrl) =>
        !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : RedirectToAction(nameof(Index));
}
