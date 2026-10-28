# Chuyển các chương viết bằng Markdown (Chuong*.md) sang Word (.docx)
# B1: Markdown → HTML định dạng báo cáo (A4, Times New Roman 13pt, giãn dòng 1,5)
# B2: Microsoft Word mở HTML, nhúng ảnh vào tệp và lưu thành .docx (qua PowerShell COM)
# Cách dùng: python xuat_docx.py Chuong1.md Chuong2.md [--ghi-de]
import html
import os
import re
import subprocess
import sys

from PIL import Image

HERE = os.path.dirname(os.path.abspath(__file__))
MAX_IMG_PX = 605  # ≈ 16 cm khi Word quy đổi 96 dpi

CSS = """
@page WordSection1 { size: 21cm 29.7cm; margin: 2cm 2cm 2cm 3cm; }
div.WordSection1 { page: WordSection1; }
body { font-family: 'Times New Roman'; font-size: 13pt; }
p { margin: 0 0 6pt 0; line-height: 150%; text-align: justify; text-indent: 1cm; }
h1 { font-family: 'Times New Roman'; font-size: 14pt; font-weight: bold; text-align: center; text-transform: uppercase; margin: 0 0 18pt 0; color: #000; }
h2 { font-family: 'Times New Roman'; font-size: 13pt; font-weight: bold; margin: 12pt 0 6pt 0; color: #000; }
h3 { font-family: 'Times New Roman'; font-size: 13pt; font-weight: bold; font-style: italic; margin: 10pt 0 6pt 0; color: #000; }
ul, ol { margin: 0 0 6pt 0; }
li { line-height: 150%; text-align: justify; margin-bottom: 3pt; }
table { border-collapse: collapse; width: 100%; margin: 0 0 10pt 0; }
td, th { border: 1px solid #000; padding: 3pt 5pt; font-size: 12pt; vertical-align: top; line-height: 120%; }
th { background: #d9e2f3; font-weight: bold; text-align: center; }
td p, th p { text-indent: 0; margin: 0; text-align: left; line-height: 120%; }
p.caption-table { text-align: center; text-indent: 0; font-weight: bold; margin: 8pt 0 4pt 0; page-break-after: avoid; }
p.caption-fig { text-align: center; text-indent: 0; font-style: italic; margin: 4pt 0 12pt 0; }
p.img { text-align: center; text-indent: 0; margin: 8pt 0 0 0; line-height: 100%; page-break-after: avoid; }
h2, h3 { page-break-after: avoid; }
p.ref { text-indent: -1.1cm; margin-left: 1.1cm; text-align: left; }
p.code { font-family: Consolas; font-size: 10.5pt; line-height: 115%; text-align: left; text-indent: 0;
         background: #f2f2f2; border: 1px solid #999; padding: 4pt 6pt; margin: 4pt 0 10pt 0; page-break-inside: avoid; }
code { font-family: Consolas; font-size: 11pt; }
"""


def inline(text):
    """Định dạng trong dòng: **đậm**, *nghiêng*, `code`, giữ <br>"""
    parts = re.split(r"(<br>)", text)
    out = []
    for part in parts:
        if part == "<br>":
            out.append("<br>")
            continue
        s = html.escape(part, quote=False)
        s = re.sub(r"`([^`]+)`", r"<code>\1</code>", s)
        s = re.sub(r"\*\*\*(.+?)\*\*\*", r"<b><i>\1</i></b>", s)
        s = re.sub(r"\*\*(.+?)\*\*", r"<b>\1</b>", s)
        s = re.sub(r"(?<![\w*])\*(?!\s)(.+?)(?<!\s)\*(?![\w*])", r"<i>\1</i>", s)
        out.append(s)
    return "".join(out)


def table_html(rows):
    cells = [[c.strip() for c in r.strip().strip("|").split("|")] for r in rows]
    header, body = cells[0], cells[2:]  # bỏ dòng |---|
    # Bọc nội dung ô trong <p> lề 0 để Word không thêm khoảng trắng dưới mỗi ô
    cell = '<p style="margin:0;text-indent:0">{}</p>'
    h = ["<table>", "<tr>" + "".join(f"<th>{cell.format(inline(c))}</th>" for c in header) + "</tr>"]
    for r in body:
        h.append("<tr>" + "".join(f"<td>{cell.format(inline(c))}</td>" for c in r) + "</tr>")
    h.append("</table>")
    return "\n".join(h)


def convert(md_path):
    lines = open(md_path, encoding="utf-8").read().splitlines()
    out, i, pending_caption = [], 0, None
    while i < len(lines):
        line = lines[i]
        if not line.strip():
            i += 1
            continue
        if line.startswith("```"):
            # Khối mã nguồn: giữ nguyên khoảng trắng, font Consolas, nền xám nhạt
            code = []
            i += 1
            while i < len(lines) and not lines[i].startswith("```"):
                code.append(html.escape(lines[i]).replace(" ", "&nbsp;") or "&nbsp;")
                i += 1
            i += 1
            out.append('<p class="code">' + "<br>".join(code) + "</p>")
            continue
        m = re.match(r"^(#{1,3}) (.+)$", line)
        if m:
            level = len(m.group(1))
            out.append(f"<h{level}>{inline(m.group(2))}</h{level}>")
            i += 1
            continue
        m = re.match(r"^!\[(.+?)\]\((.+?)\)$", line.strip())
        if m:
            caption, rel = m.group(1), m.group(2)
            path = os.path.abspath(os.path.join(os.path.dirname(md_path), rel))
            w, h = Image.open(path).size
            w2 = min(MAX_IMG_PX, w // 2)
            h2 = round(h * w2 / w)
            url = "file:///" + path.replace("\\", "/")
            out.append(f'<p class="img"><img src="{url}" width="{w2}" height="{h2}"></p>')
            out.append(f'<p class="caption-fig">{inline(caption)}</p>')
            i += 1
            continue
        if line.startswith("|"):
            rows = []
            while i < len(lines) and lines[i].startswith("|"):
                rows.append(lines[i])
                i += 1
            if pending_caption:
                out.append(f'<p class="caption-table">{inline(pending_caption)}</p>')
                pending_caption = None
            out.append(table_html(rows))
            continue
        if re.match(r"^\s*[-*] ", line):
            items = []
            while i < len(lines) and re.match(r"^\s*[-*] ", lines[i]):
                items.append(re.sub(r"^\s*[-*] ", "", lines[i]))
                i += 1
            out.append("<ul>" + "".join(f"<li>{inline(t)}</li>" for t in items) + "</ul>")
            continue
        if re.match(r"^\d+\. ", line):
            items = []
            while i < len(lines) and re.match(r"^\d+\. ", lines[i]):
                items.append(re.sub(r"^\d+\. ", "", lines[i]))
                i += 1
            out.append("<ol>" + "".join(f"<li>{inline(t)}</li>" for t in items) + "</ol>")
            continue
        # Dòng "Bảng x.y. ..." đứng ngay trước bảng → chú thích bảng
        if re.match(r"^Bảng \d+\.\d+\.", line) and i + 2 < len(lines) and lines[i + 2].startswith("|"):
            pending_caption = line
            i += 1
            continue
        # Mục tài liệu tham khảo "[n] ..." → thụt lề treo
        css = ' class="ref"' if re.match(r"^\[\d+\] ", line) else ""
        out.append(f"<p{css}>{inline(line)}</p>")
        i += 1

    body = "\n".join(out)
    return (f'<html><head><meta charset="utf-8"><style>{CSS}</style></head>'
            f'<body><div class="WordSection1">{body}</div></body></html>')


def to_docx(html_path, docx_path):
    ps = f"""
$ErrorActionPreference = 'Stop'
$word = New-Object -ComObject Word.Application
$word.Visible = $false
try {{
  $doc = $word.Documents.Open('{html_path}')
  foreach ($s in $doc.InlineShapes) {{
    if ($s.LinkFormat -ne $null) {{ $s.LinkFormat.SavePictureWithDocument = $true; $s.LinkFormat.BreakLink() }}
  }}
  $doc.SaveAs2('{docx_path}', 16)
  $doc.Close()
}} finally {{ $word.Quit() }}
"""
    subprocess.run(["powershell", "-NoProfile", "-Command", ps], check=True)


if __name__ == "__main__":
    # Mặc định KHÔNG ghi đè tệp .docx đã có (có thể đã được chỉnh sửa trực tiếp trong Word);
    # thêm tham số --ghi-de để buộc xuất lại.
    overwrite = "--ghi-de" in sys.argv
    names = [a for a in sys.argv[1:] if not a.startswith("--")]
    for name in names:
        md = os.path.join(HERE, name)
        base = os.path.splitext(md)[0]
        if os.path.exists(base + ".docx") and not overwrite:
            print("Bỏ qua (đã có tệp Word, dùng --ghi-de để xuất lại):", base + ".docx")
            continue
        html_path = base + ".html"
        open(html_path, "w", encoding="utf-8").write(convert(md))
        to_docx(html_path, base + ".docx")
        os.remove(html_path)
        print("Đã tạo", base + ".docx")
