using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TVUStartup.Web.Models.Entities;

namespace TVUStartup.Web.Models.ViewModels;

/// <summary>Một dòng trong danh sách "Dự án của tôi"</summary>
public class MyProjectItemViewModel
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Logo { get; set; }
    public string CategoryName { get; set; } = null!;
    public ProjectStatus Status { get; set; }
    public string? RejectReason { get; set; }
    public int ViewCount { get; set; }
    public int VoteCount { get; set; }
    public int CommentCount { get; set; }
    public int NewContactCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>Form thêm / sửa thông tin chung của dự án</summary>
public class ProjectFormModel
{
    public int ProjectId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên dự án")]
    [StringLength(150, ErrorMessage = "Tên dự án tối đa 150 ký tự")]
    [Display(Name = "Tên dự án / sản phẩm")]
    public string ProjectName { get; set; } = "";

    [StringLength(200, ErrorMessage = "Slogan tối đa 200 ký tự")]
    [Display(Name = "Slogan")]
    public string? Slogan { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mô tả ngắn")]
    [StringLength(500, MinimumLength = 20, ErrorMessage = "Mô tả ngắn từ 20 đến 500 ký tự")]
    [Display(Name = "Mô tả ngắn")]
    public string ShortDescription { get; set; } = "";

    [Display(Name = "Giới thiệu chi tiết")]
    public string? Content { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn lĩnh vực")]
    [Display(Name = "Lĩnh vực")]
    public int? CategoryId { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn khoa")]
    [Display(Name = "Khoa")]
    public int? FacultyId { get; set; }

    [Display(Name = "Giai đoạn")]
    public ProjectStage Stage { get; set; } = ProjectStage.YTuong;

    [Display(Name = "Ngày bắt đầu")]
    [DataType(DataType.Date)]
    public DateOnly? FoundedDate { get; set; }

    [Url(ErrorMessage = "Đường dẫn không hợp lệ (bắt đầu bằng http:// hoặc https://)")]
    [StringLength(255)]
    [Display(Name = "Website / trang sản phẩm")]
    public string? WebsiteUrl { get; set; }

    [Url(ErrorMessage = "Đường dẫn không hợp lệ (bắt đầu bằng http:// hoặc https://)")]
    [StringLength(255)]
    [Display(Name = "Fanpage / nhóm cộng đồng")]
    public string? CommunityUrl { get; set; }

    [Url(ErrorMessage = "Đường dẫn không hợp lệ (bắt đầu bằng http:// hoặc https://)")]
    [StringLength(255)]
    [Display(Name = "Video YouTube giới thiệu")]
    public string? VideoUrl { get; set; }

    [Display(Name = "Logo")]
    public IFormFile? LogoFile { get; set; }

    [Display(Name = "Ảnh bìa")]
    public IFormFile? CoverFile { get; set; }

    // Hiển thị ảnh hiện tại khi sửa
    public string? Logo { get; set; }
    public string? CoverImage { get; set; }

    public List<SelectListItem> Categories { get; set; } = new();
    public List<SelectListItem> Faculties { get; set; } = new();
}

/// <summary>Trang sửa dự án: thông tin chung + các danh sách con</summary>
public class ProjectEditViewModel
{
    public ProjectFormModel Form { get; set; } = new();
    public string Slug { get; set; } = "";
    public ProjectStatus Status { get; set; }
    public string? RejectReason { get; set; }
    public string Tab { get; set; } = "info";

    public List<ProjectImage> Images { get; set; } = new();
    public List<ProjectFeature> Features { get; set; } = new();
    public List<ProjectVersion> Versions { get; set; } = new();
    public List<TeamMember> Members { get; set; } = new();

    public FeatureFormModel NewFeature { get; set; } = new();
    public VersionFormModel NewVersion { get; set; } = new();
    public MemberFormModel NewMember { get; set; } = new();
}

public class FeatureFormModel
{
    [Required(ErrorMessage = "Vui lòng nhập tên tính năng")]
    [StringLength(150)]
    [Display(Name = "Tên tính năng")]
    public string Title { get; set; } = "";

    [StringLength(500)]
    [Display(Name = "Mô tả")]
    public string? Description { get; set; }

    [StringLength(100)]
    [Display(Name = "Biểu tượng")]
    public string? Icon { get; set; }
}

public class VersionFormModel
{
    [Required(ErrorMessage = "Vui lòng nhập tên phiên bản")]
    [StringLength(100)]
    [Display(Name = "Tên phiên bản")]
    public string VersionName { get; set; } = "";

    [StringLength(100)]
    [Display(Name = "Nền tảng")]
    public string? Platform { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày phát hành")]
    public DateOnly? ReleaseDate { get; set; }

    [Url(ErrorMessage = "Đường dẫn không hợp lệ")]
    [StringLength(255)]
    [Display(Name = "Link tải về")]
    public string? DownloadUrl { get; set; }

    [StringLength(2000)]
    [Display(Name = "Thay đổi trong phiên bản")]
    public string? ChangeLog { get; set; }

    [Display(Name = "Công khai phiên bản này")]
    public bool IsPublic { get; set; } = true;
}

public class MemberFormModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    [StringLength(100)]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = "";

    [StringLength(100)]
    [Display(Name = "Vai trò")]
    public string? Position { get; set; }

    [StringLength(20)]
    [Display(Name = "MSSV")]
    public string? StudentCode { get; set; }

    [StringLength(50)]
    [Display(Name = "Lớp")]
    public string? ClassName { get; set; }

    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [StringLength(100)]
    [Display(Name = "Email")]
    public string? Email { get; set; }
}

/// <summary>Danh sách liên hệ gửi tới các dự án của sinh viên</summary>
public class MyContactViewModel
{
    public ContactRequest Request { get; set; } = null!;
    public string ProjectName { get; set; } = "";
    public string ProjectSlug { get; set; } = "";
}
