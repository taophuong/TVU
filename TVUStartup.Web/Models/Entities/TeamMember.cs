namespace TVUStartup.Web.Models.Entities;

/// <summary>Thành viên đội ngũ thực hiện dự án</summary>
public class TeamMember
{
    public int MemberId { get; set; }
    public int ProjectId { get; set; }
    public int? AccountId { get; set; }
    public string FullName { get; set; } = null!;
    public string? Position { get; set; }
    public string? StudentCode { get; set; }
    public string? ClassName { get; set; }
    public string? Avatar { get; set; }
    public string? Email { get; set; }
    public int DisplayOrder { get; set; }

    public Project Project { get; set; } = null!;
    public Account? Account { get; set; }
}
