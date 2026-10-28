# TVU Startup – Website giới thiệu sản phẩm khởi nghiệp của Sinh viên Trường Đại học Trà Vinh

Đồ án **Chuyên đề ASP.NET** – Sinh viên thực hiện: **Lê Văn Bình Phương** – MSSV **170125077** – Lớp **DK25TTG2**.

Website là nơi sinh viên Trường Đại học Trà Vinh đăng và giới thiệu dự án khởi nghiệp. Ban quản trị duyệt dự án trước khi công khai. Khách truy cập (doanh nghiệp, nhà đầu tư, cộng đồng) có thể tìm kiếm dự án, bình luận, bình chọn và gửi yêu cầu hợp tác.

> Ngoài dự án **Zalord** (sản phẩm thật của sinh viên thực hiện đồ án), các sinh viên, dự án, bình luận, tin tức… đều là **dữ liệu mẫu (hư cấu)** để minh họa.

## 1. Công nghệ & phiên bản

| Thành phần | Phiên bản |
|---|---|
| .NET SDK | 8.0 (đã thử với 8.0.419) |
| ASP.NET Core MVC | 8.0 |
| Entity Framework Core (SqlServer, Tools, Design) | 8.0.10 |
| SQL Server | 2022 Express (instance `.\SQLEXPRESS`) – dùng được với SQL Server 2019 trở lên hoặc LocalDB |
| HtmlSanitizer | 8.1.870 (lọc HTML chống XSS) |
| Giao diện | Bootstrap 5.1, jQuery, Bootstrap Icons 1.11.3, Quill 1.3.7, Chart.js 4.4.1 |

## 2. Cấu trúc thư mục

```
PkuongProject/
├── TVUStartup.sln
├── TVUStartup.Web/                 # Dự án ASP.NET Core MVC
│   ├── Areas/Admin/                # Khu vực quản trị (/quan-tri)
│   ├── Controllers/                # Trang người dùng: Home, Projects, News, Contact, Account, MyProjects
│   ├── Data/                       # AppDbContext, Migrations, DbSeeder (dữ liệu mẫu), truy vấn dùng chung
│   ├── Helpers/                    # Hiển thị nhãn tiếng Việt, tạo slug, đọc thông tin đăng nhập
│   ├── Models/Entities/            # 14 lớp thực thể ↔ 14 bảng
│   ├── Models/ViewModels/          # Dữ liệu cho View và form (kèm kiểm tra hợp lệ)
│   ├── Services/                   # Lưu ảnh tải lên, quy tắc trạng thái dự án
│   ├── Views/                      # Giao diện Razor
│   └── wwwroot/                    # CSS, JS, ảnh (uploads/sample: ảnh dữ liệu mẫu)
├── database/TVUStartupDB_schema.sql  # Script tạo CSDL (sinh từ migration)
├── docs/                           # Thiết kế CSDL, phân tích yêu cầu, tư liệu Chương 3 & 5
│   └── diagrams/                   # Sơ đồ UML, ERD, kiến trúc, wireframe (PNG) + mã nguồn sơ đồ (src/)
└── tests/kiem_thu_nhanh.py         # Kiểm thử tự động các luồng chính (62 ca)
```

## 3. Cài đặt và chạy

### Bước 1 – Chuỗi kết nối
Mở `TVUStartup.Web/appsettings.json`, sửa `DefaultConnection` cho đúng máy của bạn:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.\\SQLEXPRESS;Database=TVUStartupDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```
Dùng LocalDB: `Server=(localdb)\\MSSQLLocalDB;Database=TVUStartupDB;Trusted_Connection=True;MultipleActiveResultSets=true`

### Bước 2 – Tạo cơ sở dữ liệu (chọn 1 trong 3 cách)

**Cách 1 (đơn giản nhất):** chỉ cần chạy dự án. Khi khởi động, chương trình tự áp dụng migration và nạp dữ liệu mẫu nếu CSDL còn trống (`Data/DbSeeder.cs`).

**Cách 2 – dùng lệnh EF Core:**
```bash
cd TVUStartup.Web
dotnet tool restore            # cài dotnet-ef theo .config/dotnet-tools.json
dotnet ef database update
```

**Cách 3 – dùng script SQL:** tạo CSDL rỗng tên `TVUStartupDB`, chạy `database/TVUStartupDB_schema.sql` trong SSMS. Dữ liệu mẫu được nạp ở lần chạy đầu tiên.

### Bước 3 – Chạy dự án
```bash
cd PkuongProject
dotnet run --project TVUStartup.Web
```
Mở trình duyệt tại địa chỉ hiển thị trong cửa sổ lệnh (mặc định `http://localhost:5xxx`) hoặc chạy bằng Visual Studio 2022 (mở `TVUStartup.sln`, nhấn F5).

### Tạo lại dữ liệu mẫu từ đầu
```bash
cd TVUStartup.Web
dotnet ef database drop --force
dotnet run
```

## 4. Tài khoản demo

| Vai trò | Email | Mật khẩu |
|---|---|---|
| Quản trị viên | `admin@tvu.edu.vn` | `Admin@123` |
| Sinh viên (chủ dự án Zalord) | `170125077@st.tvu.edu.vn` | `SinhVien@123` |
| Sinh viên mẫu khác | `110122001@st.tvu.edu.vn`, `111122015@st.tvu.edu.vn`, … | `SinhVien@123` |

## 5. Chức năng chính

**Khách:** trang chủ (thống kê, dự án nổi bật, lĩnh vực, banner, tin tức) · tìm kiếm không phân biệt dấu, lọc theo lĩnh vực / khoa / giai đoạn, sắp xếp, phân trang · chi tiết dự án · tin tức, sự kiện · gửi liên hệ hợp tác · đăng ký.

**Sinh viên:** đăng nhập / đăng xuất · hồ sơ, ảnh đại diện, đổi mật khẩu · đăng dự án (trình soạn thảo, logo, ảnh bìa) · quản lý hình ảnh, tính năng, phiên bản (công khai / nội bộ), thành viên · gửi duyệt, rút lại · bình luận, chấm sao, bình chọn (AJAX) · trả lời bình luận trên dự án của mình · xem và cập nhật trạng thái liên hệ gửi tới dự án.

**Quản trị viên** (`/quan-tri`): dashboard thống kê + biểu đồ · duyệt / từ chối (có lý do) / ẩn / đánh dấu nổi bật / xóa dự án · quản lý lĩnh vực, khoa, tin tức, banner · kiểm duyệt bình luận · xử lý liên hệ · khóa / mở khóa tài khoản, đặt lại mật khẩu.

**Vòng đời dự án:** Nháp → Chờ duyệt → Đã duyệt ⇄ Ẩn; Chờ duyệt → Từ chối → (sửa) → Chờ duyệt; Chờ duyệt → (rút lại) → Nháp.

## 6. Bảo mật đã áp dụng
- Mật khẩu băm bằng `PasswordHasher` (PBKDF2 + salt) của ASP.NET Core.
- Xác thực Cookie, phân quyền `[Authorize(Roles = ...)]`; tài khoản bị khóa bị đăng xuất ở request kế tiếp.
- Chống CSRF: `[ValidateAntiForgeryToken]` / `[AutoValidateAntiforgeryToken]` cho mọi form POST.
- Chống SQL Injection: truy vấn qua EF Core / LINQ (tham số hóa).
- Chống XSS: Razor tự mã hóa đầu ra; nội dung HTML (giới thiệu dự án, tin tức) được lọc bằng HtmlSanitizer trước khi lưu.
- Tải ảnh: chỉ nhận .jpg/.png/.webp ≤ 2 MB, kiểm tra chữ ký tệp (magic number), đổi tên ngẫu nhiên.
- Sinh viên chỉ thao tác được trên dự án của chính mình.

## 7. Kiểm thử nhanh
Khi web đang chạy ở `http://localhost:5080`:
```bash
dotnet run --project TVUStartup.Web --urls http://localhost:5080
python -X utf8 tests/kiem_thu_nhanh.py
```
Script chạy 62 ca kiểm thử (khách, sinh viên, quản trị, bảo mật) và in kết quả PASS/FAIL. Script có tạo thêm dữ liệu thử (bình luận, liên hệ, tài khoản) – tạo lại dữ liệu mẫu theo mục 3 nếu cần.

## 8. Khai báo sử dụng AI
Đồ án có sử dụng công cụ AI (Claude) hỗ trợ: gợi ý cấu trúc mã nguồn, giao diện, dữ liệu mẫu và kịch bản kiểm thử. Sinh viên đã đọc hiểu, chỉnh sửa và chịu trách nhiệm toàn bộ mã nguồn.
