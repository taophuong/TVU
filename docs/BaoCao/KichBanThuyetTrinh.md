# Kịch bản thuyết trình đồ án

Kịch bản đọc kèm bộ slide thuyết trình (17 slide), mỗi mục dưới đây ứng với một slide. Thời lượng ước tính theo tốc độ nói vừa phải khoảng 140 tiếng/phút: phần đọc khoảng 9 phút 35 giây, cộng thời gian chuyển slide là khoảng 10 phút.

Nội dung này cũng nằm trong phần ghi chú (Notes) của từng slide; khi trình chiếu, bật Presenter View (Slide Show → Use Presenter View) để vừa xem slide vừa đọc kịch bản trên màn hình máy tính.

Bảng 1. Phân bổ thời gian theo slide

| Slide | Nội dung | Thời lượng | Mốc kết thúc |
|---|---|---|---|
| 1 | Tiêu đề | 25 giây | 0:25 |
| 2 | Nội dung trình bày | 15 giây | 0:40 |
| 3 | Lý do chọn đề tài | 55 giây | 1:35 |
| 4 | Mục tiêu và phạm vi | 45 giây | 2:20 |
| 5 | Công nghệ sử dụng | 30 giây | 2:50 |
| 6 | Kiến trúc hệ thống theo mô hình MVC | 30 giây | 3:20 |
| 7 | Sơ đồ use case tổng quát | 35 giây | 3:55 |
| 8 | Thiết kế cơ sở dữ liệu | 35 giây | 4:30 |
| 9 | Quy trình kiểm duyệt dự án | 50 giây | 5:20 |
| 10 | Giao diện người dùng | 30 giây | 5:50 |
| 11 | Giao diện sinh viên | 30 giây | 6:20 |
| 12 | Giao diện quản trị | 35 giây | 6:55 |
| 13 | Bảo mật ứng dụng | 35 giây | 7:30 |
| 14 | Kiểm thử | 45 giây | 8:15 |
| 15 | Đánh giá so với mục tiêu | 25 giây | 8:40 |
| 16 | Hạn chế và hướng phát triển | 25 giây | 9:05 |
| 17 | Kết luận | 30 giây | 9:35 |

**Gợi ý khi trình bày:**

- Nói chậm, rõ ở slide 9 (quy trình kiểm duyệt) vì đây là nghiệp vụ chính của đề tài.
- Nếu đến slide 9 đã quá 5 phút, rút gọn các slide giao diện (10–12) – chỉ đọc đoạn đầu.
- Khi nhắc tới hình, dùng con trỏ chỉ vào đúng phần đang nói.

<<<PAGEBREAK>>>

## Slide 1. Tiêu đề (khoảng 25 giây)

Em kính chào quý thầy cô trong hội đồng.

Em tên là Lê Văn Bình Phương, mã số sinh viên 170125077, lớp DK25TTG2. Hôm nay em xin trình bày đồ án chuyên đề ASP.NET với đề tài: Xây dựng website giới thiệu các sản phẩm khởi nghiệp của sinh viên Trường Đại học Trà Vinh, dưới sự hướng dẫn của TS. Nguyễn Nhứt Lam.

## Slide 2. Nội dung trình bày (khoảng 15 giây)

Bài trình bày gồm năm phần: giới thiệu đề tài; phân tích và thiết kế hệ thống; cài đặt và giao diện; kiểm thử, đánh giá; và cuối cùng là kết luận.

## Slide 3. Lý do chọn đề tài (khoảng 55 giây)

Em chọn đề tài này vì ba lý do.

Thứ nhất, Nhà nước đang khuyến khích sinh viên khởi nghiệp qua Đề án 1665 của Thủ tướng Chính phủ; phong trào khởi nghiệp trong các trường rất sôi nổi, và sinh viên Trà Vinh có nhiều sản phẩm gắn với thế mạnh địa phương.

Thứ hai, các sản phẩm này chủ yếu được giới thiệu qua mạng xã hội hoặc ngày hội, thông tin rời rạc, nên doanh nghiệp và nhà đầu tư khó tìm kiếm, khó liên hệ.

Thứ ba, bản thân em đang phát triển sản phẩm Zalord, tiện ích cá nhân hóa giao diện Zalo. Khi giới thiệu sản phẩm, em thấy rất cần một kênh chính thống, có kiểm duyệt của nhà trường.

## Slide 4. Mục tiêu và phạm vi (khoảng 45 giây)

Mục tiêu là xây dựng một website chung, nơi nhiều sinh viên tự đăng và quản lý dự án; dự án phải được ban quản trị kiểm duyệt rồi mới công khai. Website kết nối sinh viên với doanh nghiệp qua bình luận, đánh giá, bình chọn và liên hệ hợp tác, đồng thời có khu quản trị, thống kê.

Hệ thống có ba tác nhân, hai mươi bốn chức năng và mười bốn bảng dữ liệu. Theo định hướng của giảng viên hướng dẫn, em dùng dữ liệu mẫu với mười ba dự án; riêng Zalord là sản phẩm thật.

## Slide 5. Công nghệ sử dụng (khoảng 30 giây)

Hệ thống được xây dựng trên ASP.NET Core MVC, .NET 8 – phiên bản hỗ trợ dài hạn. Dữ liệu truy cập bằng Entity Framework Core theo hướng Code First, lưu trên SQL Server 2022.

Đăng nhập dùng Cookie Authentication, mật khẩu băm bằng PBKDF2. Giao diện dùng Bootstrap 5 và jQuery; Chart.js để vẽ biểu đồ, Quill làm trình soạn thảo, và HtmlSanitizer để lọc mã HTML độc hại.

## Slide 6. Kiến trúc hệ thống theo mô hình MVC (khoảng 30 giây)

Hệ thống theo mô hình MVC. Yêu cầu từ trình duyệt đi qua chuỗi middleware gồm định tuyến, xác thực và phân quyền, rồi đến controller. Khu quản trị được tách riêng thành Area Admin.

Controller dùng các service được cấp qua Dependency Injection như băm mật khẩu, lưu ảnh, lọc HTML; truy xuất dữ liệu qua AppDbContext, rồi truyền ViewModel sang View Razor để sinh trang HTML.

## Slide 7. Sơ đồ use case tổng quát (khoảng 35 giây)

Hệ thống có ba tác nhân. Khách có thể xem, tìm kiếm dự án, đọc tin tức, gửi liên hệ hợp tác và đăng ký tài khoản.

Sinh viên kế thừa chức năng của khách, thêm đăng và quản lý dự án, gửi duyệt; bình luận, đánh giá sao và bình chọn cho dự án khác.

Quản trị viên duyệt dự án, quản lý danh mục, tài khoản, nội dung và xem thống kê. Tổng cộng có hai mươi bốn use case.

## Slide 8. Thiết kế cơ sở dữ liệu (khoảng 35 giây)

Cơ sở dữ liệu gồm mười bốn bảng, tạo theo hướng Code First. Bảng Project là trung tâm, liên kết với lĩnh vực, khoa, chủ dự án và bảy bảng con như hình ảnh, tính năng, phiên bản, bình luận.

Các ràng buộc được khai báo rõ: bảng Vote dùng khóa ghép để mỗi người chỉ bình chọn một lần cho một dự án; số sao được kiểm tra từ một đến năm. Collation Vietnamese_CI_AI giúp gõ không dấu vẫn tìm được dự án có dấu.

## Slide 9. Quy trình kiểm duyệt dự án (khoảng 50 giây)

Đây là nghiệp vụ chính của hệ thống.

Dự án mới tạo ở trạng thái Nháp, chỉ chủ dự án thấy và sửa. Khi đã có ảnh bìa và ít nhất một tính năng, sinh viên được gửi duyệt, dự án chuyển sang Chờ duyệt, và vẫn có thể rút lại.

Nếu quản trị viên duyệt, dự án hiển thị công khai. Nếu từ chối, quản trị viên bắt buộc ghi lý do để sinh viên sửa và gửi lại. Dự án đã duyệt có thể bị ẩn và hiện lại.

Các điều kiện chuyển trạng thái được gom vào lớp ProjectRules, nên mọi nơi dùng chung một bộ quy tắc, tránh sai lệch.

## Slide 10. Giao diện người dùng (khoảng 30 giây)

Bên trái là trang chủ với số liệu tổng quan, dự án nổi bật, lĩnh vực và tin tức. Trang danh sách cho phép lọc theo lĩnh vực, khoa, giai đoạn và tìm kiếm không dấu.

Bên phải là trang chi tiết dự án Zalord, gồm giới thiệu, tính năng, hình ảnh, phiên bản, đội ngũ và nút liên hệ hợp tác. Giao diện hiển thị tốt trên cả điện thoại.

## Slide 11. Giao diện sinh viên (khoảng 30 giây)

Bên trái là form đăng dự án mới: sinh viên nhập thông tin, chọn lĩnh vực, khoa, tải logo, ảnh bìa và viết giới thiệu bằng trình soạn thảo; sau đó bổ sung tính năng, hình ảnh, thành viên rồi gửi duyệt.

Bên phải là phần bình luận trên dự án đã công khai: sinh viên bình luận kèm đánh giá sao, và chủ dự án được trả lời trực tiếp.

## Slide 12. Giao diện quản trị (khoảng 35 giây)

Trang tổng quan của quản trị hiển thị bảy chỉ số và bốn biểu đồ thống kê, kèm danh sách dự án chờ duyệt và liên hệ mới. Số liệu được tính bằng truy vấn nhóm ngay trong SQL Server rồi vẽ bằng Chart.js.

Bên phải là trang duyệt dự án: lọc theo trạng thái, dự án chờ duyệt được đưa lên đầu, với các thao tác duyệt, từ chối kèm lý do, ẩn, hoặc đánh dấu nổi bật để đưa ra trang chủ.

## Slide 13. Bảo mật ứng dụng (khoảng 35 giây)

Về bảo mật, mật khẩu được băm PBKDF2 có salt. Phân quyền theo vai trò; tài khoản bị khóa bị đăng xuất ngay ở yêu cầu kế tiếp. Truy vấn qua EF Core được tham số hóa nên chống SQL Injection.

Nội dung HTML được lọc bằng HtmlSanitizer để chống XSS, mọi form POST có token chống CSRF. Tệp tải lên được kiểm tra định dạng, dung lượng và chữ ký tệp, rồi đổi tên ngẫu nhiên khi lưu.

## Slide 14. Kiểm thử (khoảng 45 giây)

Em kiểm thử theo phương pháp hộp đen với bốn mươi bốn test case, chia bốn nhóm: tài khoản và phân quyền mười bốn, chức năng khách mười, chức năng sinh viên mười bốn và chức năng quản trị sáu.

Ngoài kiểm thử thủ công, em viết kịch bản Python gửi yêu cầu như trình duyệt thật, có cookie đăng nhập và token CSRF, thực hiện sáu mươi hai lần kiểm tra. Toàn bộ đều đạt. Trong quá trình kiểm thử, em cũng phát hiện và sửa một số lỗi, ví dụ giao diện bị tràn ngang trên điện thoại.

## Slide 15. Đánh giá so với mục tiêu (khoảng 25 giây)

So với mục tiêu ban đầu, hệ thống đạt bảy trên tám mục tiêu, từ giới thiệu dự án tập trung, quy trình kiểm duyệt, kết nối doanh nghiệp đến quản trị, thống kê và an toàn thông tin. Mục tiêu chưa đạt là triển khai lên Internet: em đã publish thử thành công nhưng chưa đưa lên máy chủ thật.

## Slide 16. Hạn chế và hướng phát triển (khoảng 25 giây)

Hệ thống còn hạn chế: chưa gửi email, chưa đăng nhập bằng tài khoản của Trường, chưa triển khai thật và dữ liệu còn là dữ liệu mẫu.

Hướng phát triển là tích hợp email, đăng nhập một lần bằng Google hoặc Microsoft, triển khai lên máy chủ, nâng cấp .NET 10 và xây dựng Web API cho ứng dụng di động.

## Slide 17. Kết luận (khoảng 30 giây)

Tóm lại, đồ án đã xây dựng được website hoàn chỉnh với quy trình kiểm duyệt rõ ràng, áp dụng ASP.NET Core MVC, Entity Framework Core, SQL Server và các kỹ thuật bảo mật web; toàn bộ test case đều đạt. Mã nguồn được công khai trên GitHub.

Em xin chân thành cảm ơn quý thầy cô đã lắng nghe, và rất mong nhận được góp ý của hội đồng.
