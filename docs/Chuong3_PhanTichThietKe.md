# Tư liệu Chương 3 – Phân tích và thiết kế hệ thống

> Tư liệu hỗ trợ viết Chương 3 theo hướng dẫn của GVHD. Các bảng, sơ đồ dưới đây phản ánh đúng mã nguồn hiện tại.
> **Lưu ý:** phần lời văn trong báo cáo sinh viên tự viết lại theo cách hiểu của mình; tài liệu này chỉ là dàn ý + số liệu + hình.
> Ảnh sơ đồ nằm trong `docs/diagrams/*.png` (độ phân giải gấp đôi, chèn vào Word để in rõ nét). Mã nguồn sơ đồ trong `docs/diagrams/src/`.

## Danh mục hình đề xuất cho Chương 3

| Hình | Tệp | Mục |
|---|---|---|
| Hình 3.1. Sơ đồ use case tổng quát | `01_use_case_tong_quat.png` | 3.1 |
| Hình 3.2. Sơ đồ thực thể – liên kết (ERD) | `02_erd.png` | 3.2 |
| Hình 3.3. Sơ đồ tuần tự chức năng Đăng nhập | `03_seq_dang_nhap.png` | 3.3 |
| Hình 3.4. Sơ đồ tuần tự chức năng Đăng dự án và gửi duyệt | `04_seq_dang_du_an.png` | 3.3 |
| Hình 3.5. Sơ đồ tuần tự chức năng Duyệt dự án | `05_seq_duyet_du_an.png` | 3.3 |
| Hình 3.6. Sơ đồ trạng thái của dự án | `06_state_du_an.png` | 3.3 |
| Hình 3.7. Sơ đồ hoạt động quy trình đăng dự án đến khi công khai | `07_activity_dang_du_an.png` | 3.3 |
| Hình 3.8. Sơ đồ hoạt động gửi liên hệ hợp tác | `08_activity_lien_he.png` | 3.3 |
| Hình 3.9. Kiến trúc phân lớp theo mô hình MVC | `09_kien_truc_mvc.png` | 3.4 |
| Hình 3.10. Wireframe Trang chủ | `10_wf_trang_chu.png` | 3.4 |
| Hình 3.11. Wireframe Danh sách dự án | `11_wf_danh_sach_du_an.png` | 3.4 |
| Hình 3.12. Wireframe Chi tiết dự án | `12_wf_chi_tiet_du_an.png` | 3.4 |
| Hình 3.13. Wireframe Đăng / sửa dự án (Sinh viên) | `13_wf_dang_du_an.png` | 3.4 |
| Hình 3.14. Wireframe Duyệt dự án (Quản trị viên) | `14_wf_admin_duyet_du_an.png` | 3.4 |
| Hình 3.15. Wireframe Dashboard quản trị | `15_wf_admin_dashboard.png` | 3.4 |

---

## 3.1. Phân tích yêu cầu

- Danh sách tác nhân, yêu cầu chức năng (UC01–UC24), yêu cầu phi chức năng: xem `docs/PhanTich_YeuCau.md`.
- Quan hệ giữa các tác nhân: **Sinh viên kế thừa Khách** (làm được mọi việc của Khách); Quản trị viên dùng chung chức năng Đăng nhập.

### Đặc tả các use case chính

**Bảng 3.x. Đặc tả use case UC07 – Đăng nhập**

| Mục | Nội dung |
|---|---|
| Tác nhân | Sinh viên, Quản trị viên |
| Mô tả | Người dùng đăng nhập bằng email và mật khẩu để sử dụng các chức năng theo vai trò |
| Tiền điều kiện | Đã có tài khoản; chưa đăng nhập |
| Luồng chính | 1. Người dùng mở trang `/dang-nhap`. 2. Nhập email, mật khẩu (tùy chọn “Ghi nhớ đăng nhập”), bấm Đăng nhập. 3. Hệ thống kiểm tra token chống CSRF và dữ liệu nhập. 4. Hệ thống tìm tài khoản theo email, so khớp mật khẩu với chuỗi băm (PBKDF2). 5. Hệ thống tạo cookie đăng nhập chứa mã tài khoản, họ tên, email, vai trò. 6. Chuyển Quản trị viên tới `/quan-tri`, Sinh viên tới trang trước đó hoặc trang chủ. |
| Luồng thay thế | 4a. Email không tồn tại hoặc sai mật khẩu → báo “Email hoặc mật khẩu không đúng”, quay lại bước 2. 4b. Tài khoản bị khóa → báo “Tài khoản đã bị khóa”. 3a. Bỏ trống / sai định dạng email → báo lỗi ngay tại ô nhập. |
| Hậu điều kiện | Người dùng ở trạng thái đã đăng nhập; menu hiển thị chức năng theo vai trò |

**Bảng 3.x. Đặc tả use case UC09 + UC11 – Đăng dự án và gửi duyệt**

| Mục | Nội dung |
|---|---|
| Tác nhân | Sinh viên |
| Mô tả | Sinh viên tạo dự án khởi nghiệp, bổ sung nội dung và gửi cho Ban quản trị duyệt |
| Tiền điều kiện | Đã đăng nhập với vai trò Sinh viên |
| Luồng chính | 1. Chọn “Đăng dự án mới”. 2. Nhập tên, mô tả ngắn, lĩnh vực, khoa, giai đoạn, giới thiệu chi tiết; chọn logo, ảnh bìa. 3. Hệ thống kiểm tra dữ liệu, lọc HTML phần giới thiệu, lưu ảnh, tạo đường dẫn (slug) không trùng, lưu dự án ở trạng thái **Nháp**; người tạo tự động là thành viên đầu tiên. 4. Sinh viên bổ sung tính năng, hình ảnh, phiên bản, thành viên. 5. Bấm “Gửi duyệt”. 6. Hệ thống kiểm tra dự án có ảnh bìa và ít nhất 1 tính năng, chuyển trạng thái **Chờ duyệt**. |
| Luồng thay thế | 3a. Thiếu trường bắt buộc / ảnh sai định dạng (> 2 MB, không phải .jpg/.png/.webp) → báo lỗi, giữ dữ liệu đã nhập. 6a. Thiếu ảnh bìa hoặc tính năng → báo “Chưa thể gửi duyệt, dự án còn thiếu …”. 6b. Khi đang Chờ duyệt, sinh viên có thể “Rút lại” → trạng thái Nháp. |
| Hậu điều kiện | Dự án ở trạng thái Chờ duyệt, xuất hiện trong danh sách duyệt của Quản trị viên |

**Bảng 3.x. Đặc tả use case UC16 – Duyệt dự án**

| Mục | Nội dung |
|---|---|
| Tác nhân | Quản trị viên |
| Mô tả | Kiểm tra nội dung dự án sinh viên gửi lên, quyết định công khai hoặc từ chối |
| Tiền điều kiện | Đã đăng nhập với vai trò Admin; có dự án ở trạng thái Chờ duyệt |
| Luồng chính | 1. Mở “Duyệt dự án”, lọc trạng thái Chờ duyệt. 2. Bấm “Xem” để xem trước trang dự án. 3. Bấm “Duyệt”. 4. Hệ thống chuyển trạng thái **Đã duyệt**, ghi thời điểm và người duyệt. 5. Dự án hiển thị ở trang chủ, danh sách, kết quả tìm kiếm. |
| Luồng thay thế | 3a. Bấm “Từ chối”, nhập lý do → trạng thái **Bị từ chối**; sinh viên thấy lý do tại “Dự án của tôi”. 3b. Bỏ trống lý do → báo “Vui lòng nhập lý do từ chối”. 3c. Dự án đã duyệt có thể bị **Ẩn** / hiện lại hoặc **đánh dấu nổi bật**. |
| Hậu điều kiện | Trạng thái dự án được cập nhật |

**Bảng 3.x. Đặc tả use case UC02 – Tìm kiếm, lọc dự án**

| Mục | Nội dung |
|---|---|
| Tác nhân | Khách (và mọi người dùng) |
| Mô tả | Tìm dự án theo từ khóa, lọc theo lĩnh vực, khoa, giai đoạn; sắp xếp và phân trang |
| Tiền điều kiện | Không |
| Luồng chính | 1. Nhập từ khóa ở thanh tìm kiếm hoặc mở “Dự án khởi nghiệp”. 2. Chọn lĩnh vực, khoa, giai đoạn, kiểu sắp xếp (mới nhất, xem nhiều, yêu thích, tên A–Z). 3. Hệ thống chỉ lấy dự án Đã duyệt, so khớp từ khóa **không phân biệt hoa thường và dấu tiếng Việt** (collation `Vietnamese_CI_AI`) trên tên, slogan, mô tả. 4. Hiển thị 9 dự án / trang kèm số kết quả. |
| Luồng thay thế | 3a. Không có kết quả → hiển thị thông báo và nút xóa bộ lọc. |
| Hậu điều kiện | Không thay đổi dữ liệu |

**Bảng 3.x. Đặc tả use case UC12 – Bình luận, đánh giá**

| Mục | Nội dung |
|---|---|
| Tác nhân | Sinh viên (đã đăng nhập); chủ dự án / Quản trị viên được trả lời |
| Mô tả | Viết bình luận và chấm 1–5 sao cho dự án đã công khai |
| Tiền điều kiện | Đã đăng nhập; dự án ở trạng thái Đã duyệt |
| Luồng chính | 1. Ở trang chi tiết dự án, chọn số sao và nhập nội dung (≤ 1000 ký tự). 2. Bấm “Gửi bình luận”. 3. Hệ thống lưu bình luận; điểm trung bình được tính lại khi hiển thị. |
| Luồng thay thế | 1a. Chủ dự án không được tự chấm sao; mỗi tài khoản chấm sao 1 lần / dự án (lần sau chỉ lưu nội dung). 2a. Chủ dự án / Admin bấm “Trả lời” dưới bình luận. 2b. Người viết hoặc Admin xóa bình luận (xóa kèm câu trả lời). |
| Hậu điều kiện | Bình luận hiển thị dưới trang dự án; nội dung được mã hóa HTML khi hiển thị (chống XSS) |

**Bảng 3.x. Đặc tả use case UC05 – Gửi liên hệ hợp tác**

| Mục | Nội dung |
|---|---|
| Tác nhân | Khách (doanh nghiệp, nhà đầu tư…), không cần đăng nhập |
| Mô tả | Gửi đề nghị hợp tác / đầu tư / sử dụng sản phẩm tới nhóm dự án |
| Tiền điều kiện | Dự án ở trạng thái Đã duyệt |
| Luồng chính | 1. Bấm “Liên hệ hợp tác” trên trang dự án. 2. Nhập họ tên, email, số điện thoại, đơn vị, nhu cầu, nội dung. 3. Hệ thống kiểm tra token chống CSRF và dữ liệu, lưu yêu cầu ở trạng thái **Mới**. 4. Sinh viên chủ dự án (mục “Liên hệ hợp tác”) và Quản trị viên thấy yêu cầu, liên hệ lại, cập nhật **Đã liên hệ / Đã đóng**. |
| Luồng thay thế | 3a. Dữ liệu không hợp lệ → báo lỗi. Trang “Liên hệ” chung gửi yêu cầu tới Ban quản trị (không gắn dự án). |
| Hậu điều kiện | Có bản ghi ContactRequest; xóa dự án vẫn giữ lại yêu cầu liên hệ |

---

## 3.2. Thiết kế cơ sở dữ liệu

- **Hình 3.2** – ERD (14 bảng nghiệp vụ). Ký hiệu: `||` một, `o|` không hoặc một, `o{` không hoặc nhiều; PK khóa chính, FK khóa ngoại, UK duy nhất.
- Mô tả chi tiết từng bảng (kiểu dữ liệu, ràng buộc, mặc định): `docs/ThietKe_CSDL.md`.

### Lược đồ quan hệ
(**Đậm** = khóa chính, *nghiêng* = khóa ngoại)

- Role(**RoleId**, RoleName, Description)
- Faculty(**FacultyId**, FacultyName, Description, DisplayOrder)
- Account(**AccountId**, Email, PasswordHash, FullName, StudentCode, ClassName, Phone, Avatar, *FacultyId*, *RoleId*, Status, CreatedAt, UpdatedAt)
- Category(**CategoryId**, CategoryName, Slug, Icon, Description, DisplayOrder, IsActive)
- Project(**ProjectId**, ProjectName, Slug, Slogan, ShortDescription, Content, Logo, CoverImage, VideoUrl, WebsiteUrl, CommunityUrl, *CategoryId*, *FacultyId*, *OwnerId*, Stage, Status, RejectReason, IsFeatured, ViewCount, FoundedDate, CreatedAt, UpdatedAt, ApprovedAt, *ApprovedBy*)
- ProjectImage(**ImageId**, *ProjectId*, ImageUrl, Caption, DisplayOrder)
- ProjectFeature(**FeatureId**, *ProjectId*, Title, Description, Icon, DisplayOrder)
- ProjectVersion(**VersionId**, *ProjectId*, VersionName, Platform, ReleaseDate, ChangeLog, DownloadUrl, IsPublic)
- TeamMember(**MemberId**, *ProjectId*, *AccountId*, FullName, Position, StudentCode, ClassName, Avatar, Email, DisplayOrder)
- Comment(**CommentId**, *ProjectId*, *AccountId*, *ParentId*, Content, Rating, IsHidden, CreatedAt)
- Vote(***ProjectId***, ***AccountId***, CreatedAt)
- ContactRequest(**ContactId**, *ProjectId*, FullName, Email, Phone, Organization, RequestType, Message, Status, CreatedAt, *HandledBy*)
- News(**NewsId**, Title, Slug, Thumbnail, Summary, Content, NewsType, EventDate, Location, *AuthorId*, IsPublished, ViewCount, CreatedAt, UpdatedAt)
- Banner(**BannerId**, Title, ImageUrl, LinkUrl, DisplayOrder, IsActive)

### Hành vi khi xóa (khóa ngoại)

| Quan hệ | Khi xóa bản ghi cha |
|---|---|
| Project → ProjectImage, ProjectFeature, ProjectVersion, TeamMember, Comment, Vote | Xóa theo (CASCADE) |
| Project → ContactRequest | Giữ lại liên hệ, đặt ProjectId = NULL (SET NULL) |
| Category / Faculty / Account (chủ dự án) → Project | Không cho xóa (NO ACTION) – ứng dụng báo lỗi |
| Comment → Comment (trả lời) | Xóa câu trả lời ở tầng ứng dụng (SQL Server không cho cascade vòng) |

---

## 3.3. Thiết kế chức năng

| Sơ đồ | Ý chính cần trình bày |
|---|---|
| Tuần tự – Đăng nhập (H3.3) | Vai trò của AccountController, IPasswordHasher, AppDbContext; 3 nhánh: sai thông tin / bị khóa / hợp lệ; tạo cookie chứa Claims |
| Tuần tự – Đăng dự án (H3.4) | Lọc HTML, lưu ảnh, slug; vòng lặp bổ sung nội dung; điều kiện gửi duyệt |
| Tuần tự – Duyệt dự án (H3.5) | Admin xem trước dự án chưa công khai; duyệt ghi ApprovedAt/ApprovedBy; từ chối bắt buộc lý do |
| Trạng thái – Dự án (H3.6) | 5 trạng thái (giá trị cột Status 0–4) và các sự kiện chuyển trạng thái |
| Hoạt động – Đăng dự án → công khai (H3.7) | 3 làn: Sinh viên – Hệ thống – Quản trị viên; 3 điểm quyết định |
| Hoạt động – Liên hệ hợp tác (H3.8) | Khách gửi, hệ thống kiểm tra, chủ dự án / Admin xử lý |

## 3.4. Thiết kế kiến trúc và giao diện

**Kiến trúc (H3.9)** – các thành phần chính:

| Lớp | Thành phần trong dự án | Nhiệm vụ |
|---|---|---|
| Middleware | `Program.cs`: StaticFiles → Routing → Authentication → Authorization | Xử lý request trước khi tới Controller |
| Controller | `Controllers/*`, `Areas/Admin/Controllers/*` | Nhận request, kiểm tra quyền, gọi Data/Service, chọn View |
| Model | `Models/Entities` (ánh xạ bảng), `Models/ViewModels` (dữ liệu cho View, kiểm tra nhập liệu bằng DataAnnotations) | Mô tả dữ liệu |
| View | `Views/*`, `Areas/Admin/Views/*` (Razor, Layout, Partial, Tag Helper) | Sinh HTML trả về trình duyệt |
| Service | `Services/FileStorageService`, `ProjectRules`, `IPasswordHasher`, `IHtmlSanitizer` (đăng ký DI trong Program.cs) | Nghiệp vụ dùng chung |
| Data | `Data/AppDbContext`, `Migrations`, `DbSeeder`, `ProjectQueries` | Truy cập SQL Server qua EF Core |

**Bản đồ URL chính**

| URL | Controller / Action | Quyền |
|---|---|---|
| `/`, `/du-an`, `/du-an/{slug}`, `/tin-tuc`, `/lien-he` | Home, Projects, News, Contact | Mọi người |
| `/dang-nhap`, `/dang-ky`, `/dang-xuat` | Account | Mọi người |
| `/tai-khoan`, `/tai-khoan/doi-mat-khau` | Account | Đã đăng nhập |
| `/du-an-cua-toi/...` | MyProjects | Sinh viên |
| `/quan-tri/{Controller}/{Action}` | Areas/Admin | Admin |
| `/loi/{code}` | Home/StatusCodePage | Trang lỗi 400/403/404 |

**Wireframe (H3.10 – H3.15)**: bản phác thảo bố cục trước khi lập trình (đen trắng, không màu). So sánh với ảnh chụp giao diện thực tế ở Chương 4 (mục 4.6).
