using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TVUStartup.Web.Data;
using TVUStartup.Web.Models.Entities;
using TVUStartup.Web.Models.ViewModels;

namespace TVUStartup.Web.Controllers;

/// <summary>Tin tức, sự kiện, cuộc thi khởi nghiệp</summary>
public class NewsController : Controller
{
    private const int PageSize = 6;
    private readonly AppDbContext _db;

    public NewsController(AppDbContext db)
    {
        _db = db;
    }

    // GET /tin-tuc?type=&page=
    [HttpGet("tin-tuc")]
    public async Task<IActionResult> Index(NewsType? type, int page = 1)
    {
        var query = _db.News.Where(n => n.IsPublished);
        if (type.HasValue) query = query.Where(n => n.NewsType == type);

        var total = await query.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        page = Math.Clamp(page, 1, totalPages);

        var model = new NewsListViewModel
        {
            Type = type,
            Page = page,
            TotalPages = totalPages,
            Items = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .AsNoTracking()
                .ToListAsync()
        };
        return View(model);
    }

    // GET /tin-tuc/{slug}
    [HttpGet("tin-tuc/{slug}")]
    public async Task<IActionResult> Detail(string slug)
    {
        var news = await _db.News.Include(n => n.Author).FirstOrDefaultAsync(n => n.Slug == slug && n.IsPublished);
        if (news == null) return NotFound();

        await _db.News.Where(n => n.NewsId == news.NewsId)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.ViewCount, n => n.ViewCount + 1));
        news.ViewCount++;

        ViewBag.Related = await _db.News
            .Where(n => n.IsPublished && n.NewsId != news.NewsId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(3)
            .AsNoTracking()
            .ToListAsync();
        return View(news);
    }
}
