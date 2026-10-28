# Vẽ các sơ đồ UML dạng SVG (use case, hoạt động, kiến trúc) bằng tọa độ đặt tay
# Cách dùng: python ve_so_do.py <thư_mục_xuất>
import html
import os
import sys

FONT = "Segoe UI, Arial, sans-serif"


class Svg:
    def __init__(self, w, h):
        self.w, self.h, self.items = w, h, []

    def add(self, s):
        self.items.append(s)

    # ----- Chữ (nhiều dòng, ngăn cách bằng \n) -----
    def text(self, x, y, s, size=14, anchor="middle", weight="normal", color="#111", italic=False):
        lines = s.split("\n")
        lh = size * 1.3
        y0 = y - (len(lines) - 1) * lh / 2 + size * 0.35
        style = "font-style:italic;" if italic else ""
        spans = "".join(f'<tspan x="{x}" y="{y0 + i * lh:.1f}">{html.escape(l)}</tspan>' for i, l in enumerate(lines))
        self.add(f'<text font-family="{FONT}" font-size="{size}" font-weight="{weight}" fill="{color}" '
                 f'text-anchor="{anchor}" style="{style}">{spans}</text>')

    # ----- Hình cơ bản -----
    def rect(self, cx, cy, w, h, label="", rx=10, fill="#f8fafc", stroke="#334155", size=14, dash=False, weight="normal"):
        d = ' stroke-dasharray="6 4"' if dash else ""
        self.add(f'<rect x="{cx - w / 2}" y="{cy - h / 2}" width="{w}" height="{h}" rx="{rx}" fill="{fill}" stroke="{stroke}" stroke-width="1.5"{d}/>')
        if label:
            self.text(cx, cy, label, size, weight=weight)

    def box(self, x, y, w, h, title="", fill="none", stroke="#64748b", dash=False, title_size=15):
        d = ' stroke-dasharray="7 5"' if dash else ""
        self.add(f'<rect x="{x}" y="{y}" width="{w}" height="{h}" rx="12" fill="{fill}" stroke="{stroke}" stroke-width="1.5"{d}/>')
        if title:
            self.text(x + w / 2, y + 18, title, title_size, weight="bold", color="#0f172a")

    def ellipse(self, cx, cy, rx, ry, label, size=14):
        self.add(f'<ellipse cx="{cx}" cy="{cy}" rx="{rx}" ry="{ry}" fill="#f1f5f9" stroke="#334155" stroke-width="1.5"/>')
        self.text(cx, cy, label, size)

    def diamond(self, cx, cy, hw, hh, label, size=13):
        pts = f"{cx},{cy - hh} {cx + hw},{cy} {cx},{cy + hh} {cx - hw},{cy}"
        self.add(f'<polygon points="{pts}" fill="#fff7ed" stroke="#334155" stroke-width="1.5"/>')
        self.text(cx, cy, label, size)

    def start(self, cx, cy):
        self.add(f'<circle cx="{cx}" cy="{cy}" r="12" fill="#111"/>')

    def end(self, cx, cy):
        self.add(f'<circle cx="{cx}" cy="{cy}" r="14" fill="#fff" stroke="#111" stroke-width="2"/>')
        self.add(f'<circle cx="{cx}" cy="{cy}" r="8" fill="#111"/>')

    def actor(self, cx, cy, label):
        # Hình người: đầu tại cy-45, chân kết thúc tại cy+40
        s = 'stroke="#111" stroke-width="2" fill="none"'
        self.add(f'<circle cx="{cx}" cy="{cy - 45}" r="14" {s}/>')
        self.add(f'<line x1="{cx}" y1="{cy - 31}" x2="{cx}" y2="{cy + 10}" {s}/>')
        self.add(f'<line x1="{cx - 24}" y1="{cy - 15}" x2="{cx + 24}" y2="{cy - 15}" {s}/>')
        self.add(f'<line x1="{cx}" y1="{cy + 10}" x2="{cx - 20}" y2="{cy + 40}" {s}/>')
        self.add(f'<line x1="{cx}" y1="{cy + 10}" x2="{cx + 20}" y2="{cy + 40}" {s}/>')
        self.text(cx, cy + 60, label, 15, weight="bold")

    def cylinder(self, cx, cy, w, h, label):
        rx, ry = w / 2, 12
        top, bot = cy - h / 2, cy + h / 2
        self.add(f'<path d="M{cx - rx},{top} L{cx - rx},{bot} A{rx},{ry} 0 0 0 {cx + rx},{bot} L{cx + rx},{top}" fill="#f1f5f9" stroke="#334155" stroke-width="1.5"/>')
        self.add(f'<ellipse cx="{cx}" cy="{top}" rx="{rx}" ry="{ry}" fill="#e2e8f0" stroke="#334155" stroke-width="1.5"/>')
        self.text(cx, cy + 8, label, 14)

    # ----- Đường nối -----
    def line(self, pts, arrow="open", dash=False, color="#334155", label=None, label_at=None, label_anchor="middle"):
        """arrow: None | 'open' (mũi tên thường) | 'filled' | 'hollow' (tam giác rỗng – quan hệ kế thừa) | 'both'"""
        d = ' stroke-dasharray="6 4"' if dash else ""
        p = " ".join(f"{x},{y}" for x, y in pts)
        start_m = ' marker-start="url(#arrowStart)"' if arrow == "both" else ""
        end_m = {"open": ' marker-end="url(#arrow)"', "filled": ' marker-end="url(#arrow)"',
                 "hollow": ' marker-end="url(#hollow)"', "both": ' marker-end="url(#arrow)"'}.get(arrow, "")
        self.add(f'<polyline points="{p}" fill="none" stroke="{color}" stroke-width="1.6"{d}{start_m}{end_m}/>')
        if label:
            lx, ly = label_at
            w = len(label) * 7.2 + 10
            x0 = lx - w / 2 if label_anchor == "middle" else lx - 5
            self.add(f'<rect x="{x0}" y="{ly - 11}" width="{w}" height="20" fill="#fff"/>')
            self.text(lx, ly, label, 13, anchor=label_anchor, color="#334155", italic=True)

    def save(self, path):
        defs = """<defs>
<marker id="arrow" viewBox="0 0 10 10" refX="9.5" refY="5" markerWidth="9" markerHeight="9" orient="auto-start-reverse"><path d="M0,0 L10,5 L0,10 z" fill="#334155"/></marker>
<marker id="arrowStart" viewBox="0 0 10 10" refX="0.5" refY="5" markerWidth="9" markerHeight="9" orient="auto"><path d="M10,0 L0,5 L10,10 z" fill="#334155"/></marker>
<marker id="hollow" viewBox="0 0 12 12" refX="11.5" refY="6" markerWidth="14" markerHeight="14" orient="auto"><path d="M0,0 L12,6 L0,12 z" fill="#fff" stroke="#334155" stroke-width="1.2"/></marker>
</defs>"""
        body = "\n".join(self.items)
        svg = (f'<svg xmlns="http://www.w3.org/2000/svg" width="{self.w}" height="{self.h}" viewBox="0 0 {self.w} {self.h}">'
               f'<rect width="100%" height="100%" fill="#fff"/>{defs}\n{body}\n</svg>')
        open(path, "w", encoding="utf-8").write(svg)


def lanes(svg, titles, x0, widths, top, bottom):
    """Vẽ các làn (swimlane) theo chiều dọc"""
    x = x0
    for title, w in zip(titles, widths):
        svg.add(f'<rect x="{x}" y="{top}" width="{w}" height="{bottom - top}" fill="none" stroke="#334155" stroke-width="1.5"/>')
        svg.add(f'<rect x="{x}" y="{top}" width="{w}" height="44" fill="#e2e8f0" stroke="#334155" stroke-width="1.5"/>')
        svg.text(x + w / 2, top + 22, title, 16, weight="bold")
        x += w


# =====================================================================
# 1. Sơ đồ Use Case tổng quát
# =====================================================================
def use_case(out):
    s = Svg(1520, 1150)
    s.box(250, 30, 1020, 1100, "Hệ thống TVU Startup – Website giới thiệu sản phẩm khởi nghiệp SV TVU", fill="#ffffff", stroke="#0b3d91")

    rx, ry = 128, 30
    col_a = ["UC01 Xem trang chủ", "UC02 Tìm kiếm, lọc dự án", "UC03 Xem chi tiết dự án",
             "UC04 Xem tin tức, sự kiện", "UC05 Gửi liên hệ hợp tác", "UC06 Đăng ký tài khoản"]
    col_b = ["UC08 Cập nhật hồ sơ,\nđổi mật khẩu", "UC09 Quản lý dự án của tôi", "UC10 Quản lý ảnh, tính năng,\nphiên bản, thành viên",
             "UC11 Gửi duyệt / rút lại", "UC12 Bình luận, đánh giá", "UC13 Bình chọn dự án", "UC14 Xem liên hệ\ngửi tới dự án"]
    col_c = ["UC15 Xem thống kê (dashboard)", "UC16 Duyệt / từ chối / ẩn dự án", "UC17 Đánh dấu dự án nổi bật",
             "UC18 Quản lý lĩnh vực", "UC19 Quản lý khoa", "UC20 Quản lý tài khoản", "UC21 Quản lý tin tức, sự kiện",
             "UC22 Quản lý banner", "UC23 Kiểm duyệt bình luận", "UC24 Xử lý liên hệ hợp tác"]
    ax, bx, cx = 430, 760, 1095
    a_pos = [(ax, 110 + 75 * i) for i in range(len(col_a))]
    b_pos = [(bx, 560 + 72 * i) for i in range(len(col_b))]
    c_pos = [(cx, 105 + 74 * i) for i in range(len(col_c))]
    login = (bx, 1075)

    # Tác nhân
    guest, student, admin = (115, 310), (115, 800), (1400, 900)
    s.actor(*guest, "Khách")
    s.actor(*student, "Sinh viên")
    s.actor(*admin, "Quản trị viên")

    for (x, y) in a_pos:
        s.line([(guest[0] + 30, guest[1] - 15), (x - rx, y)], arrow=None)
    for (x, y) in b_pos + [login]:
        s.line([(student[0] + 30, student[1] - 15), (x - rx, y)], arrow=None)
    for (x, y) in c_pos:
        s.line([(admin[0] - 30, admin[1] - 15), (x + rx, y)], arrow=None)
    s.line([(admin[0] - 30, admin[1] - 15), (login[0] + rx, login[1])], arrow=None)
    # Sinh viên kế thừa Khách (tam giác rỗng hướng về Khách)
    s.line([(student[0], student[1] - 60), (guest[0], guest[1] + 82)], arrow="hollow")
    s.text(guest[0] + 12, (guest[1] + student[1]) / 2 + 10, "kế thừa", 13, anchor="start", italic=True, color="#334155")

    for (x, y), label in zip(a_pos, col_a):
        s.ellipse(x, y, rx, ry, label)
    for (x, y), label in zip(b_pos, col_b):
        s.ellipse(x, y, rx, ry + (6 if "\n" in label else 0), label)
    for (x, y), label in zip(c_pos, col_c):
        s.ellipse(x, y, rx + 12, ry, label)
    s.ellipse(*login, rx, ry, "UC07 Đăng nhập / đăng xuất")
    s.save(out)


# =====================================================================
# 2. Sơ đồ hoạt động: đăng dự án → duyệt → công khai
# =====================================================================
def activity_project(out):
    s = Svg(1222, 1440)
    lanes(s, ["Sinh viên", "Hệ thống", "Quản trị viên"], 1, [380, 460, 380], 1, 1438)
    SV, HT, AD = 190, 610, 1030

    s.start(SV, 80)
    s.rect(SV, 150, 180, 44, "Đăng nhập")
    s.rect(SV, 240, 210, 52, "Nhập thông tin chung\n(tên, mô tả, lĩnh vực, ảnh)")
    s.diamond(HT, 330, 110, 38, "Dữ liệu hợp lệ?")
    s.rect(HT, 420, 220, 50, "Lưu dự án\ntrạng thái Nháp")
    s.rect(SV, 510, 220, 56, "Bổ sung tính năng, hình ảnh,\nphiên bản, thành viên")
    s.rect(SV, 600, 180, 46, "Bấm Gửi duyệt")
    s.diamond(HT, 690, 120, 38, "Có ảnh bìa và\n≥ 1 tính năng?")
    s.rect(HT, 780, 220, 50, "Cập nhật trạng thái\nChờ duyệt")
    s.rect(AD, 860, 240, 50, "Xem danh sách Chờ duyệt,\nxem trước dự án")
    s.diamond(AD, 950, 110, 38, "Nội dung\nđạt yêu cầu?")
    s.rect(908, 1040, 118, 50, "Từ chối,\nnhập lý do")
    s.rect(1150, 1040, 110, 50, "Bấm Duyệt")
    s.rect(500, 1130, 220, 50, "Lưu trạng thái Bị từ chối\nkèm lý do")
    s.rect(SV, 1130, 220, 56, "Xem lý do từ chối,\nchỉnh sửa dự án")
    s.rect(720, 1220, 220, 50, "Lưu Đã duyệt, ghi\nApprovedAt, ApprovedBy")
    s.rect(720, 1310, 220, 50, "Hiển thị công khai: danh sách,\ntìm kiếm, trang chủ")
    s.end(720, 1395)

    L = s.line
    L([(SV, 92), (SV, 128)])
    L([(SV, 172), (SV, 214)])
    L([(295, 240), (HT, 240), (HT, 292)])
    L([(500, 330), (SV, 330), (SV, 266)], label="Không", label_at=(440, 330))
    L([(HT, 368), (HT, 395)], label="Có", label_at=(HT + 22, 380), label_anchor="start")
    L([(HT, 445), (HT, 498), (300, 498)])
    L([(SV, 538), (SV, 577)])
    L([(280, 600), (HT, 600), (HT, 652)])
    L([(730, 690), (800, 690), (800, 524), (300, 524)], label="Không", label_at=(770, 670))
    L([(HT, 728), (HT, 755)], label="Có", label_at=(HT + 22, 740), label_anchor="start")
    L([(720, 780), (AD, 780), (AD, 835)])
    L([(AD, 885), (AD, 912)])
    L([(920, 950), (908, 950), (908, 1015)], label="Không", label_at=(880, 925))
    L([(1140, 950), (1150, 950), (1150, 1015)], label="Có", label_at=(1172, 925))
    L([(908, 1065), (908, 1130), (610, 1130)])
    L([(1150, 1065), (1150, 1220), (830, 1220)])
    L([(390, 1130), (300, 1130)])
    L([(SV, 1102), (SV, 623)], label="Gửi duyệt lại", label_at=(SV + 50, 860))
    L([(720, 1245), (720, 1285)])
    L([(720, 1335), (720, 1381)])
    s.save(out)


# =====================================================================
# 3. Sơ đồ hoạt động: gửi liên hệ hợp tác
# =====================================================================
def activity_contact(out):
    s = Svg(1222, 1040)
    lanes(s, ["Khách / Doanh nghiệp", "Hệ thống", "Sinh viên chủ dự án / Quản trị viên"], 1, [380, 460, 380], 1, 1038)
    K, HT, X = 190, 610, 1030

    s.start(K, 80)
    s.rect(K, 160, 230, 50, "Tìm kiếm, xem chi tiết dự án")
    s.rect(K, 240, 200, 44, "Bấm Liên hệ hợp tác")
    s.rect(HT, 330, 260, 50, "Hiển thị form liên hệ\n(điền sẵn nếu đã đăng nhập)")
    s.rect(K, 422, 230, 56, "Nhập họ tên, email, nhu cầu,\nnội dung và bấm Gửi")
    s.diamond(HT, 520, 125, 40, "Token chống CSRF\nvà dữ liệu hợp lệ?")
    s.rect(HT, 620, 230, 50, "Lưu ContactRequest\ntrạng thái Mới")
    s.rect(K, 710, 220, 50, "Nhận thông báo\ngửi thành công")
    s.end(K, 800)
    s.rect(X, 710, 250, 56, "Xem liên hệ mới (Dự án của tôi /\nDashboard quản trị)")
    s.rect(X, 800, 250, 50, "Liên hệ lại qua email,\nđiện thoại")
    s.rect(X, 890, 250, 50, "Cập nhật trạng thái\nĐã liên hệ / Đã đóng")
    s.rect(HT, 890, 230, 50, "Lưu trạng thái và\nngười xử lý")
    s.end(HT, 980)

    L = s.line
    L([(K, 92), (K, 135)])
    L([(K, 185), (K, 218)])
    L([(290, 240), (HT, 240), (HT, 305)])
    L([(HT, 355), (HT, 410), (305, 410)])
    L([(K, 450), (K, 520), (485, 520)])
    L([(735, 520), (800, 520), (800, 436), (305, 436)], label="Không – báo lỗi", label_at=(760, 498))
    L([(HT, 560), (HT, 595)], label="Có", label_at=(HT + 22, 577), label_anchor="start")
    L([(495, 620), (K, 620), (K, 685)])
    L([(K, 735), (K, 786)])
    L([(725, 620), (X, 620), (X, 682)])
    L([(X, 738), (X, 775)])
    L([(X, 825), (X, 865)])
    L([(905, 890), (725, 890)])
    L([(HT, 915), (HT, 966)])
    s.save(out)


# =====================================================================
# 4. Kiến trúc phân lớp / MVC
# =====================================================================
def architecture(out):
    s = Svg(1200, 960)
    s.rect(600, 55, 300, 56, "Người dùng – Trình duyệt web\n(HTML, CSS, JavaScript)", fill="#e0e7ff")
    s.box(40, 115, 1120, 680, fill="#ffffff", stroke="#0b3d91")
    s.text(60, 135, "Ứng dụng ASP.NET Core MVC (.NET 8) – TVUStartup.Web", 15, anchor="start", weight="bold", color="#0f172a")

    # Middleware
    s.box(130, 160, 940, 100, "Pipeline middleware (Program.cs)", fill="#f8fafc")
    mids = ["UseStaticFiles\n(wwwroot)", "UseRouting", "UseAuthentication\n(Cookie)", "UseAuthorization\n(Role)"]
    xs = [255, 485, 715, 945]
    for x, m in zip(xs, mids):
        s.rect(x, 222, 190, 48, m, fill="#ffffff")
    for a, b in zip(xs, xs[1:]):
        s.line([(a + 95, 222), (b - 95, 222)])

    # Controller
    s.box(130, 290, 940, 120, "Controller", fill="#f8fafc")
    s.rect(390, 360, 440, 66, "Controllers (người dùng)\nHome, Projects, News, Contact, Account, MyProjects", fill="#ffffff", size=13)
    s.rect(830, 360, 400, 66, "Areas/Admin/Controllers\nDashboard, Projects, Accounts, Categories, Faculties,\nNews, Banners, Comments, Contacts", fill="#ffffff", size=13)

    # View – Model – Service
    s.box(70, 450, 330, 140, "View (Razor)", fill="#f8fafc")
    s.text(235, 525, "_Layout, _AdminLayout\nPartial View, Tag Helper\nBootstrap 5, jQuery", 13)
    s.box(435, 450, 330, 140, "Model", fill="#f8fafc")
    s.text(600, 525, "ViewModels + DataAnnotations\n(kiểm tra dữ liệu nhập)\nEntities: 14 lớp thực thể", 13)
    s.box(800, 450, 330, 140, "Service (Dependency Injection)", fill="#f8fafc")
    s.text(965, 525, "IPasswordHasher (PBKDF2)\nIFileStorageService, IHtmlSanitizer\nProjectRules (quy tắc trạng thái)", 13)

    # Data
    s.box(435, 640, 330, 120, "Data – Truy cập dữ liệu", fill="#f8fafc")
    s.text(600, 705, "AppDbContext (EF Core 8 + LINQ)\nMigrations, DbSeeder\nProjectQueries, LookupQueries", 13)

    s.cylinder(600, 880, 230, 70, "SQL Server – TVUStartupDB")

    L = s.line
    L([(600, 83), (600, 160)], label="HTTP request", label_at=(615, 99), label_anchor="start")
    L([(600, 260), (600, 290)])
    L([(300, 410), (235, 450)], label="ViewModel", label_at=(225, 428))
    L([(600, 410), (600, 450)])
    L([(900, 410), (965, 450)])
    L([(415, 410), (415, 700), (435, 700)])
    L([(600, 640), (600, 590)], label="ánh xạ", label_at=(640, 615), label_anchor="start")
    L([(600, 760), (600, 845)], arrow="both", label="truy vấn SQL (tham số hóa)", label_at=(615, 815), label_anchor="start")
    L([(70, 520), (20, 520), (20, 55), (450, 55)], label="HTML response", label_at=(30, 300), label_anchor="start")
    s.save(out)


if __name__ == "__main__":
    out_dir = sys.argv[1] if len(sys.argv) > 1 else "."
    os.makedirs(out_dir, exist_ok=True)
    use_case(os.path.join(out_dir, "01_use_case_tong_quat.svg"))
    activity_project(os.path.join(out_dir, "07_activity_dang_du_an.svg"))
    activity_contact(os.path.join(out_dir, "08_activity_lien_he.svg"))
    architecture(os.path.join(out_dir, "09_kien_truc_mvc.svg"))
    print("ok")
