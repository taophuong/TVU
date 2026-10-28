namespace TVUStartup.Web.Models.Entities;

/// <summary>Lĩnh vực của dự án khởi nghiệp</summary>
public class Category
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Icon { get; set; }
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Project> Projects { get; set; } = new List<Project>();
}
