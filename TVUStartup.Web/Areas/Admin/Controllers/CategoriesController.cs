using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TVUStartup.Web.Areas.Admin.Models;
using TVUStartup.Web.Data;
using TVUStartup.Web.Helpers;
using TVUStartup.Web.Models.Entities;

namespace TVUStartup.Web.Areas.Admin.Controllers;

/// <summary>Quản lý lĩnh vực (CRUD)</summary>
public class CategoriesController : AdminBaseController
{
    private readonly AppDbContext _db;

    public CategoriesController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _db.Categories
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new { Category = c, ProjectCount = c.Projects.Count })
            .ToListAsync();
        ViewBag.ProjectCounts = items.ToDictionary(x => x.Category.CategoryId, x => x.ProjectCount);
        return View(items.Select(x => x.Category).ToList());
    }

    public IActionResult Create() => View("Form", new CategoryFormModel { DisplayOrder = 99 });

    public async Task<IActionResult> Edit(int id)
    {
        var c = await _db.Categories.FindAsync(id);
        if (c == null) return NotFound();
        return View("Form", new CategoryFormModel
        {
            CategoryId = c.CategoryId, CategoryName = c.CategoryName, Icon = c.Icon,
            Description = c.Description, DisplayOrder = c.DisplayOrder, IsActive = c.IsActive
        });
    }

    [HttpPost]
    public async Task<IActionResult> Save(CategoryFormModel form)
    {
        var slug = SlugHelper.ToSlug(form.CategoryName);
        if (await _db.Categories.AnyAsync(c => c.CategoryId != form.CategoryId && (c.CategoryName == form.CategoryName || c.Slug == slug)))
            ModelState.AddModelError(nameof(form.CategoryName), "Tên lĩnh vực đã tồn tại.");
        if (!ModelState.IsValid) return View("Form", form);

        var category = form.CategoryId == 0 ? new Category() : await _db.Categories.FindAsync(form.CategoryId);
        if (category == null) return NotFound();

        category.CategoryName = form.CategoryName.Trim();
        category.Slug = slug;
        category.Icon = form.Icon;
        category.Description = form.Description;
        category.DisplayOrder = form.DisplayOrder;
        category.IsActive = form.IsActive;
        if (category.CategoryId == 0) _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        Success($"Đã lưu lĩnh vực \"{category.CategoryName}\".");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category == null) return NotFound();

        // Ràng buộc: không xóa lĩnh vực đang có dự án (khóa ngoại ON DELETE NO ACTION)
        if (await _db.Projects.AnyAsync(p => p.CategoryId == id))
        {
            Error($"Không thể xóa \"{category.CategoryName}\" vì đang có dự án thuộc lĩnh vực này. Bạn có thể tắt \"Đang sử dụng\" thay vì xóa.");
            return RedirectToAction(nameof(Index));
        }

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();
        Success($"Đã xóa lĩnh vực \"{category.CategoryName}\".");
        return RedirectToAction(nameof(Index));
    }
}
