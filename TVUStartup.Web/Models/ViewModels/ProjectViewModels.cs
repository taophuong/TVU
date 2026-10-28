using TVUStartup.Web.Models.Entities;

namespace TVUStartup.Web.Models.ViewModels;

/// <summary>Thông tin rút gọn để hiển thị thẻ dự án</summary>
public class ProjectCardViewModel
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Slogan { get; set; }
    public string ShortDescription { get; set; } = null!;
    public string? Logo { get; set; }
    public string? CoverImage { get; set; }
    public string CategoryName { get; set; } = null!;
    public string? CategoryIcon { get; set; }
    public string FacultyName { get; set; } = null!;
    public ProjectStage Stage { get; set; }
    public bool IsFeatured { get; set; }
    public int ViewCount { get; set; }
    public int VoteCount { get; set; }
}

/// <summary>Dữ liệu trang chủ</summary>
public class HomeViewModel
{
    public int ProjectCount { get; set; }
    public int CategoryCount { get; set; }
    public int FacultyCount { get; set; }
    public int TotalViews { get; set; }
    public List<ProjectCardViewModel> FeaturedProjects { get; set; } = new();
    public List<ProjectCardViewModel> LatestProjects { get; set; } = new();
    public List<CategoryCountViewModel> Categories { get; set; } = new();
    public List<Banner> Banners { get; set; } = new();
    public List<News> LatestNews { get; set; } = new();
}

public class CategoryCountViewModel
{
    public string CategoryName { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Icon { get; set; }
    public int ProjectCount { get; set; }
}

/// <summary>Bộ lọc + kết quả của trang danh sách dự án</summary>
public class ProjectListViewModel
{
    public const int PageSize = 9;

    // Tham số lọc (nhận từ query string)
    public string? Q { get; set; }
    public string? Category { get; set; }
    public int? Faculty { get; set; }
    public ProjectStage? Stage { get; set; }
    public string Sort { get; set; } = "newest";
    public int Page { get; set; } = 1;

    // Kết quả
    public List<ProjectCardViewModel> Projects { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    // Dữ liệu cho các ô chọn
    public List<CategoryCountViewModel> Categories { get; set; } = new();
    public List<Faculty> Faculties { get; set; } = new();

    public bool HasFilter => !string.IsNullOrWhiteSpace(Q) || !string.IsNullOrEmpty(Category) || Faculty.HasValue || Stage.HasValue;

    /// <summary>Tạo bộ tham số route giữ nguyên bộ lọc hiện tại, chỉ đổi số trang</summary>
    public Dictionary<string, string> RouteFor(int page)
    {
        var route = new Dictionary<string, string>();
        if (!string.IsNullOrWhiteSpace(Q)) route["q"] = Q;
        if (!string.IsNullOrEmpty(Category)) route["category"] = Category;
        if (Faculty.HasValue) route["faculty"] = Faculty.Value.ToString();
        if (Stage.HasValue) route["stage"] = ((byte)Stage.Value).ToString();
        if (Sort != "newest") route["sort"] = Sort;
        if (page > 1) route["page"] = page.ToString();
        return route;
    }
}

/// <summary>Dữ liệu trang chi tiết dự án</summary>
public class ProjectDetailViewModel
{
    public Project Project { get; set; } = null!;

    /// <summary>Phiên bản được phép xem: khách chỉ thấy bản công khai; chủ dự án / Admin thấy cả bản nội bộ</summary>
    public List<ProjectVersion> Versions { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();
    public int VoteCount { get; set; }
    public double? AverageRating { get; set; }
    public int RatingCount { get; set; }
    public string? VideoEmbedUrl { get; set; }
    public List<ProjectCardViewModel> RelatedProjects { get; set; } = new();

    // Trạng thái theo người đang xem
    public bool IsPreview { get; set; }      // dự án chưa công khai, chỉ chủ dự án / Admin xem được
    public bool IsOwner { get; set; }
    public bool IsAdmin { get; set; }
    public int? CurrentAccountId { get; set; }
    public bool HasVoted { get; set; }
    public bool HasRated { get; set; }
    public bool CanInteract => !IsPreview && CurrentAccountId.HasValue;

    public ContactFormModel ContactForm { get; set; } = new();
}
