namespace TVUStartup.Web.Models.Entities;

// Các enum được lưu xuống CSDL dưới dạng tinyint (byte)

/// <summary>Trạng thái tài khoản (Account.Status)</summary>
public enum AccountStatus : byte
{
    HoatDong = 1,
    BiKhoa = 2
}

/// <summary>Giai đoạn phát triển của dự án (Project.Stage)</summary>
public enum ProjectStage : byte
{
    YTuong = 1,
    NguyenMau = 2,
    DaRaMat = 3,
    DangTangTruong = 4
}

/// <summary>Trạng thái duyệt của dự án (Project.Status)</summary>
public enum ProjectStatus : byte
{
    Nhap = 0,
    ChoDuyet = 1,
    DaDuyet = 2,
    TuChoi = 3,
    An = 4
}

/// <summary>Loại yêu cầu liên hệ (ContactRequest.RequestType)</summary>
public enum RequestType : byte
{
    HopTac = 1,
    DauTu = 2,
    SuDungSanPham = 3,
    Khac = 4
}

/// <summary>Trạng thái xử lý yêu cầu liên hệ (ContactRequest.Status)</summary>
public enum ContactStatus : byte
{
    Moi = 0,
    DaLienHe = 1,
    DaDong = 2
}

/// <summary>Loại bài viết (News.NewsType)</summary>
public enum NewsType : byte
{
    TinTuc = 1,
    SuKien = 2,
    CuocThi = 3
}
