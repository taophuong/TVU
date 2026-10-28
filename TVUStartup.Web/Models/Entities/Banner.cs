namespace TVUStartup.Web.Models.Entities;

/// <summary>Banner trình chiếu trên trang chủ</summary>
public class Banner
{
    public int BannerId { get; set; }
    public string? Title { get; set; }
    public string ImageUrl { get; set; } = null!;
    public string? LinkUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
