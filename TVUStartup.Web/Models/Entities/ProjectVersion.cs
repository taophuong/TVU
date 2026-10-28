namespace TVUStartup.Web.Models.Entities;

/// <summary>Phiên bản phát hành của sản phẩm</summary>
public class ProjectVersion
{
    public int VersionId { get; set; }
    public int ProjectId { get; set; }
    public string VersionName { get; set; } = null!;
    public string? Platform { get; set; }
    public DateOnly? ReleaseDate { get; set; }
    public string? ChangeLog { get; set; }
    public string? DownloadUrl { get; set; }

    /// <summary>true: bản công khai; false: bản nội bộ / nghiên cứu (chỉ Admin và chủ dự án thấy)</summary>
    public bool IsPublic { get; set; } = true;

    public Project Project { get; set; } = null!;
}
