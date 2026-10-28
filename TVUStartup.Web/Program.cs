using System.Text.Encodings.Web;
using System.Text.Unicode;
using Ganss.Xss;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.WebEncoders;
using Microsoft.EntityFrameworkCore;
using TVUStartup.Web.Data;
using TVUStartup.Web.Helpers;
using TVUStartup.Web.Models.Entities;
using TVUStartup.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// ===== Đăng ký dịch vụ (Dependency Injection) =====
builder.Services.AddControllersWithViews();

// Razor không mã hóa ký tự tiếng Việt thành &#x...; (HTML gọn, dễ đọc); ký tự đặc biệt < > & " vẫn được mã hóa
builder.Services.Configure<WebEncoderOptions>(options =>
    options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All));

// EF Core + SQL Server, chuỗi kết nối đọc từ appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Băm mật khẩu PBKDF2 (có salt) của ASP.NET Core
builder.Services.AddScoped<IPasswordHasher<Account>, PasswordHasher<Account>>();

// Lưu ảnh tải lên
builder.Services.AddScoped<IFileStorageService, FileStorageService>();

// Lọc HTML do người dùng nhập (chống XSS) – chỉ giữ các thẻ định dạng an toàn
builder.Services.AddSingleton<IHtmlSanitizer>(_ =>
{
    var sanitizer = new HtmlSanitizer();
    sanitizer.AllowedAttributes.Remove("style");
    sanitizer.AllowedTags.Remove("img");
    return sanitizer;
});

// Xác thực bằng Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/dang-nhap";
        options.LogoutPath = "/dang-xuat";
        options.AccessDeniedPath = "/loi/403";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;

        // Mỗi request kiểm tra lại tài khoản: bị khóa hoặc bị xóa thì đăng xuất ngay
        options.Events.OnValidatePrincipal = async context =>
        {
            var accountId = context.Principal?.GetAccountId();
            var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
            var active = accountId.HasValue &&
                         await db.Accounts.AnyAsync(a => a.AccountId == accountId && a.Status == AccountStatus.HoatDong);
            if (!active)
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Tạo / cập nhật CSDL và nạp dữ liệu mẫu khi khởi động
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Account>>();
    await DbSeeder.SeedAsync(db, hasher);
}

// ===== Pipeline xử lý request =====
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/loi/{0}");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Khu vực quản trị: /quan-tri/{controller}/{action}/{id?}
app.MapAreaControllerRoute(
    name: "admin",
    areaName: "Admin",
    pattern: "quan-tri/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
