namespace TVUStartup.Web.Models.Entities;

/// <summary>Bình luận và đánh giá (sao) của người dùng</summary>
public class Comment
{
    public int CommentId { get; set; }
    public int ProjectId { get; set; }
    public int AccountId { get; set; }

    /// <summary>NULL: bình luận gốc; có giá trị: câu trả lời cho bình luận khác</summary>
    public int? ParentId { get; set; }
    public string Content { get; set; } = null!;

    /// <summary>Số sao 1–5, chỉ áp dụng cho bình luận gốc</summary>
    public byte? Rating { get; set; }
    public bool IsHidden { get; set; }
    public DateTime CreatedAt { get; set; }

    public Project Project { get; set; } = null!;
    public Account Account { get; set; } = null!;
    public Comment? Parent { get; set; }
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();
}
