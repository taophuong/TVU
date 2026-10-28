using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TVUStartup.Web.Areas.Admin.Models;
using TVUStartup.Web.Data;
using TVUStartup.Web.Helpers;
using TVUStartup.Web.Models.Entities;

namespace TVUStartup.Web.Areas.Admin.Controllers;

/// <summary>Trang tổng quan: thống kê, biểu đồ, việc cần xử lý</summary>
public class DashboardController : AdminBaseController
{
    private readonly AppDbContext _db;

    public DashboardController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var approved = _db.Projects.Where(p => p.Status == ProjectStatus.DaDuyet);

        // Thống kê theo trạng thái / lĩnh vực / khoa: GROUP BY thực hiện trong SQL Server
        var byStatus = await _db.Projects
            .GroupBy(p => p.Status)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync();
        var byCategory = await approved
            .GroupBy(p => p.Category.CategoryName)
            .Select(g => new { g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync();
        var byFaculty = await approved
            .GroupBy(p => p.Faculty.FacultyName)
            .Select(g => new { g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        // Số dự án được tạo trong 6 tháng gần nhất
        var from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-5);
        var byMonthRaw = await _db.Projects
            .Where(p => p.CreatedAt >= from)
            .GroupBy(p => new { p.CreatedAt.Year, p.CreatedAt.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
            .ToListAsync();
        var byMonth = Enumerable.Range(0, 6)
            .Select(i => from.AddMonths(i))
            .Select(m => ($"{m.Month:00}/{m.Year}", byMonthRaw.FirstOrDefault(x => x.Year == m.Year && x.Month == m.Month)?.Count ?? 0))
            .ToList();

        var model = new DashboardViewModel
        {
            ApprovedCount = await approved.CountAsync(),
            PendingCount = await _db.Projects.CountAsync(p => p.Status == ProjectStatus.ChoDuyet),
            StudentCount = await _db.Accounts.CountAsync(a => a.RoleId == Role.SinhVienId),
            NewContactCount = await _db.ContactRequests.CountAsync(c => c.Status == ContactStatus.Moi),
            TotalViews = await approved.SumAsync(p => (int?)p.ViewCount) ?? 0,
            TotalVotes = await _db.Votes.CountAsync(),
            TotalComments = await _db.Comments.CountAsync(),
            ByStatus = Enum.GetValues<ProjectStatus>()
                .Select(s => (s.ToLabel(), byStatus.FirstOrDefault(x => x.Key == s)?.Count ?? 0))
                .ToList(),
            ByCategory = byCategory.Select(x => (x.Key, x.Count)).ToList(),
            ByFaculty = byFaculty.Select(x => (x.Key, x.Count)).ToList(),
            ByMonth = byMonth,
            PendingProjects = await _db.Projects
                .Where(p => p.Status == ProjectStatus.ChoDuyet)
                .Include(p => p.Owner)
                .Include(p => p.Category)
                .OrderBy(p => p.UpdatedAt ?? p.CreatedAt)
                .Take(5)
                .AsNoTracking()
                .ToListAsync(),
            TopProjects = await approved
                .OrderByDescending(p => p.ViewCount)
                .Take(5)
                .AsNoTracking()
                .ToListAsync(),
            NewContacts = await _db.ContactRequests
                .Where(c => c.Status == ContactStatus.Moi)
                .Include(c => c.Project)
                .OrderByDescending(c => c.CreatedAt)
                .Take(5)
                .AsNoTracking()
                .ToListAsync()
        };
        return View(model);
    }
}
