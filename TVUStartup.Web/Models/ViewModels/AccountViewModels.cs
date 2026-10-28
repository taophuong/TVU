using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TVUStartup.Web.Models.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [Display(Name = "Email")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string Password { get; set; } = "";

    [Display(Name = "Ghi nhớ đăng nhập")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}

public class RegisterViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [StringLength(100)]
    [Display(Name = "Email")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập mã số sinh viên")]
    [RegularExpression(@"^\d{6,12}$", ErrorMessage = "MSSV chỉ gồm 6–12 chữ số")]
    [Display(Name = "Mã số sinh viên")]
    public string StudentCode { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập lớp")]
    [StringLength(50)]
    [Display(Name = "Lớp")]
    public string ClassName { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng chọn khoa")]
    [Display(Name = "Khoa")]
    public int? FacultyId { get; set; }

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [StringLength(15)]
    [Display(Name = "Số điện thoại")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu từ 6 ký tự trở lên")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string Password { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu")]
    [Compare(nameof(Password), ErrorMessage = "Mật khẩu nhập lại không khớp")]
    [DataType(DataType.Password)]
    [Display(Name = "Nhập lại mật khẩu")]
    public string ConfirmPassword { get; set; } = "";

    public List<SelectListItem> Faculties { get; set; } = new();
}

public class ProfileViewModel
{
    public string Email { get; set; } = "";
    public string? Avatar { get; set; }
    public bool IsStudent { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    [StringLength(100)]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = "";

    [RegularExpression(@"^\d{6,12}$", ErrorMessage = "MSSV chỉ gồm 6–12 chữ số")]
    [Display(Name = "Mã số sinh viên")]
    public string? StudentCode { get; set; }

    [StringLength(50)]
    [Display(Name = "Lớp")]
    public string? ClassName { get; set; }

    [Display(Name = "Khoa")]
    public int? FacultyId { get; set; }

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [StringLength(15)]
    [Display(Name = "Số điện thoại")]
    public string? Phone { get; set; }

    [Display(Name = "Ảnh đại diện")]
    public IFormFile? AvatarFile { get; set; }

    public List<SelectListItem> Faculties { get; set; } = new();
}

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu hiện tại")]
    public string CurrentPassword { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu từ 6 ký tự trở lên")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu mới")]
    public string NewPassword { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu mới")]
    [Compare(nameof(NewPassword), ErrorMessage = "Mật khẩu nhập lại không khớp")]
    [DataType(DataType.Password)]
    [Display(Name = "Nhập lại mật khẩu mới")]
    public string ConfirmPassword { get; set; } = "";
}
