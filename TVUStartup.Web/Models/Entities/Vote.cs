namespace TVUStartup.Web.Models.Entities;

/// <summary>Lượt bình chọn / yêu thích dự án. Khóa chính ghép (ProjectId, AccountId)</summary>
public class Vote
{
    public int ProjectId { get; set; }
    public int AccountId { get; set; }
    public DateTime CreatedAt { get; set; }

    public Project Project { get; set; } = null!;
    public Account Account { get; set; } = null!;
}
