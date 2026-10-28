using Microsoft.EntityFrameworkCore;
using TVUStartup.Web.Models.Entities;
using TVUStartup.Web.Models.ViewModels;

namespace TVUStartup.Web.Data;

/// <summary>Các truy vấn dùng chung cho dự án</summary>
public static class ProjectQueries
{
    /// <summary>Chỉ lấy dự án đã được duyệt (hiển thị công khai)</summary>
    public static IQueryable<Project> Published(this IQueryable<Project> query) =>
        query.Where(p => p.Status == ProjectStatus.DaDuyet);

    /// <summary>Tìm kiếm không phân biệt hoa thường và dấu tiếng Việt ("khoi nghiep" khớp "Khởi nghiệp")</summary>
    public static IQueryable<Project> Search(this IQueryable<Project> query, string keyword)
    {
        var pattern = $"%{keyword.Trim()}%";
        const string collation = "Vietnamese_CI_AI";
        return query.Where(p =>
            EF.Functions.Like(EF.Functions.Collate(p.ProjectName, collation), pattern) ||
            EF.Functions.Like(EF.Functions.Collate(p.Slogan!, collation), pattern) ||
            EF.Functions.Like(EF.Functions.Collate(p.ShortDescription, collation), pattern));
    }

    /// <summary>Chuyển sang ViewModel thẻ dự án (EF chỉ SELECT các cột cần thiết)</summary>
    public static IQueryable<ProjectCardViewModel> ToCards(this IQueryable<Project> query) =>
        query.Select(p => new ProjectCardViewModel
        {
            ProjectId = p.ProjectId,
            ProjectName = p.ProjectName,
            Slug = p.Slug,
            Slogan = p.Slogan,
            ShortDescription = p.ShortDescription,
            Logo = p.Logo,
            CoverImage = p.CoverImage,
            CategoryName = p.Category.CategoryName,
            CategoryIcon = p.Category.Icon,
            FacultyName = p.Faculty.FacultyName,
            Stage = p.Stage,
            IsFeatured = p.IsFeatured,
            ViewCount = p.ViewCount,
            VoteCount = p.Votes.Count
        });
}
