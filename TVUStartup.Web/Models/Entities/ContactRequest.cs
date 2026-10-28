namespace TVUStartup.Web.Models.Entities;

/// <summary>Yêu cầu hợp tác, đầu tư, sử dụng sản phẩm</summary>
public class ContactRequest
{
    public int ContactId { get; set; }

    /// <summary>NULL: liên hệ chung với ban quản trị website</summary>
    public int? ProjectId { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Organization { get; set; }
    public RequestType RequestType { get; set; }
    public string Message { get; set; } = null!;
    public ContactStatus Status { get; set; } = ContactStatus.Moi;
    public DateTime CreatedAt { get; set; }
    public int? HandledBy { get; set; }

    public Project? Project { get; set; }
    public Account? Handler { get; set; }
}
