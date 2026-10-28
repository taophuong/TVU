# PHỤ LỤC

## Phụ lục 1. Hướng dẫn cài đặt và chạy chương trình

Yêu cầu: .NET SDK 8.0 trở lên; Microsoft SQL Server 2019 trở lên (bản Express hoặc LocalDB).

1. Tải mã nguồn từ GitHub: `git clone https://github.com/taophuong/TVU.git`.
2. Mở tệp `TVUStartup.Web/appsettings.json`, sửa chuỗi kết nối `DefaultConnection` cho phù hợp với máy (mặc định dùng instance `.\SQLEXPRESS`, xác thực Windows).
3. Tạo cơ sở dữ liệu theo một trong các cách: (a) chạy ứng dụng, hệ thống tự áp dụng migration và nạp dữ liệu mẫu; (b) chạy lệnh `dotnet tool restore` rồi `dotnet ef database update` trong thư mục `TVUStartup.Web`; (c) chạy script `database/TVUStartupDB_schema.sql` bằng SQL Server Management Studio.
4. Chạy ứng dụng: `dotnet run --project TVUStartup.Web` (hoặc mở `TVUStartup.sln` bằng Visual Studio 2022 và nhấn F5), sau đó mở trình duyệt theo địa chỉ hiển thị trong cửa sổ lệnh.
5. Tạo lại dữ liệu mẫu từ đầu: chạy `dotnet ef database drop --force` rồi chạy lại ứng dụng.
6. Kiểm thử tự động (khi website đang chạy ở cổng 5080): `python -X utf8 tests/kiem_thu_nhanh.py`.

## Phụ lục 2. Tài khoản demo

Bảng PL.1. Tài khoản dùng để chạy thử hệ thống

| Vai trò | Email | Mật khẩu |
|---|---|---|
| Quản trị viên | admin@tvu.edu.vn | Admin@123 |
| Sinh viên (chủ dự án Zalord) | 170125077@st.tvu.edu.vn | SinhVien@123 |
| Sinh viên mẫu khác | 110122001@st.tvu.edu.vn, 111122015@st.tvu.edu.vn, … | SinhVien@123 |

## Phụ lục 3. Sản phẩm nộp kèm

Bảng PL.2. Danh sách sản phẩm nộp kèm báo cáo

| Sản phẩm | Vị trí / liên kết |
|---|---|
| Mã nguồn, script cơ sở dữ liệu, tài liệu, kịch bản kiểm thử | https://github.com/taophuong/TVU |
| Thư mục nộp bài trên Google Drive (báo cáo, slide, video demo, mã nguồn) | [chèn liên kết rút gọn] |
| Video demo sản phẩm (khoảng 10 phút) | [chèn liên kết] |
| Slide thuyết trình kèm bản ghi | [chèn liên kết] |

## Phụ lục 4. Khai báo sử dụng công cụ AI

Theo quy định về liêm chính học thuật của học phần, việc sử dụng công cụ trí tuệ nhân tạo trong đồ án được khai báo như sau.

Bảng PL.3. Khai báo sử dụng công cụ AI

| Công cụ | Công việc được hỗ trợ | Trách nhiệm của sinh viên |
|---|---|---|
| Claude (Anthropic) | Gợi ý cấu trúc mã nguồn và viết mã cho các chức năng; thiết kế giao diện; tạo dữ liệu mẫu; viết kịch bản kiểm thử tự động; vẽ sơ đồ UML, ERD, wireframe; soạn thảo bản nháp nội dung các chương báo cáo | Đặt yêu cầu, quyết định phạm vi và chức năng; đọc hiểu, chạy thử, kiểm tra và chỉnh sửa toàn bộ mã nguồn; rà soát, chỉnh sửa nội dung báo cáo; chịu trách nhiệm về sản phẩm cuối cùng |
