namespace TVUStartup.Web.Models.Entities;

/// <summary>Tính năng nổi bật của sản phẩm</summary>
public class ProjectFeature
{
    public int FeatureId { get; set; }
    public int ProjectId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; }

    public Project Project { get; set; } = null!;
}
