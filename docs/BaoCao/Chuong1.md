# CHƯƠNG 1. GIỚI THIỆU TỔNG QUAN

## 1.1. Lý do chọn đề tài

Trong những năm gần đây, khởi nghiệp đổi mới sáng tạo được Nhà nước và các cơ sở giáo dục đặc biệt quan tâm. Đề án “Hỗ trợ học sinh, sinh viên khởi nghiệp đến năm 2025” được Thủ tướng Chính phủ phê duyệt theo Quyết định số 1665/QĐ-TTg [1] đặt mục tiêu trang bị kiến thức, kỹ năng khởi nghiệp và tạo môi trường hỗ trợ để học sinh, sinh viên hình thành, phát triển các ý tưởng, dự án khởi nghiệp ngay trong thời gian học tập. Nhờ đó, phong trào khởi nghiệp trong các trường đại học ngày càng sôi nổi với nhiều cuộc thi ý tưởng, ngày hội khởi nghiệp và các sản phẩm do chính sinh viên làm ra.

Tại Trường Đại học Trà Vinh, sinh viên thuộc nhiều khoa có những ý tưởng và sản phẩm khởi nghiệp gắn với thế mạnh địa phương như nông nghiệp, thủy sản, chế biến nông sản, du lịch cộng đồng, văn hóa Khmer Nam Bộ, cũng như các sản phẩm công nghệ phần mềm. Tuy nhiên, các sản phẩm này thường chỉ được giới thiệu rời rạc qua mạng xã hội cá nhân, nhóm chat hoặc trong khuôn khổ các ngày hội, cuộc thi. Sau khi sự kiện kết thúc, thông tin về sản phẩm dễ bị thất lạc; doanh nghiệp, nhà đầu tư hay người dùng quan tâm cũng khó tìm được một nơi tập trung để tra cứu, so sánh và liên hệ trực tiếp với nhóm sinh viên.

Bản thân tác giả cũng là một sinh viên khởi nghiệp: sản phẩm **Zalord** – tiện ích mở rộng giúp cá nhân hóa giao diện và tăng quyền riêng tư cho Zalo PC và Zalo Web – được tác giả tự phát triển, chia sẻ tới cộng đồng người dùng và nhận nhiều phản hồi tích cực. Trong quá trình giới thiệu sản phẩm, tác giả nhận thấy rõ nhu cầu về một kênh chính thống, có kiểm duyệt của nhà trường để sinh viên trình bày sản phẩm một cách chuyên nghiệp, đầy đủ thông tin (tính năng, hình ảnh, phiên bản, đội ngũ) và để cộng đồng có thể phản hồi, kết nối.

Bên cạnh đó, học phần Chuyên đề ASP.NET yêu cầu sinh viên vận dụng kiến thức về nền tảng ASP.NET vào một bài toán thực tế. Một website giới thiệu sản phẩm khởi nghiệp có đủ các nghiệp vụ tiêu biểu của một hệ thống web hiện đại: quản lý người dùng và phân quyền, quy trình đăng – kiểm duyệt nội dung, tìm kiếm, tương tác cộng đồng, quản trị và thống kê. Đây là điều kiện tốt để áp dụng toàn diện ASP.NET Core MVC, Entity Framework Core và SQL Server.

Xuất phát từ những lý do trên, tác giả chọn đề tài **“Xây dựng website giới thiệu các sản phẩm khởi nghiệp của Sinh viên Trường Đại học Trà Vinh”**.

## 1.2. Mục tiêu của đồ án

### 1.2.1. Mục tiêu tổng quát

Xây dựng một website hoàn chỉnh trên nền tảng ASP.NET Core MVC, đóng vai trò là nơi giới thiệu tập trung các sản phẩm, dự án khởi nghiệp của sinh viên Trường Đại học Trà Vinh, có quy trình kiểm duyệt nội dung và kết nối các dự án với cộng đồng, doanh nghiệp, nhà đầu tư.

### 1.2.2. Mục tiêu cụ thể

Về sản phẩm, đồ án hướng tới các mục tiêu sau:

1. Xây dựng website giới thiệu tập trung các dự án khởi nghiệp của sinh viên, có trang chủ, trang danh sách và trang chi tiết dự án với đầy đủ thông tin: giới thiệu, tính năng, hình ảnh, phiên bản phát hành, đội ngũ thực hiện.
2. Cho phép nhiều sinh viên tự đăng ký tài khoản, đăng và quản lý dự án của mình.
3. Xây dựng quy trình kiểm duyệt: dự án chỉ được công khai sau khi Ban quản trị duyệt; trường hợp từ chối phải nêu rõ lý do để sinh viên chỉnh sửa.
4. Kết nối dự án với cộng đồng, doanh nghiệp, nhà đầu tư thông qua tìm kiếm, bình luận, đánh giá, bình chọn và gửi yêu cầu liên hệ hợp tác.
5. Hỗ trợ công tác quản trị: quản lý danh mục, tài khoản, tin tức, banner, bình luận, liên hệ và xem thống kê bằng biểu đồ.
6. Áp dụng các kỹ thuật của ASP.NET Core vào dự án thực tế: mô hình MVC, Area, Razor, Entity Framework Core (Code First, Migration), LINQ, Dependency Injection, xác thực và phân quyền.
7. Bảo đảm an toàn thông tin ở mức cần thiết cho một website công khai: bảo vệ mật khẩu, phân quyền, chống các lỗ hổng phổ biến như SQL Injection, XSS, CSRF.
8. Hướng tới triển khai thử nghiệm website lên môi trường Internet khi có điều kiện.

Về kiến thức và kỹ năng, thông qua đồ án tác giả rèn luyện khả năng phân tích, thiết kế hệ thống (use case, cơ sở dữ liệu, sơ đồ UML), lập trình web với ASP.NET Core, kiểm thử phần mềm và trình bày báo cáo kỹ thuật.

## 1.3. Phạm vi nghiên cứu và triển khai

**Đối tượng nghiên cứu:**

- Nghiệp vụ giới thiệu, kiểm duyệt và kết nối các dự án khởi nghiệp của sinh viên trong phạm vi một trường đại học.
- Nền tảng ASP.NET Core MVC (.NET 8), Entity Framework Core 8, hệ quản trị cơ sở dữ liệu Microsoft SQL Server và các kỹ thuật bảo mật ứng dụng web.

**Phạm vi chức năng:** hệ thống phục vụ ba nhóm người dùng với 24 chức năng (chi tiết ở Chương 3):

- **Khách:** xem trang chủ, tìm kiếm và lọc dự án, xem chi tiết dự án, xem tin tức, sự kiện, gửi liên hệ hợp tác, đăng ký tài khoản.
- **Sinh viên:** đăng nhập, cập nhật hồ sơ, đăng và quản lý dự án (hình ảnh, tính năng, phiên bản, thành viên), gửi duyệt, bình luận, đánh giá, bình chọn, theo dõi liên hệ hợp tác.
- **Quản trị viên:** xem thống kê, duyệt dự án, quản lý lĩnh vực, khoa, tài khoản, tin tức, banner, kiểm duyệt bình luận, xử lý liên hệ.

**Phạm vi dữ liệu:** theo định hướng của giảng viên hướng dẫn, hệ thống sử dụng dữ liệu mẫu để minh họa, gồm 7 khoa, 8 lĩnh vực, 8 tài khoản sinh viên và 13 dự án ở đủ các trạng thái. Trong đó, dự án Zalord là sản phẩm thật của tác giả; các sinh viên, dự án còn lại cùng bình luận, tin tức là dữ liệu hư cấu, được ghi chú rõ trên giao diện.

**Phạm vi triển khai:** website được cài đặt và vận hành trên máy tính cá nhân (Windows, SQL Server Express); mã nguồn được quản lý trên GitHub, kèm hướng dẫn cài đặt, script tạo cơ sở dữ liệu và dữ liệu mẫu tự nạp khi chạy lần đầu.

**Ngoài phạm vi đồ án:** gửi email (xác thực tài khoản, quên mật khẩu, thông báo), đăng nhập bằng tài khoản Google / Microsoft của Trường, ứng dụng di động, thanh toán trực tuyến. Các nội dung này được đề xuất trong hướng phát triển ở Chương 5.

## 1.4. Tóm tắt công nghệ sử dụng

Hệ thống được xây dựng theo mô hình MVC trên nền tảng ASP.NET Core [2], sử dụng Entity Framework Core để làm việc với cơ sở dữ liệu SQL Server. Các công nghệ chính được tóm tắt trong Bảng 1.1; cơ sở lý thuyết của từng công nghệ được trình bày ở Chương 2.

Bảng 1.1. Các công nghệ sử dụng trong đồ án

| Thành phần | Công nghệ | Phiên bản | Vai trò trong đồ án |
|---|---|---|---|
| Nền tảng | .NET SDK, ASP.NET Core MVC | 8.0 | Xây dựng ứng dụng web theo mô hình MVC |
| Ngôn ngữ | C#, Razor (HTML + C#) | C# 12 | Lập trình xử lý phía máy chủ và giao diện |
| Truy cập dữ liệu | Entity Framework Core, LINQ | 8.0.10 | Ánh xạ đối tượng – quan hệ, Code First, Migration |
| Cơ sở dữ liệu | Microsoft SQL Server Express | 2022 | Lưu trữ dữ liệu của hệ thống |
| Xác thực, bảo mật | Cookie Authentication, PasswordHasher, HtmlSanitizer | 8.0 / 8.1.870 | Đăng nhập, phân quyền, băm mật khẩu, lọc mã HTML |
| Giao diện | Bootstrap, Bootstrap Icons | 5.1 / 1.11.3 | Bố cục responsive, biểu tượng |
| Phía trình duyệt | jQuery, jQuery Validation | 3.6.0 / 1.19.5 | Tương tác AJAX, kiểm tra dữ liệu nhập phía client |
| Thư viện bổ trợ | Quill, Chart.js | 1.3.7 / 4.4.1 | Trình soạn thảo văn bản, biểu đồ thống kê |

Bảng 1.2. Công cụ hỗ trợ phát triển

| Công cụ | Mục đích sử dụng |
|---|---|
| Visual Studio Code, .NET CLI, dotnet-ef | Viết mã nguồn, biên dịch, chạy ứng dụng, tạo migration |
| SQL Server 2022 Express | Máy chủ cơ sở dữ liệu trên máy phát triển |
| Git, GitHub | Quản lý phiên bản mã nguồn, nộp bài |
| Microsoft Edge, Google Chrome | Chạy thử, kiểm tra giao diện trên máy tính và điện thoại |
| Python | Viết kịch bản kiểm thử tự động |
| Claude (Anthropic) | Công cụ AI hỗ trợ: gợi ý cấu trúc mã nguồn, giao diện, dữ liệu mẫu, kịch bản kiểm thử, vẽ sơ đồ và soạn thảo bản nháp một số nội dung báo cáo; toàn bộ kết quả được tác giả kiểm tra, chỉnh sửa và chịu trách nhiệm |

## Tiểu kết chương 1

Chương 1 đã trình bày lý do chọn đề tài xuất phát từ nhu cầu thực tế của phong trào khởi nghiệp sinh viên và trải nghiệm của chính tác giả với sản phẩm Zalord; xác định mục tiêu, phạm vi nghiên cứu, triển khai và tóm tắt các công nghệ được sử dụng. Chương 2 sẽ trình bày cơ sở lý thuyết của các công nghệ này.
