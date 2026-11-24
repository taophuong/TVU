# Kịch bản quay video demo

Video demo dài khoảng 10 phút. Video bắt đầu từ lúc **cơ sở dữ liệu đã được tạo và nạp dữ liệu mẫu** (không quay phần cài đặt, tạo CSDL): mở SSMS cho thấy cơ sở dữ liệu → chạy website lên localhost → đi trọn một vòng đời của dự án khởi nghiệp: khách xem và tìm kiếm dự án → sinh viên đăng dự án mới → quản trị viên từ chối, sinh viên sửa và gửi lại → quản trị viên duyệt, dự án lên trang chủ → người dùng khác bình luận, bình chọn → kiểm tra phân quyền và khóa tài khoản → **chạy bộ kiểm thử tự động (62 ca) để kết thúc**.

Mỗi cảnh gồm cột **Thao tác** (làm gì trên màn hình) và **Lời thoại** (nói gì trong lúc làm). Lời thoại chỉ là gợi ý, có thể nói tự nhiên theo ý mình.

## 1. Phần mềm và trạng thái bắt đầu

### 1.1. Phần mềm cần có

| Phần mềm | Ghi chú | Cách kiểm tra |
|---|---|---|
| .NET 8 SDK | Bản SDK 8.0.x cho Windows x64 | `dotnet --list-sdks` → có dòng bắt đầu bằng 8.0 |
| SQL Server 2022 Express | Instance mặc định SQLEXPRESS | services.msc → SQL Server (SQLEXPRESS) đang Running |
| SQL Server Management Studio (SSMS) | Xem cơ sở dữ liệu trong video | Kết nối `.\SQLEXPRESS`, Windows Authentication |
| Visual Studio 2022 | Chỉ dùng để mở code ở cảnh mở đầu | Mở được tệp TVUStartup.sln |
| Python 3.x | Chạy bộ kiểm thử tự động ở Cảnh 9 (chỉ dùng thư viện chuẩn, không cần cài thêm gói) | `python --version` |

### 1.2. Trạng thái đầu video

- Cơ sở dữ liệu **TVUStartupDB đã tồn tại**, đã có 14 bảng và dữ liệu mẫu (13 dự án, 9 tài khoản…).
- Website **chưa chạy** (không có cửa sổ nào đang nghe cổng 5080).
- Để đưa máy về đúng trạng thái này, chạy tệp `docs/demo/dat_lai_du_lieu.ps1` (mục 2.1).

### 1.3. Lệnh chạy website (dùng ở Cảnh 2)

Trong PowerShell, tại thư mục gốc mã nguồn (thư mục chứa TVUStartup.sln):

```
dotnet run --project TVUStartup.Web --no-build --urls http://localhost:5080
```

Khi hiện dòng `Now listening on: http://localhost:5080` là website đã chạy. Giữ nguyên cửa sổ này trong suốt lúc quay; **không** dùng Ctrl+C cho đến khi quay xong Cảnh 9, vì bộ kiểm thử cần website đang chạy.

### 1.4. Lỗi thường gặp

| Hiện tượng | Nguyên nhân và cách xử lý |
|---|---|
| A network-related or instance-specific error… / Cannot open database | SQL Server chưa chạy hoặc sai tên instance. Bật dịch vụ SQL Server (SQLEXPRESS) trong services.msc, kiểm tra `DefaultConnection` trong `TVUStartup.Web/appsettings.json`. |
| Failed to bind to address … address already in use | Website đang chạy ở một cửa sổ khác. Đóng cửa sổ đó (Ctrl+C) hoặc chạy lại dat_lai_du_lieu.ps1 (tệp này tự dừng website cũ). |
| Không chạy được tệp .ps1 | Windows chặn script. Chạy: `powershell -ExecutionPolicy Bypass -File docs\demo\dat_lai_du_lieu.ps1`. |
| Trang web hiện nhưng không có dự án nào | CSDL trống. Chạy dat_lai_du_lieu.ps1 để tạo lại và nạp dữ liệu mẫu. |
| Cảnh 9: `python` không nhận lệnh | Chưa cài Python hoặc chưa tick "Add to PATH". Thử lệnh `py tests\kiem_thu_nhanh.py`. |
| Cảnh 9: có ca FAIL | Dữ liệu đã bị thay đổi quá nhiều so với ban đầu. Chạy lại dat_lai_du_lieu.ps1 rồi chạy website và chạy lại bộ kiểm thử trước khi quay. |

## 2. Chuẩn bị trước mỗi lần quay (không quay phần này)

### 2.1. Đặt lại dữ liệu

- Chạy tệp `docs/demo/dat_lai_du_lieu.ps1` (chuột phải → Run with PowerShell). Tệp này dừng website nếu đang chạy, xóa và tạo lại CSDL TVUStartupDB cùng dữ liệu mẫu, xóa ảnh đã tải lên khi chạy thử và build sẵn mã nguồn. Khi cửa sổ báo "Đã sẵn sàng quay: CSDL đã có dữ liệu mẫu, website chưa chạy" thì nhấn Enter để đóng.
- Nếu quay hỏng giữa chừng: nhấn Ctrl+C ở cửa sổ đang chạy website, chạy lại tệp này rồi quay lại từ đầu.
- Chỉ muốn chạy thử website (không quay): `powershell -ExecutionPolicy Bypass -File docs\demo\dat_lai_du_lieu.ps1 -ChayWeb`.

### 2.2. Tài khoản và dữ liệu nhập

| Vai trò | Email | Mật khẩu | Dùng trong |
|---|---|---|---|
| Quản trị viên | admin@tvu.edu.vn | Admin@123 | Cửa sổ B |
| Sinh viên (chủ dự án) | 170125077@st.tvu.edu.vn | SinhVien@123 | Cửa sổ A |
| Sinh viên mới (đăng ký khi quay) | tu.demo@st.tvu.edu.vn | Demo@123 | Cửa sổ C |

Dữ liệu nhập cho dự án demo:

| Trường | Nội dung nhập cho dự án demo |
|---|---|
| Tên dự án | TVU Tìm Đồ |
| Slogan | Mất đồ trong trường? Có TVU Tìm Đồ |
| Mô tả ngắn | Ứng dụng web giúp sinh viên đăng tin tìm đồ thất lạc và trả lại đồ nhặt được trong khuôn viên Trường Đại học Trà Vinh. |
| Lĩnh vực / Khoa / Giai đoạn | Công nghệ phần mềm / Khoa Kỹ thuật và Công nghệ / Nguyên mẫu (MVP) |
| Giới thiệu chi tiết | 2–3 câu, bôi đậm một cụm từ và tạo một danh sách gạch đầu dòng để thấy trình soạn thảo hoạt động |
| Logo / Ảnh bìa | docs/demo/anh_demo/logo.png / anh_bia.png |
| Tính năng 1 | Đăng tin tìm đồ – Chụp ảnh, mô tả và chọn khu vực bị mất |
| Tính năng 2 | Thông báo khi có đồ trùng khớp – Gợi ý tin đăng có mô tả giống nhau |
| Hình ảnh | anh_1.png, anh_2.png (và anh_gia_mao.png để thử chặn tệp giả) |
| Phiên bản | 1.0 – Bản thử nghiệm đầu tiên, có đăng tin và tìm kiếm |
| Thành viên | Trần Minh Tú – Thiết kế giao diện – MSSV 170125999 – Lớp DK25TTG2 |
| Lý do từ chối (quản trị) | Vui lòng bổ sung đường dẫn website hoặc trang thử nghiệm của sản phẩm. |
| Website (khi sửa lại) | https://tvu-timdo.example.com |

### 2.3. Màn hình và trình duyệt

- Độ phân giải màn hình 1920×1080, thu phóng trình duyệt 100%. Tắt thông báo Windows (Focus Assist) và các ứng dụng nhắn tin.
- Mở sẵn PowerShell tại thư mục gốc mã nguồn, phóng to chữ (Ctrl + lăn chuột) để người xem đọc được lệnh. Có thể dùng Terminal trong Visual Studio thay cho PowerShell.
- Mở sẵn SSMS, đã kết nối `.\SQLEXPRESS`, mở rộng Databases → TVUStartupDB → Tables.
- Mở sẵn 3 cửa sổ trình duyệt ở trang trống (website chưa chạy nên chưa mở địa chỉ), mỗi cửa sổ một phiên đăng nhập riêng:
- **Cửa sổ A – Chrome thường:** sinh viên Lê Văn Bình Phương (chưa đăng nhập, sẽ đăng nhập khi quay).
- **Cửa sổ B – Microsoft Edge:** quản trị viên (chưa đăng nhập).
- **Cửa sổ C – Chrome ẩn danh (Ctrl+Shift+N):** khách, sau đó đăng ký thành sinh viên mới.
- Mở sẵn Visual Studio (solution TVUStartup) cho cảnh mở đầu.
- Mở sẵn thư mục docs/demo/anh_demo để chọn ảnh nhanh.
- Phần mềm quay: OBS Studio (Display Capture, 1920×1080, 30 fps, có micro) hoặc Xbox Game Bar (Win+Alt+R). Quay thử 10 giây để kiểm tra tiếng trước khi quay thật.

<<<PAGEBREAK>>>

## 3. Kịch bản chi tiết

### Cảnh 1. Mở đầu và giới thiệu cơ sở dữ liệu (0:00 – 1:10)

| Thao tác | Lời thoại |
|---|---|
| Màn hình Visual Studio, mở Solution Explorer, lướt qua các thư mục Controllers, Areas/Admin, Data, Services, Views | Em chào thầy cô. Em là Lê Văn Bình Phương, MSSV 170125077. Sau đây em xin demo website giới thiệu sản phẩm khởi nghiệp của sinh viên Trường Đại học Trà Vinh, xây dựng bằng ASP.NET Core MVC .NET 8, tổ chức theo mô hình MVC. |
| Mở TVUStartup.Web/appsettings.json, trỏ vào dòng DefaultConnection | Website kết nối SQL Server Express trên máy, cơ sở dữ liệu tên TVUStartupDB, đăng nhập bằng Windows Authentication. |
| Chuyển sang SSMS: Databases → TVUStartupDB → Tables | Cơ sở dữ liệu đã được tạo sẵn bằng migration của Entity Framework Core theo hướng Code First, gồm 14 bảng như Account, Project, Comment, Vote, ContactRequest… |
| SSMS: New Query → gõ `SELECT ProjectName, Status FROM Project` → F5 | Dữ liệu mẫu đã được nạp sẵn: bảng Project có 13 dự án ở đủ các trạng thái nháp, chờ duyệt, đã duyệt và bị từ chối. Toàn bộ dữ liệu này là dữ liệu mẫu hư cấu. |

### Cảnh 2. Chạy website trên localhost (1:10 – 1:35)

| Thao tác | Lời thoại |
|---|---|
| Chuyển sang PowerShell, gõ lệnh ở mục 1.3 → chờ dòng **Now listening on: http://localhost:5080** | Bây giờ em chạy website. Website kết nối vào cơ sở dữ liệu vừa xem và lắng nghe tại cổng 5080. |
| Mở cửa sổ C, gõ http://localhost:5080. Mở thêm địa chỉ này ở cửa sổ A và B | Website đã chạy trên localhost. Em sẽ demo theo đúng vòng đời của một dự án, từ lúc sinh viên đăng cho đến khi được duyệt và công khai, cuối cùng chạy bộ kiểm thử tự động. |

### Cảnh 3. Khách xem và tìm kiếm dự án (1:35 – 3:00) – cửa sổ C

| Thao tác | Lời thoại |
|---|---|
| Trang chủ: chờ banner chuyển ảnh, cuộn xuống phần số liệu, Dự án nổi bật, Lĩnh vực, Tin tức | Đây là trang chủ khi chưa đăng nhập. Trang có banner, số liệu tổng quan, các dự án nổi bật do quản trị viên chọn, các lĩnh vực và tin tức mới. |
| Bấm menu **Dự án khởi nghiệp**, chọn lọc Lĩnh vực = Nông nghiệp công nghệ cao | Trang danh sách cho phép lọc theo lĩnh vực, khoa, giai đoạn và sắp xếp. |
| Xóa bộ lọc, gõ tìm **tra mang cau** (không dấu) → Enter | Em gõ không dấu "tra mang cau" vẫn tìm được dự án Trà Mãng Cầu Xiêm Xanh, nhờ collation tiếng Việt không phân biệt dấu của SQL Server. |
| Mở dự án **StudyMate TVU**: cuộn qua Giới thiệu, Tính năng, bấm một ảnh trong Hình ảnh, Phiên bản, Đội ngũ, Bình luận | Trang chi tiết gồm giới thiệu, tính năng, thư viện ảnh, lịch sử phiên bản, đội ngũ và bình luận, đánh giá. Đây là dự án mẫu, dữ liệu hư cấu. |
| Bấm **Liên hệ hợp tác** → nhập Họ và tên: Nguyễn Văn An, Email: an.nguyen@example.com, Nhu cầu: Hợp tác, Nội dung: một câu → **Gửi yêu cầu** | Khách không cần đăng nhập vẫn gửi được yêu cầu hợp tác tới chủ dự án. Yêu cầu này sẽ hiện trong trang của chủ dự án và của quản trị viên. |
| (Nếu còn thời gian) Nhấn F12 → Ctrl+Shift+M, chọn iPhone, cuộn trang chủ, mở menu ☰ → tắt F12 | Giao diện responsive, hiển thị tốt trên điện thoại. |

### Cảnh 4. Đăng ký tài khoản (3:00 – 3:30) – cửa sổ C

| Thao tác | Lời thoại |
|---|---|
| Bấm **Đăng ký**. Nhập MSSV: **17012abc**, mật khẩu nhập lại khác mật khẩu → bấm Đăng ký | Form đăng ký có kiểm tra dữ liệu: mã số sinh viên chỉ gồm chữ số, mật khẩu nhập lại phải khớp. |
| Sửa lại đúng: Họ tên Trần Minh Tú, Email tu.demo@st.tvu.edu.vn, MSSV 170125999, Lớp DK25TTG2, Khoa Kỹ thuật và Công nghệ, Mật khẩu Demo@123 (2 lần) → Đăng ký | Đăng ký thành công thì hệ thống đăng nhập luôn. Mật khẩu được băm bằng PBKDF2 trước khi lưu. Em để tài khoản này đó, lát nữa dùng để bình luận. |

### Cảnh 5. Sinh viên đăng dự án (3:30 – 5:40) – cửa sổ A

| Thao tác | Lời thoại |
|---|---|
| Đăng nhập 170125077@st.tvu.edu.vn / SinhVien@123 → menu tên người dùng → **Dự án của tôi** | Bây giờ em đăng nhập bằng tài khoản sinh viên. Trang Dự án của tôi liệt kê dự án theo trạng thái. |
| Bấm **Đăng dự án mới**, nhập theo bảng dữ liệu ở mục 2.2, chọn Logo và Ảnh bìa → **Lưu nháp và tiếp tục** | Sinh viên nhập thông tin, viết giới thiệu bằng trình soạn thảo, tải logo và ảnh bìa. Dự án được lưu ở trạng thái Nháp. |
| Bấm ngay **Gửi duyệt** → thông báo lỗi "Chưa thể gửi duyệt, dự án còn thiếu…" | Nếu gửi duyệt ngay sẽ bị chặn vì chưa có tính năng nào. Hệ thống yêu cầu có ảnh bìa và ít nhất một tính năng. |
| Tab **Tính năng**: thêm 2 tính năng | Em bổ sung tính năng cho dự án. |
| Tab **Hình ảnh**: chọn anh_gia_mao.png → tải lên → báo lỗi | Em thử tải lên một tệp văn bản đã đổi đuôi thành .png. Hệ thống kiểm tra chữ ký tệp nên từ chối tệp giả mạo này. |
| Tải lên anh_1.png và anh_2.png | Ảnh thật thì tải lên bình thường, tên tệp được đổi ngẫu nhiên khi lưu. |
| Tab **Phiên bản**: thêm phiên bản 1.0. Tab **Thành viên**: thêm Trần Minh Tú | Sinh viên cập nhật lịch sử phiên bản và thành viên của nhóm. |
| Bấm **Xem trước**, cuộn nhanh, quay lại | Chủ dự án xem trước được dự án đang ở trạng thái nháp, người khác không xem được. |
| Bấm **Gửi duyệt** → trạng thái Chờ duyệt | Đủ điều kiện, em gửi duyệt. Dự án chuyển sang Chờ duyệt và không cho sửa nữa. |

### Cảnh 6. Quản trị viên kiểm duyệt (5:40 – 7:10) – cửa sổ B và A

| Thao tác | Lời thoại |
|---|---|
| Cửa sổ B: đăng nhập admin@tvu.edu.vn / Admin@123 → **Trang quản trị** → Tổng quan | Đây là khu quản trị. Trang tổng quan có các chỉ số, biểu đồ thống kê và danh sách dự án đang chờ duyệt, trong đó có dự án vừa gửi. |
| Menu **Duyệt dự án**, lọc Chờ duyệt → bấm **Từ chối**, bỏ trống lý do → bấm Từ chối → trình duyệt báo phải nhập | Khi từ chối, quản trị viên bắt buộc phải ghi lý do. |
| Nhập lý do theo mục 2.2 → **Từ chối** | Em nhập lý do: cần bổ sung đường dẫn website. |
| Cửa sổ A: F5 trang dự án → thấy trạng thái Bị từ chối và lý do | Sinh viên thấy ngay dự án bị từ chối kèm lý do. |
| Tab Thông tin chung: nhập Website → **Lưu thông tin** → **Gửi duyệt** | Sinh viên sửa theo góp ý và gửi duyệt lại. |
| Cửa sổ B: F5 → **Duyệt** → bấm nút ngôi sao (Đánh dấu nổi bật) | Lần này quản trị viên duyệt, đồng thời đánh dấu dự án là nổi bật. |
| Bấm **Xem website** → trang chủ: dự án TVU Tìm Đồ ở mục Dự án nổi bật | Dự án đã công khai và xuất hiện ở trang chủ. |
| Quay lại quản trị → menu **Liên hệ hợp tác**: thấy yêu cầu của Nguyễn Văn An | Yêu cầu hợp tác khách gửi lúc nãy hiện trong trang quản trị để theo dõi. |

### Cảnh 7. Bình luận, đánh giá, bình chọn (7:10 – 7:50) – cửa sổ C và A

| Thao tác | Lời thoại |
|---|---|
| Cửa sổ C (Trần Minh Tú): mở dự án TVU Tìm Đồ → bấm **Bình chọn** | Sinh viên khác vào xem và bình chọn. Bình chọn dùng AJAX nên số lượt tăng ngay mà không tải lại trang. Mỗi người chỉ bình chọn một lần. |
| Chọn 5 sao, nhập bình luận "Ý tưởng rất hữu ích, nhóm có định làm app di động không?" → **Gửi bình luận** | Người dùng đánh giá sao và bình luận. |
| Cửa sổ A (chủ dự án): F5 dự án → **Trả lời** bình luận → **Gửi trả lời** | Chủ dự án trả lời bình luận, có nhãn chủ dự án để phân biệt. |
| (Nếu còn thời gian) Bấm Bình chọn trên dự án của chính mình → báo không được | Chủ dự án không thể tự bình chọn cho dự án của mình. |

### Cảnh 8. Phân quyền và khóa tài khoản (7:50 – 8:30)

| Thao tác | Lời thoại |
|---|---|
| Cửa sổ A: gõ địa chỉ localhost:5080/quan-tri → trang 403 | Tài khoản sinh viên cố vào khu quản trị sẽ bị chặn với trang lỗi 403. |
| Cửa sổ B: menu **Tài khoản** → bấm nút Khóa ở dòng Trần Minh Tú → OK | Quản trị viên có thể khóa tài khoản vi phạm. |
| Cửa sổ C: F5 → bị đăng xuất; đăng nhập lại bằng tu.demo@st.tvu.edu.vn → báo tài khoản đã bị khóa | Tài khoản bị khóa bị đăng xuất ngay ở lần tải trang kế tiếp và không đăng nhập lại được. |

### Cảnh 9. Chạy bộ kiểm thử tự động và kết thúc (8:30 – 9:50)

Website vẫn đang chạy ở cửa sổ PowerShell của Cảnh 2. Mở một **cửa sổ PowerShell thứ hai** tại thư mục gốc mã nguồn để chạy kiểm thử.

| Thao tác | Lời thoại |
|---|---|
| Mở Visual Studio, mở tests/kiem_thu_nhanh.py, cuộn nhanh qua các nhóm: Khách, Sinh viên, Quản trị, Bảo mật | Ngoài kiểm thử thủ công vừa rồi, em viết thêm một kịch bản kiểm thử tự động bằng Python. Kịch bản gửi các yêu cầu HTTP tới website đang chạy, giống như một trình duyệt. |
| Chuyển sang PowerShell thứ hai, gõ `python -X utf8 tests\kiem_thu_nhanh.py` → Enter | Em chạy bộ kiểm thử. Kịch bản đăng nhập bằng từng loại tài khoản và kiểm tra lần lượt các luồng chính. |
| Chờ các dòng **PASS** chạy ra; chỉ vào các nhóm: xem trang công khai, tìm kiếm không dấu, ẩn phiên bản nội bộ với khách, bình chọn bật/tắt, chặn tệp giả mạo, chặn thiếu token AntiForgery, phân quyền 403, khóa tài khoản | Các kiểm tra gồm: khách xem được trang công khai nhưng không xem được dự án chưa duyệt; tìm kiếm không dấu; bình chọn bằng AJAX; chặn tệp ảnh giả mạo; chặn yêu cầu thiếu token chống giả mạo CSRF; sinh viên không vào được khu quản trị; tài khoản bị khóa bị đăng xuất ngay. |
| Dòng cuối: **62/62 PASS**, giữ màn hình 3 giây | Kết quả: cả 62 ca kiểm thử đều đạt. Bộ kiểm thử này giúp em kiểm tra lại nhanh toàn bộ chức năng chính sau mỗi lần sửa mã. |
| (Tùy chọn) SSMS: chạy lại `SELECT COUNT(*) FROM Project` | Dữ liệu trong cơ sở dữ liệu thay đổi theo các thao tác vừa rồi, cho thấy kiểm thử chạy trên hệ thống thật chứ không phải dữ liệu giả lập. |
| Quay về trang chủ (cửa sổ B hoặc A) | Phần demo của em đến đây là kết thúc. Mã nguồn được công khai trên GitHub tại github.com/taophuong/TVU. Em cảm ơn thầy cô đã theo dõi. |

## 4. Sau khi quay

- Xem lại toàn bộ video: tiếng rõ, không lộ thông tin cá nhân (thông báo, tab khác, mật khẩu thật).
- Nếu video dài hơn 10 phút, cắt bớt các đoạn chờ tải trang và đoạn nhập liệu (có thể tua nhanh 2x phần gõ form).
- Xuất video MP4 (H.264, 1080p), đặt tên 170125077_LeVanBinhPhuong_VideoDemo.mp4, tải lên Google Drive hoặc YouTube (chế độ không công khai) và dán đường dẫn vào Phụ lục 3 của báo cáo.
- Nhấn Ctrl+C ở cửa sổ PowerShell để dừng website. Chạy lại dat_lai_du_lieu.ps1 nếu cần quay lại từ đầu.
