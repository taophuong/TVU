# Tư liệu Chương 5 – Kiểm thử (mục 5.1)

> Kết quả thực tế ghi nhận ngày **28/10/2026** trên máy phát triển. Lời văn nhận xét, đánh giá (5.2–5.4) sinh viên tự viết.

## Môi trường và phương pháp kiểm thử

| Mục | Nội dung |
|---|---|
| Môi trường | Windows 10 Pro, .NET SDK 8.0.419, SQL Server 2022 Express, trình duyệt Microsoft Edge, Chrome |
| Dữ liệu | Dữ liệu mẫu nạp tự động (`DbSeeder`): 1 Admin, 8 sinh viên, 13 dự án ở đủ các trạng thái |
| Phương pháp | Kiểm thử chức năng hộp đen: (1) thao tác thủ công trên trình duyệt; (2) script tự động `tests/kiem_thu_nhanh.py` gửi request HTTP như trình duyệt thật (có cookie đăng nhập, token chống CSRF) và kiểm tra kết quả trả về |
| Kết quả tổng hợp | Script tự động: **62/62 lần kiểm tra đạt**; bảng dưới gộp thành 44 test case |

## Bảng test case

### A. Tài khoản, xác thực, phân quyền

| Mã | Chức năng / Đầu vào | Kết quả mong đợi | Kết quả thực tế | Đạt? |
|---|---|---|---|---|
| TC01 | Đăng nhập sai mật khẩu (`170125077@st.tvu.edu.vn` / `SaiMatKhau`) | Báo lỗi, không vào hệ thống | Hiển thị “Email hoặc mật khẩu không đúng.”, vẫn ở trang đăng nhập | Đạt |
| TC02 | Đăng nhập đúng tài khoản sinh viên | Vào hệ thống, menu hiển thị tên người dùng | Chuyển về trang chủ, menu hiển thị “Lê Văn Bình Phương” | Đạt |
| TC03 | Đăng nhập đúng tài khoản Admin | Chuyển tới trang quản trị | Chuyển tới `/quan-tri` (Dashboard) | Đạt |
| TC04 | Đăng nhập bằng tài khoản đã bị khóa | Không cho đăng nhập | Hiển thị “Tài khoản của bạn đã bị khóa…” | Đạt |
| TC05 | Đăng ký với dữ liệu hợp lệ | Tạo tài khoản, tự đăng nhập | Tạo tài khoản, chuyển tới “Dự án của tôi” | Đạt |
| TC06 | Đăng ký: email đã tồn tại, MSSV “1”, mật khẩu nhập lại không khớp | Báo lỗi từng ô, không tạo tài khoản | Hiển thị “Email này đã được đăng ký”, “MSSV chỉ gồm 6–12 chữ số”, “Mật khẩu nhập lại không khớp” | Đạt |
| TC07 | Đổi mật khẩu, nhập sai mật khẩu hiện tại | Báo lỗi | Hiển thị “Mật khẩu hiện tại không đúng.” | Đạt |
| TC08 | Đổi mật khẩu hợp lệ, đăng nhập lại bằng mật khẩu mới | Đăng nhập thành công | Đăng nhập thành công bằng mật khẩu mới | Đạt |
| TC09 | Khách (chưa đăng nhập) mở `/du-an-cua-toi` | Chuyển tới trang đăng nhập | Chuyển tới `/dang-nhap?ReturnUrl=...` | Đạt |
| TC10 | Khách mở `/quan-tri` | Chuyển tới trang đăng nhập | Chuyển tới `/dang-nhap` | Đạt |
| TC11 | Sinh viên mở `/quan-tri` | Từ chối truy cập | Trang “403 – Bạn không có quyền truy cập trang này” | Đạt |
| TC12 | Admin mở `/du-an-cua-toi` (chức năng của sinh viên) | Từ chối truy cập | Chuyển tới trang 403 | Đạt |
| TC13 | Admin khóa tài khoản sinh viên đang đăng nhập; sinh viên tải lại trang | Sinh viên bị đăng xuất | Request kế tiếp bị chuyển về trang đăng nhập | Đạt |
| TC14 | Kiểm tra cột `PasswordHash` trong CSDL | Không lưu mật khẩu gốc | Chuỗi băm PBKDF2 dạng `AQAAAAIAAYag…` (84 ký tự) | Đạt |

### B. Chức năng dành cho khách

| Mã | Chức năng / Đầu vào | Kết quả mong đợi | Kết quả thực tế | Đạt? |
|---|---|---|---|---|
| TC15 | Mở trang chủ, danh sách dự án, chi tiết Zalord, tin tức, liên hệ, đăng nhập, đăng ký | Hiển thị đúng, không lỗi | 9/9 trang trả về mã 200, hiển thị đúng nội dung | Đạt |
| TC16 | Tìm kiếm từ khóa không dấu “tuy bien” | Tìm thấy dự án có chữ “Tùy biến” | Kết quả có dự án Zalord | Đạt |
| TC17 | Lọc theo lĩnh vực / khoa / giai đoạn, sắp xếp, chuyển trang | Kết quả đúng điều kiện, giữ bộ lọc khi chuyển trang | Đúng (kiểm tra thủ công) | Đạt |
| TC18 | Khách mở trang dự án đang Chờ duyệt (`/du-an/xa-phong-thao-duoc-moc-an`) | Không cho xem | Trang 404 | Đạt |
| TC19 | Khách mở bài tin chưa xuất bản | Không cho xem | Trang 404 | Đạt |
| TC20 | Mở đường dẫn không tồn tại `/abc` | Trang lỗi thân thiện | Trang 404 tiếng Việt, có nút về trang chủ | Đạt |
| TC21 | Khách xem Zalord | Chỉ thấy phiên bản công khai | Thấy Small v3.6, Elite; không thấy Full Dev Menu, Premium | Đạt |
| TC22 | Khách gửi liên hệ hợp tác (đầy đủ thông tin) | Lưu yêu cầu, thông báo thành công | Thông báo “Đã gửi yêu cầu liên hệ…”, yêu cầu xuất hiện ở trang quản trị | Đạt |
| TC23 | Gửi form POST không có token chống CSRF | Từ chối request | Mã 400 “Yêu cầu không hợp lệ” | Đạt |
| TC24 | Mở website trên màn hình điện thoại (rộng 390–500px) | Bố cục co giãn, không tràn ngang | Menu thu gọn, nội dung xếp 1 cột, không cuộn ngang | Đạt |

### C. Chức năng dành cho sinh viên

| Mã | Chức năng / Đầu vào | Kết quả mong đợi | Kết quả thực tế | Đạt? |
|---|---|---|---|---|
| TC25 | Bấm “Yêu thích” 2 lần trên dự án của người khác | Lần 1 thích, lần 2 bỏ thích, không tải lại trang | Số lượt và biểu tượng tim đổi ngay (AJAX) | Đạt |
| TC26 | Bình chọn cho dự án của chính mình | Không cho phép | Thông báo “Bạn không thể bình chọn cho dự án của chính mình.” | Đạt |
| TC27 | Gửi bình luận kèm 4 sao | Bình luận hiển thị, điểm trung bình cập nhật | Bình luận hiển thị với 4 sao | Đạt |
| TC28 | Bình luận có nội dung `<script>alert(1)</script>` | Hiển thị dạng chữ, không thực thi | Hiển thị `&lt;script&gt;…` dạng văn bản | Đạt |
| TC29 | Chủ dự án mở trang Zalord | Thấy cả phiên bản nội bộ | Thấy Full Dev Menu, Premium kèm nhãn “Nội bộ”; có nút “Chỉnh sửa dự án” | Đạt |
| TC30 | Tạo dự án mới tên “Dự án Kiểm Thử Tự Động” | Lưu trạng thái Nháp, tạo slug | Trạng thái Nháp, slug `du-an-kiem-thu-tu-dong` | Đạt |
| TC31 | Giới thiệu chi tiết chứa `<script>`, `<img onerror=…>` | Loại bỏ mã độc, giữ định dạng an toàn | `<strong>` được giữ; `<script>`, `onerror` bị loại bỏ | Đạt |
| TC32 | Chủ dự án xem trang dự án đang Nháp | Cho xem trước | Hiển thị thanh “Bản xem trước – trạng thái Nháp” | Đạt |
| TC33 | Gửi duyệt khi chưa có ảnh bìa và tính năng | Không cho gửi | Thông báo “Chưa thể gửi duyệt, dự án còn thiếu: ảnh bìa, ít nhất 1 tính năng nổi bật.” | Đạt |
| TC34 | Tải lên tệp `fake.png` (nội dung là mã script) | Từ chối tệp | Thông báo “Tệp tải lên không phải là ảnh hợp lệ.” | Đạt |
| TC35 | Tải lên ảnh bìa PNG hợp lệ | Lưu ảnh | Ảnh lưu vào `wwwroot/uploads/projects/{id}/` với tên ngẫu nhiên | Đạt |
| TC36 | Gửi duyệt khi đã đủ ảnh bìa và tính năng | Chuyển Chờ duyệt | Thông báo “Đã gửi dự án tới Ban quản trị”, trạng thái Chờ duyệt | Đạt |
| TC37 | Rút lại dự án đang Chờ duyệt, sau đó gửi duyệt lại | Về Nháp, rồi về Chờ duyệt | Đúng như mong đợi | Đạt |
| TC38 | Sinh viên mở trang sửa dự án của sinh viên khác (`/du-an-cua-toi/2/sua`) | Không cho truy cập | Trang 404 | Đạt |

### D. Chức năng quản trị

| Mã | Chức năng / Đầu vào | Kết quả mong đợi | Kết quả thực tế | Đạt? |
|---|---|---|---|---|
| TC39 | Mở các trang quản trị: Dashboard, Duyệt dự án, Liên hệ, Bình luận, Tin tức (+ form), Banner (+ form), Lĩnh vực (+ form sửa), Khoa, Tài khoản | Hiển thị đúng | 13/13 trang trả về mã 200; Dashboard hiển thị 7 chỉ số và 4 biểu đồ | Đạt |
| TC40 | Từ chối dự án nhưng bỏ trống lý do | Báo lỗi, trạng thái không đổi | Thông báo “Vui lòng nhập lý do từ chối…”, dự án vẫn Chờ duyệt | Đạt |
| TC41 | Duyệt dự án Chờ duyệt | Dự án công khai | Khách mở được trang dự án (mã 200) | Đạt |
| TC42 | Xóa lĩnh vực đang có dự án | Không cho xóa | Thông báo “Không thể xóa … vì đang có dự án thuộc lĩnh vực này” | Đạt |
| TC43 | Xóa dự án | Xóa dự án cùng ảnh, tính năng, bình luận, bình chọn | Xóa thành công, dữ liệu con bị xóa theo | Đạt |
| TC44 | Đặt lại mật khẩu tài khoản | Sinh mật khẩu tạm, hiển thị một lần | Hiển thị mật khẩu dạng `Tvu@xxxxxx`, đăng nhập được bằng mật khẩu này | Đạt |

## Ghi chú cho mục 5.2 – 5.4 (sinh viên tự viết lời văn)
- Kịch bản demo gợi ý (≈ 10 phút): Khách tìm kiếm “tuy bien” → xem Zalord → gửi liên hệ hợp tác → Sinh viên đăng ký, đăng dự án mới, thêm tính năng, ảnh bìa, gửi duyệt → Admin xem Dashboard, duyệt dự án → Khách thấy dự án mới, bình luận, bình chọn → Sinh viên xem liên hệ, trả lời bình luận.
- Hạn chế có thể nêu: chưa gửi email (quên mật khẩu, xác thực đăng ký, thông báo kết quả duyệt); chưa có đăng nhập bằng tài khoản Google/Microsoft của Trường; chưa triển khai lên hosting; tìm kiếm chưa xếp hạng theo độ liên quan; dữ liệu là dữ liệu mẫu.
- Hướng phát triển: gửi email thông báo (SMTP), API + JWT cho ứng dụng di động, triển khai IIS/Docker, thống kê theo thời gian thực, tích hợp đăng nhập SSO của Trường.
