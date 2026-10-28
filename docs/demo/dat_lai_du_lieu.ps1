# Đặt lại dữ liệu mẫu trước khi quay video demo
# Xóa CSDL TVUStartupDB và ảnh đã tải lên khi chạy thử, sau đó chạy lại website:
# lúc khởi động, website tự tạo lại CSDL bằng migration và nạp dữ liệu mẫu.
# Cách dùng: chuột phải → Run with PowerShell (hoặc: powershell -File dat_lai_du_lieu.ps1)
$ErrorActionPreference = 'Stop'
$web = Join-Path $PSScriptRoot '..\..\TVUStartup.Web' | Resolve-Path

Write-Host '1/4 Dừng website nếu đang chạy...'
Get-Process TVUStartup.Web -ErrorAction SilentlyContinue | Stop-Process -Force

Write-Host '2/4 Xóa CSDL TVUStartupDB...'
Push-Location $web
try { dotnet ef database drop --force } finally { Pop-Location }

Write-Host '3/4 Xóa ảnh tải lên khi chạy thử (giữ ảnh mẫu trong uploads\sample)...'
Get-ChildItem (Join-Path $web 'wwwroot\uploads') -Exclude sample | Remove-Item -Recurse -Force

Write-Host '4/4 Chạy website tại http://localhost:5080 (nhấn Ctrl+C để dừng)...'
dotnet run --project $web --urls http://localhost:5080
