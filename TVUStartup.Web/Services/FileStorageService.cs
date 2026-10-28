namespace TVUStartup.Web.Services;

/// <summary>Lưu / xóa ảnh do người dùng tải lên trong thư mục wwwroot/uploads</summary>
public interface IFileStorageService
{
    /// <summary>Lưu ảnh vào wwwroot/uploads/{folder}, trả về đường dẫn web (vd: /uploads/projects/abc.png)</summary>
    Task<string> SaveImageAsync(IFormFile file, string folder);

    /// <summary>Xóa tệp đã tải lên (bỏ qua nếu không phải tệp trong /uploads/)</summary>
    void Delete(string? webPath);

    /// <summary>Kiểm tra tệp có phải ảnh hợp lệ không; trả về thông báo lỗi hoặc null nếu hợp lệ</summary>
    string? Validate(IFormFile? file);
}

public class FileStorageService : IFileStorageService
{
    public const long MaxBytes = 2 * 1024 * 1024; // 2 MB
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

    private readonly IWebHostEnvironment _env;

    public FileStorageService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public string? Validate(IFormFile? file)
    {
        if (file == null || file.Length == 0) return "Vui lòng chọn tệp ảnh.";
        if (file.Length > MaxBytes) return "Ảnh không được vượt quá 2 MB.";

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext)) return "Chỉ chấp nhận ảnh .jpg, .png hoặc .webp.";

        // Kiểm tra vài byte đầu (magic number) để tránh đổi đuôi tệp độc hại thành .png
        using var stream = file.OpenReadStream();
        var header = new byte[12];
        var read = stream.Read(header, 0, header.Length);
        if (read < 4 || !IsImageHeader(header)) return "Tệp tải lên không phải là ảnh hợp lệ.";

        return null;
    }

    public async Task<string> SaveImageAsync(IFormFile file, string folder)
    {
        var error = Validate(file);
        if (error != null) throw new InvalidOperationException(error);

        // Đặt tên mới ngẫu nhiên, không dùng tên gốc của người dùng
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid():N}{ext}";
        var dir = Path.Combine(_env.WebRootPath, "uploads", folder);
        Directory.CreateDirectory(dir);

        await using (var output = File.Create(Path.Combine(dir, fileName)))
        {
            await file.CopyToAsync(output);
        }
        return $"/uploads/{folder}/{fileName}";
    }

    public void Delete(string? webPath)
    {
        // Chỉ xóa tệp do người dùng tải lên, không xóa ảnh dữ liệu mẫu (/uploads/sample/...)
        if (string.IsNullOrEmpty(webPath) || !webPath.StartsWith("/uploads/") || webPath.StartsWith("/uploads/sample/"))
            return;

        var fullPath = Path.GetFullPath(Path.Combine(_env.WebRootPath, webPath.TrimStart('/')));
        var uploadsRoot = Path.GetFullPath(Path.Combine(_env.WebRootPath, "uploads"));
        if (fullPath.StartsWith(uploadsRoot) && File.Exists(fullPath))
            File.Delete(fullPath);
    }

    private static bool IsImageHeader(byte[] h) =>
        (h[0] == 0xFF && h[1] == 0xD8 && h[2] == 0xFF) ||                       // JPEG
        (h[0] == 0x89 && h[1] == 0x50 && h[2] == 0x4E && h[3] == 0x47) ||      // PNG
        (h[0] == 0x52 && h[1] == 0x49 && h[2] == 0x46 && h[3] == 0x46 &&        // WEBP: "RIFF....WEBP"
         h[8] == 0x57 && h[9] == 0x45 && h[10] == 0x42 && h[11] == 0x50);
}
