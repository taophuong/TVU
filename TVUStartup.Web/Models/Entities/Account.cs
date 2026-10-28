namespace TVUStartup.Web.Models.Entities;

/// <summary>Tài khoản người dùng (quản trị viên, sinh viên)</summary>
public class Account
{
    public int AccountId { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string? StudentCode { get; set; }
    public string? ClassName { get; set; }
    public string? Phone { get; set; }
    public string? Avatar { get; set; }
    public int? FacultyId { get; set; }
    public int RoleId { get; set; }
    public AccountStatus Status { get; set; } = AccountStatus.HoatDong;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Faculty? Faculty { get; set; }
    public Role Role { get; set; } = null!;
    public ICollection<Project> OwnedProjects { get; set; } = new List<Project>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
}
