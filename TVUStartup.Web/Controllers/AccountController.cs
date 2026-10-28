using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TVUStartup.Web.Data;
using TVUStartup.Web.Helpers;
using TVUStartup.Web.Models.Entities;
using TVUStartup.Web.Models.ViewModels;
using TVUStartup.Web.Services;

namespace TVUStartup.Web.Controllers;

/// <summary>Đăng ký, đăng nhập, đăng xuất, hồ sơ cá nhân, đổi mật khẩu</summary>
public class AccountController : Controller
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher<Account> _hasher;
    private readonly IFileStorageService _files;

    public AccountController(AppDbContext db, IPasswordHasher<Account> hasher, IFileStorageService files)
    {
        _db = db;
        _hasher = hasher;
        _files = files;
    }

    // ===== Đăng nhập =====
    [HttpGet("dang-nhap")]
    public IActionResult Login(string? returnUrl)
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Home");
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost("dang-nhap")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var email = model.Email.Trim().ToLowerInvariant();
        var account = await _db.Accounts.Include(a => a.Role).FirstOrDefaultAsync(a => a.Email == email);

        // Không cho biết cụ thể sai email hay sai mật khẩu (tránh dò tài khoản)
        var result = account == null
            ? PasswordVerificationResult.Failed
            : _hasher.VerifyHashedPassword(account, account.PasswordHash, model.Password);
        if (account == null || result == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
            return View(model);
        }
        if (account.Status == AccountStatus.BiKhoa)
        {
            ModelState.AddModelError("", "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ Ban quản trị.");
            return View(model);
        }

        // Thuật toán băm được nâng cấp → băm lại mật khẩu theo chuẩn mới
        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            account.PasswordHash = _hasher.HashPassword(account, model.Password);
            await _db.SaveChangesAsync();
        }

        await SignInAsync(account, model.RememberMe);

        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return Redirect(model.ReturnUrl);
        return account.RoleId == Role.AdminId
            ? RedirectToAction("Index", "Dashboard", new { area = "Admin" })
            : RedirectToAction("Index", "Home");
    }

    // ===== Đăng ký =====
    [HttpGet("dang-ky")]
    public async Task<IActionResult> Register()
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Home");
        return View(new RegisterViewModel { Faculties = await FacultyOptionsAsync() });
    }

    [HttpPost("dang-ky")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        var email = model.Email.Trim().ToLowerInvariant();
        if (await _db.Accounts.AnyAsync(a => a.Email == email))
            ModelState.AddModelError(nameof(model.Email), "Email này đã được đăng ký.");
        if (model.FacultyId.HasValue && !await _db.Faculties.AnyAsync(f => f.FacultyId == model.FacultyId))
            ModelState.AddModelError(nameof(model.FacultyId), "Khoa không hợp lệ.");

        if (!ModelState.IsValid)
        {
            model.Faculties = await FacultyOptionsAsync();
            return View(model);
        }

        var account = new Account
        {
            Email = email,
            FullName = model.FullName.Trim(),
            StudentCode = model.StudentCode,
            ClassName = model.ClassName.Trim(),
            FacultyId = model.FacultyId,
            Phone = model.Phone,
            RoleId = Role.SinhVienId,
            Status = AccountStatus.HoatDong,
            CreatedAt = DateTime.Now
        };
        account.PasswordHash = _hasher.HashPassword(account, model.Password);
        _db.Accounts.Add(account);
        await _db.SaveChangesAsync();

        account.Role = await _db.Roles.FindAsync(Role.SinhVienId) ?? throw new InvalidOperationException();
        await SignInAsync(account, false);
        TempData["Success"] = "Đăng ký thành công! Bạn có thể đăng dự án khởi nghiệp của mình ngay.";
        return RedirectToAction("Index", "MyProjects");
    }

    // ===== Đăng xuất =====
    [HttpPost("dang-xuat")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    // ===== Hồ sơ cá nhân =====
    [Authorize]
    [HttpGet("tai-khoan")]
    public async Task<IActionResult> Profile()
    {
        var account = await CurrentAccountAsync();
        if (account == null) return NotFound();

        var model = new ProfileViewModel
        {
            Email = account.Email,
            Avatar = account.Avatar,
            IsStudent = account.RoleId == Role.SinhVienId,
            FullName = account.FullName,
            StudentCode = account.StudentCode,
            ClassName = account.ClassName,
            FacultyId = account.FacultyId,
            Phone = account.Phone,
            Faculties = await FacultyOptionsAsync()
        };
        return View(model);
    }

    [Authorize]
    [HttpPost("tai-khoan")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ProfileViewModel model)
    {
        var account = await CurrentAccountAsync();
        if (account == null) return NotFound();

        if (model.AvatarFile != null)
        {
            var error = _files.Validate(model.AvatarFile);
            if (error != null) ModelState.AddModelError(nameof(model.AvatarFile), error);
        }
        if (!ModelState.IsValid)
        {
            model.Email = account.Email;
            model.Avatar = account.Avatar;
            model.IsStudent = account.RoleId == Role.SinhVienId;
            model.Faculties = await FacultyOptionsAsync();
            return View(model);
        }

        account.FullName = model.FullName.Trim();
        account.Phone = model.Phone;
        if (account.RoleId == Role.SinhVienId)
        {
            account.StudentCode = model.StudentCode;
            account.ClassName = model.ClassName;
            account.FacultyId = model.FacultyId;
        }
        if (model.AvatarFile != null)
        {
            _files.Delete(account.Avatar);
            account.Avatar = await _files.SaveImageAsync(model.AvatarFile, "avatars");
        }
        account.UpdatedAt = DateTime.Now;
        await _db.SaveChangesAsync();

        // Đăng nhập lại để cập nhật họ tên / ảnh đại diện trong cookie
        await SignInAsync(account, true);
        TempData["Success"] = "Đã cập nhật hồ sơ.";
        return RedirectToAction(nameof(Profile));
    }

    // ===== Đổi mật khẩu =====
    [Authorize]
    [HttpGet("tai-khoan/doi-mat-khau")]
    public IActionResult ChangePassword() => View(new ChangePasswordViewModel());

    [Authorize]
    [HttpPost("tai-khoan/doi-mat-khau")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var account = await CurrentAccountAsync();
        if (account == null) return NotFound();

        if (_hasher.VerifyHashedPassword(account, account.PasswordHash, model.CurrentPassword) == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(nameof(model.CurrentPassword), "Mật khẩu hiện tại không đúng.");
            return View(model);
        }

        account.PasswordHash = _hasher.HashPassword(account, model.NewPassword);
        account.UpdatedAt = DateTime.Now;
        await _db.SaveChangesAsync();

        TempData["Success"] = "Đổi mật khẩu thành công.";
        return RedirectToAction(nameof(Profile));
    }

    // ===== Hàm hỗ trợ =====

    /// <summary>Tạo cookie đăng nhập chứa các thông tin (claim) của tài khoản</summary>
    private async Task SignInAsync(Account account, bool persistent)
    {
        var role = account.Role ?? await _db.Roles.FirstAsync(r => r.RoleId == account.RoleId);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
            new(ClaimTypes.Name, account.FullName),
            new(ClaimTypes.Email, account.Email),
            new(ClaimTypes.Role, role.RoleName)
        };
        if (!string.IsNullOrEmpty(account.Avatar)) claims.Add(new Claim("Avatar", account.Avatar));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = persistent });
    }

    private async Task<Account?> CurrentAccountAsync()
    {
        var id = User.GetAccountId();
        return id == null ? null : await _db.Accounts.Include(a => a.Role).FirstOrDefaultAsync(a => a.AccountId == id);
    }

    private Task<List<SelectListItem>> FacultyOptionsAsync() => _db.FacultyOptionsAsync();
}
