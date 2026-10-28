namespace TVUStartup.Web.Models.Entities;

/// <summary>Vai trò / phân quyền tài khoản</summary>
public class Role
{
    // Mã vai trò cố định (khớp dữ liệu khởi tạo)
    public const int AdminId = 1;
    public const int SinhVienId = 2;

    // Tên vai trò dùng trong [Authorize(Roles = ...)]
    public const string Admin = "Admin";
    public const string SinhVien = "SinhVien";

    public int RoleId { get; set; }
    public string RoleName { get; set; } = null!;
    public string? Description { get; set; }

    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}
