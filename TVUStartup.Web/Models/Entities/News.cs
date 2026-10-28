namespace TVUStartup.Web.Models.Entities;

/// <summary>Tin tức, sự kiện, cuộc thi khởi nghiệp</summary>
public class News
{
    public int NewsId { get; set; }
    public string Title { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Thumbnail { get; set; }
    public string? Summary { get; set; }
    public string Content { get; set; } = null!;
    public NewsType NewsType { get; set; } = NewsType.TinTuc;
    public DateTime? EventDate { get; set; }
    public string? Location { get; set; }
    public int AuthorId { get; set; }
    public bool IsPublished { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Account Author { get; set; } = null!;
}
