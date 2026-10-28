# CHƯƠNG 5. KIỂM THỬ, KẾT QUẢ VÀ ĐÁNH GIÁ

Chương này trình bày quá trình kiểm thử hệ thống sau khi cài đặt, tổng hợp các kết quả đạt được, đánh giá mức độ hoàn thành so với mục tiêu ban đầu, đồng thời chỉ ra những hạn chế và hướng phát triển của đồ án.

## 5.1. Kiểm thử

### 5.1.1. Môi trường và phương pháp kiểm thử

Hệ thống được kiểm thử theo phương pháp kiểm thử chức năng (kiểm thử hộp đen): người kiểm thử chỉ quan tâm dữ liệu đầu vào và kết quả đầu ra của từng chức năng, không xét đến cấu trúc mã nguồn bên trong. Việc kiểm thử được thực hiện theo hai cách bổ trợ nhau:

- **Kiểm thử thủ công:** thao tác trực tiếp trên trình duyệt với các vai trò Khách, Sinh viên, Quản trị viên; kiểm tra giao diện trên máy tính và điện thoại.
- **Kiểm thử tự động:** viết kịch bản kiểm thử bằng Python (`tests/kiem_thu_nhanh.py`) gửi các yêu cầu HTTP giống như trình duyệt thật (có lưu cookie đăng nhập và token chống giả mạo), sau đó so sánh kết quả trả về với kết quả mong đợi. Cách làm này cho phép chạy lại toàn bộ kiểm thử sau mỗi lần sửa mã nguồn để phát hiện lỗi phát sinh.

Bảng 5.1. Môi trường kiểm thử

| Thành phần | Thông tin |
|---|---|
| Hệ điều hành | Windows 10 Pro 64-bit |
| Nền tảng | .NET SDK 8.0.419, ASP.NET Core MVC 8.0 |
| Hệ quản trị CSDL | Microsoft SQL Server 2022 Express |
| Trình duyệt | Microsoft Edge, Google Chrome (máy tính và chế độ giả lập điện thoại) |
| Dữ liệu kiểm thử | Dữ liệu mẫu nạp tự động: 1 quản trị viên, 8 sinh viên, 7 khoa, 8 lĩnh vực, 13 dự án ở đủ các trạng thái, bình luận, bình chọn, liên hệ, tin tức, banner |
| Tài khoản kiểm thử | Quản trị viên: `admin@tvu.edu.vn`; Sinh viên: `170125077@st.tvu.edu.vn` và các tài khoản sinh viên mẫu |

### 5.1.2. Kết quả kiểm thử

Các trường hợp kiểm thử (test case) được chia thành bốn nhóm: tài khoản và phân quyền, chức năng dành cho khách, chức năng dành cho sinh viên, chức năng quản trị. Kết quả được ghi nhận ngày 28/10/2026.

Bảng 5.2. Test case nhóm tài khoản, xác thực và phân quyền

| Mã | Chức năng / Đầu vào | Kết quả mong đợi | Kết quả thực tế | Đạt? |
|---|---|---|---|---|
| TC01 | Đăng nhập sai mật khẩu | Báo lỗi, không vào hệ thống | Hiển thị “Email hoặc mật khẩu không đúng”, vẫn ở trang đăng nhập | Đạt |
| TC02 | Đăng nhập đúng tài khoản sinh viên | Vào hệ thống, menu hiển thị tên người dùng | Chuyển về trang chủ, menu hiển thị họ tên sinh viên | Đạt |
| TC03 | Đăng nhập đúng tài khoản quản trị viên | Chuyển tới trang quản trị | Chuyển tới trang Tổng quan (Dashboard) | Đạt |
| TC04 | Đăng nhập bằng tài khoản đã bị khóa | Không cho đăng nhập | Hiển thị “Tài khoản của bạn đã bị khóa” | Đạt |
| TC05 | Đăng ký với dữ liệu hợp lệ | Tạo tài khoản và tự đăng nhập | Tạo tài khoản, chuyển tới trang “Dự án của tôi” | Đạt |
| TC06 | Đăng ký với email đã tồn tại, MSSV “1”, mật khẩu nhập lại không khớp | Báo lỗi từng ô, không tạo tài khoản | Hiển thị “Email này đã được đăng ký”, “MSSV chỉ gồm 6–12 chữ số”, “Mật khẩu nhập lại không khớp” | Đạt |
| TC07 | Đổi mật khẩu, nhập sai mật khẩu hiện tại | Báo lỗi | Hiển thị “Mật khẩu hiện tại không đúng” | Đạt |
| TC08 | Đổi mật khẩu hợp lệ rồi đăng nhập bằng mật khẩu mới | Đăng nhập thành công | Đăng nhập thành công | Đạt |
| TC09 | Khách truy cập trang “Dự án của tôi” | Chuyển tới trang đăng nhập | Chuyển tới trang đăng nhập, giữ địa chỉ trang cần quay lại | Đạt |
| TC10 | Khách truy cập trang quản trị | Chuyển tới trang đăng nhập | Chuyển tới trang đăng nhập | Đạt |
| TC11 | Sinh viên truy cập trang quản trị | Từ chối truy cập | Hiển thị trang 403 “Bạn không có quyền truy cập trang này” | Đạt |
| TC12 | Quản trị viên truy cập chức năng của sinh viên | Từ chối truy cập | Hiển thị trang 403 | Đạt |
| TC13 | Quản trị viên khóa tài khoản sinh viên đang đăng nhập, sinh viên tải lại trang | Sinh viên bị đăng xuất | Sinh viên bị chuyển về trang đăng nhập ở yêu cầu kế tiếp | Đạt |
| TC14 | Kiểm tra cột mật khẩu trong CSDL | Không lưu mật khẩu gốc | Lưu chuỗi băm PBKDF2 dài 84 ký tự | Đạt |

Bảng 5.3. Test case nhóm chức năng dành cho khách

| Mã | Chức năng / Đầu vào | Kết quả mong đợi | Kết quả thực tế | Đạt? |
|---|---|---|---|---|
| TC15 | Mở trang chủ, danh sách dự án, chi tiết dự án, tin tức, liên hệ, đăng nhập, đăng ký | Hiển thị đúng, không lỗi | Cả 9 trang hiển thị đúng nội dung | Đạt |
| TC16 | Tìm kiếm từ khóa không dấu “tuy bien” | Tìm được dự án có chữ “Tùy biến” | Kết quả có dự án Zalord | Đạt |
| TC17 | Lọc theo lĩnh vực, khoa, giai đoạn; sắp xếp; chuyển trang | Kết quả đúng điều kiện, giữ bộ lọc khi chuyển trang | Kết quả đúng, bộ lọc được giữ nguyên | Đạt |
| TC18 | Khách mở trang dự án đang Chờ duyệt | Không cho xem | Hiển thị trang 404 | Đạt |
| TC19 | Khách mở bài tin chưa xuất bản | Không cho xem | Hiển thị trang 404 | Đạt |
| TC20 | Mở đường dẫn không tồn tại | Hiển thị trang lỗi thân thiện | Trang 404 tiếng Việt có nút quay về trang chủ | Đạt |
| TC21 | Khách xem dự án có phiên bản nội bộ | Chỉ thấy phiên bản công khai | Chỉ thấy 2 phiên bản công khai, 2 phiên bản nội bộ bị ẩn | Đạt |
| TC22 | Khách gửi liên hệ hợp tác với thông tin đầy đủ | Lưu yêu cầu, thông báo thành công | Thông báo gửi thành công, yêu cầu xuất hiện ở trang quản trị | Đạt |
| TC23 | Gửi form không kèm token chống giả mạo | Từ chối yêu cầu | Trả về lỗi 400 “Yêu cầu không hợp lệ” | Đạt |
| TC24 | Mở website trên màn hình điện thoại | Bố cục co giãn, không tràn ngang | Menu thu gọn, nội dung xếp một cột, không cuộn ngang | Đạt |

Bảng 5.4. Test case nhóm chức năng dành cho sinh viên

| Mã | Chức năng / Đầu vào | Kết quả mong đợi | Kết quả thực tế | Đạt? |
|---|---|---|---|---|
| TC25 | Bấm “Yêu thích” hai lần trên dự án của người khác | Lần 1 thích, lần 2 bỏ thích, không tải lại trang | Số lượt và biểu tượng thay đổi ngay | Đạt |
| TC26 | Bình chọn cho dự án của chính mình | Không cho phép | Thông báo không thể bình chọn dự án của chính mình | Đạt |
| TC27 | Gửi bình luận kèm đánh giá 4 sao | Bình luận hiển thị, điểm trung bình cập nhật | Bình luận hiển thị với 4 sao | Đạt |
| TC28 | Bình luận chứa đoạn mã `<script>alert(1)</script>` | Hiển thị dạng văn bản, không thực thi | Đoạn mã hiển thị như chữ thường, không thực thi | Đạt |
| TC29 | Chủ dự án mở trang dự án của mình | Thấy cả phiên bản nội bộ và nút chỉnh sửa | Thấy phiên bản nội bộ có nhãn “Nội bộ” và nút “Chỉnh sửa dự án” | Đạt |
| TC30 | Tạo dự án mới tên “Dự án Kiểm Thử Tự Động” | Lưu ở trạng thái Nháp, tạo đường dẫn không dấu | Trạng thái Nháp, đường dẫn `du-an-kiem-thu-tu-dong` | Đạt |
| TC31 | Phần giới thiệu chứa thẻ `<script>` và thuộc tính `onerror` | Loại bỏ mã độc, giữ định dạng an toàn | Thẻ in đậm được giữ, mã độc bị loại bỏ | Đạt |
| TC32 | Chủ dự án xem trang dự án đang Nháp | Cho xem trước | Hiển thị trang kèm thanh “Bản xem trước” | Đạt |
| TC33 | Gửi duyệt khi chưa có ảnh bìa và tính năng | Không cho gửi | Thông báo dự án còn thiếu ảnh bìa và tính năng | Đạt |
| TC34 | Tải lên tệp giả mạo đuôi .png (nội dung là mã script) | Từ chối tệp | Thông báo “Tệp tải lên không phải là ảnh hợp lệ” | Đạt |
| TC35 | Tải lên ảnh bìa PNG hợp lệ | Lưu ảnh thành công | Ảnh được lưu với tên ngẫu nhiên và hiển thị trên trang dự án | Đạt |
| TC36 | Gửi duyệt khi đã đủ điều kiện | Chuyển trạng thái Chờ duyệt | Thông báo đã gửi tới Ban quản trị, trạng thái Chờ duyệt | Đạt |
| TC37 | Rút lại dự án đang Chờ duyệt rồi gửi duyệt lại | Về Nháp, sau đó về Chờ duyệt | Đúng như mong đợi | Đạt |
| TC38 | Sinh viên mở trang sửa dự án của sinh viên khác | Không cho truy cập | Hiển thị trang 404 | Đạt |

Bảng 5.5. Test case nhóm chức năng quản trị

| Mã | Chức năng / Đầu vào | Kết quả mong đợi | Kết quả thực tế | Đạt? |
|---|---|---|---|---|
| TC39 | Mở các trang quản trị: Tổng quan, Duyệt dự án, Liên hệ, Bình luận, Tin tức, Banner, Lĩnh vực, Khoa, Tài khoản | Hiển thị đúng | Cả 13 trang hiển thị đúng; trang Tổng quan có 7 số liệu và 4 biểu đồ | Đạt |
| TC40 | Từ chối dự án nhưng để trống lý do | Báo lỗi, trạng thái không đổi | Thông báo yêu cầu nhập lý do, dự án vẫn Chờ duyệt | Đạt |
| TC41 | Duyệt dự án đang Chờ duyệt | Dự án được công khai | Khách mở được trang dự án vừa duyệt | Đạt |
| TC42 | Xóa lĩnh vực đang có dự án | Không cho xóa | Thông báo không thể xóa vì đang có dự án thuộc lĩnh vực | Đạt |
| TC43 | Xóa dự án | Xóa dự án cùng dữ liệu liên quan | Dự án, ảnh, tính năng, bình luận, bình chọn bị xóa; liên hệ được giữ lại | Đạt |
| TC44 | Đặt lại mật khẩu cho một tài khoản | Sinh mật khẩu tạm và hiển thị một lần | Hiển thị mật khẩu tạm, đăng nhập được bằng mật khẩu này | Đạt |

### 5.1.3. Tổng hợp kết quả kiểm thử

Bảng 5.6. Tổng hợp kết quả kiểm thử

| Nhóm chức năng | Số test case | Đạt | Không đạt |
|---|---|---|---|
| Tài khoản, xác thực, phân quyền | 14 | 14 | 0 |
| Chức năng dành cho khách | 10 | 10 | 0 |
| Chức năng dành cho sinh viên | 14 | 14 | 0 |
| Chức năng quản trị | 6 | 6 | 0 |
| **Tổng cộng** | **44** | **44** | **0** |

Toàn bộ 44 test case đều đạt. Kịch bản kiểm thử tự động thực hiện 62 lần kiểm tra và đều cho kết quả đúng. Trong quá trình kiểm thử, một số lỗi đã được phát hiện và khắc phục trước khi chốt kết quả, ví dụ: trang báo lỗi chỉ nhận phương thức GET nên trả về mã 405 thay vì 400 khi yêu cầu POST bị chặn; ký tự tiếng Việt bị mã hóa thành dạng thực thể HTML làm mã trang khó đọc; giao diện bị tràn ngang trên màn hình điện thoại do khoảng cách lưới lớn hơn lề.

## 5.2. Kết quả

### 5.2.1. Các chức năng đã hoàn thành

Bảng 5.7. Tổng hợp chức năng đã hoàn thành

| Nhóm | Chức năng | Kết quả |
|---|---|---|
| Khách | Trang chủ, tìm kiếm không dấu, lọc, sắp xếp, phân trang, chi tiết dự án, tin tức, liên hệ, đăng ký (UC01–UC06) | Hoàn thành |
| Sinh viên | Đăng nhập, hồ sơ, đổi mật khẩu, đăng và quản lý dự án, hình ảnh, tính năng, phiên bản, thành viên, gửi duyệt / rút lại, bình luận, đánh giá, bình chọn, xem liên hệ (UC07–UC14) | Hoàn thành |
| Quản trị viên | Dashboard thống kê với biểu đồ, duyệt / từ chối / ẩn / nổi bật dự án, quản lý lĩnh vực, khoa, tài khoản, tin tức, banner, bình luận, liên hệ (UC15–UC24) | Hoàn thành |
| Bảo mật | Băm mật khẩu PBKDF2, phân quyền theo vai trò, chống SQL Injection, XSS, CSRF, kiểm tra tệp tải lên, đăng xuất tài khoản bị khóa | Hoàn thành |
| Triển khai | Migration EF Core, script SQL, dữ liệu mẫu tự nạp, tệp README, mã nguồn trên GitHub | Hoàn thành |

Về quy mô, mã nguồn gồm 16 controller (6 controller phía người dùng, 10 controller khu vực quản trị kể cả lớp cơ sở), 14 lớp thực thể tương ứng 14 bảng dữ liệu, cùng các View Razor cho từng chức năng. Giao diện thực tế của từng chức năng được trình bày tại mục 4.6.

### 5.2.2. Kịch bản demo và kết quả đầu ra

Kịch bản demo được xây dựng theo đúng luồng nghiệp vụ chính của hệ thống, thực hiện trong khoảng 10 phút:

1. **Khách tìm hiểu dự án:** mở trang chủ, xem số liệu tổng quan và dự án nổi bật; tìm kiếm từ khóa không dấu “tuy bien” và mở trang chi tiết dự án Zalord; gửi một yêu cầu liên hệ hợp tác.
2. **Sinh viên đăng dự án:** đăng ký tài khoản mới; tạo dự án, nhập giới thiệu bằng trình soạn thảo, tải logo, ảnh bìa; thêm tính năng, hình ảnh, phiên bản; xem trước và gửi duyệt.
3. **Quản trị viên duyệt dự án:** đăng nhập trang quản trị, xem Dashboard; lọc dự án Chờ duyệt, xem trước rồi duyệt; đánh dấu dự án nổi bật; xem yêu cầu liên hệ mới.
4. **Tương tác trên dự án đã công khai:** khách thấy dự án mới trên trang chủ và trang danh sách; sinh viên khác bình luận, chấm sao, bình chọn; chủ dự án trả lời bình luận và cập nhật trạng thái liên hệ.

Kết quả đầu ra sau khi chạy kịch bản: dự án mới chuyển lần lượt qua các trạng thái Nháp → Chờ duyệt → Đã duyệt và hiển thị công khai; số liệu trên Dashboard (số dự án đã duyệt, lượt xem, liên hệ mới, bình luận) và các biểu đồ cập nhật theo dữ liệu mới; điểm đánh giá trung bình, số lượt yêu thích của dự án thay đổi tương ứng với các thao tác.

## 5.3. Đánh giá

Bảng 5.8. Đánh giá kết quả so với mục tiêu ban đầu

| Mục tiêu | Kết quả đạt được | Mức độ |
|---|---|---|
| Xây dựng website giới thiệu tập trung các dự án khởi nghiệp của sinh viên TVU | Website có trang chủ, danh sách, chi tiết dự án với đầy đủ giới thiệu, tính năng, hình ảnh, phiên bản, đội ngũ | Đạt |
| Cho phép nhiều sinh viên tự đăng và quản lý dự án | Sinh viên đăng ký, đăng và cập nhật dự án; mỗi sinh viên chỉ thao tác trên dự án của mình | Đạt |
| Có quy trình kiểm duyệt trước khi công khai | Quy trình Nháp → Chờ duyệt → Đã duyệt / Bị từ chối → Ẩn, từ chối bắt buộc nêu lý do | Đạt |
| Kết nối dự án với cộng đồng, doanh nghiệp, nhà đầu tư | Bình luận, đánh giá, bình chọn, gửi liên hệ hợp tác không cần đăng nhập, theo dõi xử lý liên hệ | Đạt |
| Hỗ trợ quản trị và thống kê | Dashboard với 7 chỉ số và 4 biểu đồ; quản lý đầy đủ danh mục, tài khoản, nội dung | Đạt |
| Áp dụng các kỹ thuật ASP.NET Core vào dự án thực tế | MVC, Area, Razor, Tag Helper, EF Core Code First, LINQ, Dependency Injection, Cookie Authentication | Đạt |
| Bảo đảm an toàn thông tin | Đáp ứng các yêu cầu bảo mật đã đặt ra; kiểm thử với các tình huống tấn công thường gặp đều đạt | Đạt |
| Triển khai lên môi trường Internet | Chưa triển khai lên hosting, hiện chạy trên máy cục bộ | Chưa đạt |

Nhìn chung, hệ thống đáp ứng được các yêu cầu chức năng và phi chức năng đã phân tích ở Chương 3. Điểm mạnh của đồ án là quy trình nghiệp vụ rõ ràng, có kiểm duyệt nội dung; giao diện hiện đại, tương thích điện thoại; chú trọng bảo mật ở nhiều lớp; và có bộ kiểm thử tự động giúp kiểm tra lại nhanh sau mỗi lần thay đổi. Dự án Zalord – sản phẩm thật của sinh viên thực hiện đồ án – được dùng làm dữ liệu minh họa, cho thấy website có thể đưa vào sử dụng để giới thiệu các sản phẩm thực tế.

## 5.4. Hạn chế và hướng phát triển

### 5.4.1. Hạn chế

- Chưa có chức năng gửi email: quên mật khẩu, xác thực email khi đăng ký, thông báo cho sinh viên khi dự án được duyệt hoặc bị từ chối, thông báo cho nhóm dự án khi có liên hệ mới.
- Chưa tích hợp đăng nhập bằng tài khoản email của Trường (Google / Microsoft).
- Chưa triển khai lên máy chủ thật nên chưa đánh giá được hiệu năng khi có nhiều người truy cập đồng thời.
- Tìm kiếm dựa trên so khớp chuỗi, chưa sắp xếp kết quả theo mức độ liên quan.
- Dữ liệu hiện tại chủ yếu là dữ liệu mẫu, chưa thu thập dự án thật của sinh viên các khoa.

### 5.4.2. Hướng phát triển

- Tích hợp dịch vụ gửi email (SMTP) cho các chức năng thông báo và khôi phục mật khẩu.
- Hỗ trợ đăng nhập một lần (SSO) bằng tài khoản Google / Microsoft do Trường cấp.
- Triển khai hệ thống lên IIS hoặc Docker trên máy chủ của Trường, cấu hình tên miền và HTTPS.
- Xây dựng Web API (xác thực bằng JWT) để phát triển ứng dụng di động.
- Bổ sung tìm kiếm toàn văn (Full-Text Search), gợi ý dự án theo sở thích người dùng.
- Mở rộng chức năng cho cuộc thi khởi nghiệp: nộp hồ sơ dự thi, chấm điểm của ban giám khảo, công bố kết quả.

## Tiểu kết chương 5

Chương 5 đã trình bày kết quả kiểm thử với 44 test case thuộc bốn nhóm chức năng, tất cả đều đạt; tổng hợp các chức năng đã hoàn thành và kịch bản demo; đánh giá hệ thống đạt hầu hết mục tiêu đề ra, đồng thời nêu rõ các hạn chế và hướng phát triển trong thời gian tới.
