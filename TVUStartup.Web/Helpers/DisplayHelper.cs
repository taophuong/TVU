using System.Text.RegularExpressions;
using TVUStartup.Web.Models.Entities;

namespace TVUStartup.Web.Helpers;

/// <summary>Chuyển giá trị enum / dữ liệu thô thành nội dung hiển thị tiếng Việt</summary>
public static partial class DisplayHelper
{
    public static string ToLabel(this ProjectStage stage) => stage switch
    {
        ProjectStage.YTuong => "Ý tưởng",
        ProjectStage.NguyenMau => "Nguyên mẫu (MVP)",
        ProjectStage.DaRaMat => "Đã ra mắt",
        ProjectStage.DangTangTruong => "Đang tăng trưởng",
        _ => "Không xác định"
    };

    public static string ToBadgeClass(this ProjectStage stage) => stage switch
    {
        ProjectStage.YTuong => "stage-idea",
        ProjectStage.NguyenMau => "stage-mvp",
        ProjectStage.DaRaMat => "stage-launched",
        ProjectStage.DangTangTruong => "stage-growth",
        _ => "stage-idea"
    };

    public static string ToLabel(this ProjectStatus status) => status switch
    {
        ProjectStatus.Nhap => "Nháp",
        ProjectStatus.ChoDuyet => "Chờ duyệt",
        ProjectStatus.DaDuyet => "Đã duyệt",
        ProjectStatus.TuChoi => "Bị từ chối",
        ProjectStatus.An => "Ẩn",
        _ => "Không xác định"
    };

    public static string ToBadgeClass(this ProjectStatus status) => status switch
    {
        ProjectStatus.Nhap => "text-bg-secondary",
        ProjectStatus.ChoDuyet => "text-bg-warning",
        ProjectStatus.DaDuyet => "text-bg-success",
        ProjectStatus.TuChoi => "text-bg-danger",
        _ => "text-bg-dark"
    };

    public static string ToLabel(this RequestType type) => type switch
    {
        RequestType.HopTac => "Hợp tác",
        RequestType.DauTu => "Đầu tư",
        RequestType.SuDungSanPham => "Sử dụng sản phẩm",
        _ => "Khác"
    };

    public static string ToLabel(this ContactStatus status) => status switch
    {
        ContactStatus.Moi => "Mới",
        ContactStatus.DaLienHe => "Đã liên hệ",
        ContactStatus.DaDong => "Đã đóng",
        _ => "Không xác định"
    };

    public static string ToBadgeClass(this ContactStatus status) => status switch
    {
        ContactStatus.Moi => "text-bg-primary",
        ContactStatus.DaLienHe => "text-bg-success",
        _ => "text-bg-secondary"
    };

    public static string ToLabel(this NewsType type) => type switch
    {
        NewsType.TinTuc => "Tin tức",
        NewsType.SuKien => "Sự kiện",
        NewsType.CuocThi => "Cuộc thi",
        _ => "Khác"
    };

    public static string ToLabel(this AccountStatus status) => status switch
    {
        AccountStatus.HoatDong => "Hoạt động",
        AccountStatus.BiKhoa => "Bị khóa",
        _ => "Không xác định"
    };

    /// <summary>Tạo danh sách lựa chọn cho thẻ select từ một enum (giá trị là tên enum, hiển thị nhãn tiếng Việt)</summary>
    public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Options<TEnum>(Func<TEnum, string> label) where TEnum : struct, Enum =>
        Enum.GetValues<TEnum>()
            .Select(e => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(label(e), e.ToString()))
            .ToList();

    /// <summary>Chuyển link YouTube (watch / youtu.be / shorts) thành link nhúng iframe; trả về null nếu không nhận dạng được</summary>
    public static string? ToYouTubeEmbedUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return null;
        var match = YouTubeIdRegex().Match(url);
        return match.Success ? $"https://www.youtube.com/embed/{match.Groups[1].Value}" : null;
    }

    /// <summary>Hiển thị số gọn: 1200 → 1,2K</summary>
    public static string ToCompact(this int number) => number switch
    {
        >= 1_000_000 => $"{number / 1_000_000d:0.#}M",
        >= 1_000 => $"{number / 1_000d:0.#}K",
        _ => number.ToString()
    };

    /// <summary>Lấy chữ cái đầu của tên để làm avatar mặc định: "Lê Văn Bình Phương" → "P"</summary>
    public static string Initial(string fullName)
    {
        var last = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
        return string.IsNullOrEmpty(last) ? "?" : last[..1].ToUpperInvariant();
    }

    [GeneratedRegex(@"(?:youtube\.com/(?:watch\?v=|embed/|shorts/)|youtu\.be/)([A-Za-z0-9_-]{11})")]
    private static partial Regex YouTubeIdRegex();
}
