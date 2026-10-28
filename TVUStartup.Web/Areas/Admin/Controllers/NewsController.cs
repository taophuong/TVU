using Ganss.Xss;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TVUStartup.Web.Areas.Admin.Models;
using TVUStartup.Web.Data;
using TVUStartup.Web.Helpers;
using TVUStartup.Web.Models.Entities;
using TVUStartup.Web.Services;

namespace TVUStartup.Web.Areas.Admin.Controllers;

/// <summary>Quản lý tin tức, sự kiện, cuộc thi (CRUD)</summary>
public class NewsController : AdminBaseController
{
    private readonly AppDbContext _db;
    private readonly IFileStorageService _files;
    private readonly IHtmlSanitizer _sanitizer;

    public NewsController(AppDbContext db, IFileStorageService files, IHtmlSanitizer sanitizer)
    {
        _db = db;
        _files = files;
        _sanitizer = sanitizer;
    }

    public async Task<IActionResult> Index() =>
        View(await _db.News.Include(n => n.Author).OrderByDescending(n => n.CreatedAt).AsNoTracking().ToListAsync());

    public IActionResult Create() => View("Form", new NewsFormModel { IsPublished = true });

    public async Task<IActionResult> Edit(int id)
    {
        var n = await _db.News.FindAsync(id);
        if (n == null) return NotFound();
        return View("Form", new NewsFormModel
        {
            NewsId = n.NewsId, Title = n.Title, Summary = n.Summary, Content = n.Content, NewsType = n.NewsType,
            EventDate = n.EventDate, Location = n.Location, IsPublished = n.IsPublished, Thumbnail = n.Thumbnail
        });
    }

    [HttpPost]
    public async Task<IActionResult> Save(NewsFormModel form)
    {
        if (form.ThumbnailFile != null && _files.Validate(form.ThumbnailFile) is { } error)
            ModelState.AddModelError(nameof(form.ThumbnailFile), error);
        if (!ModelState.IsValid) return View("Form", form);

        var news = form.NewsId == 0 ? new News { AuthorId = CurrentId, CreatedAt = DateTime.Now } : await _db.News.FindAsync(form.NewsId);
        if (news == null) return NotFound();

        if (news.NewsId == 0)
        {
            // Slug không trùng
            var baseSlug = SlugHelper.ToSlug(form.Title);
            var slug = baseSlug;
            for (var i = 2; await _db.News.AnyAsync(n => n.Slug == slug); i++) slug = $"{baseSlug}-{i}";
            news.Slug = slug;
        }
        news.Title = form.Title.Trim();
        news.Summary = form.Summary?.Trim();
        news.Content = _sanitizer.Sanitize(form.Content);
        news.NewsType = form.NewsType;
        news.EventDate = form.EventDate;
        news.Location = form.Location;
        news.IsPublished = form.IsPublished;
        if (form.ThumbnailFile != null)
        {
            _files.Delete(news.Thumbnail);
            news.Thumbnail = await _files.SaveImageAsync(form.ThumbnailFile, "news");
        }
        if (news.NewsId == 0) _db.News.Add(news);
        else news.UpdatedAt = DateTime.Now;
        await _db.SaveChangesAsync();

        Success($"Đã lưu bài viết \"{news.Title}\".");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var news = await _db.News.FindAsync(id);
        if (news == null) return NotFound();

        _files.Delete(news.Thumbnail);
        _db.News.Remove(news);
        await _db.SaveChangesAsync();
        Success($"Đã xóa bài viết \"{news.Title}\".");
        return RedirectToAction(nameof(Index));
    }
}
