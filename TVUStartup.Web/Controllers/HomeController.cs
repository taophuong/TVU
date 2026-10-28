using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TVUStartup.Web.Data;
using TVUStartup.Web.Models;
using TVUStartup.Web.Models.ViewModels;

namespace TVUStartup.Web.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;

    public HomeController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var published = _db.Projects.Published();

        var model = new HomeViewModel
        {
            ProjectCount = await published.CountAsync(),
            CategoryCount = await _db.Categories.CountAsync(c => c.IsActive),
            FacultyCount = await published.Select(p => p.FacultyId).Distinct().CountAsync(),
            TotalViews = await published.SumAsync(p => (int?)p.ViewCount) ?? 0,

            FeaturedProjects = await published
                .Where(p => p.IsFeatured)
                .OrderByDescending(p => p.ApprovedAt)
                .Take(3)
                .ToCards()
                .ToListAsync(),

            LatestProjects = await published
                .OrderByDescending(p => p.ApprovedAt)
                .Take(6)
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
                    ProjectCount = c.Projects.Count(p => p.Status == Models.Entities.ProjectStatus.DaDuyet)
                })
                .ToListAsync(),

            Banners = await _db.Banners
                .Where(b => b.IsActive)
                .OrderBy(b => b.DisplayOrder)
                .AsNoTracking()
                .ToListAsync(),

            LatestNews = await _db.News
                .Where(n => n.IsPublished)
                .OrderByDescending(n => n.CreatedAt)
                .Take(3)
                .AsNoTracking()
                .ToListAsync()
        };

        return View(model);
    }

    // Trang báo lỗi theo mã trạng thái HTTP (404, 403...).
    // Dùng [Route] (mọi phương thức) vì lỗi có thể phát sinh từ request POST và được thực thi lại tại đây.
    [Route("loi/{code:int}")]
    public IActionResult StatusCodePage(int code)
    {
        ViewData["Code"] = code;
        return View("StatusCode");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
