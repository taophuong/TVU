# Phân tích yêu cầu – Website giới thiệu sản phẩm khởi nghiệp SV TVU

> Tài liệu làm việc cho Chương 3 (mục 3.1, 3.3, 3.4) theo *Hướng dẫn đồ án ASP.NET 28.10.2026*.
> Phạm vi đã được GVHD xác nhận: **website chung** – nhiều sinh viên đăng dự án, quản trị viên duyệt; dùng **dữ liệu mẫu**; ASP.NET Core MVC (.NET 8) + SQL Server.

## 1. Tác nhân (Actor)

| Tác nhân | Mô tả |
|---|---|
| Khách | Người truy cập chưa đăng nhập: nhà đầu tư, doanh nghiệp, giảng viên, sinh viên khác… |
| Sinh viên | Sinh viên TVU có tài khoản; đăng và quản lý dự án khởi nghiệp của mình. Kế thừa mọi chức năng của Khách. |
| Quản trị viên | Cán bộ quản lý (Trung tâm/Phòng hỗ trợ khởi nghiệp); duyệt dự án và quản lý dữ liệu hệ thống. |

## 2. Yêu cầu chức năng

### 2.1. Khách
| Mã | Use case | Ghi chú |
|---|---|---|
| UC01 | Xem trang chủ | Thống kê, dự án nổi bật, lĩnh vực, dự án mới, banner |
| UC02 | Tìm kiếm, lọc dự án | Từ khóa (không phân biệt dấu), lĩnh vực, khoa, giai đoạn; sắp xếp; phân trang |
| UC03 | Xem chi tiết dự án | Giới thiệu, tính năng, hình ảnh, phiên bản công khai, đội ngũ, bình luận |
| UC04 | Xem tin tức / sự kiện / cuộc thi | |
| UC05 | Gửi yêu cầu liên hệ hợp tác | Hợp tác / đầu tư / sử dụng sản phẩm; không cần đăng nhập |
| UC06 | Đăng ký tài khoản | |

### 2.2. Sinh viên
| Mã | Use case | Ghi chú |
|---|---|---|
| UC07 | Đăng nhập / đăng xuất | Dùng chung với Quản trị viên |
| UC08 | Cập nhật hồ sơ, đổi mật khẩu | |
| UC09 | Quản lý dự án của tôi | Thêm / sửa / xóa khi ở trạng thái Nháp hoặc Từ chối |
| UC10 | Quản lý hình ảnh, tính năng, phiên bản, thành viên của dự án | |
| UC11 | Gửi dự án chờ duyệt / rút lại | Nháp, Từ chối → Chờ duyệt (cần ảnh bìa + ≥ 1 tính năng); Chờ duyệt → Nháp |
| UC12 | Bình luận, đánh giá sao | Mỗi tài khoản chấm sao 1 lần / dự án; chủ dự án được trả lời |
| UC13 | Bình chọn (thích) dự án | Bật / tắt; mỗi tài khoản 1 phiếu / dự án |
| UC14 | Xem yêu cầu liên hệ gửi đến dự án của tôi | |

### 2.3. Quản trị viên
| Mã | Use case | Ghi chú |
|---|---|---|
| UC15 | Xem bảng thống kê (dashboard) | Số dự án theo trạng thái / lĩnh vực / khoa, lượt xem, liên hệ mới |
| UC16 | Duyệt / từ chối / ẩn dự án | Từ chối phải nhập lý do |
| UC17 | Đánh dấu dự án nổi bật | |
| UC18 | Quản lý lĩnh vực | CRUD |
| UC19 | Quản lý khoa | CRUD |
| UC20 | Quản lý tài khoản | Khóa / mở khóa, đặt lại mật khẩu |
| UC21 | Quản lý tin tức, sự kiện | CRUD |
| UC22 | Quản lý banner | CRUD |
| UC23 | Kiểm duyệt bình luận | Ẩn / hiện |
| UC24 | Xử lý yêu cầu liên hệ | Mới → Đã liên hệ → Đã đóng |

### 2.4. Ngoài phạm vi (đưa vào "Hướng phát triển")
- Quên mật khẩu qua email, xác thực email khi đăng ký (cần máy chủ SMTP).
- Đăng nhập bằng tài khoản Google / Microsoft của Trường.
- API cho ứng dụng di động (JWT).

## 3. Yêu cầu phi chức năng

| Nhóm | Yêu cầu | Cách đáp ứng |
|---|---|---|
| Bảo mật | Mật khẩu không lưu dạng rõ | `PasswordHasher` (PBKDF2 + salt) của ASP.NET Core |
| | Phân quyền theo vai trò | Cookie Authentication + `[Authorize(Roles = "...")]` |
| | Chống SQL Injection | Truy vấn qua EF Core / LINQ (tham số hóa) |
| | Chống XSS | Razor tự mã hóa HTML; lọc HTML phần giới thiệu dự án trước khi lưu |
| | Chống CSRF | `[ValidateAntiForgeryToken]` trên mọi form POST |
| | Tải tệp an toàn | Chỉ nhận ảnh .jpg/.png/.webp, giới hạn dung lượng, đổi tên tệp |
| Hiệu năng | Trang danh sách tải nhanh | Phân trang, `AsNoTracking`, chỉ lấy cột cần thiết (projection) |
| Khả dụng | Dùng tốt trên điện thoại | Bootstrap 5, giao diện responsive |
| | Tiếng Việt | Tìm kiếm không phân biệt dấu (collation `Vietnamese_CI_AI`) |
| Bảo trì | Dễ cài đặt lại | EF Core Migration + dữ liệu mẫu tự nạp khi chạy lần đầu |

## 4. Đặc tả use case chính (sẽ viết chi tiết trong báo cáo)
UC07 Đăng nhập · UC09/UC11 Đăng dự án và gửi duyệt · UC16 Duyệt dự án · UC02 Tìm kiếm, lọc · UC12 Bình luận, đánh giá · UC05 Gửi liên hệ hợp tác.

Mẫu đặc tả: Tên – Tác nhân – Mô tả – Tiền điều kiện – Luồng chính – Luồng thay thế / ngoại lệ – Hậu điều kiện.

## 5. Sơ đồ cần vẽ (mục 3.3)

| Loại sơ đồ | Chức năng |
|---|---|
| Use case tổng quát | Toàn hệ thống (3 tác nhân) |
| Tuần tự (sequence) | Đăng nhập; Đăng dự án và gửi duyệt; Duyệt dự án |
| Trạng thái (state) | Vòng đời dự án: Nháp → Chờ duyệt → Đã duyệt / Từ chối → Ẩn |
| Hoạt động (activity) | Quy trình từ lúc sinh viên đăng dự án đến khi được công khai; Gửi liên hệ hợp tác |

## 6. Kiến trúc (mục 3.4)
- Mô hình **MVC** của ASP.NET Core; khu vực quản trị tách riêng bằng **Area `Admin`**.
- Lớp dữ liệu: `AppDbContext` (EF Core) + các truy vấn dùng chung (`ProjectQueries`).
- Lớp dịch vụ: băm mật khẩu, lưu tệp tải lên, lọc HTML.
- Dependency Injection đăng ký trong `Program.cs`.
- Wireframe: Trang chủ, Danh sách dự án, Chi tiết dự án, Form đăng dự án, Trang duyệt dự án (Admin), Dashboard.
