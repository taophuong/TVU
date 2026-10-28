using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TVUStartup.Web.Areas.Admin.Models;
using TVUStartup.Web.Data;
using TVUStartup.Web.Models.Entities;
using TVUStartup.Web.Services;

namespace TVUStartup.Web.Areas.Admin.Controllers;

/// <summary>Quản lý banner trình chiếu trên trang chủ (CRUD)</summary>
public class BannersController : AdminBaseController
{
    private readonly AppDbContext _db;
    private readonly IFileStorageService _files;

    public BannersController(AppDbContext db, IFileStorageService files)
    {
        _db = db;
        _files = files;
    }

    public async Task<IActionResult> Index() =>
        View(await _db.Banners.OrderBy(b => b.DisplayOrder).AsNoTracking().ToListAsync());

    public IActionResult Create() => View("Form", new BannerFormModel());

    public async Task<IActionResult> Edit(int id)
    {
        var b = await _db.Banners.FindAsync(id);
        if (b == null) return NotFound();
        return View("Form", new BannerFormModel
        {
            BannerId = b.BannerId, Title = b.Title, LinkUrl = b.LinkUrl,
            DisplayOrder = b.DisplayOrder, IsActive = b.IsActive, ImageUrl = b.ImageUrl
        });
    }

    [HttpPost]
    public async Task<IActionResult> Save(BannerFormModel form)
    {
        if (form.ImageFile != null && _files.Validate(form.ImageFile) is { } error)
            ModelState.AddModelError(nameof(form.ImageFile), error);
        if (form.BannerId == 0 && form.ImageFile == null)
            ModelState.AddModelError(nameof(form.ImageFile), "Vui lòng chọn ảnh banner.");
        if (!string.IsNullOrEmpty(form.LinkUrl) && !IsSafeLink(form.LinkUrl))
            ModelState.AddModelError(nameof(form.LinkUrl), "Đường dẫn phải bắt đầu bằng / hoặc http(s)://");
        if (!ModelState.IsValid) return View("Form", form);

        var banner = form.BannerId == 0 ? new Banner() : await _db.Banners.FindAsync(form.BannerId);
        if (banner == null) return NotFound();

        banner.Title = form.Title;
        banner.LinkUrl = form.LinkUrl;
        banner.DisplayOrder = form.DisplayOrder;
        banner.IsActive = form.IsActive;
        if (form.ImageFile != null)
        {
            _files.Delete(banner.ImageUrl);
            banner.ImageUrl = await _files.SaveImageAsync(form.ImageFile, "banners");
        }
        if (banner.BannerId == 0) _db.Banners.Add(banner);
        await _db.SaveChangesAsync();

        Success("Đã lưu banner.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var banner = await _db.Banners.FindAsync(id);
        if (banner == null) return NotFound();

        _files.Delete(banner.ImageUrl);
        _db.Banners.Remove(banner);
        await _db.SaveChangesAsync();
        Success("Đã xóa banner.");
        return RedirectToAction(nameof(Index));
    }

    /// <summary>Chỉ cho phép đường dẫn nội bộ ("/...") hoặc http(s) – chặn "javascript:" và "//trang-khac"</summary>
    private static bool IsSafeLink(string url)
    {
        if (url.StartsWith('/')) return !url.StartsWith("//");
        return Uri.TryCreate(url, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}
