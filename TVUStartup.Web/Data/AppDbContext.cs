using Microsoft.EntityFrameworkCore;
using TVUStartup.Web.Models.Entities;

namespace TVUStartup.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Faculty> Faculties => Set<Faculty>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectImage> ProjectImages => Set<ProjectImage>();
    public DbSet<ProjectFeature> ProjectFeatures => Set<ProjectFeature>();
    public DbSet<ProjectVersion> ProjectVersions => Set<ProjectVersion>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Vote> Votes => Set<Vote>();
    public DbSet<ContactRequest> ContactRequests => Set<ContactRequest>();
    public DbSet<News> News => Set<News>();
    public DbSet<Banner> Banners => Set<Banner>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ===== Role =====
        modelBuilder.Entity<Role>(e =>
        {
            e.ToTable("Role");
            e.HasKey(x => x.RoleId);
            e.Property(x => x.RoleName).HasMaxLength(50).IsRequired();
            e.Property(x => x.Description).HasMaxLength(200);
            e.HasIndex(x => x.RoleName).IsUnique();
        });

        // ===== Faculty =====
        modelBuilder.Entity<Faculty>(e =>
        {
            e.ToTable("Faculty");
            e.HasKey(x => x.FacultyId);
            e.Property(x => x.FacultyName).HasMaxLength(150).IsRequired();
            e.Property(x => x.Description).HasMaxLength(500);
            e.Property(x => x.DisplayOrder).HasDefaultValue(0);
            e.HasIndex(x => x.FacultyName).IsUnique();
        });

        // ===== Account =====
        modelBuilder.Entity<Account>(e =>
        {
            e.ToTable("Account");
            e.HasKey(x => x.AccountId);
            e.Property(x => x.Email).HasMaxLength(100).IsUnicode(false).IsRequired();
            e.Property(x => x.PasswordHash).HasMaxLength(200).IsUnicode(false).IsRequired();
            e.Property(x => x.FullName).HasMaxLength(100).IsRequired();
            e.Property(x => x.StudentCode).HasMaxLength(20).IsUnicode(false);
            e.Property(x => x.ClassName).HasMaxLength(50);
            e.Property(x => x.Phone).HasMaxLength(15).IsUnicode(false);
            e.Property(x => x.Avatar).HasMaxLength(255);
            e.Property(x => x.Status).HasColumnType("tinyint").HasDefaultValue(AccountStatus.HoatDong).HasSentinel((AccountStatus)0);
            e.Property(x => x.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
            e.Property(x => x.UpdatedAt).HasColumnType("datetime");
            e.HasIndex(x => x.Email).IsUnique();

            e.HasOne(x => x.Role).WithMany(r => r.Accounts)
                .HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Faculty).WithMany(f => f.Accounts)
                .HasForeignKey(x => x.FacultyId).OnDelete(DeleteBehavior.ClientSetNull);
        });

        // ===== Category =====
        modelBuilder.Entity<Category>(e =>
        {
            e.ToTable("Category");
            e.HasKey(x => x.CategoryId);
            e.Property(x => x.CategoryName).HasMaxLength(100).IsRequired();
            e.Property(x => x.Slug).HasMaxLength(120).IsUnicode(false).IsRequired();
            e.Property(x => x.Icon).HasMaxLength(100);
            e.Property(x => x.Description).HasMaxLength(500);
            e.Property(x => x.DisplayOrder).HasDefaultValue(0);
            e.Property(x => x.IsActive).HasDefaultValue(true).HasSentinel(true);
            e.HasIndex(x => x.CategoryName).IsUnique();
            e.HasIndex(x => x.Slug).IsUnique();
        });

        // ===== Project =====
        modelBuilder.Entity<Project>(e =>
        {
            e.ToTable("Project");
            e.HasKey(x => x.ProjectId);
            e.Property(x => x.ProjectName).HasMaxLength(150).IsRequired();
            e.Property(x => x.Slug).HasMaxLength(170).IsUnicode(false).IsRequired();
            e.Property(x => x.Slogan).HasMaxLength(200);
            e.Property(x => x.ShortDescription).HasMaxLength(500).IsRequired();
            e.Property(x => x.Content); // nvarchar(MAX)
            e.Property(x => x.Logo).HasMaxLength(255);
            e.Property(x => x.CoverImage).HasMaxLength(255);
            e.Property(x => x.VideoUrl).HasMaxLength(255);
            e.Property(x => x.WebsiteUrl).HasMaxLength(255);
            e.Property(x => x.CommunityUrl).HasMaxLength(255);
            // Enum không có giá trị 0 → khai báo sentinel = 0 để EF biết khi nào dùng giá trị mặc định của CSDL
            e.Property(x => x.Stage).HasColumnType("tinyint").HasDefaultValue(ProjectStage.YTuong).HasSentinel((ProjectStage)0);
            e.Property(x => x.Status).HasColumnType("tinyint").HasDefaultValue(ProjectStatus.Nhap);
            e.Property(x => x.RejectReason).HasMaxLength(500);
            e.Property(x => x.IsFeatured).HasDefaultValue(false);
            e.Property(x => x.ViewCount).HasDefaultValue(0);
            e.Property(x => x.FoundedDate).HasColumnType("date");
            e.Property(x => x.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
            e.Property(x => x.UpdatedAt).HasColumnType("datetime");
            e.Property(x => x.ApprovedAt).HasColumnType("datetime");
            e.HasIndex(x => x.Slug).IsUnique();
            e.HasIndex(x => x.Status);

            e.HasOne(x => x.Category).WithMany(c => c.Projects)
                .HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Faculty).WithMany(f => f.Projects)
                .HasForeignKey(x => x.FacultyId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Owner).WithMany(a => a.OwnedProjects)
                .HasForeignKey(x => x.OwnerId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Approver).WithMany()
                .HasForeignKey(x => x.ApprovedBy).OnDelete(DeleteBehavior.ClientSetNull);
        });

        // ===== ProjectImage =====
        modelBuilder.Entity<ProjectImage>(e =>
        {
            e.ToTable("ProjectImage");
            e.HasKey(x => x.ImageId);
            e.Property(x => x.ImageUrl).HasMaxLength(255).IsRequired();
            e.Property(x => x.Caption).HasMaxLength(200);
            e.Property(x => x.DisplayOrder).HasDefaultValue(0);
            e.HasOne(x => x.Project).WithMany(p => p.Images)
                .HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
        });

        // ===== ProjectFeature =====
        modelBuilder.Entity<ProjectFeature>(e =>
        {
            e.ToTable("ProjectFeature");
            e.HasKey(x => x.FeatureId);
            e.Property(x => x.Title).HasMaxLength(150).IsRequired();
            e.Property(x => x.Description).HasMaxLength(500);
            e.Property(x => x.Icon).HasMaxLength(100);
            e.Property(x => x.DisplayOrder).HasDefaultValue(0);
            e.HasOne(x => x.Project).WithMany(p => p.Features)
                .HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
        });

        // ===== ProjectVersion =====
        modelBuilder.Entity<ProjectVersion>(e =>
        {
            e.ToTable("ProjectVersion");
            e.HasKey(x => x.VersionId);
            e.Property(x => x.VersionName).HasMaxLength(100).IsRequired();
            e.Property(x => x.Platform).HasMaxLength(100);
            e.Property(x => x.ReleaseDate).HasColumnType("date");
            e.Property(x => x.ChangeLog); // nvarchar(MAX)
            e.Property(x => x.DownloadUrl).HasMaxLength(255);
            e.Property(x => x.IsPublic).HasDefaultValue(true).HasSentinel(true);
            e.HasOne(x => x.Project).WithMany(p => p.Versions)
                .HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
        });

        // ===== TeamMember =====
        modelBuilder.Entity<TeamMember>(e =>
        {
            e.ToTable("TeamMember");
            e.HasKey(x => x.MemberId);
            e.Property(x => x.FullName).HasMaxLength(100).IsRequired();
            e.Property(x => x.Position).HasMaxLength(100);
            e.Property(x => x.StudentCode).HasMaxLength(20).IsUnicode(false);
            e.Property(x => x.ClassName).HasMaxLength(50);
            e.Property(x => x.Avatar).HasMaxLength(255);
            e.Property(x => x.Email).HasMaxLength(100).IsUnicode(false);
            e.Property(x => x.DisplayOrder).HasDefaultValue(0);
            e.HasOne(x => x.Project).WithMany(p => p.TeamMembers)
                .HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Account).WithMany()
                .HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.ClientSetNull);
        });

        // ===== Comment =====
        modelBuilder.Entity<Comment>(e =>
        {
            e.ToTable("Comment", t => t.HasCheckConstraint("CK_Comment_Rating", "[Rating] IS NULL OR [Rating] BETWEEN 1 AND 5"));
            e.HasKey(x => x.CommentId);
            e.Property(x => x.Content).HasMaxLength(1000).IsRequired();
            e.Property(x => x.Rating).HasColumnType("tinyint");
            e.Property(x => x.IsHidden).HasDefaultValue(false);
            e.Property(x => x.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
            e.HasOne(x => x.Project).WithMany(p => p.Comments)
                .HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Account).WithMany(a => a.Comments)
                .HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.Restrict);
            // SQL Server không cho phép cascade vòng trên cùng bảng → xóa câu trả lời ở tầng ứng dụng
            e.HasOne(x => x.Parent).WithMany(c => c.Replies)
                .HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.ClientCascade);
        });

        // ===== Vote =====
        modelBuilder.Entity<Vote>(e =>
        {
            e.ToTable("Vote");
            e.HasKey(x => new { x.ProjectId, x.AccountId });
            e.Property(x => x.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
            e.HasOne(x => x.Project).WithMany(p => p.Votes)
                .HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Account).WithMany(a => a.Votes)
                .HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.Restrict);
        });

        // ===== ContactRequest =====
        modelBuilder.Entity<ContactRequest>(e =>
        {
            e.ToTable("ContactRequest");
            e.HasKey(x => x.ContactId);
            e.Property(x => x.FullName).HasMaxLength(100).IsRequired();
            e.Property(x => x.Email).HasMaxLength(100).IsUnicode(false).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(15).IsUnicode(false);
            e.Property(x => x.Organization).HasMaxLength(150);
            e.Property(x => x.RequestType).HasColumnType("tinyint");
            e.Property(x => x.Message).HasMaxLength(2000).IsRequired();
            e.Property(x => x.Status).HasColumnType("tinyint").HasDefaultValue(ContactStatus.Moi);
            e.Property(x => x.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
            // Xóa dự án vẫn giữ lại yêu cầu liên hệ (ProjectId → NULL)
            e.HasOne(x => x.Project).WithMany(p => p.ContactRequests)
                .HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(x => x.Handler).WithMany()
                .HasForeignKey(x => x.HandledBy).OnDelete(DeleteBehavior.ClientSetNull);
        });

        // ===== News =====
        modelBuilder.Entity<News>(e =>
        {
            e.ToTable("News");
            e.HasKey(x => x.NewsId);
            e.Property(x => x.Title).HasMaxLength(200).IsRequired();
            e.Property(x => x.Slug).HasMaxLength(220).IsUnicode(false).IsRequired();
            e.Property(x => x.Thumbnail).HasMaxLength(255);
            e.Property(x => x.Summary).HasMaxLength(500);
            e.Property(x => x.Content).IsRequired(); // nvarchar(MAX)
            e.Property(x => x.NewsType).HasColumnType("tinyint").HasDefaultValue(NewsType.TinTuc).HasSentinel((NewsType)0);
            e.Property(x => x.EventDate).HasColumnType("datetime");
            e.Property(x => x.Location).HasMaxLength(200);
            e.Property(x => x.IsPublished).HasDefaultValue(false);
            e.Property(x => x.ViewCount).HasDefaultValue(0);
            e.Property(x => x.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
            e.Property(x => x.UpdatedAt).HasColumnType("datetime");
            e.HasIndex(x => x.Slug).IsUnique();
            e.HasOne(x => x.Author).WithMany()
                .HasForeignKey(x => x.AuthorId).OnDelete(DeleteBehavior.Restrict);
        });

        // ===== Banner =====
        modelBuilder.Entity<Banner>(e =>
        {
            e.ToTable("Banner");
            e.HasKey(x => x.BannerId);
            e.Property(x => x.Title).HasMaxLength(150);
            e.Property(x => x.ImageUrl).HasMaxLength(255).IsRequired();
            e.Property(x => x.LinkUrl).HasMaxLength(255);
            e.Property(x => x.DisplayOrder).HasDefaultValue(0);
            e.Property(x => x.IsActive).HasDefaultValue(true).HasSentinel(true);
        });
    }
}
