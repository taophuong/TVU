namespace TVUStartup.Web.Models.Entities;

/// <summary>Dự án / sản phẩm khởi nghiệp (bảng trung tâm)</summary>
public class Project
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Slogan { get; set; }
    public string ShortDescription { get; set; } = null!;
    public string? Content { get; set; }
    public string? Logo { get; set; }
    public string? CoverImage { get; set; }
    public string? VideoUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? CommunityUrl { get; set; }
    public int CategoryId { get; set; }
    public int FacultyId { get; set; }
    public int OwnerId { get; set; }
    public ProjectStage Stage { get; set; } = ProjectStage.YTuong;
    public ProjectStatus Status { get; set; } = ProjectStatus.Nhap;
    public string? RejectReason { get; set; }
    public bool IsFeatured { get; set; }
    public int ViewCount { get; set; }
    public DateOnly? FoundedDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public int? ApprovedBy { get; set; }

    public Category Category { get; set; } = null!;
    public Faculty Faculty { get; set; } = null!;
    public Account Owner { get; set; } = null!;
    public Account? Approver { get; set; }

    public ICollection<ProjectImage> Images { get; set; } = new List<ProjectImage>();
    public ICollection<ProjectFeature> Features { get; set; } = new List<ProjectFeature>();
    public ICollection<ProjectVersion> Versions { get; set; } = new List<ProjectVersion>();
    public ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
    public ICollection<ContactRequest> ContactRequests { get; set; } = new List<ContactRequest>();
}
