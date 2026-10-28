using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace TVUStartup.Web.Data;

/// <summary>Dữ liệu cho các ô chọn (dropdown) dùng ở nhiều form</summary>
public static class LookupQueries
{
    public static Task<List<SelectListItem>> CategoryOptionsAsync(this AppDbContext db) =>
        db.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new SelectListItem(c.CategoryName, c.CategoryId.ToString()))
            .ToListAsync();

    public static Task<List<SelectListItem>> FacultyOptionsAsync(this AppDbContext db) =>
        db.Faculties
            .OrderBy(f => f.DisplayOrder)
            .Select(f => new SelectListItem(f.FacultyName, f.FacultyId.ToString()))
            .ToListAsync();
}
