using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TVUStartup.Web.Data;
using TVUStartup.Web.Models.Entities;

namespace TVUStartup.Web.Areas.Admin.Controllers;

/// <summary>Quản lý tài khoản: tìm kiếm, khóa / mở khóa, đặt lại mật khẩu</summary>
public class AccountsController : AdminBaseController
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher<Account> _hasher;

    public AccountsController(AppDbContext db, IPasswordHasher<Account> hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    public async Task<IActionResult> Index(string? q, int? role)
    {
        var query = _db.Accounts.Include(a => a.Role).Include(a => a.Faculty).AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            var keyword = q.Trim();
            query = query.Where(a => a.FullName.Contains(keyword) || a.Email.Contains(keyword) || (a.StudentCode != null && a.StudentCode.Contains(keyword)));
        }
        if (role.HasValue) query = query.Where(a => a.RoleId == role);

        ViewBag.Q = q;
        ViewBag.RoleFilter = role;
        ViewBag.ProjectCounts = await _db.Projects.GroupBy(p => p.OwnerId)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count);
        return View(await query.OrderBy(a => a.RoleId).ThenByDescending(a => a.CreatedAt).AsNoTracking().ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> ToggleLock(int id)
    {
        if (id == CurrentId)
        {
            Error("Bạn không thể tự khóa tài khoản của mình.");
            return RedirectToAction(nameof(Index));
        }
        var account = await _db.Accounts.FindAsync(id);
        if (account == null) return NotFound();

        // Tài khoản bị khóa sẽ bị đăng xuất ở request kế tiếp (xem OnValidatePrincipal trong Program.cs)
        account.Status = account.Status == AccountStatus.BiKhoa ? AccountStatus.HoatDong : AccountStatus.BiKhoa;
        account.UpdatedAt = DateTime.Now;
        await _db.SaveChangesAsync();
        Success(account.Status == AccountStatus.BiKhoa ? $"Đã khóa tài khoản {account.Email}." : $"Đã mở khóa tài khoản {account.Email}.");
        return RedirectToAction(nameof(Index));
    }

    /// <summary>Tạo mật khẩu tạm ngẫu nhiên, hiển thị một lần cho Admin để báo lại người dùng</summary>
    [HttpPost]
    public async Task<IActionResult> ResetPassword(int id)
    {
        var account = await _db.Accounts.FindAsync(id);
        if (account == null) return NotFound();

        var tempPassword = "Tvu@" + RandomNumberGenerator.GetInt32(100000, 999999);
        account.PasswordHash = _hasher.HashPassword(account, tempPassword);
        account.UpdatedAt = DateTime.Now;
        await _db.SaveChangesAsync();

        Success($"Mật khẩu mới của {account.Email} là: {tempPassword} – hãy gửi cho người dùng và yêu cầu đổi mật khẩu sau khi đăng nhập.");
        return RedirectToAction(nameof(Index));
    }
}
