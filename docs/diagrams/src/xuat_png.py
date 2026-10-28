# Dựng ảnh PNG từ các tệp .mmd (Mermaid) / .html bằng Edge headless rồi cắt viền trắng
# Cách dùng: python xuat_png.py <thư_mục_src> <thư_mục_png> [tên_tệp ...]
import html, os, shutil, subprocess, sys, tempfile, time
from PIL import Image, ImageChops

EDGE = r"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe"
src_dir, out_dir = sys.argv[1], sys.argv[2]
only = set(sys.argv[3:])
work = tempfile.mkdtemp(prefix="mmd_")

TEMPLATE = """<!doctype html><html><head><meta charset="utf-8">
<style>body{{margin:0;padding:24px;background:#fff}} .mermaid{{display:inline-block}}</style>
<script src="https://cdn.jsdelivr.net/npm/mermaid@10.9.1/dist/mermaid.min.js"></script></head>
<body><pre class="mermaid">{code}</pre>
<script>
mermaid.initialize({{ startOnLoad: true, theme: 'neutral', securityLevel: 'loose',
  themeVariables: {{ fontFamily: 'Segoe UI, Arial, sans-serif', fontSize: '15px' }},
  flowchart: {{ useMaxWidth: false, htmlLabels: true, curve: 'basis', nodeSpacing: 40, rankSpacing: 50 }},
  sequence: {{ useMaxWidth: false, mirrorActors: false, wrap: false }},
  er: {{ useMaxWidth: false, layoutDirection: 'TB', minEntityWidth: 120 }},
  state: {{ useMaxWidth: false }} }});
</script></body></html>"""

def crop(path, pad=24):
    img = Image.open(path).convert("RGB")
    bg = Image.new("RGB", img.size, (255, 255, 255))
    box = ImageChops.difference(img, bg).getbbox()
    if box:
        l, t, r, b = box
        img = img.crop((max(l - pad, 0), max(t - pad, 0), min(r + pad, img.width), min(b + pad, img.height)))
    img.save(path, optimize=True)
    return img.size

def shoot(url, out, w, h):
    subprocess.run([EDGE, "--headless=new", "--disable-gpu", "--hide-scrollbars", "--allow-file-access-from-files",
                    "--force-device-scale-factor=2", "--virtual-time-budget=8000",
                    f"--window-size={w},{h}", f"--screenshot={out}", url], check=True,
                   stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
    # msedge.exe có thể trả về trước khi ghi xong ảnh → chờ tệp xuất hiện và kích thước ổn định
    last = -1
    for _ in range(120):
        if os.path.exists(out):
            cur = os.path.getsize(out)
            if cur > 0 and cur == last: return
            last = cur
        time.sleep(0.5)
    raise RuntimeError("Không tạo được ảnh: " + out)

os.makedirs(out_dir, exist_ok=True)
for name in sorted(os.listdir(src_dir)):
    base, ext = os.path.splitext(name)
    if only and base not in only: continue
    out = os.path.join(out_dir, base + ".png")
    if ext == ".mmd":
        code = open(os.path.join(src_dir, name), encoding="utf-8").read()
        page = os.path.join(work, base + ".html")
        open(page, "w", encoding="utf-8").write(TEMPLATE.format(code=html.escape(code)))
        url = "file:///" + page.replace("\\", "/")
        size = (3000, 4200)
    elif ext == ".svg":
        import re
        svg_text = open(os.path.join(src_dir, name), encoding="utf-8").read(400)
        w = int(float(re.search(r'width="([\d.]+)"', svg_text).group(1)))
        h = int(float(re.search(r'height="([\d.]+)"', svg_text).group(1)))
        page = os.path.join(work, base + ".html")
        shutil.copy(os.path.join(src_dir, name), os.path.join(work, name))
        open(page, "w", encoding="utf-8").write(
            f'<!doctype html><html><body style="margin:0;background:#fff"><img src="{name}" width="{w}" height="{h}" style="display:block"></body></html>')
        url = "file:///" + page.replace("\\", "/")
        size = (w + 40, h + 40)
    elif ext == ".html":
        url = "file:///" + os.path.abspath(os.path.join(src_dir, name)).replace("\\", "/")
        size = (1220, 1700)
    else:
        continue
    # Edge ghi ảnh vào thư mục tạm (đường dẫn ASCII) rồi chuyển sang thư mục đích
    tmp_png = os.path.join(work, base + ".png")
    shoot(url, tmp_png, *size)
    size = crop(tmp_png)
    shutil.move(tmp_png, os.path.abspath(out))
    print(f"{base}.png", size)
