using System.Security.Claims;
using TVUStartup.Web.Models.Entities;

namespace TVUStartup.Web.Helpers;

/// <summary>Đọc thông tin tài khoản đang đăng nhập từ cookie (Claims)</summary>
public static class UserExtensions
{
    /// <summary>Mã tài khoản (AccountId) hoặc null nếu chưa đăng nhập</summary>
    public static int? GetAccountId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out var id) ? id : null;
    }

    public static bool IsAdmin(this ClaimsPrincipal user) => user.IsInRole(Role.Admin);

    public static string? GetAvatar(this ClaimsPrincipal user) => user.FindFirstValue("Avatar");
}
