namespace TVUStartup.Web.Models.Entities;

/// <summary>Khoa / đơn vị của Trường Đại học Trà Vinh</summary>
public class Faculty
{
    public int FacultyId { get; set; }
    public string FacultyName { get; set; } = null!;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }

    public ICollection<Account> Accounts { get; set; } = new List<Account>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}
