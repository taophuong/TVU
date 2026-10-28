using TVUStartup.Web.Models.Entities;

namespace TVUStartup.Web.Models.ViewModels;

/// <summary>Trang danh sách tin tức (lọc theo loại + phân trang)</summary>
public class NewsListViewModel
{
    public NewsType? Type { get; set; }
    public int Page { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public List<News> Items { get; set; } = new();
}
