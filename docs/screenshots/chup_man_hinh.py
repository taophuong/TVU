# Chụp ảnh giao diện website cho Chương 4 (mục 4.6) bằng Microsoft Edge headless
# Yêu cầu: website đang chạy ở http://localhost:5080 với dữ liệu mẫu.
# Trang cần đăng nhập: tải HTML bằng phiên đăng nhập, chèn <base> rồi chụp tệp HTML cục bộ.
# Cách dùng: python -X utf8 chup_man_hinh.py
import http.cookiejar
import os
import re
import shutil
import subprocess
import tempfile
import time
import urllib.parse
import urllib.request

from PIL import Image

BASE = "http://localhost:5080"
EDGE = r"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe"
OUT = os.path.dirname(os.path.abspath(__file__))
WORK = tempfile.mkdtemp(prefix="shots_")

# Chèn sau khi trang tải xong: cuộn tới phần tử / mở hộp thoại
JS_SCROLL = "<script>window.addEventListener('load',()=>{{const e=document.querySelector('{sel}');if(e)window.scrollTo(0,e.getBoundingClientRect().top+window.scrollY-{offset});}});</script>"
JS_MODAL = "<script>window.addEventListener('load',()=>{{setTimeout(()=>{{const m=document.querySelector('{sel}');if(m){{{prep}new bootstrap.Modal(m).show();}}}},300);}});</script>"


def session(email=None, pw=None):
    op = urllib.request.build_opener(urllib.request.HTTPCookieProcessor(http.cookiejar.CookieJar()))
    if email:
        html = op.open(BASE + "/dang-nhap").read().decode()
        tok = re.search(r'name="__RequestVerificationToken" type="hidden" value="([^"]+)"', html).group(1)
        op.open(BASE + "/dang-nhap", urllib.parse.urlencode({"Email": email, "Password": pw, "__RequestVerificationToken": tok}).encode())
    return op


def snapshot(op, path, name, extra=""):
    """Lưu trang (đã đăng nhập) thành tệp HTML cục bộ có <base> trỏ về website"""
    html = op.open(BASE + path).read().decode()
    html = html.replace("<head>", f'<head><base href="{BASE}/">', 1).replace("</body>", extra + "</body>", 1)
    file = os.path.join(WORK, name + ".html")
    open(file, "w", encoding="utf-8").write(html)
    return "file:///" + file.replace("\\", "/")


def shoot(url, name, w, h):
    tmp = os.path.join(WORK, name + ".png")
    subprocess.run([EDGE, "--headless=new", "--disable-gpu", "--hide-scrollbars", "--allow-file-access-from-files",
                    "--virtual-time-budget=6000", f"--window-size={w},{h}", f"--screenshot={tmp}", url],
                   stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
    for _ in range(120):  # Edge có thể trả về trước khi ghi xong ảnh
        if os.path.exists(tmp) and os.path.getsize(tmp) > 0:
            time.sleep(0.5)
            break
        time.sleep(0.5)
    shutil.move(tmp, os.path.join(OUT, name + ".png"))
    print("  ", name)


guest = session()
sv = session("170125077@st.tvu.edu.vn", "SinhVien@123")
khoa = session("110122001@st.tvu.edu.vn", "SinhVien@123")
admin = session("admin@tvu.edu.vn", "Admin@123")
draft_id = re.search(r"/du-an-cua-toi/(\d+)/sua", khoa.open(BASE + "/du-an-cua-toi").read().decode()).group(1)

import sys
only = set(sys.argv[1:])
W = 1366
shots = [
    # --- Khách ---
    ("k01_trang_chu", snapshot(guest, "/", "k01"), W, 1500),
    ("k02_danh_sach_du_an", BASE + "/du-an", W, 1150),
    ("k03_tim_kiem_khong_dau", BASE + "/du-an?q=tuy%20bien", W, 760),
    ("k04_chi_tiet_du_an", BASE + "/du-an/zalord", W, 1450),
    ("k05_lien_he_hop_tac", snapshot(guest, "/du-an/agrisense", "k05", JS_MODAL.format(sel="#contactModal", prep="")), W, 900),
    ("k06_tin_tuc", BASE + "/tin-tuc", W, 1000),
    ("k07_dang_nhap", BASE + "/dang-nhap", W, 860),
    ("k08_dang_ky", BASE + "/dang-ky", W, 960),
    ("k09_trang_404", BASE + "/abc", W, 620),
    ("k10_dien_thoai_trang_chu", BASE + "/", 500, 1000),
    ("k11_dien_thoai_chi_tiet", BASE + "/du-an/zalord", 500, 1000),
    # --- Sinh viên ---
    ("s01_du_an_cua_toi", snapshot(sv, "/du-an-cua-toi", "s01"), W, 700),
    ("s02_dang_du_an_moi", snapshot(sv, "/du-an-cua-toi/tao-moi", "s02"), W, 1250),
    ("s03_sua_du_an_tinh_nang", snapshot(khoa, f"/du-an-cua-toi/{draft_id}/sua?tab=features", "s03"), W, 760),
    ("s04_sua_du_an_hinh_anh", snapshot(sv, "/du-an-cua-toi/1/sua?tab=images", "s04"), W, 820),
    ("s05_binh_luan_danh_gia", snapshot(sv, "/du-an/agrisense", "s05"), W, 2400),
    ("s06_xem_truoc_du_an_nhap", snapshot(khoa, "/du-an/tvu-smart-parking", "s06"), W, 760),
    ("s07_lien_he_cua_sinh_vien", snapshot(sv, "/du-an-cua-toi/lien-he", "s07"), W, 700),
    ("s08_ho_so_ca_nhan", snapshot(sv, "/tai-khoan", "s08"), W, 820),
    # --- Quản trị ---
    ("a01_dashboard", snapshot(admin, "/quan-tri", "a01"), W, 1500),
    ("a02_duyet_du_an", snapshot(admin, "/quan-tri/Projects", "a02"), W, 1100),
    ("a03_tu_choi_du_an", snapshot(admin, "/quan-tri/Projects?status=ChoDuyet", "a03",
                                   JS_MODAL.format(sel="#rejectModal", prep="document.getElementById('rejectName').textContent='Xà phòng thảo dược Mộc An';document.getElementById('rejectReason').value='Cần bổ sung hình ảnh sản phẩm thực tế và giấy kiểm nghiệm.';")), W, 760),
    ("a04_lien_he_hop_tac", snapshot(admin, "/quan-tri/Contacts", "a04"), W, 900),
    ("a05_binh_luan", snapshot(admin, "/quan-tri/Comments", "a05"), W, 900),
    ("a06_tin_tuc_form", snapshot(admin, "/quan-tri/News/Edit/1", "a06"), W, 1150),
    ("a07_banner", snapshot(admin, "/quan-tri/Banners", "a07"), W, 560),
    ("a08_linh_vuc", snapshot(admin, "/quan-tri/Categories", "a08"), W, 900),
    ("a09_tai_khoan", snapshot(admin, "/quan-tri/Accounts", "a09"), W, 860),
]

# Ảnh cần cắt lấy một vùng (chụp cả trang với cửa sổ cao rồi cắt theo vị trí phần tử)
CROP = {"s05_binh_luan_danh_gia": "comments"}

print("Đang chụp", len(shots), "màn hình vào", OUT)
for name, url, w, h in shots:
    if only and name not in only:
        continue
    shoot(url, name, w, h)
    if name in CROP:
        path = os.path.join(OUT, name + ".png")
        img = Image.open(path)
        top = int(open(os.path.join(WORK, CROP[name] + ".top")).read()) if os.path.exists(os.path.join(WORK, CROP[name] + ".top")) else 1250
        img.crop((100, top + 95, 890, top + 720)).save(path)
