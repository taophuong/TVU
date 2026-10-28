using System.ComponentModel.DataAnnotations;
using TVUStartup.Web.Models.Entities;

namespace TVUStartup.Web.Models.ViewModels;

/// <summary>Form bình luận / đánh giá dự án</summary>
public class CommentFormModel
{
    [Required(ErrorMessage = "Vui lòng nhập nội dung bình luận")]
    [StringLength(1000, ErrorMessage = "Bình luận tối đa 1000 ký tự")]
    [Display(Name = "Nội dung")]
    public string Content { get; set; } = "";

    [Range(1, 5, ErrorMessage = "Số sao từ 1 đến 5")]
    public byte? Rating { get; set; }
}

/// <summary>Form gửi yêu cầu liên hệ hợp tác (dùng cho trang dự án và trang Liên hệ chung)</summary>
public class ContactFormModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    [StringLength(100)]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [StringLength(100)]
    [Display(Name = "Email")]
    public string Email { get; set; } = "";

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [StringLength(15)]
    [Display(Name = "Số điện thoại")]
    public string? Phone { get; set; }

    [StringLength(150)]
    [Display(Name = "Đơn vị / Tổ chức")]
    public string? Organization { get; set; }

    [Display(Name = "Nhu cầu")]
    public RequestType RequestType { get; set; } = RequestType.HopTac;

    [Required(ErrorMessage = "Vui lòng nhập nội dung")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Nội dung từ 10 đến 2000 ký tự")]
    [Display(Name = "Nội dung")]
    public string Message { get; set; } = "";
}
