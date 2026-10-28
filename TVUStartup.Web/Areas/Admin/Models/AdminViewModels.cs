using System.ComponentModel.DataAnnotations;
using TVUStartup.Web.Models.Entities;

namespace TVUStartup.Web.Areas.Admin.Models;

/// <summary>Số liệu cho trang tổng quan (dashboard)</summary>
public class DashboardViewModel
{
    public int ApprovedCount { get; set; }
    public int PendingCount { get; set; }
    public int StudentCount { get; set; }
    public int NewContactCount { get; set; }
    public int TotalViews { get; set; }
    public int TotalVotes { get; set; }
    public int TotalComments { get; set; }

    public List<(string Label, int Count)> ByStatus { get; set; } = new();
    public List<(string Label, int Count)> ByCategory { get; set; } = new();
    public List<(string Label, int Count)> ByFaculty { get; set; } = new();
    public List<(string Label, int Count)> ByMonth { get; set; } = new();

    public List<Project> PendingProjects { get; set; } = new();
    public List<Project> TopProjects { get; set; } = new();
    public List<ContactRequest> NewContacts { get; set; } = new();
}

/// <summary>Danh sách dự án trong khu quản trị</summary>
public class AdminProjectListViewModel
{
    public ProjectStatus? Status { get; set; }
    public string? Q { get; set; }
    public List<Project> Items { get; set; } = new();
    public Dictionary<ProjectStatus, int> StatusCounts { get; set; } = new();
}

public class CategoryFormModel
{
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên lĩnh vực")]
    [StringLength(100)]
    [Display(Name = "Tên lĩnh vực")]
    public string CategoryName { get; set; } = "";

    [StringLength(100)]
    [RegularExpression(@"^bi-[a-z0-9-]+$", ErrorMessage = "Tên biểu tượng Bootstrap Icons, ví dụ: bi-tree")]
    [Display(Name = "Biểu tượng (Bootstrap Icons)")]
    public string? Icon { get; set; }

    [StringLength(500)]
    [Display(Name = "Mô tả")]
    public string? Description { get; set; }

    [Range(0, 999)]
    [Display(Name = "Thứ tự hiển thị")]
    public int DisplayOrder { get; set; }

    [Display(Name = "Đang sử dụng")]
    public bool IsActive { get; set; } = true;
}

public class FacultyFormModel
{
    public int FacultyId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên khoa")]
    [StringLength(150)]
    [Display(Name = "Tên khoa")]
    public string FacultyName { get; set; } = "";

    [StringLength(500)]
    [Display(Name = "Mô tả")]
    public string? Description { get; set; }

    [Range(0, 999)]
    [Display(Name = "Thứ tự hiển thị")]
    public int DisplayOrder { get; set; }
}

public class NewsFormModel
{
    public int NewsId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
    [StringLength(200)]
    [Display(Name = "Tiêu đề")]
    public string Title { get; set; } = "";

    [StringLength(500)]
    [Display(Name = "Tóm tắt")]
    public string? Summary { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập nội dung")]
    [Display(Name = "Nội dung")]
    public string Content { get; set; } = "";

    [Display(Name = "Loại bài viết")]
    public NewsType NewsType { get; set; } = NewsType.TinTuc;

    [Display(Name = "Thời gian diễn ra")]
    public DateTime? EventDate { get; set; }

    [StringLength(200)]
    [Display(Name = "Địa điểm")]
    public string? Location { get; set; }

    [Display(Name = "Xuất bản")]
    public bool IsPublished { get; set; }

    [Display(Name = "Ảnh đại diện")]
    public IFormFile? ThumbnailFile { get; set; }
    public string? Thumbnail { get; set; }
}

public class BannerFormModel
{
    public int BannerId { get; set; }

    [StringLength(150)]
    [Display(Name = "Tiêu đề")]
    public string? Title { get; set; }

    [StringLength(255)]
    [Display(Name = "Đường dẫn khi bấm")]
    public string? LinkUrl { get; set; }

    [Range(0, 999)]
    [Display(Name = "Thứ tự hiển thị")]
    public int DisplayOrder { get; set; }

    [Display(Name = "Đang hiển thị")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Ảnh banner")]
    public IFormFile? ImageFile { get; set; }
    public string? ImageUrl { get; set; }
}
