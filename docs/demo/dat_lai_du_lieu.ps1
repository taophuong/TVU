# Đưa máy về trạng thái sẵn sàng quay video demo
# Dừng website, xóa CSDL TVUStartupDB và ảnh đã tải lên khi chạy thử, rồi build sẵn mã nguồn và tạo lại CSDL cùng dữ liệu mẫu
# để trong video CSDL đã có sẵn dữ liệu mẫu, chỉ cần chạy website (không phải chờ build, không hiện cảnh báo build).
# Cách dùng:
#   chuột phải → Run with PowerShell           : dọn dẹp, build, tạo lại CSDL (dùng trước khi quay)
#   powershell -File dat_lai_du_lieu.ps1 -ChayWeb : dọn dẹp xong chạy luôn website (dùng khi chỉ muốn test)
param([switch]$ChayWeb)
$ErrorActionPreference = 'Stop'
$web = Join-Path $PSScriptRoot '..\..\TVUStartup.Web' | Resolve-Path

Write-Host '1/4 Dừng website nếu đang chạy...'
Get-Process TVUStartup.Web -ErrorAction SilentlyContinue | Stop-Process -Force

Write-Host '2/4 Xóa CSDL TVUStartupDB...'
Push-Location $web
try {
    dotnet tool restore | Out-Null   # cài công cụ dotnet-ef nếu máy chưa có
    dotnet ef database drop --force
} finally { Pop-Location }

Write-Host '3/4 Xóa ảnh tải lên khi chạy thử (giữ ảnh mẫu trong uploads\sample)...'
Get-ChildItem (Join-Path $web 'wwwroot\uploads') -Exclude sample | Remove-Item -Recurse -Force

Write-Host '4/4 Build mã nguồn...'
dotnet build $web -v q -nologo

if ($ChayWeb) {
    Write-Host 'Chạy website tại http://localhost:5080 (nhấn Ctrl+C để dừng)...'
    dotnet run --project $web --no-build --urls http://localhost:5080
} else {
    # Chạy website một lần để tạo CSDL (migration) và nạp dữ liệu mẫu, rồi dừng website
    Write-Host 'Tạo CSDL và nạp dữ liệu mẫu (chạy website một lần rồi dừng)...'
    $p = Start-Process dotnet -ArgumentList "run --project `"$web`" --no-build --urls http://localhost:5080" -WindowStyle Hidden -PassThru
    $ok = $false
    for ($i = 0; $i -lt 60 -and -not $ok; $i++) {
        Start-Sleep -Seconds 2
        try { $ok = (Invoke-WebRequest http://localhost:5080/ -UseBasicParsing -TimeoutSec 5).StatusCode -eq 200 } catch { }
    }
    Get-Process TVUStartup.Web -ErrorAction SilentlyContinue | Stop-Process -Force
    if (-not $p.HasExited) { Stop-Process -Id $p.Id -Force }
    if (-not $ok) { Write-Host 'Website không khởi động được, kiểm tra SQL Server và chuỗi kết nối.' -ForegroundColor Red }
    Write-Host ''
    Write-Host 'Đã sẵn sàng quay: CSDL đã có dữ liệu mẫu, website chưa chạy.' -ForegroundColor Green
    Read-Host 'Nhấn Enter để đóng cửa sổ'
}
