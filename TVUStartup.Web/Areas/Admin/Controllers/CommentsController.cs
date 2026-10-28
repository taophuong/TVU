using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TVUStartup.Web.Data;

namespace TVUStartup.Web.Areas.Admin.Controllers;

/// <summary>Kiểm duyệt bình luận: ẩn / hiện / xóa</summary>
public class CommentsController : AdminBaseController
{
    private const int PageSize = 20;
    private readonly AppDbContext _db;

    public CommentsController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(bool hiddenOnly = false, int page = 1)
    {
        var query = _db.Comments.AsQueryable();
        if (hiddenOnly) query = query.Where(c => c.IsHidden);

        var total = await query.CountAsync();
        ViewBag.HiddenOnly = hiddenOnly;
        ViewBag.Page = page;
        ViewBag.TotalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));

        return View(await query
            .Include(c => c.Account)
            .Include(c => c.Project)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .AsNoTracking()
            .ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> ToggleHidden(int id)
    {
        var comment = await _db.Comments.FindAsync(id);
        if (comment == null) return NotFound();

        comment.IsHidden = !comment.IsHidden;
        await _db.SaveChangesAsync();
        Success(comment.IsHidden ? "Đã ẩn bình luận." : "Đã hiện lại bình luận.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var comment = await _db.Comments.Include(c => c.Replies).FirstOrDefaultAsync(c => c.CommentId == id);
        if (comment == null) return NotFound();

        _db.Comments.Remove(comment); // các câu trả lời bị xóa kèm (ClientCascade)
        await _db.SaveChangesAsync();
        Success("Đã xóa bình luận.");
        return RedirectToAction(nameof(Index));
    }
}
