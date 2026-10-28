# Kiểm thử nhanh các luồng chính của website TVU Startup
import re, uuid, json, struct, zlib, urllib.request, urllib.parse, urllib.error, http.cookiejar

BASE = "http://localhost:5080"
results = []

class NoRedirect(urllib.request.HTTPRedirectHandler):
    def redirect_request(self, *a, **k): return None

def client():
    jar = http.cookiejar.CookieJar()
    return urllib.request.build_opener(urllib.request.HTTPCookieProcessor(jar), NoRedirect())

def req(op, path, data=None, headers=None, files=None):
    headers = dict(headers or {})
    body = None
    if files is not None:
        boundary = uuid.uuid4().hex
        parts = []
        for k, v in (data or {}).items():
            parts.append(f'--{boundary}\r\nContent-Disposition: form-data; name="{k}"\r\n\r\n{v}\r\n'.encode())
        for k, (fname, content, ctype) in files:
            parts.append(f'--{boundary}\r\nContent-Disposition: form-data; name="{k}"; filename="{fname}"\r\nContent-Type: {ctype}\r\n\r\n'.encode() + content + b'\r\n')
        parts.append(f'--{boundary}--\r\n'.encode())
        body = b''.join(parts)
        headers['Content-Type'] = f'multipart/form-data; boundary={boundary}'
    elif data is not None:
        body = urllib.parse.urlencode(data).encode()
        headers['Content-Type'] = 'application/x-www-form-urlencoded'
    r = urllib.request.Request(BASE + path, data=body, headers=headers, method='POST' if body is not None else 'GET')
    try:
        resp = op.open(r)
        return resp.status, resp.headers.get('Location'), resp.read().decode('utf-8', 'replace')
    except urllib.error.HTTPError as e:
        return e.code, e.headers.get('Location'), e.read().decode('utf-8', 'replace')

def token(html):
    m = re.search(r'name="__RequestVerificationToken" type="hidden" value="([^"]+)"', html)
    return m.group(1) if m else ''

def check(name, cond, info=''):
    results.append((name, bool(cond), info))

def login(op, email, pw):
    _, _, html = req(op, '/dang-nhap')
    return req(op, '/dang-nhap', {'Email': email, 'Password': pw, '__RequestVerificationToken': token(html)})

def png_bytes():
    raw = b''.join(b'\x00' + b'\xff\x80\x00' * 4 for _ in range(4))
    def chunk(t, d): return struct.pack('>I', len(d)) + t + d + struct.pack('>I', zlib.crc32(t + d) & 0xffffffff)
    return b'\x89PNG\r\n\x1a\n' + chunk(b'IHDR', struct.pack('>IIBBBBB', 4, 4, 8, 2, 0, 0, 0)) + chunk(b'IDAT', zlib.compress(raw)) + chunk(b'IEND', b'')

# ===== Khách =====
guest = client()
for path in ['/', '/du-an', '/du-an/zalord', '/du-an?q=tuy%20bien', '/tin-tuc', '/tin-tuc/ngay-hoi-khoi-nghiep-sinh-vien-2026', '/lien-he', '/dang-nhap', '/dang-ky']:
    s, _, _ = req(guest, path)
    check(f'Khách GET {path}', s == 200, s)
s, _, html = req(guest, '/du-an/zalord')
check('Zalord ẩn bản Full/Premium với khách', 'Full Dev Menu' not in html and 'Premium' not in html and 'Zalord Elite' in html)
s, _, html = req(guest, '/du-an?q=tuy%20bien')
check('Tìm kiếm không dấu ra Zalord', 'Zalord' in html)
s, _, _ = req(guest, '/du-an/xa-phong-thao-duoc-moc-an')
check('Khách không xem được dự án Chờ duyệt (404)', s == 404, s)
s, _, _ = req(guest, '/tin-tuc/ket-noi-doanh-nghiep-voi-du-an-sinh-vien')
check('Tin chưa xuất bản trả 404', s == 404, s)
s, loc, _ = req(guest, '/du-an-cua-toi')
check('Khách vào Dự án của tôi → chuyển tới đăng nhập', s == 302 and '/dang-nhap' in (loc or ''), loc)
s, loc, _ = req(guest, '/quan-tri')
check('Khách vào /quan-tri → chuyển tới đăng nhập', s == 302 and '/dang-nhap' in (loc or ''), loc)

# Gửi liên hệ hợp tác khi chưa đăng nhập
_, _, html = req(guest, '/du-an/agrisense')
pid = re.search(r'/du-an/(\d+)/lien-he', html).group(1)
s, loc, _ = req(guest, f'/du-an/{pid}/lien-he', {'ContactForm.FullName': 'Khách Kiểm Thử', 'ContactForm.Email': 'khach@example.com',
    'ContactForm.RequestType': 'DauTu', 'ContactForm.Message': 'Tôi muốn tìm hiểu cơ hội đầu tư vào dự án.', '__RequestVerificationToken': token(html)})
check('Khách gửi liên hệ hợp tác', s == 302, s)
s, _, _ = req(guest, f'/du-an/{pid}/lien-he', {'ContactForm.FullName': 'X', 'ContactForm.Email': 'x@example.com', 'ContactForm.Message': 'Thiếu token chống CSRF'})
check('POST thiếu AntiForgery token bị chặn (400)', s == 400, s)

# Đăng nhập sai
s, _, html = login(client(), '170125077@st.tvu.edu.vn', 'SaiMatKhau')
check('TC: Đăng nhập sai mật khẩu báo lỗi', s == 200 and 'Email hoặc mật khẩu không đúng' in html, s)

# ===== Sinh viên =====
sv = client()
s, loc, _ = login(sv, '170125077@st.tvu.edu.vn', 'SinhVien@123')
check('Sinh viên đăng nhập thành công', s == 302, loc)
s, _, html = req(sv, '/du-an/zalord')
check('Chủ dự án thấy bản nội bộ (Full/Premium)', 'Full Dev Menu' in html and 'Nội bộ' in html)
s, _, _ = req(sv, '/quan-tri')
check('Sinh viên vào /quan-tri bị từ chối (403)', s in (302, 403), s)

# Bình chọn bằng AJAX dự án agrisense
_, _, html = req(sv, '/du-an/agrisense')
s, _, body = req(sv, f'/du-an/{pid}/binh-chon', {'__RequestVerificationToken': token(html)}, headers={'X-Requested-With': 'XMLHttpRequest'})
first = json.loads(body) if s == 200 else {}
s2, _, body2 = req(sv, f'/du-an/{pid}/binh-chon', {'__RequestVerificationToken': token(html)}, headers={'X-Requested-With': 'XMLHttpRequest'})
second = json.loads(body2) if s2 == 200 else {}
check('Bình chọn bật / tắt (AJAX)', s == 200 and first.get('voted') != second.get('voted'), (first, second))

# Không tự bình chọn dự án của mình
_, _, html = req(sv, '/du-an/zalord')
zid = re.search(r'/du-an/(\d+)/binh-luan', html).group(1)
s, _, _ = req(sv, f'/du-an/{zid}/binh-chon', {'__RequestVerificationToken': token(html)}, headers={'X-Requested-With': 'XMLHttpRequest'})
check('Không bình chọn được dự án của chính mình', s == 400, s)

# Bình luận + chấm sao + XSS
_, _, html = req(sv, '/du-an/agrisense')
s, _, _ = req(sv, f'/du-an/{pid}/binh-luan', {'Content': '<script>alert(1)</script> Bình luận kiểm thử', 'Rating': '4', '__RequestVerificationToken': token(html)})
_, _, html = req(sv, '/du-an/agrisense')
check('Gửi bình luận', s == 302 and 'Bình luận kiểm thử' in html)
check('Bình luận chứa <script> được mã hóa (chống XSS)', '<script>alert(1)</script>' not in html and '&lt;script&gt;' in html)

# Tạo dự án mới
_, _, html = req(sv, '/du-an-cua-toi/tao-moi')
s, loc, _ = req(sv, '/du-an-cua-toi/tao-moi', {
    'ProjectName': 'Dự án Kiểm Thử Tự Động', 'ShortDescription': 'Mô tả ngắn cho dự án kiểm thử tự động, đủ hai mươi ký tự.',
    'Content': '<p>Nội dung <strong>đậm</strong></p><script>alert("xss")</script><img src=x onerror=alert(1)>',
    'CategoryId': '1', 'FacultyId': '1', 'Stage': 'NguyenMau', '__RequestVerificationToken': token(html)}, files=[])
check('Sinh viên tạo dự án (Nháp)', s == 302 and '/sua' in (loc or ''), loc)
new_id = re.search(r'/du-an-cua-toi/(\d+)/sua', loc or '').group(1)
_, _, html = req(sv, f'/du-an-cua-toi/{new_id}/sua')
slug = re.search(r'/du-an/(du-an-kiem-thu-tu-dong[^"?#]*)', html).group(1)
check('Slug sinh từ tên tiếng Việt', slug.startswith('du-an-kiem-thu-tu-dong'), slug)
_, _, detail = req(sv, f'/du-an/{slug}')
check('Nội dung HTML được lọc (bỏ script, img)', '<strong>đậm</strong>' in detail and 'alert("xss")' not in detail and 'onerror' not in detail)
check('Chủ dự án xem trước dự án Nháp', 'Bản xem trước' in detail)

# Gửi duyệt khi thiếu ảnh bìa và tính năng → bị từ chối
s, loc, _ = req(sv, f'/du-an-cua-toi/{new_id}/gui-duyet', {'__RequestVerificationToken': token(html)})
_, _, html = req(sv, f'/du-an-cua-toi/{new_id}/sua')
check('Không gửi duyệt khi thiếu ảnh bìa / tính năng', 'Chưa thể gửi duyệt' in html)

# Thêm tính năng, tải ảnh bìa (PNG hợp lệ) và thử tải tệp giả ảnh
req(sv, f'/du-an-cua-toi/{new_id}/tinh-nang', {'NewFeature.Title': 'Tính năng A', 'NewFeature.Icon': 'bi-star', '__RequestVerificationToken': token(html)})
_, _, html = req(sv, f'/du-an-cua-toi/{new_id}/sua')
form = {'Form.ProjectId': new_id, 'Form.ProjectName': 'Dự án Kiểm Thử Tự Động', 'Form.ShortDescription': 'Mô tả ngắn cho dự án kiểm thử tự động, đủ hai mươi ký tự.',
        'Form.CategoryId': '1', 'Form.FacultyId': '1', 'Form.Stage': 'NguyenMau', '__RequestVerificationToken': token(html)}
s, _, page = req(sv, f'/du-an-cua-toi/{new_id}/sua', form, files=[('Form.CoverFile', ('fake.png', b'<script>not an image</script>', 'image/png'))])
check('Chặn tệp giả mạo đuôi .png', s == 200 and 'không phải là ảnh hợp lệ' in page, s)
s, loc, _ = req(sv, f'/du-an-cua-toi/{new_id}/sua', form, files=[('Form.CoverFile', ('cover.png', png_bytes(), 'image/png'))])
check('Tải ảnh bìa PNG hợp lệ', s == 302, s)
_, _, html = req(sv, f'/du-an-cua-toi/{new_id}/sua')
s, loc, _ = req(sv, f'/du-an-cua-toi/{new_id}/gui-duyet', {'__RequestVerificationToken': token(html)})
_, _, html = req(sv, '/du-an-cua-toi')
check('Gửi duyệt thành công → Chờ duyệt', 'Đã gửi dự án tới Ban quản trị' in html)

# Sinh viên không sửa được dự án của người khác
s, _, _ = req(sv, '/du-an-cua-toi/2/sua')
check('Không sửa được dự án của sinh viên khác (404)', s == 404, s)

# ===== Quản trị viên =====
ad = client()
s, loc, _ = login(ad, 'admin@tvu.edu.vn', 'Admin@123')
check('Admin đăng nhập → chuyển tới /quan-tri', s == 302 and 'quan-tri' in (loc or '').lower(), loc)
for path in ['/quan-tri', '/quan-tri/Projects', '/quan-tri/Projects?status=ChoDuyet', '/quan-tri/Contacts', '/quan-tri/Comments',
             '/quan-tri/News', '/quan-tri/News/Create', '/quan-tri/Banners', '/quan-tri/Banners/Create', '/quan-tri/Categories',
             '/quan-tri/Categories/Edit/1', '/quan-tri/Faculties', '/quan-tri/Accounts', '/du-an-cua-toi']:
    s, loc, _ = req(ad, path)
    if path == '/du-an-cua-toi':
        check('Admin vào Dự án của tôi → trang 403', s == 302 and '/loi/403' in (loc or ''), (s, loc))
    else:
        check(f'Admin GET {path}', s == 200, s)

_, _, html = req(ad, '/quan-tri/Projects?status=ChoDuyet')
s, _, _ = req(ad, f'/quan-tri/Projects/Reject/{new_id}', {'reason': '', '__RequestVerificationToken': token(html)})
_, _, html = req(ad, '/quan-tri/Projects?status=ChoDuyet')
check('Từ chối bắt buộc nhập lý do', 'Vui lòng nhập lý do' in html)
s, _, _ = req(ad, f'/quan-tri/Projects/Approve/{new_id}', {'__RequestVerificationToken': token(html)})
s2, _, _ = req(guest, f'/du-an/{slug}')
check('Admin duyệt → khách xem được dự án', s == 302 and s2 == 200, (s, s2))

_, _, html = req(ad, '/quan-tri/Categories')
s, _, _ = req(ad, '/quan-tri/Categories/Delete/1', {'__RequestVerificationToken': token(html)})
_, _, html = req(ad, '/quan-tri/Categories')
check('Không xóa lĩnh vực đang có dự án', 'Không thể xóa' in html)

# Khóa tài khoản sinh viên → bị đăng xuất ngay
_, _, html = req(ad, '/quan-tri/Accounts?q=170125077')
acc_id = re.search(r'/quan-tri/Accounts/ToggleLock/(\d+)', html).group(1)
req(ad, f'/quan-tri/Accounts/ToggleLock/{acc_id}', {'__RequestVerificationToken': token(html)})
s, loc, _ = req(sv, '/du-an-cua-toi')
check('Tài khoản bị khóa bị đăng xuất ngay', s == 302 and '/dang-nhap' in (loc or ''), (s, loc))
s, _, page = login(client(), '170125077@st.tvu.edu.vn', 'SinhVien@123')
check('Tài khoản bị khóa không đăng nhập được', 'đã bị khóa' in page, s)
_, _, html = req(ad, '/quan-tri/Accounts?q=170125077')
req(ad, f'/quan-tri/Accounts/ToggleLock/{acc_id}', {'__RequestVerificationToken': token(html)})  # mở khóa lại

# Dọn dẹp: xóa dự án kiểm thử
_, _, html = req(ad, '/quan-tri/Projects')
s, _, _ = req(ad, f'/quan-tri/Projects/Delete/{new_id}', {'__RequestVerificationToken': token(html)})
check('Admin xóa dự án', s == 302, s)

# ===== Đăng ký =====
reg = client()
_, _, html = req(reg, '/dang-ky')
email = f'test{uuid.uuid4().hex[:6]}@st.tvu.edu.vn'
s, loc, _ = req(reg, '/dang-ky', {'FullName': 'Sinh Viên Mới', 'Email': email, 'StudentCode': '110199999', 'ClassName': 'DA22TT',
    'FacultyId': '1', 'Password': 'MatKhau@1', 'ConfirmPassword': 'MatKhau@1', '__RequestVerificationToken': token(html)})
check('Đăng ký tài khoản mới', s == 302, (s, loc))
bad = client()
_, _, html = req(bad, '/dang-ky')
s, _, page = req(bad, '/dang-ky', {'FullName': 'A', 'Email': '170125077@st.tvu.edu.vn', 'StudentCode': '1', 'ClassName': 'X',
    'FacultyId': '1', 'Password': '123', 'ConfirmPassword': '456', '__RequestVerificationToken': token(html)})
check('Đăng ký trùng email / sai MSSV / mật khẩu không khớp bị báo lỗi',
      s == 200 and 'đã được đăng ký' in page and 'MSSV chỉ gồm' in page and 'không khớp' in page, s)

# ===== Đổi mật khẩu (dùng tài khoản vừa đăng ký) =====
_, _, html = req(reg, '/tai-khoan/doi-mat-khau')
s, _, page = req(reg, '/tai-khoan/doi-mat-khau', {'CurrentPassword': 'SaiMatKhau', 'NewPassword': 'MoiMoi@2',
    'ConfirmPassword': 'MoiMoi@2', '__RequestVerificationToken': token(html)})
check('Đổi mật khẩu sai mật khẩu hiện tại bị báo lỗi', s == 200 and 'Mật khẩu hiện tại không đúng' in page, s)
s, loc, _ = req(reg, '/tai-khoan/doi-mat-khau', {'CurrentPassword': 'MatKhau@1', 'NewPassword': 'MoiMoi@2',
    'ConfirmPassword': 'MoiMoi@2', '__RequestVerificationToken': token(html)})
s2, loc2, _ = login(client(), email, 'MoiMoi@2')
check('Đổi mật khẩu thành công, đăng nhập bằng mật khẩu mới', s == 302 and s2 == 302, (s, s2))

# ===== Rút lại yêu cầu duyệt rồi gửi duyệt lại (dự án mẫu "Xà phòng thảo dược Mộc An") =====
han = client()
login(han, '111122015@st.tvu.edu.vn', 'SinhVien@123')
_, _, html = req(han, '/du-an-cua-toi')
xp = re.search(r'/du-an-cua-toi/(\d+)/rut-lai', html).group(1)
req(han, f'/du-an-cua-toi/{xp}/rut-lai', {'__RequestVerificationToken': token(html)})
_, _, html = req(han, '/du-an-cua-toi')
check('Rút lại yêu cầu duyệt → dự án về Nháp', 'trở về trạng thái Nháp' in html and f'/du-an-cua-toi/{xp}/gui-duyet' in html)
req(han, f'/du-an-cua-toi/{xp}/gui-duyet', {'__RequestVerificationToken': token(html)})
_, _, html = req(han, '/du-an-cua-toi')
check('Gửi duyệt lại dự án đã rút', f'/du-an-cua-toi/{xp}/rut-lai' in html)

# ===== Admin đặt lại mật khẩu cho tài khoản vừa đăng ký =====
_, _, html = req(ad, f'/quan-tri/Accounts?q={urllib.parse.quote(email)}')
rid = re.search(r'/quan-tri/Accounts/ResetPassword/(\d+)', html).group(1)
req(ad, f'/quan-tri/Accounts/ResetPassword/{rid}', {'__RequestVerificationToken': token(html)})
_, _, html = req(ad, '/quan-tri/Accounts')
temp = re.search(r'là: (Tvu@\d{6})', html)
s, _, _ = login(client(), email, temp.group(1)) if temp else (0, None, None)
check('Admin đặt lại mật khẩu, đăng nhập bằng mật khẩu tạm', temp is not None and s == 302, s)

ok = sum(1 for _, c, _ in results if c)
for name, c, info in results:
    print(('PASS ' if c else 'FAIL ') + name + ('' if c else f'  -> {info}'))
print(f'\n{ok}/{len(results)} PASS')
