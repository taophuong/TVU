using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TVUStartup.Web.Areas.Admin.Models;
using TVUStartup.Web.Data;
using TVUStartup.Web.Models.Entities;

namespace TVUStartup.Web.Areas.Admin.Controllers;

/// <summary>Quản lý khoa (CRUD)</summary>
public class FacultiesController : AdminBaseController
{
    private readonly AppDbContext _db;

    public FacultiesController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _db.Faculties
            .OrderBy(f => f.DisplayOrder)
            .Select(f => new { Faculty = f, Projects = f.Projects.Count, Accounts = f.Accounts.Count })
            .ToListAsync();
        ViewBag.Counts = items.ToDictionary(x => x.Faculty.FacultyId, x => (x.Projects, x.Accounts));
        return View(items.Select(x => x.Faculty).ToList());
    }

    public IActionResult Create() => View("Form", new FacultyFormModel { DisplayOrder = 99 });

    public async Task<IActionResult> Edit(int id)
    {
        var f = await _db.Faculties.FindAsync(id);
        if (f == null) return NotFound();
        return View("Form", new FacultyFormModel
        {
            FacultyId = f.FacultyId, FacultyName = f.FacultyName, Description = f.Description, DisplayOrder = f.DisplayOrder
        });
    }

    [HttpPost]
    public async Task<IActionResult> Save(FacultyFormModel form)
    {
        if (await _db.Faculties.AnyAsync(f => f.FacultyId != form.FacultyId && f.FacultyName == form.FacultyName))
            ModelState.AddModelError(nameof(form.FacultyName), "Tên khoa đã tồn tại.");
        if (!ModelState.IsValid) return View("Form", form);

        var faculty = form.FacultyId == 0 ? new Faculty() : await _db.Faculties.FindAsync(form.FacultyId);
        if (faculty == null) return NotFound();

        faculty.FacultyName = form.FacultyName.Trim();
        faculty.Description = form.Description;
        faculty.DisplayOrder = form.DisplayOrder;
        if (faculty.FacultyId == 0) _db.Faculties.Add(faculty);
        await _db.SaveChangesAsync();

        Success($"Đã lưu \"{faculty.FacultyName}\".");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var faculty = await _db.Faculties.FindAsync(id);
        if (faculty == null) return NotFound();

        if (await _db.Projects.AnyAsync(p => p.FacultyId == id) || await _db.Accounts.AnyAsync(a => a.FacultyId == id))
        {
            Error($"Không thể xóa \"{faculty.FacultyName}\" vì đang có dự án hoặc tài khoản thuộc khoa này.");
            return RedirectToAction(nameof(Index));
        }

        _db.Faculties.Remove(faculty);
        await _db.SaveChangesAsync();
        Success($"Đã xóa \"{faculty.FacultyName}\".");
        return RedirectToAction(nameof(Index));
    }
}
