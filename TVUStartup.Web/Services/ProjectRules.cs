using TVUStartup.Web.Models.Entities;

namespace TVUStartup.Web.Services;

/// <summary>
/// Quy tắc nghiệp vụ theo trạng thái dự án (vòng đời):
/// Nháp → (gửi duyệt) → Chờ duyệt → (Admin duyệt) → Đã duyệt ⇄ Ẩn
///                       Chờ duyệt → (Admin từ chối) → Từ chối → (sửa, gửi lại) → Chờ duyệt
///                       Chờ duyệt → (sinh viên rút lại) → Nháp
/// </summary>
public static class ProjectRules
{
    /// <summary>Sinh viên được sửa khi dự án ở trạng thái Nháp, Từ chối hoặc Đã duyệt</summary>
    public static bool CanEdit(ProjectStatus status) =>
        status is ProjectStatus.Nhap or ProjectStatus.TuChoi or ProjectStatus.DaDuyet;

    /// <summary>Sinh viên chỉ được xóa dự án chưa từng công khai</summary>
    public static bool CanDelete(ProjectStatus status) =>
        status is ProjectStatus.Nhap or ProjectStatus.TuChoi;

    public static bool CanSubmit(ProjectStatus status) =>
        status is ProjectStatus.Nhap or ProjectStatus.TuChoi;

    public static bool CanWithdraw(ProjectStatus status) =>
        status == ProjectStatus.ChoDuyet;

    /// <summary>Danh sách biểu tượng (Bootstrap Icons) cho tính năng sản phẩm</summary>
    public static readonly (string Icon, string Label)[] FeatureIcons =
    [
        ("bi-check2-circle", "Dấu tích"), ("bi-lightning-charge", "Tia sét"), ("bi-star", "Ngôi sao"),
        ("bi-shield-check", "Bảo mật"), ("bi-phone", "Điện thoại"), ("bi-graph-up", "Biểu đồ"),
        ("bi-people", "Cộng đồng"), ("bi-bell", "Thông báo"), ("bi-cpu", "Phần cứng"),
        ("bi-palette", "Thiết kế"), ("bi-flower1", "Tự nhiên"), ("bi-heart-pulse", "Sức khỏe"),
        ("bi-cash-coin", "Tiết kiệm"), ("bi-globe", "Toàn cầu"), ("bi-gear", "Cài đặt"), ("bi-search", "Tìm kiếm")
    ];
}
