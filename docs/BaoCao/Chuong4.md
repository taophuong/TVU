# CHƯƠNG 4. CÀI ĐẶT VÀ TRIỂN KHAI

Chương này trình bày quá trình hiện thực hóa thiết kế ở Chương 3 thành website hoàn chỉnh: môi trường phát triển, cấu trúc dự án, cấu hình, các module chính kèm đoạn mã minh họa, các biện pháp bảo mật, giao diện từng module và cách triển khai hệ thống.

## 4.1. Môi trường phát triển

Bảng 4.1. Môi trường phát triển

| Thành phần | Thông tin |
|---|---|
| Hệ điều hành | Windows 10 Pro 64-bit |
| Nền tảng | .NET SDK 8.0.419 (ASP.NET Core 8.0, C# 12) |
| Công cụ lập trình | Visual Studio Code 1.143, .NET CLI, công cụ `dotnet-ef` 8.0.10 (khai báo trong `.config/dotnet-tools.json`) |
| Hệ quản trị CSDL | Microsoft SQL Server 2022 Express, instance `.\SQLEXPRESS`, xác thực Windows |
| Trình duyệt kiểm thử | Microsoft Edge, Google Chrome |
| Quản lý mã nguồn | Git, GitHub (https://github.com/taophuong/TVU) |

Bảng 4.2. Các thư viện sử dụng

| Thư viện | Phiên bản | Nguồn | Mục đích |
|---|---|---|---|
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.10 | NuGet | Kết nối SQL Server qua EF Core |
| Microsoft.EntityFrameworkCore.Design, Tools | 8.0.10 | NuGet | Tạo và áp dụng migration |
| HtmlSanitizer | 8.1.870 | NuGet | Lọc mã HTML không an toàn |
| Bootstrap, jQuery, jQuery Validation | 5.1.0 / 3.6.0 / 1.19.5 | wwwroot/lib | Giao diện, AJAX, kiểm tra dữ liệu phía trình duyệt |
| Bootstrap Icons | 1.11.3 | CDN jsDelivr | Biểu tượng |
| Quill | 1.3.7 | CDN jsDelivr | Trình soạn thảo nội dung |
| Chart.js | 4.4.1 | CDN jsDelivr | Biểu đồ thống kê |
| Be Vietnam Pro | – | Google Fonts | Phông chữ hỗ trợ tốt tiếng Việt |

## 4.2. Cấu trúc dự án

Mã nguồn được tổ chức thành một solution `TVUStartup.sln` gồm một project web `TVUStartup.Web`, phân chia thư mục theo đúng các lớp của kiến trúc đã thiết kế ở mục 3.4.

```
PkuongProject/
├── TVUStartup.sln
├── TVUStartup.Web/
│   ├── Areas/Admin/            Khu vực quản trị: Controllers, Views, Models
│   ├── Controllers/            Home, Projects, News, Contact, Account, MyProjects
│   ├── Data/                   AppDbContext, Migrations, DbSeeder, ProjectQueries, LookupQueries
│   ├── Helpers/                DisplayHelper, SlugHelper, UserExtensions
│   ├── Models/Entities/        14 lớp thực thể ánh xạ 14 bảng
│   ├── Models/ViewModels/      Dữ liệu cho View và form nhập liệu
│   ├── Services/               FileStorageService, ProjectRules
│   ├── Views/                  Giao diện Razor (Shared: Layout, Partial View)
│   ├── wwwroot/                css, js, lib, uploads (ảnh)
│   ├── appsettings.json        Cấu hình chuỗi kết nối
│   └── Program.cs              Đăng ký dịch vụ và chuỗi middleware
├── database/                   Script SQL tạo cơ sở dữ liệu
├── docs/                       Tài liệu thiết kế, sơ đồ
├── tests/                      Kịch bản kiểm thử tự động
└── README.md                   Hướng dẫn cài đặt, tài khoản demo
```

Bảng 4.3. Vai trò của các thư mục chính

| Thư mục | Thành phần tiêu biểu | Vai trò |
|---|---|---|
| Controllers | `ProjectsController`, `MyProjectsController`, `AccountController` | Xử lý yêu cầu phía người dùng (khách, sinh viên) |
| Areas/Admin | `AdminBaseController`, `DashboardController`, `ProjectsController`… | Xử lý yêu cầu khu vực quản trị |
| Models/Entities | `Project`, `Account`, `Comment`, `Vote`… | Lớp thực thể, ánh xạ bảng dữ liệu |
| Models/ViewModels | `ProjectFormModel`, `LoginViewModel`… | Dữ liệu truyền giữa Controller và View, quy tắc kiểm tra dữ liệu |
| Data | `AppDbContext`, `DbSeeder` | Cấu hình ánh xạ (Fluent API), migration, dữ liệu mẫu |
| Services | `FileStorageService`, `ProjectRules` | Lưu tệp tải lên, quy tắc chuyển trạng thái dự án |
| Views | `_Layout`, `_ProjectCard`, `Detail.cshtml`… | Giao diện Razor |

Về quy mô, mã nguồn gồm 48 tệp C# (khoảng 4.200 dòng, không tính mã migration sinh tự động), 44 tệp giao diện Razor và khoảng 500 dòng CSS tự viết.

## 4.3. Cấu hình

### 4.3.1. Chuỗi kết nối cơ sở dữ liệu

Chuỗi kết nối được khai báo trong tệp `appsettings.json`, tách khỏi mã nguồn để dễ thay đổi khi chuyển sang máy khác:

```
"ConnectionStrings": {
  "DefaultConnection": "Server=.\\SQLEXPRESS;Database=TVUStartupDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

### 4.3.2. Đăng ký dịch vụ và chuỗi middleware

Tệp `Program.cs` đăng ký các dịch vụ vào bộ chứa Dependency Injection, cấu hình xác thực bằng cookie và khai báo chuỗi middleware (trích):

```
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IPasswordHasher<Account>, PasswordHasher<Account>>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/dang-nhap";
        options.AccessDeniedPath = "/loi/403";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });
...
app.UseStatusCodePagesWithReExecute("/loi/{0}");
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapAreaControllerRoute("admin", "Admin", "quan-tri/{controller=Dashboard}/{action=Index}/{id?}");
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
```

Ngoài ra, ứng dụng cấu hình bộ mã hóa HTML cho phép toàn bộ bảng mã Unicode để ký tự tiếng Việt không bị chuyển thành dạng thực thể (`&#x...;`), giúp mã trang gọn và dễ đọc.

### 4.3.3. Tạo cơ sở dữ liệu và dữ liệu mẫu

Cơ sở dữ liệu được tạo từ migration `InitialCreate` bằng lệnh `dotnet ef database update`. Khi khởi động, lớp `DbSeeder` gọi `Database.MigrateAsync()` để tự động áp dụng migration, sau đó nạp dữ liệu mẫu nếu cơ sở dữ liệu còn trống (vai trò, khoa, lĩnh vực, tài khoản, 13 dự án, bình luận, tin tức, banner). Nhờ vậy, người dùng chỉ cần cấu hình chuỗi kết nối và chạy lệnh `dotnet run` là có ngay hệ thống với dữ liệu minh họa. Script SQL tương đương được xuất bằng lệnh `dotnet ef migrations script --idempotent` và lưu tại `database/TVUStartupDB_schema.sql`.

Cấu hình ánh xạ chi tiết được viết bằng Fluent API trong `AppDbContext`, ví dụ với bảng `Project`:

```
modelBuilder.Entity<Project>(e =>
{
    e.ToTable("Project");
    e.Property(x => x.Slug).HasMaxLength(170).IsUnicode(false).IsRequired();
    e.Property(x => x.Status).HasColumnType("tinyint").HasDefaultValue(ProjectStatus.Nhap);
    e.HasIndex(x => x.Slug).IsUnique();
    e.HasOne(x => x.Category).WithMany(c => c.Projects)
        .HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
});
```

## 4.4. Các module chính

### 4.4.1. Module tài khoản: đăng ký, đăng nhập, phân quyền

Khi đăng nhập, `AccountController` tìm tài khoản theo email, dùng `IPasswordHasher` so khớp mật khẩu với chuỗi băm. Thông báo lỗi không cho biết cụ thể sai email hay sai mật khẩu để tránh bị dò tài khoản (trích):

```
var email = model.Email.Trim().ToLowerInvariant();
var account = await _db.Accounts.Include(a => a.Role).FirstOrDefaultAsync(a => a.Email == email);

var result = account == null
    ? PasswordVerificationResult.Failed
    : _hasher.VerifyHashedPassword(account, account.PasswordHash, model.Password);
if (account == null || result == PasswordVerificationResult.Failed)
{
    ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
    return View(model);
}
if (account.Status == AccountStatus.BiKhoa) { ... }
await SignInAsync(account, model.RememberMe);
```

Hàm `SignInAsync` tạo danh sách claim và ghi cookie đăng nhập:

```
var claims = new List<Claim>
{
    new(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
    new(ClaimTypes.Name, account.FullName),
    new(ClaimTypes.Email, account.Email),
    new(ClaimTypes.Role, role.RoleName)
};
var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
    new ClaimsPrincipal(identity), new AuthenticationProperties { IsPersistent = persistent });
```

Khi đăng ký, mật khẩu được băm bằng `_hasher.HashPassword(...)` trước khi lưu; email được kiểm tra trùng. Để tài khoản bị khóa không tiếp tục sử dụng cookie cũ, sự kiện `OnValidatePrincipal` của cookie kiểm tra lại trạng thái tài khoản ở mỗi yêu cầu và đăng xuất ngay nếu tài khoản bị khóa.

### 4.4.2. Module tìm kiếm, lọc dự án

Trang danh sách dự án kết hợp nhiều điều kiện lọc trên cùng một truy vấn `IQueryable`, chỉ thực thi xuống cơ sở dữ liệu khi lấy kết quả. Tìm kiếm không phân biệt dấu được cài đặt bằng cách áp dụng collation `Vietnamese_CI_AI` ngay trong truy vấn LINQ:

```
public static IQueryable<Project> Search(this IQueryable<Project> query, string keyword)
{
    var pattern = $"%{keyword.Trim()}%";
    const string collation = "Vietnamese_CI_AI";
    return query.Where(p =>
        EF.Functions.Like(EF.Functions.Collate(p.ProjectName, collation), pattern) ||
        EF.Functions.Like(EF.Functions.Collate(p.Slogan!, collation), pattern) ||
        EF.Functions.Like(EF.Functions.Collate(p.ShortDescription, collation), pattern));
}
```

Kết quả được phân trang (9 dự án / trang) và chỉ chọn các cột cần cho thẻ dự án bằng phép chiếu `Select` sang `ProjectCardViewModel`, giảm lượng dữ liệu truy vấn.

### 4.4.3. Module chi tiết dự án và tương tác

Trang chi tiết dùng đường dẫn thân thiện `/du-an/{slug}`; lượt xem được tăng trực tiếp trong cơ sở dữ liệu bằng `ExecuteUpdateAsync` để không bị ghi đè khi nhiều người xem cùng lúc. Chủ dự án và Quản trị viên được xem trước dự án chưa công khai và thấy cả các phiên bản nội bộ.

Chức năng bình chọn gửi yêu cầu AJAX, máy chủ bật / tắt lượt bình chọn và trả về JSON để cập nhật nút mà không tải lại trang (trích phía máy chủ):

```
var vote = await _db.Votes.FindAsync(id, accountId);
if (vote == null) { _db.Votes.Add(new Vote { ProjectId = id, AccountId = accountId, CreatedAt = DateTime.Now }); voted = true; }
else { _db.Votes.Remove(vote); voted = false; }
await _db.SaveChangesAsync();
if (IsAjax())
    return Json(new { voted, count = await _db.Votes.CountAsync(v => v.ProjectId == id) });
```

Bình luận được kiểm tra các quy tắc nghiệp vụ: chủ dự án không tự chấm sao, mỗi tài khoản chấm sao một lần; chỉ chủ dự án và Quản trị viên được trả lời. Form liên hệ hợp tác cho phép khách gửi yêu cầu không cần đăng nhập.

### 4.4.4. Module quản lý dự án của sinh viên

`MyProjectsController` gắn thuộc tính `[Authorize(Roles = Role.SinhVien)]` và luôn lấy dự án kèm điều kiện `OwnerId == tài khoản hiện tại`, nên sinh viên không thể sửa dự án của người khác dù đổi mã dự án trên URL. Khi tạo dự án, đường dẫn được sinh từ tên tiếng Việt (bỏ dấu, ví dụ “Dự án Kiểm Thử” → `du-an-kiem-thu`) và tự thêm hậu tố nếu trùng. Chức năng gửi duyệt kiểm tra điều kiện tối thiểu:

```
var missing = new List<string>();
if (string.IsNullOrEmpty(project.CoverImage)) missing.Add("ảnh bìa");
if (project.Features.Count == 0) missing.Add("ít nhất 1 tính năng nổi bật");
if (missing.Count > 0)
{
    TempData["Error"] = "Chưa thể gửi duyệt, dự án còn thiếu: " + string.Join(", ", missing) + ".";
    return RedirectToAction(nameof(Edit), new { id });
}
project.Status = ProjectStatus.ChoDuyet;
project.RejectReason = null;
await _db.SaveChangesAsync();
```

Các quy tắc “được sửa / được xóa / được gửi duyệt / được rút lại” theo từng trạng thái được tập trung trong lớp tĩnh `ProjectRules`, dùng chung cho cả controller và giao diện (ẩn hoặc hiện các nút tương ứng).

### 4.4.5. Module quản trị

Tất cả controller khu vực quản trị kế thừa `AdminBaseController`:

```
[Area("Admin")]
[Authorize(Roles = Role.Admin)]
[AutoValidateAntiforgeryToken]
public abstract class AdminBaseController : Controller { ... }
```

Chức năng duyệt dự án chỉ chấp nhận dự án đang Chờ duyệt, ghi nhận thời điểm và người duyệt:

```
if (project.Status != ProjectStatus.ChoDuyet) { Error("Chỉ duyệt được dự án đang ở trạng thái Chờ duyệt."); return Back(returnUrl); }
project.Status = ProjectStatus.DaDuyet;
project.ApprovedAt = DateTime.Now;
project.ApprovedBy = CurrentId;
await _db.SaveChangesAsync();
```

Trang thống kê dùng truy vấn nhóm (`GroupBy`) thực hiện trực tiếp trong SQL Server, sau đó truyền dữ liệu sang Chart.js để vẽ biểu đồ:

```
var byCategory = await approved
    .GroupBy(p => p.Category.CategoryName)
    .Select(g => new { g.Key, Count = g.Count() })
    .OrderByDescending(x => x.Count)
    .ToListAsync();
```

Các chức năng quản lý lĩnh vực, khoa, tin tức, banner được cài đặt theo mẫu CRUD thống nhất (danh sách – form thêm / sửa – xóa); thao tác xóa kiểm tra ràng buộc trước, ví dụ không cho xóa lĩnh vực đang có dự án.

### 4.4.6. Module tải ảnh lên

Dịch vụ `FileStorageService` kiểm tra tệp trước khi lưu: định dạng, dung lượng tối đa 2 MB và chữ ký nhận dạng ở các byte đầu tệp; tệp hợp lệ được đổi tên ngẫu nhiên bằng GUID khi lưu vào `wwwroot/uploads`:

```
if (file.Length > MaxBytes) return "Ảnh không được vượt quá 2 MB.";
var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
if (!AllowedExtensions.Contains(ext)) return "Chỉ chấp nhận ảnh .jpg, .png hoặc .webp.";
using var stream = file.OpenReadStream();
var header = new byte[12];
var read = stream.Read(header, 0, header.Length);
if (read < 4 || !IsImageHeader(header)) return "Tệp tải lên không phải là ảnh hợp lệ.";
```

## 4.5. Bảo mật

Bảng 4.4. Các biện pháp bảo mật đã cài đặt

| Nguy cơ | Biện pháp | Vị trí cài đặt |
|---|---|---|
| Lộ mật khẩu | Băm PBKDF2 có salt bằng `PasswordHasher`; không lưu mật khẩu gốc | `AccountController`, `DbSeeder` |
| Truy cập trái phép | Cookie Authentication, `[Authorize(Roles = ...)]`; trang 403 khi không đủ quyền | `Program.cs`, `AdminBaseController`, `MyProjectsController` |
| Sửa dữ liệu của người khác | Luôn lọc theo chủ sở hữu khi truy vấn dự án của sinh viên | `MyProjectsController.LoadOwnAsync` |
| Tài khoản bị khóa vẫn dùng phiên cũ | Kiểm tra trạng thái tài khoản ở mỗi yêu cầu (`OnValidatePrincipal`) | `Program.cs` |
| SQL Injection | Truy vấn qua EF Core / LINQ, tham số hóa tự động | Toàn bộ tầng dữ liệu |
| XSS | Razor tự mã hóa đầu ra; lọc HTML nội dung dự án, tin tức bằng HtmlSanitizer | Views, `MyProjectsController`, `Admin/NewsController` |
| CSRF | `[ValidateAntiForgeryToken]`, `[AutoValidateAntiforgeryToken]` trên mọi form POST | Các controller |
| Tải tệp độc hại | Giới hạn định dạng, dung lượng, kiểm tra chữ ký tệp, đổi tên ngẫu nhiên | `FileStorageService` |
| Chuyển hướng mở (open redirect) | Chỉ chuyển hướng tới `returnUrl` nội bộ (`Url.IsLocalUrl`); banner chỉ nhận liên kết nội bộ hoặc http(s) | `AccountController`, `BannersController` |

Bộ lọc HTML được đăng ký một lần ở dạng Singleton, loại bỏ thuộc tính `style` và thẻ `img` để nội dung chỉ giữ các định dạng văn bản an toàn:

```
builder.Services.AddSingleton<IHtmlSanitizer>(_ =>
{
    var sanitizer = new HtmlSanitizer();
    sanitizer.AllowedAttributes.Remove("style");
    sanitizer.AllowedTags.Remove("img");
    return sanitizer;
});
```

## 4.6. Giao diện

### 4.6.1. Giao diện dành cho khách

Trang chủ (Hình 4.1) gồm phần giới thiệu kèm ô tìm kiếm và bốn số liệu thống kê, dự án nổi bật, các lĩnh vực, dự án mới nhất, banner sự kiện và tin tức.

![Hình 4.1. Giao diện trang chủ](../screenshots/k01_trang_chu.png)

Trang danh sách dự án (Hình 4.2) có bộ lọc theo từ khóa, lĩnh vực, khoa, giai đoạn và ô sắp xếp; mỗi dự án hiển thị dạng thẻ với ảnh bìa, giai đoạn, lượt xem, lượt yêu thích. Hình 4.3 cho thấy tìm kiếm từ khóa không dấu “tuy bien” vẫn tìm được dự án Zalord.

![Hình 4.2. Giao diện danh sách dự án](../screenshots/k02_danh_sach_du_an.png)

![Hình 4.3. Kết quả tìm kiếm không phân biệt dấu](../screenshots/k03_tim_kiem_khong_dau.png)

Trang chi tiết dự án (Hình 4.4) hiển thị logo, slogan, các nhãn phân loại, nút truy cập trang sản phẩm; nội dung giới thiệu, tính năng nổi bật, hình ảnh, phiên bản phát hành và đội ngũ thực hiện. Khách bấm “Liên hệ hợp tác” để mở form gửi yêu cầu (Hình 4.5).

![Hình 4.4. Giao diện chi tiết dự án Zalord](../screenshots/k04_chi_tiet_du_an.png)

![Hình 4.5. Form gửi liên hệ hợp tác](../screenshots/k05_lien_he_hop_tac.png)

![Hình 4.6. Giao diện tin tức và sự kiện](../screenshots/k06_tin_tuc.png)

Trang đăng nhập (Hình 4.7) và đăng ký (Hình 4.8) kiểm tra dữ liệu ngay trên trình duyệt; đường dẫn không tồn tại hiển thị trang lỗi 404 tiếng Việt (Hình 4.9).

![Hình 4.7. Giao diện đăng nhập](../screenshots/k07_dang_nhap.png)

![Hình 4.8. Giao diện đăng ký tài khoản sinh viên](../screenshots/k08_dang_ky.png)

![Hình 4.9. Trang báo lỗi 404](../screenshots/k09_trang_404.png)

Giao diện tự co giãn trên điện thoại: menu thu gọn thành nút, các khối nội dung xếp thành một cột (Hình 4.10, Hình 4.11).

![Hình 4.10. Trang chủ trên điện thoại](../screenshots/k10_dien_thoai_trang_chu.png)

![Hình 4.11. Trang chi tiết dự án trên điện thoại](../screenshots/k11_dien_thoai_chi_tiet.png)

### 4.6.2. Giao diện dành cho sinh viên

Sau khi đăng nhập, sinh viên quản lý các dự án của mình tại trang “Dự án của tôi” (Hình 4.12) với trạng thái, số liệu và các nút thao tác phù hợp từng trạng thái.

![Hình 4.12. Giao diện “Dự án của tôi”](../screenshots/s01_du_an_cua_toi.png)

Form đăng dự án mới (Hình 4.13) gồm thông tin chung, ảnh logo, ảnh bìa và trình soạn thảo phần giới thiệu. Sau khi lưu nháp, sinh viên bổ sung tính năng, hình ảnh, phiên bản, thành viên qua các tab (Hình 4.14, Hình 4.15).

![Hình 4.13. Form đăng dự án mới](../screenshots/s02_dang_du_an_moi.png)

![Hình 4.14. Tab quản lý tính năng nổi bật](../screenshots/s03_sua_du_an_tinh_nang.png)

![Hình 4.15. Tab quản lý hình ảnh dự án](../screenshots/s04_sua_du_an_hinh_anh.png)

Chủ dự án xem trước dự án chưa công khai với thanh thông báo trạng thái (Hình 4.16). Trên dự án đã công khai, sinh viên đánh giá sao, bình luận; chủ dự án trả lời bình luận với nhãn “Chủ dự án” (Hình 4.17).

![Hình 4.16. Xem trước dự án đang ở trạng thái Nháp](../screenshots/s06_xem_truoc_du_an_nhap.png)

![Hình 4.17. Khu vực bình luận và đánh giá](../screenshots/s05_binh_luan_danh_gia.png)

![Hình 4.18. Danh sách liên hệ hợp tác gửi tới dự án của sinh viên](../screenshots/s07_lien_he_cua_sinh_vien.png)

![Hình 4.19. Giao diện hồ sơ cá nhân](../screenshots/s08_ho_so_ca_nhan.png)

### 4.6.3. Giao diện quản trị

Khu vực quản trị có thanh menu bên trái và layout riêng. Trang tổng quan (Hình 4.20) hiển thị 7 chỉ số, 4 biểu đồ, danh sách dự án chờ duyệt, liên hệ mới và dự án được xem nhiều nhất.

![Hình 4.20. Trang tổng quan (Dashboard) của quản trị viên](../screenshots/a01_dashboard.png)

Trang duyệt dự án (Hình 4.21) lọc theo trạng thái, đưa dự án chờ duyệt lên đầu; nút thao tác thay đổi theo trạng thái. Khi từ chối, quản trị viên bắt buộc nhập lý do trong hộp thoại (Hình 4.22).

![Hình 4.21. Giao diện duyệt dự án](../screenshots/a02_duyet_du_an.png)

![Hình 4.22. Hộp thoại từ chối dự án](../screenshots/a03_tu_choi_du_an.png)

![Hình 4.23. Giao diện xử lý liên hệ hợp tác](../screenshots/a04_lien_he_hop_tac.png)

![Hình 4.24. Giao diện kiểm duyệt bình luận](../screenshots/a05_binh_luan.png)

![Hình 4.25. Form soạn tin tức, sự kiện](../screenshots/a06_tin_tuc_form.png)

![Hình 4.26. Giao diện quản lý banner trang chủ](../screenshots/a07_banner.png)

![Hình 4.27. Giao diện quản lý lĩnh vực](../screenshots/a08_linh_vuc.png)

![Hình 4.28. Giao diện quản lý tài khoản](../screenshots/a09_tai_khoan.png)

## 4.7. Triển khai

**Chạy trên máy phát triển.** Hệ thống được chạy và kiểm thử trên máy cá nhân theo các bước: (1) cài .NET SDK 8 và SQL Server; (2) sửa chuỗi kết nối trong `appsettings.json`; (3) chạy lệnh `dotnet run --project TVUStartup.Web`; cơ sở dữ liệu và dữ liệu mẫu được tạo tự động ở lần chạy đầu. Hướng dẫn chi tiết và tài khoản demo được ghi trong tệp `README.md` và Phụ lục.

**Đóng gói để triển khai.** Lệnh `dotnet publish TVUStartup.Web -c Release -o publish` đã được chạy thử thành công, tạo thư mục triển khai (khoảng 27 MB) gồm tệp `TVUStartup.Web.dll`, các thư viện phụ thuộc và thư mục `wwwroot`. Thư mục này có thể triển khai lên máy chủ theo một trong các cách:

- **IIS trên Windows Server:** cài ASP.NET Core Hosting Bundle phiên bản tương ứng, tạo Application Pool ở chế độ “No Managed Code”, trỏ website tới thư mục publish, cấu hình chuỗi kết nối tới SQL Server của máy chủ và cấp quyền ghi cho thư mục `wwwroot/uploads`.
- **Docker:** đóng gói ứng dụng vào image dựa trên `mcr.microsoft.com/dotnet/aspnet:8.0`, cơ sở dữ liệu chạy ở container SQL Server riêng.
- **Dịch vụ hosting hỗ trợ ASP.NET Core:** tải thư mục publish lên và cấu hình chuỗi kết nối.

Trong phạm vi đồ án, hệ thống chưa được triển khai lên máy chủ Internet; việc triển khai thực tế được đề xuất trong hướng phát triển.

**Quản lý mã nguồn.** Toàn bộ mã nguồn, script cơ sở dữ liệu, tài liệu và kịch bản kiểm thử được quản lý bằng Git và lưu trữ tại kho GitHub https://github.com/taophuong/TVU.

## Tiểu kết chương 4

Chương 4 đã trình bày môi trường phát triển, cấu trúc dự án và cấu hình; cài đặt các module chính với các đoạn mã minh họa tiêu biểu; các biện pháp bảo mật đã áp dụng; giao diện thực tế của 28 màn hình thuộc ba nhóm người dùng và phương án triển khai hệ thống. Kết quả kiểm thử và đánh giá được trình bày ở Chương 5.
