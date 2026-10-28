# Kịch bản quay video demo

Video demo dài khoảng 10 phút, đi trọn một vòng đời của dự án khởi nghiệp: khách xem và tìm kiếm dự án → sinh viên đăng dự án mới → quản trị viên từ chối, sinh viên sửa và gửi lại → quản trị viên duyệt, dự án lên trang chủ → người dùng khác bình luận, bình chọn → kiểm tra phân quyền và khóa tài khoản.

Mỗi cảnh gồm cột **Thao tác** (làm gì trên màn hình) và **Lời thoại** (nói gì trong lúc làm). Lời thoại chỉ là gợi ý, có thể nói tự nhiên theo ý mình.

## 1. Cài đặt, tạo cơ sở dữ liệu và chạy website trên localhost

Phần này chỉ cần làm một lần trên máy dùng để quay. Nếu website đã chạy được tại http://localhost:5080 thì chuyển sang mục 2.

### 1.1. Phần mềm cần có

| Phần mềm | Ghi chú | Cách kiểm tra |
|---|---|---|
| .NET 8 SDK | Tải tại dotnet.microsoft.com, bản SDK 8.0.x cho Windows x64 | Mở PowerShell, gõ `dotnet --list-sdks` → có dòng bắt đầu bằng 8.0 |
| SQL Server 2022 Express | Khi cài chọn Basic; tên instance mặc định là SQLEXPRESS | Mở services.msc → dịch vụ SQL Server (SQLEXPRESS) ở trạng thái Running |
| SQL Server Management Studio (SSMS) | Để xem cơ sở dữ liệu khi quay cảnh mở đầu | Mở SSMS, kết nối Server name `.\SQLEXPRESS`, Authentication: Windows Authentication |
| Visual Studio 2022 | Workload ASP.NET and web development; chỉ dùng để mở code khi quay | Mở được tệp TVUStartup.sln |
| Git (không bắt buộc) | Dùng để tải mã nguồn từ GitHub | `git --version` |

### 1.2. Lấy mã nguồn

Mở PowerShell tại thư mục muốn lưu, chạy:

```
git clone https://github.com/taophuong/TVU.git
cd TVU
```

Hoặc giải nén tệp zip mã nguồn của đồ án. Các lệnh bên dưới đều chạy trong thư mục gốc của mã nguồn (thư mục chứa TVUStartup.sln).

### 1.3. Tạo cơ sở dữ liệu

**Bước 1 – Kiểm tra chuỗi kết nối.** Mở `TVUStartup.Web/appsettings.json`, dòng `DefaultConnection` đang trỏ tới `Server=.\SQLEXPRESS;Database=TVUStartupDB;Trusted_Connection=True;...`. Nếu máy cài SQL Server với tên instance khác thì sửa phần Server cho đúng (ví dụ bản Developer dùng `Server=.`, LocalDB dùng `Server=(localdb)\MSSQLLocalDB`).

**Bước 2 – Tạo cơ sở dữ liệu (chọn 1 trong 2 cách).**

- **Cách 1 (đơn giản nhất):** không cần làm gì thêm. Lần đầu chạy website (mục 1.4), chương trình tự tạo cơ sở dữ liệu TVUStartupDB bằng migration của Entity Framework Core và nạp dữ liệu mẫu.
- **Cách 2 – tạo trước bằng lệnh EF Core:**

```
cd TVUStartup.Web
dotnet tool restore
dotnet ef database update
cd ..
```

Lệnh `dotnet tool restore` cài công cụ dotnet-ef (phiên bản 8.0.10) theo tệp .config/dotnet-tools.json; lệnh `dotnet ef database update` tạo cơ sở dữ liệu và 14 bảng. Dữ liệu mẫu được nạp ở lần chạy website đầu tiên.

**Bước 3 – Kiểm tra trong SSMS** (sau khi đã chạy website ít nhất một lần): kết nối `.\SQLEXPRESS` → Databases → TVUStartupDB → Tables có 14 bảng (Account, Banner, Category, Comment, ContactRequest, Faculty, News, Project, ProjectFeature, ProjectImage, ProjectVersion, Role, TeamMember, Vote) cùng bảng __EFMigrationsHistory. Chạy thử truy vấn `SELECT COUNT(*) FROM Project` → kết quả 13 dự án mẫu.

### 1.4. Chạy website lên localhost

Trong PowerShell, tại thư mục gốc mã nguồn:

```
dotnet run --project TVUStartup.Web --urls http://localhost:5080
```

- Lần đầu sẽ mất khoảng 30 giây – 1 phút để build và tạo cơ sở dữ liệu. Khi cửa sổ hiện dòng `Now listening on: http://localhost:5080` là website đã chạy.
- Mở trình duyệt, vào http://localhost:5080 → thấy trang chủ TVU Startup với banner và dự án nổi bật là thành công.
- Giữ nguyên cửa sổ PowerShell trong suốt lúc quay; nhấn Ctrl+C trong cửa sổ đó để dừng website.
- Có thể chạy bằng Visual Studio (mở TVUStartup.sln, nhấn F5) nhưng khi đó cổng sẽ khác 5080 (ví dụ http://localhost:5247). Nên dùng lệnh trên để địa chỉ khớp với kịch bản.

### 1.5. Lỗi thường gặp

| Hiện tượng | Nguyên nhân và cách xử lý |
|---|---|
| A network-related or instance-specific error… / Cannot open database | SQL Server chưa chạy hoặc sai tên instance. Bật dịch vụ SQL Server (SQLEXPRESS) trong services.msc, hoặc sửa chuỗi kết nối ở mục 1.3. |
| Failed to bind to address http://127.0.0.1:5080: address already in use | Website đang chạy ở một cửa sổ khác. Đóng cửa sổ đó (Ctrl+C), hoặc chạy dat_lai_du_lieu.ps1 (tệp này tự dừng website cũ). |
| Could not execute because the specified command or file was not found (dotnet-ef) | Chưa cài công cụ EF Core. Chạy `dotnet tool restore` trong thư mục mã nguồn. |
| The command could not be loaded / không chạy được tệp .ps1 | Windows chặn chạy script. Chạy bằng lệnh `powershell -ExecutionPolicy Bypass -File docs\demo\dat_lai_du_lieu.ps1`. |
| Trang web hiện nhưng không có dự án nào | Cơ sở dữ liệu được tạo nhưng chưa nạp dữ liệu mẫu. Chạy dat_lai_du_lieu.ps1 để tạo lại từ đầu. |

## 2. Chuẩn bị trước mỗi lần quay (không quay phần này)

### 2.1. Đặt lại dữ liệu

- Chạy tệp docs/demo/dat_lai_du_lieu.ps1 (chuột phải → Run with PowerShell). Tệp này dừng website, xóa CSDL, xóa ảnh đã tải lên khi chạy thử rồi chạy lại website tại http://localhost:5080 với dữ liệu mẫu sạch.
- Giữ nguyên cửa sổ PowerShell đó trong suốt lúc quay (đóng là website dừng).
- Nếu quay hỏng giữa chừng, chạy lại tệp này rồi quay lại từ đầu, vì dự án demo đã được tạo ở lần quay trước.

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

- Độ phân giải màn hình 1920×1080, thu phóng trình duyệt 100%. Tắt thông báo Windows (Focus Assist), Zalo, Messenger.
- Mở sẵn 3 cửa sổ trình duyệt, mỗi cửa sổ một phiên đăng nhập riêng, đặt ở http://localhost:5080:
- **Cửa sổ A – Chrome thường:** sinh viên Lê Văn Bình Phương (chưa đăng nhập, sẽ đăng nhập khi quay).
- **Cửa sổ B – Microsoft Edge:** quản trị viên (chưa đăng nhập).
- **Cửa sổ C – Chrome ẩn danh (Ctrl+Shift+N):** khách, sau đó đăng ký thành sinh viên mới.
- Mở sẵn Visual Studio (solution TVUStartup) và SQL Server Management Studio (đã kết nối .\SQLEXPRESS) cho cảnh mở đầu.
- Mở sẵn thư mục docs/demo/anh_demo để chọn ảnh nhanh.
- Phần mềm quay: OBS Studio (Display Capture, 1920×1080, 30 fps, có micro) hoặc Xbox Game Bar (Win+Alt+R). Quay thử 10 giây để kiểm tra tiếng trước khi quay thật.

## 3. Kịch bản chi tiết

### Cảnh 1. Mở đầu (0:00 – 0:40)

| Thao tác | Lời thoại |
|---|---|
| Màn hình Visual Studio, mở Solution Explorer, lướt qua các thư mục Controllers, Areas/Admin, Data, Services | Em chào thầy cô. Em là Lê Văn Bình Phương, MSSV 170125077. Sau đây em xin demo website giới thiệu sản phẩm khởi nghiệp của sinh viên Trường Đại học Trà Vinh, xây dựng bằng ASP.NET Core MVC .NET 8. |
| Chuyển sang SSMS, mở TVUStartupDB → Tables | Cơ sở dữ liệu SQL Server gồm 14 bảng, được tạo bằng migration của Entity Framework Core và nạp dữ liệu mẫu khi website khởi động. |
| Chuyển sang cửa sổ C, đang ở trang chủ | Em sẽ demo theo đúng vòng đời của một dự án, từ lúc sinh viên đăng cho đến khi được duyệt và công khai. |

### Cảnh 2. Khách xem và tìm kiếm dự án (0:40 – 2:30) – cửa sổ C

| Thao tác | Lời thoại |
|---|---|
| Trang chủ: chờ banner chuyển ảnh, cuộn xuống phần số liệu, Dự án nổi bật, Lĩnh vực, Tin tức | Đây là trang chủ khi chưa đăng nhập. Trang có banner, số liệu tổng quan, các dự án nổi bật do quản trị viên chọn, các lĩnh vực và tin tức mới. |
| Bấm menu **Dự án khởi nghiệp**, chọn lọc Lĩnh vực = Nông nghiệp công nghệ cao | Trang danh sách cho phép lọc theo lĩnh vực, khoa, giai đoạn và sắp xếp. |
| Xóa bộ lọc, gõ tìm **tra mang cau** (không dấu) → Enter | Em gõ không dấu "tra mang cau" vẫn tìm được dự án Trà Mãng Cầu Xiêm Xanh, nhờ collation tiếng Việt không phân biệt dấu của SQL Server. |
| Mở dự án **Zalord**: cuộn qua Giới thiệu, Tính năng, bấm một ảnh trong Hình ảnh, Phiên bản, Đội ngũ, Bình luận | Trang chi tiết gồm giới thiệu, tính năng, thư viện ảnh, lịch sử phiên bản, đội ngũ và bình luận, đánh giá. Zalord là sản phẩm thật của em. |
| Bấm **Liên hệ hợp tác** → nhập Họ và tên: Nguyễn Văn An, Email: an.nguyen@example.com, Nhu cầu: Hợp tác, Nội dung: một câu → **Gửi yêu cầu** | Khách không cần đăng nhập vẫn gửi được yêu cầu hợp tác tới chủ dự án. Yêu cầu này sẽ hiện trong trang của chủ dự án và của quản trị viên. |
| Nhấn F12 → Ctrl+Shift+M, chọn iPhone, cuộn trang chủ, mở menu ☰ → tắt F12 | Giao diện responsive, hiển thị tốt trên điện thoại. |

### Cảnh 3. Đăng ký tài khoản (2:30 – 3:15) – cửa sổ C

| Thao tác | Lời thoại |
|---|---|
| Bấm **Đăng ký**. Nhập MSSV: **17012abc**, mật khẩu nhập lại khác mật khẩu → bấm Đăng ký | Form đăng ký có kiểm tra dữ liệu: mã số sinh viên chỉ gồm chữ số, mật khẩu nhập lại phải khớp. |
| Sửa lại đúng: Họ tên Trần Minh Tú, Email tu.demo@st.tvu.edu.vn, MSSV 170125999, Lớp DK25TTG2, Khoa Kỹ thuật và Công nghệ, Mật khẩu Demo@123 (2 lần) → Đăng ký | Đăng ký thành công thì hệ thống đăng nhập luôn. Mật khẩu được băm bằng PBKDF2 trước khi lưu. Em để tài khoản này đó, lát nữa dùng để bình luận. |

### Cảnh 4. Sinh viên đăng dự án (3:15 – 6:00) – cửa sổ A

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

### Cảnh 5. Quản trị viên kiểm duyệt (6:00 – 8:00) – cửa sổ B và A

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

### Cảnh 6. Bình luận, đánh giá, bình chọn (8:00 – 9:00) – cửa sổ C và A

| Thao tác | Lời thoại |
|---|---|
| Cửa sổ C (Trần Minh Tú): mở dự án TVU Tìm Đồ → bấm **Bình chọn** | Sinh viên khác vào xem và bình chọn. Bình chọn dùng AJAX nên số lượt tăng ngay mà không tải lại trang. Mỗi người chỉ bình chọn một lần. |
| Chọn 5 sao, nhập bình luận "Ý tưởng rất hữu ích, nhóm có định làm app di động không?" → **Gửi bình luận** | Người dùng đánh giá sao và bình luận. |
| Cửa sổ A (chủ dự án): F5 dự án → **Trả lời** bình luận → **Gửi trả lời** | Chủ dự án trả lời bình luận, có nhãn chủ dự án để phân biệt. |
| (Nếu còn thời gian) Bấm Bình chọn trên dự án của chính mình → báo không được | Chủ dự án không thể tự bình chọn cho dự án của mình. |

### Cảnh 7. Phân quyền, khóa tài khoản và kết thúc (9:00 – 10:00)

| Thao tác | Lời thoại |
|---|---|
| Cửa sổ A: gõ địa chỉ localhost:5080/quan-tri → trang 403 | Tài khoản sinh viên cố vào khu quản trị sẽ bị chặn với trang lỗi 403. |
| Cửa sổ B: menu **Tài khoản** → bấm nút Khóa ở dòng Trần Minh Tú → OK | Quản trị viên có thể khóa tài khoản vi phạm. |
| Cửa sổ C: F5 → bị đăng xuất; đăng nhập lại bằng tu.demo@st.tvu.edu.vn → báo tài khoản đã bị khóa | Tài khoản bị khóa bị đăng xuất ngay ở lần tải trang kế tiếp và không đăng nhập lại được. |
| Quay về trang chủ (cửa sổ B hoặc A) | Phần demo của em đến đây là kết thúc. Mã nguồn được công khai trên GitHub tại github.com/taophuong/TVU. Em cảm ơn thầy cô đã theo dõi. |

## 4. Sau khi quay

- Xem lại toàn bộ video: tiếng rõ, không lộ thông tin cá nhân (thông báo, tab khác, mật khẩu thật).
- Nếu video dài hơn 10 phút, cắt bớt các đoạn chờ tải trang và đoạn nhập liệu (có thể tua nhanh 2x phần gõ form).
- Xuất video MP4 (H.264, 1080p), đặt tên 170125077_LeVanBinhPhuong_VideoDemo.mp4, tải lên Google Drive hoặc YouTube (chế độ không công khai) và dán đường dẫn vào Phụ lục 3 của báo cáo.
- Chạy lại dat_lai_du_lieu.ps1 nếu cần trả website về dữ liệu mẫu ban đầu.
