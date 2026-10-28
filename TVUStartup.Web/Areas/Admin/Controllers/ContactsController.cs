using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TVUStartup.Web.Data;
using TVUStartup.Web.Models.Entities;

namespace TVUStartup.Web.Areas.Admin.Controllers;

/// <summary>Xử lý yêu cầu liên hệ hợp tác: Mới → Đã liên hệ → Đã đóng</summary>
public class ContactsController : AdminBaseController
{
    private readonly AppDbContext _db;

    public ContactsController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(ContactStatus? status)
    {
        var query = _db.ContactRequests.Include(c => c.Project).Include(c => c.Handler).AsQueryable();
        if (status.HasValue) query = query.Where(c => c.Status == status);

        ViewBag.Status = status;
        ViewBag.Counts = await _db.ContactRequests.GroupBy(c => c.Status)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count);
        return View(await query.OrderBy(c => c.Status).ThenByDescending(c => c.CreatedAt).AsNoTracking().ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int id, ContactStatus status, ContactStatus? filter)
    {
        var contact = await _db.ContactRequests.FindAsync(id);
        if (contact == null) return NotFound();
        if (!Enum.IsDefined(status)) return BadRequest();

        contact.Status = status;
        contact.HandledBy = CurrentId;
        await _db.SaveChangesAsync();
        Success("Đã cập nhật trạng thái liên hệ.");
        return RedirectToAction(nameof(Index), new { status = filter });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var contact = await _db.ContactRequests.FindAsync(id);
        if (contact == null) return NotFound();

        _db.ContactRequests.Remove(contact);
        await _db.SaveChangesAsync();
        Success("Đã xóa yêu cầu liên hệ.");
        return RedirectToAction(nameof(Index));
    }
}
