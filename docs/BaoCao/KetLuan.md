# KẾT LUẬN

## Kết quả đạt được

Đồ án đã xây dựng thành công website giới thiệu các sản phẩm khởi nghiệp của sinh viên Trường Đại học Trà Vinh trên nền tảng ASP.NET Core MVC (.NET 8), Entity Framework Core 8 và SQL Server. Website phục vụ ba nhóm người dùng với 24 chức năng: khách tìm kiếm, xem dự án, gửi liên hệ hợp tác; sinh viên đăng và quản lý dự án, bình luận, đánh giá, bình chọn; quản trị viên kiểm duyệt dự án, quản lý danh mục, tài khoản, nội dung và theo dõi thống kê. Hệ thống có quy trình kiểm duyệt rõ ràng với năm trạng thái của dự án, áp dụng nhiều lớp bảo mật (băm mật khẩu PBKDF2, phân quyền theo vai trò, chống SQL Injection, XSS, CSRF, kiểm tra tệp tải lên) và giao diện tương thích với điện thoại. Kết quả kiểm thử 44 test case (62 lần kiểm tra tự động) đều đạt.

Song song với sản phẩm, đồ án đã hoàn thành đầy đủ hồ sơ phân tích, thiết kế: sơ đồ use case, đặc tả use case, ERD và mô tả 14 bảng dữ liệu, sơ đồ tuần tự, trạng thái, hoạt động, kiến trúc hệ thống và wireframe các màn hình chính; mã nguồn được quản lý trên GitHub kèm hướng dẫn cài đặt, script cơ sở dữ liệu và bộ kiểm thử tự động.

## Đóng góp của đồ án

- **Đối với sinh viên và nhà trường:** cung cấp một kênh tập trung, có kiểm duyệt để sinh viên các khoa giới thiệu sản phẩm khởi nghiệp một cách chuyên nghiệp; giúp nhà trường quản lý, thống kê và quảng bá phong trào khởi nghiệp của sinh viên.
- **Đối với doanh nghiệp, nhà đầu tư, cộng đồng:** dễ dàng tìm kiếm dự án theo lĩnh vực, khoa, giai đoạn và liên hệ hợp tác trực tiếp với nhóm sinh viên.
- **Đối với bản thân người thực hiện:** vận dụng tổng hợp kiến thức về ASP.NET Core MVC, Entity Framework Core, LINQ, Dependency Injection, xác thực – phân quyền và bảo mật ứng dụng web vào một sản phẩm hoàn chỉnh; rèn luyện kỹ năng phân tích thiết kế, kiểm thử và viết báo cáo kỹ thuật; đồng thời có thêm kênh giới thiệu sản phẩm Zalord của chính mình.

## Hướng phát triển

Trong thời gian tới, đồ án có thể được tiếp tục phát triển theo các hướng: tích hợp gửi email thông báo và khôi phục mật khẩu; đăng nhập một lần bằng tài khoản email của Trường; triển khai lên máy chủ Internet (IIS hoặc Docker) với tên miền và HTTPS; nâng cấp lên phiên bản .NET 10 LTS; xây dựng Web API phục vụ ứng dụng di động; mở rộng chức năng quản lý cuộc thi khởi nghiệp (nộp hồ sơ, chấm điểm, công bố kết quả) và thu thập dữ liệu dự án thật của sinh viên các khoa để đưa website vào sử dụng thực tế.
