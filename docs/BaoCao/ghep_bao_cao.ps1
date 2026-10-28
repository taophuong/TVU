# Ghép báo cáo hoàn chỉnh từ các tệp Word của từng phần (dùng Microsoft Word qua COM)
# - Khung: tệp báo cáo mẫu của Khoa (giữ trang bìa có logo, đầu trang / chân trang, lề trang)
# - Nội dung: PhanMoDau, Chuong1..5, KetLuan, TaiLieuThamKhao, PhuLuc, PhuLuc_ERD (.docx)
# - Tự tạo mục lục, danh mục bảng, danh mục hình; xuất thêm bản PDF
# Cách dùng: powershell -ExecutionPolicy Bypass -File ghep_bao_cao.ps1
$ErrorActionPreference = 'Stop'
$here = Split-Path -Parent $MyInvocation.MyCommand.Path
$root = Resolve-Path (Join-Path $here '..\..\..')
$sample = (Get-ChildItem $root -Filter '*.docx' | Where-Object { $_.Name -like 'B*O C*O*' } | Select-Object -First 1).FullName
$out = Join-Path $here 'BaoCao_170125077_LeVanBinhPhuong.docx'
$pdf = [IO.Path]::ChangeExtension($out, '.pdf')
Copy-Item $sample $out -Force

$sep = (Get-Culture).TextInfo.ListSeparator
$word = New-Object -ComObject Word.Application
$word.Visible = $false
$word.DisplayAlerts = 0
try {
    $doc = $word.Documents.Open($out)

    # ===== 1. Xóa nội dung của báo cáo mẫu, giữ bìa ngoài (section 1) và bìa trong (trang đầu section 2) =====
    # Xóa từ đoạn "NHẬN XÉT CỦA GIÁO VIÊN HƯỚNG DẪN" đến hết, chừa dấu đoạn cuối (giữ thiết lập section, đầu/chân trang)
    $start = $null
    foreach ($p in $doc.Sections(2).Range.Paragraphs) {
        # Tệp mẫu có thể gõ bằng Unicode tổ hợp → chuẩn hóa NFC trước khi so sánh
        if ($p.Range.Text.Normalize([Text.NormalizationForm]::FormC) -like '*NHẬN XÉT CỦA GIÁO VIÊN*') { $start = $p.Range.Start; break }
    }
    if ($start -eq $null) { throw 'Không tìm thấy trang NHẬN XÉT trong tệp mẫu' }
    $doc.Range($start, $doc.Content.End - 1).Delete() | Out-Null

    # ===== 2. Thay thông tin trên bìa, đầu trang, chân trang (kể cả trong khung văn bản) =====
    function Replace-All($find, $repl) {
        foreach ($story in $doc.StoryRanges) {
            $r = $story
            while ($r -ne $null) {
                $f = $r.Find
                $f.ClearFormatting(); $f.Replacement.ClearFormatting()
                [void]$f.Execute($find, $true, $false, $false, $false, $false, $true, 0, $false, $repl, 2)
                $r = $r.NextStoryRange
            }
        }
    }
    # Danh sách cặp (không dùng hashtable vì khóa không phân biệt hoa thường)
    $pairs = @(
        @('XÂY DỰNG WEBSITE BÁN LAPTOP', 'XÂY DỰNG WEBSITE GIỚI THIỆU CÁC SẢN PHẨM KHỞI NGHIỆP CỦA SINH VIÊN TRƯỜNG ĐẠI HỌC TRÀ VINH'),
        @('Xây dựng website bán laptop', 'Website giới thiệu sản phẩm khởi nghiệp SV TVU'),
        @('HỌC KỲ 5, NĂM HỌC 2024', 'HỌC KỲ 6, NĂM HỌC 2026 – 2027'),
        @('ThS. Đoàn Phước Miền', 'TS. Nguyễn Nhứt Lam'),
        @('Phạm Tuấn Anh', 'Lê Văn Bình Phương'),
        @('170122372', '170125077'),
        @('DK22TTC7', 'DK25TTG2'),
        @('TP HCM', 'Trà Vinh'),
        @('tháng 06', 'tháng 10'),
        @('năm 2024', 'năm 2026')
    )
    foreach ($pr in $pairs) { Replace-All $pr[0] $pr[1] }

    # Tên đề tài dài hơn mẫu → giảm cỡ chữ; nới rộng khung "Sinh viên thực hiện" cho đủ họ tên
    # Đầu trang / chân trang của mẫu gõ bằng Unicode tổ hợp nên Find không khớp → ghi trực tiếp
    foreach ($sec in $doc.Sections) {
        foreach ($idx in 1, 2, 3) {
            $h = $sec.Headers($idx)
            if ($h.Exists -and -not $h.LinkToPrevious -and $h.Range.Text.Trim() -ne '') {
                $r = $h.Range; [void]$r.MoveEnd(1, -1)
                $r.Text = "Website giới thiệu sản phẩm khởi nghiệp SV TVU`tGV hướng dẫn: TS. Nguyễn Nhứt Lam"
            }
            $ft = $sec.Footers($idx)
            if ($ft.Exists -and -not $ft.LinkToPrevious) {
                $r = $ft.Range
                if ($r.Find.Execute('SV th')) {        # phần chữ sau trường số trang
                    $r.End = $ft.Range.End - 1
                    $r.Text = 'SV thực hiện: Lê Văn Bình Phương'
                }
            }
        }
    }

    # Trên cả 2 bìa: tên đề tài cỡ 20, thu nhỏ 4 dòng trống ngay sau để dòng ngày tháng không bị đẩy sang trang
    $n = $doc.Paragraphs.Count
    for ($i = 1; $i -le $n; $i++) {
        $p = $doc.Paragraphs.Item($i)
        if ($p.Range.Text -like 'XÂY DỰNG WEBSITE GIỚI THIỆU*') {
            $p.Range.Font.Size = 20
            for ($j = 1; $j -le 4 -and ($i + $j) -le $n; $j++) {
                $q = $doc.Paragraphs.Item($i + $j)
                if ($q.Range.Text.Trim() -eq '') { $q.Range.Font.Size = 4; $q.SpaceBefore = 0; $q.SpaceAfter = 0 }
            }
        }
    }
    foreach ($sh in $doc.Shapes) {
        if ($sh.TextFrame.HasText -and $sh.TextFrame.TextRange.Text -like 'Sinh viên thực hiện*' -and $sh.Width -lt 200) {
            $sh.Left = $sh.Left - (200 - $sh.Width); $sh.Width = 200
        }
    }

    # ===== 3. Chèn nội dung từng phần =====
    function Insert-Part($name, $breakType) {
        $r = $doc.Content; $r.Collapse(0)
        if ($breakType) { $r.InsertBreak($breakType); $r = $doc.Content; $r.Collapse(0) }
        $r.InsertFile((Join-Path $here $name))
        Write-Host "  + $name"
    }
    Insert-Part 'PhanMoDau.docx' 7       # sang trang mới sau bìa trong
    foreach ($n in 'Chuong1', 'Chuong2', 'Chuong3', 'Chuong4', 'Chuong5', 'KetLuan', 'TaiLieuThamKhao', 'PhuLuc') {
        Insert-Part "$n.docx" 7          # 7 = ngắt trang
    }
    Insert-Part 'PhuLuc_ERD.docx' 2      # 2 = ngắt section, sang trang mới
    $last = $doc.Sections($doc.Sections.Count)
    $last.PageSetup.Orientation = 1      # trang ngang cho ERD đầy đủ
    # Section mới kế thừa "trang đầu khác" + viền trang bìa của section 2 → tắt; đánh số trang nối tiếp
    $last.PageSetup.DifferentFirstPageHeaderFooter = $false
    $last.Borders.Enable = $false
    $last.Footers(1).PageNumbers.RestartNumberingAtSection = $false

    # ===== 4. Kiểu chữ: tiêu đề chương, chú thích bảng / hình =====
    foreach ($id in -2, -3, -4) {        # Heading 1..3
        $st = $doc.Styles.Item($id)
        $st.Font.Name = 'Times New Roman'; $st.Font.Color = -16777216; $st.Font.Bold = $true
        $st.Font.Size = $(if ($id -eq -2) { 14 } else { 13 })
        $st.ParagraphFormat.Alignment = $(if ($id -eq -2) { 1 } else { 0 })   # chương: giữa; mục: trái
        $st.ParagraphFormat.FirstLineIndent = 0
    }
    foreach ($id in -20, -21, -22) {     # TOC 1..3
        $st = $doc.Styles.Item($id); $st.Font.Name = 'Times New Roman'; $st.Font.Size = 13
    }
    function New-CaptionStyle($name, $bold, $italic) {
        try { $st = $doc.Styles.Item($name) } catch { $st = $doc.Styles.Add($name, 1) }
        $st.BaseStyle = -1
        $st.Font.Name = 'Times New Roman'; $st.Font.Size = 13; $st.Font.Bold = $bold; $st.Font.Italic = $italic
        $st.ParagraphFormat.Alignment = 1; $st.ParagraphFormat.FirstLineIndent = 0
        $st.ParagraphFormat.SpaceBefore = 6; $st.ParagraphFormat.SpaceAfter = 6
        $st.ParagraphFormat.KeepWithNext = $bold   # chú thích bảng đi liền bảng
    }
    New-CaptionStyle 'Chú thích bảng' $true $false
    New-CaptionStyle 'Chú thích hình' $false $true
    $nb = 0; $nh = 0; $nl = 0
    foreach ($p in $doc.Paragraphs) {
        # Kiểu Heading của tệp mẫu có đánh số tự động → bỏ, vì tiêu đề đã ghi sẵn số (CHƯƠNG 1, 1.1, 1.1.1)
        if ($p.OutlineLevel -le 3 -and $p.Range.ListFormat.ListType -ne 0) { $p.Range.ListFormat.RemoveNumbers(); $nl++ }
        $t = $p.Range.Text
        if ($t -match '^Bảng (\d+|PL)\.\d+\. ') { $p.Style = 'Chú thích bảng'; $nb++ }
        elseif ($t -match '^Hình (\d+|PL)\.\d+\. ') { $p.Style = 'Chú thích hình'; $nh++ }
    }
    Write-Host "  Chú thích: $nb bảng, $nh hình; bỏ đánh số tự động ở $nl tiêu đề"

    # ===== 5. Mục lục, danh mục bảng, danh mục hình =====
    function Add-Toc($placeholder, $styleName) {
        $r = $doc.Content
        if (-not $r.Find.Execute($placeholder)) { throw "Không thấy $placeholder" }
        $r.Text = ''
        if ($styleName) {
            # Chèn trực tiếp mã trường TOC \t: chỉ lấy đoạn có kiểu chú thích, không lẫn tiêu đề mục
            $code = 'TOC \h \z \t "' + $styleName + $sep + '1"'
            [void]$doc.Fields.Add($r, -1, $code, $false)
        } else {
            [void]$doc.TablesOfContents.Add($r, $true, 1, 3)
        }
    }
    Add-Toc '[[TOC]]' $null
    Add-Toc '[[LOT]]' 'Chú thích bảng'
    Add-Toc '[[LOF]]' 'Chú thích hình'
    $doc.Repaginate()
    foreach ($t in $doc.TablesOfContents) { $t.Update() }
    $doc.Fields.Update() | Out-Null

    # ===== 6. Kiểm tra không còn sót nội dung của báo cáo mẫu =====
    # Quét mọi vùng văn bản (thân bài, đầu/chân trang, khung chữ) sau khi chuẩn hóa Unicode
    foreach ($story in $doc.StoryRanges) {
        $r = $story
        while ($r -ne $null) {
            $t = $r.Text.Normalize([Text.NormalizationForm]::FormC)
            foreach ($w in 'laptop', 'Tuấn Anh', 'Phước Miền', '170122372', 'DK22TTC7') {
                if ($t -match [regex]::Escape($w)) { Write-Host "  CẢNH BÁO: còn chữ '$w' (story $($r.StoryType))" }
            }
            $r = $r.NextStoryRange
        }
    }
    foreach ($pr in $pairs) { }
    $dateParas = @(); foreach ($p in $doc.Sections(1).Range.Paragraphs) { if ($p.Range.Text -like 'Trà Vinh, tháng*') { $dateParas += $p.Range.Information(3) } }
    Write-Host ("  Dòng ngày tháng trên bìa ngoài ở trang: " + ($dateParas -join ',') + "; đầu trang section 2: '" + $doc.Sections(2).Headers(1).Range.Text.Trim() + "'")
    Write-Host ("  Số trang: " + $doc.ComputeStatistics(2) + ", số bảng: " + $doc.Tables.Count + ", số hình: " + $doc.InlineShapes.Count)

    $doc.Save()
    $doc.ExportAsFixedFormat($pdf, 17)
    $doc.Close()
    Write-Host "Đã tạo: $out"
    Write-Host "Đã tạo: $pdf"
} finally {
    $word.Quit()
}
