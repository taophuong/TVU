namespace TVUStartup.Web.Models.Entities;

/// <summary>Thư viện hình ảnh của dự án</summary>
public class ProjectImage
{
    public int ImageId { get; set; }
    public int ProjectId { get; set; }
    public string ImageUrl { get; set; } = null!;
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; }

    public Project Project { get; set; } = null!;
}
