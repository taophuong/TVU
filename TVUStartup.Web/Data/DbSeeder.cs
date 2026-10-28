using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TVUStartup.Web.Models.Entities;

namespace TVUStartup.Web.Data;

/// <summary>
/// Khởi tạo CSDL và nạp dữ liệu mẫu khi CSDL còn trống.
/// Ngoài dự án Zalord, các sinh viên, dự án, bình luận, tin tức... đều là DỮ LIỆU MẪU (hư cấu) để minh họa.
/// </summary>
public static class DbSeeder
{
    private const string SamplePassword = "SinhVien@123";
    private const string AdminPassword = "Admin@123";

    public static async Task SeedAsync(AppDbContext db, IPasswordHasher<Account> hasher)
    {
        await db.Database.MigrateAsync();

        if (await db.Roles.AnyAsync()) return; // đã có dữ liệu

        // ===== Vai trò =====
        // Thêm lần lượt để RoleId khớp với hằng số Role.AdminId = 1, Role.SinhVienId = 2
        db.Roles.Add(new Role { RoleName = Role.Admin, Description = "Quản trị viên hệ thống" });
        await db.SaveChangesAsync();
        db.Roles.Add(new Role { RoleName = Role.SinhVien, Description = "Sinh viên đăng và quản lý dự án khởi nghiệp" });
        await db.SaveChangesAsync();

        // ===== Khoa =====
        var f = new[]
        {
            "Khoa Kỹ thuật và Công nghệ",                          // 0
            "Khoa Kinh tế, Luật",                                  // 1
            "Khoa Nông nghiệp - Thủy sản",                         // 2
            "Khoa Y - Dược",                                       // 3
            "Khoa Sư phạm",                                        // 4
            "Khoa Hóa học Ứng dụng",                               // 5
            "Khoa Ngôn ngữ - Văn hóa - Nghệ thuật Khmer Nam Bộ"    // 6
        }.Select((name, i) => new Faculty { FacultyName = name, DisplayOrder = i + 1 }).ToList();
        db.Faculties.AddRange(f);

        // ===== Lĩnh vực =====
        var c = new (string Name, string Slug, string Icon, string Desc)[]
        {
            ("Công nghệ phần mềm", "cong-nghe-phan-mem", "bi-code-slash", "Ứng dụng, phần mềm, nền tảng số"),
            ("Nông nghiệp công nghệ cao", "nong-nghiep-cong-nghe-cao", "bi-tree", "IoT, tự động hóa, canh tác và nuôi trồng thông minh"),
            ("Thực phẩm & Đồ uống", "thuc-pham-do-uong", "bi-cup-hot", "Chế biến nông sản, đặc sản địa phương"),
            ("Giáo dục", "giao-duc", "bi-mortarboard", "Học tập, đào tạo, kết nối người học"),
            ("Y tế & Sức khỏe", "y-te-suc-khoe", "bi-heart-pulse", "Chăm sóc sức khỏe, dược liệu"),
            ("Môi trường", "moi-truong", "bi-recycle", "Sản phẩm xanh, giảm rác thải"),
            ("Dịch vụ & Du lịch", "dich-vu-du-lich", "bi-geo-alt", "Du lịch cộng đồng, dịch vụ địa phương"),
            ("Văn hóa & Sáng tạo", "van-hoa-sang-tao", "bi-palette", "Thiết kế, thủ công, bảo tồn văn hóa")
        }.Select((x, i) => new Category { CategoryName = x.Name, Slug = x.Slug, Icon = x.Icon, Description = x.Desc, DisplayOrder = i + 1 }).ToList();
        db.Categories.AddRange(c);
        await db.SaveChangesAsync();

        // ===== Tài khoản =====
        Account NewStudent(string name, string code, string className, Faculty faculty) => new()
        {
            Email = $"{code}@st.tvu.edu.vn",
            PasswordHash = "", // gán bên dưới
            FullName = name,
            StudentCode = code,
            ClassName = className,
            FacultyId = faculty.FacultyId,
            RoleId = Role.SinhVienId,
            Status = AccountStatus.HoatDong,
            CreatedAt = new DateTime(2026, 3, 1)
        };

        var admin = new Account
        {
            Email = "admin@tvu.edu.vn",
            PasswordHash = "",
            FullName = "Ban quản trị TVU Startup",
            RoleId = Role.AdminId,
            Status = AccountStatus.HoatDong,
            CreatedAt = new DateTime(2026, 2, 1)
        };
        var phuong = NewStudent("Lê Văn Bình Phương", "170125077", "DK25TTG2", f[0]);
        var khoa = NewStudent("Nguyễn Minh Khoa", "110122001", "DA22TTA", f[0]);
        var han = NewStudent("Trần Thị Ngọc Hân", "111122015", "DA22HHA", f[5]);
        var phia = NewStudent("Thạch Sô Phia", "116122008", "DA22NNKM", f[6]);
        var bao = NewStudent("Phạm Gia Bảo", "114122030", "DA22YKA", f[3]);
        var truc = NewStudent("Huỳnh Thanh Trúc", "112122021", "DA22QTKD", f[1]);
        var nam = NewStudent("Võ Hoàng Nam", "113122044", "DA22NTTS", f[2]);
        var ngoc = NewStudent("Lâm Bảo Ngọc", "115122012", "DA22SPT", f[4]);
        var students = new[] { phuong, khoa, han, phia, bao, truc, nam, ngoc };

        // Băm mật khẩu bằng PasswordHasher (PBKDF2 + salt ngẫu nhiên)
        admin.PasswordHash = hasher.HashPassword(admin, AdminPassword);
        foreach (var s in students) s.PasswordHash = hasher.HashPassword(s, SamplePassword);

        db.Accounts.Add(admin);
        db.Accounts.AddRange(students);
        await db.SaveChangesAsync();

        // ===== Dự án =====
        TeamMember Member(Account a, string position, int order) => new()
        {
            AccountId = a.AccountId, FullName = a.FullName, Position = position,
            StudentCode = a.StudentCode, ClassName = a.ClassName, Email = a.Email, DisplayOrder = order
        };
        TeamMember Guest(string name, string position, string className, int order) => new()
        {
            FullName = name, Position = position, ClassName = className, DisplayOrder = order
        };
        ProjectFeature Feature(string title, string desc, string icon, int order) => new()
        {
            Title = title, Description = desc, Icon = icon, DisplayOrder = order
        };
        List<ProjectImage> SampleImages(string folder) =>
        [
            new() { ImageUrl = $"/uploads/sample/projects/{folder}/anh-1.svg", Caption = "Giao diện chính", DisplayOrder = 1 },
            new() { ImageUrl = $"/uploads/sample/projects/{folder}/anh-2.svg", Caption = "Sản phẩm thực tế", DisplayOrder = 2 }
        ];
        Project NewProject(string name, string slug, string folder, string slogan, string shortDesc, string content,
            Category category, Faculty faculty, Account owner, ProjectStage stage, ProjectStatus status,
            DateTime createdAt, int views, bool featured = false) => new()
        {
            ProjectName = name,
            Slug = slug,
            Slogan = slogan,
            ShortDescription = shortDesc,
            Content = content,
            Logo = $"/uploads/sample/projects/{folder}/logo.svg",
            CoverImage = $"/uploads/sample/projects/{folder}/cover.svg",
            CategoryId = category.CategoryId,
            FacultyId = faculty.FacultyId,
            OwnerId = owner.AccountId,
            Stage = stage,
            Status = status,
            IsFeatured = featured,
            ViewCount = views,
            CreatedAt = createdAt,
            FoundedDate = DateOnly.FromDateTime(createdAt.AddMonths(-4)),
            // Dự án đã duyệt: ghi nhận người duyệt và thời điểm duyệt
            ApprovedAt = status == ProjectStatus.DaDuyet ? createdAt.AddDays(2) : null,
            ApprovedBy = status == ProjectStatus.DaDuyet ? admin.AccountId : null,
            Images = SampleImages(folder)
        };

        // --- Zalord: sản phẩm thật của sinh viên Lê Văn Bình Phương ---
        var zalord = new Project
        {
            ProjectName = "Zalord",
            Slug = "zalord",
            Slogan = "Cá nhân hóa Zalo theo cách của bạn",
            ShortDescription = "Tiện ích mở rộng cho Zalo PC và Zalo Web, giúp tùy biến giao diện, tăng quyền riêng tư và hỗ trợ nhắn tin nhanh hơn, với menu riêng để bật/tắt từng chức năng.",
            Content = """
                <p><strong>Zalord</strong> là tiện ích mở rộng dành cho Zalo, giúp người dùng có thêm nhiều tùy chọn cá nhân hóa, riêng tư và hỗ trợ thao tác so với Zalo mặc định.</p>
                <p>Zalord cho phép đổi font chữ, đổi theme, thêm hình nền riêng, tùy biến khung chat; ẩn tên, avatar và nội dung tin nhắn khi quay màn hình hoặc làm việc nơi đông người; dùng reaction tùy chỉnh và quản lý bạn bè, nhóm hàng loạt. Tất cả chức năng được gom vào một menu riêng để bật/tắt theo nhu cầu.</p>
                <p>Sản phẩm có hai bản phát hành công khai: <strong>Zalord Small</strong> cho Zalo PC và <strong>Zalord Elite</strong> – tiện ích Chrome cho Zalo Web (chat.zalo.me). Sản phẩm đã được chia sẻ tới cộng đồng người dùng và nhận nhiều phản hồi tích cực.</p>
                """,
            Logo = "/uploads/sample/projects/zalord/logo.png",
            CoverImage = "/uploads/sample/projects/zalord/zalord-small-menu.png",
            WebsiteUrl = "https://github.com/taophuong/Zalordv36",
            CategoryId = c[0].CategoryId,
            FacultyId = f[0].FacultyId,
            OwnerId = phuong.AccountId,
            Stage = ProjectStage.DaRaMat,
            Status = ProjectStatus.DaDuyet,
            IsFeatured = true,
            ViewCount = 1286,
            FoundedDate = new DateOnly(2026, 6, 9),
            CreatedAt = new DateTime(2026, 10, 20),
            ApprovedAt = new DateTime(2026, 10, 22),
            ApprovedBy = admin.AccountId,
            Images =
            {
                new ProjectImage { ImageUrl = "/uploads/sample/projects/zalord/zalord-small-menu.png", Caption = "Menu Zalord bản Small", DisplayOrder = 1 }
            },
            Features =
            {
                Feature("Tùy biến giao diện", "Đổi theme sáng/tối, hình nền riêng, hiệu ứng theo mùa (Xuân, Thu, Đông).", "bi-palette", 1),
                Feature("Font chữ tùy chọn", "Áp dụng bất kỳ font nào từ Google Fonts cho toàn bộ Zalo.", "bi-fonts", 2),
                Feature("Tùy biến khung chat", "Chỉnh màu sắc, font, nền cho vùng trò chuyện; đổi màu chữ khi soạn tin và màu tên người dùng.", "bi-chat-square-heart", 3),
                Feature("Ẩn danh & riêng tư", "Ẩn tên, avatar, nội dung tin nhắn, làm mờ hội thoại khi quay màn hình; ẩn trạng thái đang soạn, đã nhận, đã xem.", "bi-incognito", 4),
                Feature("Reaction tùy chỉnh", "Mở rộng bộ biểu cảm ngoài các reaction mặc định của Zalo.", "bi-emoji-smile", 5),
                Feature("Quản lý bạn bè & nhóm", "Chọn nhiều, gửi tin nhắn hàng loạt, phân loại nhanh bạn bè.", "bi-people", 6),
                Feature("Menu bật/tắt riêng", "Gom mọi chức năng vào một menu, bật/tắt từng tính năng theo nhu cầu.", "bi-toggles", 7)
            },
            Versions =
            {
                new ProjectVersion { VersionName = "Zalord Small v3.6", Platform = "Zalo PC (Windows)", IsPublic = true },
                new ProjectVersion { VersionName = "Zalord Elite", Platform = "Chrome Extension - chat.zalo.me", ReleaseDate = new DateOnly(2026, 8, 12), IsPublic = true },
                new ProjectVersion { VersionName = "Zalord Full Dev Menu", Platform = "Zalo PC - bản nghiên cứu", IsPublic = false },
                new ProjectVersion { VersionName = "Zalord Premium", Platform = "Chrome Extension - bản nghiên cứu", IsPublic = false }
            },
            TeamMembers = { Member(phuong, "Trưởng nhóm / Lập trình viên", 1) }
        };

        var agrisense = NewProject("AgriSense", "agrisense", "agrisense",
            "Cảm biến độ mặn cho ruộng lúa vùng ven biển",
            "Hệ thống cảm biến IoT đo độ mặn, độ ẩm đất và mực nước, cảnh báo cho nông dân qua ứng dụng điện thoại khi nước mặn xâm nhập.",
            """
            <p>Mùa khô, nước mặn xâm nhập sâu vào nội đồng gây thiệt hại lớn cho lúa và hoa màu ở Trà Vinh. <strong>AgriSense</strong> đặt các trạm cảm biến giá rẻ tại cống, kênh nội đồng để đo độ mặn liên tục.</p>
            <p>Dữ liệu được gửi về máy chủ qua mạng 4G/LoRa; khi độ mặn vượt ngưỡng an toàn, ứng dụng sẽ gửi cảnh báo để nông dân chủ động đóng cống, ngưng lấy nước.</p>
            """,
            c[1], f[0], khoa, ProjectStage.NguyenMau, ProjectStatus.DaDuyet, new DateTime(2026, 9, 5), 842, featured: true);
        agrisense.Features =
        [
            Feature("Đo độ mặn thời gian thực", "Cập nhật mỗi 5 phút, hiển thị biểu đồ theo ngày.", "bi-droplet-half", 1),
            Feature("Cảnh báo tức thì", "Thông báo đẩy và SMS khi độ mặn vượt ngưỡng.", "bi-bell", 2),
            Feature("Trạm cảm biến giá rẻ", "Linh kiện phổ thông, chi phí dưới 1,5 triệu đồng/trạm.", "bi-cpu", 3),
            Feature("Bản đồ trạm đo", "Xem độ mặn các khu vực lân cận trên bản đồ.", "bi-map", 4)
        ];
        agrisense.Versions =
        [
            new() { VersionName = "AgriSense App 0.9 (beta)", Platform = "Android", ReleaseDate = new DateOnly(2026, 9, 20), IsPublic = true }
        ];
        agrisense.TeamMembers = [Member(khoa, "Trưởng nhóm / Phần cứng", 1), Guest("Đỗ Quốc Huy", "Lập trình ứng dụng", "DA22TTA", 2), Guest("Kim Thị Sà Rết", "Khảo sát thực địa", "DA22NNA", 3)];

        var traMangCau = NewProject("Trà Mãng Cầu Xiêm Xanh", "tra-mang-cau-xiem-xanh", "tra-mang-cau",
            "Đặc sản Trà Vinh trong từng túi lọc",
            "Trà túi lọc từ lá và quả mãng cầu xiêm sấy lạnh, giữ trọn hương vị tự nhiên, không chất bảo quản, nguyên liệu từ nông hộ Càng Long.",
            """
            <p>Mãng cầu xiêm là cây trồng chủ lực ở nhiều xã của huyện Càng Long, nhưng giá bán quả tươi bấp bênh. Nhóm nghiên cứu quy trình <strong>sấy lạnh</strong> để chế biến lá và quả thành trà túi lọc.</p>
            <p>Sản phẩm đã được kiểm nghiệm an toàn thực phẩm, bán thử tại các hội chợ nông sản và cửa hàng đặc sản trong tỉnh.</p>
            """,
            c[2], f[5], han, ProjectStage.DangTangTruong, ProjectStatus.DaDuyet, new DateTime(2026, 8, 12), 1530, featured: true);
        traMangCau.CommunityUrl = "https://www.facebook.com/";
        traMangCau.Features =
        [
            Feature("Sấy lạnh giữ dưỡng chất", "Nhiệt độ thấp giúp giữ màu, mùi và vitamin.", "bi-snow", 1),
            Feature("Nguyên liệu địa phương", "Thu mua trực tiếp từ nông hộ với giá ổn định.", "bi-basket", 2),
            Feature("Bao bì thân thiện", "Hộp giấy kraft, túi lọc phân hủy sinh học.", "bi-box2-heart", 3)
        ];
        traMangCau.TeamMembers = [Member(han, "Trưởng nhóm / R&D", 1), Guest("Nguyễn Thị Kiều", "Kinh doanh", "DA22QTKD", 2)];

        var eduKhmer = NewProject("EduKhmer", "edukhmer", "edukhmer",
            "Học tiếng Khmer dễ như chơi",
            "Ứng dụng học tiếng Khmer cơ bản qua trò chơi, flashcard và phát âm chuẩn, hướng tới học sinh, sinh viên và người đi làm tại Trà Vinh.",
            """
            <p>Trà Vinh có cộng đồng người Khmer đông đảo, nhu cầu học tiếng Khmer để giao tiếp, làm việc ngày càng lớn. <strong>EduKhmer</strong> thiết kế lộ trình 30 bài học ngắn, mỗi bài 10 phút.</p>
            <p>Nội dung được biên soạn và thu âm cùng giảng viên, sinh viên chuyên ngành Ngôn ngữ Khmer.</p>
            """,
            c[3], f[6], phia, ProjectStage.NguyenMau, ProjectStatus.DaDuyet, new DateTime(2026, 9, 18), 615);
        eduKhmer.Features =
        [
            Feature("Bảng chữ cái tương tác", "Chạm để nghe phát âm từng phụ âm, nguyên âm.", "bi-alphabet", 1),
            Feature("Flashcard thông minh", "Lặp lại ngắt quãng giúp nhớ từ vựng lâu hơn.", "bi-card-text", 2),
            Feature("Trò chơi ôn tập", "Ghép từ, nghe chọn đáp án, tính điểm theo tuần.", "bi-controller", 3)
        ];
        eduKhmer.Versions = [new() { VersionName = "EduKhmer Web 1.0", Platform = "Web", ReleaseDate = new DateOnly(2026, 10, 1), IsPublic = true }];
        eduKhmer.TeamMembers = [Member(phia, "Trưởng nhóm / Nội dung", 1), Guest("Trần Minh Tâm", "Lập trình viên", "DA22TTB", 2)];

        var mediCare = NewProject("MediCare TV", "medicare-tv", "medicare-tv",
            "Nhắc uống thuốc, đặt lịch tái khám",
            "Ứng dụng giúp người cao tuổi và người bệnh mạn tính nhắc giờ uống thuốc, ghi chỉ số huyết áp, đường huyết và đặt lịch tái khám.",
            """
            <p>Nhiều bệnh nhân mạn tính quên uống thuốc hoặc trễ lịch tái khám. <strong>MediCare TV</strong> đơn giản hóa việc theo dõi với giao diện chữ lớn, thao tác ít bước.</p>
            <p>Người thân có thể theo dõi tình hình dùng thuốc của ông bà, cha mẹ từ xa.</p>
            """,
            c[4], f[3], bao, ProjectStage.YTuong, ProjectStatus.DaDuyet, new DateTime(2026, 10, 2), 356);
        mediCare.Features =
        [
            Feature("Nhắc giờ uống thuốc", "Báo thức kèm hình viên thuốc, xác nhận đã uống.", "bi-alarm", 1),
            Feature("Sổ theo dõi chỉ số", "Ghi huyết áp, đường huyết, xem biểu đồ.", "bi-graph-up", 2),
            Feature("Kết nối người thân", "Chia sẻ lịch uống thuốc cho con cháu.", "bi-people", 3)
        ];
        mediCare.TeamMembers = [Member(bao, "Trưởng nhóm", 1), Guest("Lý Thị Mỹ Duyên", "Điều dưỡng – tư vấn chuyên môn", "DA22DDA", 2)];

        var greenStraw = NewProject("Ống hút cỏ GreenStraw", "ong-hut-co-greenstraw", "ong-hut-co",
            "Thay ống hút nhựa bằng cỏ bàng tự nhiên",
            "Ống hút làm từ cỏ bàng mọc tự nhiên ở vùng đất phèn, phơi sấy và tiệt trùng, phân hủy hoàn toàn sau vài tuần, tạo thêm thu nhập cho phụ nữ nông thôn.",
            """
            <p><strong>GreenStraw</strong> hợp tác với các hộ dân thu hoạch cỏ bàng, sơ chế, cắt, rửa và sấy khô theo quy trình đảm bảo vệ sinh.</p>
            <p>Sản phẩm đang cung cấp cho hơn 40 quán cà phê, nhà hàng trong và ngoài tỉnh.</p>
            """,
            c[5], f[1], truc, ProjectStage.DangTangTruong, ProjectStatus.DaDuyet, new DateTime(2026, 7, 20), 980);
        greenStraw.Features =
        [
            Feature("100% tự nhiên", "Không hóa chất, phân hủy sinh học.", "bi-flower1", 1),
            Feature("Tạo sinh kế", "Thu mua cỏ và thuê nhân công tại địa phương.", "bi-people", 2),
            Feature("Giá cạnh tranh", "Rẻ hơn ống hút giấy, bền hơn khi dùng.", "bi-tags", 3)
        ];
        greenStraw.TeamMembers = [Member(truc, "Trưởng nhóm / Kinh doanh", 1), Guest("Trương Văn Lợi", "Sản xuất", "DA22KTA", 2)];

        var homestay = NewProject("Cù Lao Homestay", "cu-lao-homestay", "homestay-tan-quy",
            "Một ngày làm nông dân miệt vườn",
            "Mô hình du lịch cộng đồng trên cù lao: lưu trú tại nhà dân, trải nghiệm làm vườn, bắt cá, nấu món quê cùng người địa phương.",
            """
            <p><strong>Cù Lao Homestay</strong> kết nối du khách với các hộ dân trên cù lao, thiết kế tour 1–2 ngày gồm đạp xe tham quan vườn cây ăn trái, tát mương bắt cá và học làm bánh dân gian.</p>
            <p>Nhóm phụ trách quảng bá, đặt phòng trực tuyến và tập huấn kỹ năng đón khách cho người dân.</p>
            """,
            c[6], f[1], truc, ProjectStage.DaRaMat, ProjectStatus.DaDuyet, new DateTime(2026, 6, 15), 1104);
        homestay.Features =
        [
            Feature("Đặt phòng trực tuyến", "Xem phòng, giá và đặt chỗ nhanh.", "bi-calendar-check", 1),
            Feature("Tour trải nghiệm", "Lịch trình linh hoạt theo mùa trái cây.", "bi-signpost-split", 2),
            Feature("Ẩm thực miệt vườn", "Học nấu món quê cùng người dân.", "bi-egg-fried", 3)
        ];
        homestay.TeamMembers = [Member(truc, "Trưởng nhóm", 1), Guest("Phan Hữu Tài", "Hướng dẫn viên", "DA22DLA", 2)];

        var saTieu = NewProject("Sa Tiêu Studio", "sa-tieu-studio", "sa-tieu-studio",
            "Hoa văn Khmer trong đời sống hiện đại",
            "Thiết kế và sản xuất quà lưu niệm, văn phòng phẩm mang hoa văn truyền thống Khmer Nam Bộ: sổ tay, túi vải, móc khóa, thiệp.",
            """
            <p><strong>Sa Tiêu Studio</strong> số hóa các mẫu hoa văn trên chùa Khmer, kết hợp phong cách thiết kế hiện đại để tạo bộ sản phẩm quà tặng.</p>
            <p>Mỗi sản phẩm kèm thẻ giới thiệu ý nghĩa hoa văn, góp phần quảng bá văn hóa Khmer tới du khách.</p>
            """,
            c[7], f[6], phia, ProjectStage.NguyenMau, ProjectStatus.DaDuyet, new DateTime(2026, 10, 10), 287);
        saTieu.Features =
        [
            Feature("Bộ sưu tập hoa văn", "Hơn 30 mẫu hoa văn được số hóa.", "bi-brush", 1),
            Feature("Đặt in theo yêu cầu", "Nhận in logo, quà tặng sự kiện.", "bi-printer", 2)
        ];
        saTieu.TeamMembers = [Member(phia, "Trưởng nhóm / Thiết kế", 1), Guest("Sơn Thị Bô Pha", "Thiết kế đồ họa", "DA22NNKM", 2)];

        var bioShrimp = NewProject("BioShrimp TV", "bioshrimp-tv", "tom-biofloc",
            "Nuôi tôm sạch, ít thay nước",
            "Mô hình nuôi tôm thẻ chân trắng theo công nghệ Biofloc trong ao lót bạt, giảm dịch bệnh và lượng nước thải ra môi trường.",
            """
            <p><strong>BioShrimp TV</strong> ứng dụng công nghệ Biofloc giúp tận dụng chất thải hữu cơ làm thức ăn cho tôm, giảm 70% lượng nước thay mỗi vụ.</p>
            <p>Nhóm đang triển khai thí điểm tại 3 hộ nuôi ở huyện Duyên Hải và cung cấp dịch vụ tư vấn kỹ thuật.</p>
            """,
            c[1], f[2], nam, ProjectStage.DangTangTruong, ProjectStatus.DaDuyet, new DateTime(2026, 8, 28), 734);
        bioShrimp.Features =
        [
            Feature("Ít thay nước", "Giảm chi phí và hạn chế ô nhiễm.", "bi-water", 1),
            Feature("Giảm dịch bệnh", "Môi trường ao ổn định, ít dùng kháng sinh.", "bi-shield-check", 2),
            Feature("Tư vấn kỹ thuật", "Hướng dẫn quy trình cho hộ nuôi.", "bi-person-workspace", 3)
        ];
        bioShrimp.TeamMembers = [Member(nam, "Trưởng nhóm / Kỹ thuật", 1)];

        var giaSu = NewProject("GiaSư Kết Nối", "gia-su-ket-noi", "gia-su-ket-noi",
            "Gia sư sinh viên sư phạm uy tín",
            "Nền tảng kết nối phụ huynh với gia sư là sinh viên sư phạm đã được xác minh, minh bạch học phí và đánh giá sau mỗi khóa học.",
            """
            <p><strong>GiaSư Kết Nối</strong> giúp phụ huynh tìm gia sư phù hợp theo môn học, khu vực và lịch rảnh; đồng thời tạo việc làm thêm đúng chuyên ngành cho sinh viên sư phạm.</p>
            <p>Gia sư được xác minh thẻ sinh viên và đánh giá công khai sau mỗi khóa học.</p>
            """,
            c[3], f[4], ngoc, ProjectStage.DaRaMat, ProjectStatus.DaDuyet, new DateTime(2026, 9, 28), 498);
        giaSu.Features =
        [
            Feature("Tìm gia sư theo môn", "Lọc theo môn, lớp, khu vực, học phí.", "bi-search", 1),
            Feature("Xác minh gia sư", "Kiểm tra thẻ sinh viên, kết quả học tập.", "bi-patch-check", 2),
            Feature("Đánh giá minh bạch", "Phụ huynh chấm điểm sau mỗi khóa.", "bi-star", 3)
        ];
        giaSu.Versions = [new() { VersionName = "GiaSư Kết Nối 1.2", Platform = "Web", ReleaseDate = new DateOnly(2026, 10, 5), IsPublic = true }];
        giaSu.TeamMembers = [Member(ngoc, "Trưởng nhóm", 1), Guest("Đặng Thị Thu Thảo", "Chăm sóc khách hàng", "DA22SPV", 2)];

        // --- Các dự án ở trạng thái khác (minh họa quy trình duyệt) ---
        var xaPhong = NewProject("Xà phòng thảo dược Mộc An", "xa-phong-thao-duoc-moc-an", "xa-phong-thao-duoc",
            "Sạch dịu nhẹ từ thảo mộc quê nhà",
            "Xà phòng thủ công từ dầu dừa Trà Vinh kết hợp nghệ, sả, lá trầu không; không chất tạo bọt tổng hợp, an toàn cho da nhạy cảm.",
            "<p>Sản phẩm được nghiên cứu tại phòng thí nghiệm Khoa Hóa học Ứng dụng, sử dụng phương pháp nấu lạnh để giữ dưỡng chất tự nhiên.</p>",
            c[4], f[5], han, ProjectStage.NguyenMau, ProjectStatus.ChoDuyet, new DateTime(2026, 10, 25), 0);
        xaPhong.Features = [Feature("Nguyên liệu tự nhiên", "Dầu dừa, nghệ, sả, lá trầu không.", "bi-flower2", 1)];
        xaPhong.TeamMembers = [Member(han, "Trưởng nhóm", 1)];

        var banDo = NewProject("Bản đồ ẩm thực Trà Vinh", "ban-do-am-thuc-tra-vinh", "ban-do-am-thuc",
            "Ăn gì ở Trà Vinh?",
            "Bản đồ trực tuyến giới thiệu các quán ăn đặc sản Trà Vinh.",
            "<p>Ý tưởng tổng hợp quán ăn đặc sản như bún nước lèo, bánh tét Trà Cuôn, chù ụ rang me.</p>",
            c[6], f[1], truc, ProjectStage.YTuong, ProjectStatus.TuChoi, new DateTime(2026, 10, 18), 0);
        banDo.RejectReason = "Nội dung giới thiệu còn sơ sài, chưa nêu rõ mô hình hoạt động và hình ảnh thực tế. Vui lòng bổ sung rồi gửi duyệt lại.";

        var smartParking = NewProject("TVU Smart Parking", "tvu-smart-parking", "smart-parking",
            "Tìm chỗ gửi xe trong 10 giây",
            "Hệ thống camera nhận diện biển số và hiển thị số chỗ trống tại các nhà xe trong khuôn viên Trường.",
            "<p>Đang hoàn thiện nội dung.</p>",
            c[0], f[0], khoa, ProjectStage.YTuong, ProjectStatus.Nhap, new DateTime(2026, 10, 26), 0);

        var approved = new[] { zalord, agrisense, traMangCau, eduKhmer, mediCare, greenStraw, homestay, saTieu, bioShrimp, giaSu };
        db.Projects.AddRange(approved);
        db.Projects.AddRange(xaPhong, banDo, smartParking);
        await db.SaveChangesAsync();

        // ===== Bình chọn (yêu thích) =====
        // Mỗi sinh viên thích một số dự án không phải của mình
        var rnd = new Random(2026);
        foreach (var s in students)
        {
            foreach (var p in approved.Where(p => p.OwnerId != s.AccountId))
            {
                if (rnd.NextDouble() < 0.55)
                    db.Votes.Add(new Vote { ProjectId = p.ProjectId, AccountId = s.AccountId, CreatedAt = p.ApprovedAt!.Value.AddDays(rnd.Next(1, 5)) });
            }
        }

        // ===== Bình luận & đánh giá =====
        Comment NewComment(Project p, Account a, string content, byte? rating, int daysAfter) => new()
        {
            ProjectId = p.ProjectId, AccountId = a.AccountId, Content = content, Rating = rating,
            CreatedAt = p.ApprovedAt!.Value.AddDays(daysAfter)
        };

        var cm1 = NewComment(zalord, khoa, "Giao diện tùy biến rất đẹp, mình dùng font riêng cho Zalo PC thấy dễ đọc hơn hẳn.", 5, 1);
        cm1.Replies.Add(new Comment { ProjectId = zalord.ProjectId, AccountId = phuong.AccountId, Content = "Cảm ơn bạn đã dùng thử! Bản sau mình sẽ thêm nhiều theme hơn.", CreatedAt = cm1.CreatedAt.AddHours(3) });
        var cm2 = NewComment(zalord, ngoc, "Chức năng ẩn nội dung khi quay màn hình rất hữu ích khi mình dạy online.", 5, 2);
        var cm3 = NewComment(zalord, bao, "Mong có thêm hướng dẫn cài đặt bằng video cho người mới.", 4, 3);
        var cm4 = NewComment(agrisense, nam, "Ý tưởng rất thiết thực cho vùng ven biển. Nhóm có dự định đo thêm độ pH không?", 5, 1);
        cm4.Replies.Add(new Comment { ProjectId = agrisense.ProjectId, AccountId = khoa.AccountId, Content = "Có bạn nhé, phiên bản trạm đo tiếp theo sẽ có cảm biến pH.", CreatedAt = cm4.CreatedAt.AddHours(5) });
        var cm5 = NewComment(agrisense, truc, "Chi phí mỗi trạm như vậy là hợp lý với hợp tác xã.", 4, 4);
        var cm6 = NewComment(traMangCau, phia, "Trà thơm, vị chua nhẹ dễ uống. Đã mua làm quà cho bạn bè.", 5, 2);
        var cm7 = NewComment(traMangCau, phuong, "Bao bì đẹp, nên bán thêm trên sàn thương mại điện tử.", 4, 6);
        var cm8 = NewComment(greenStraw, han, "Ống hút cứng cáp, không bị mềm khi uống lâu.", 5, 3);
        var cm9 = NewComment(homestay, ngoc, "Lớp mình đã đi trải nghiệm, các cô chú rất nhiệt tình!", 5, 5);
        var cm10 = NewComment(eduKhmer, bao, "Phát âm chuẩn, bài học ngắn gọn. Mong có bản Android.", 4, 2);
        var cm11 = NewComment(giaSu, truc, "Giao diện tìm gia sư dễ dùng.", null, 1);
        db.Comments.AddRange(cm1, cm2, cm3, cm4, cm5, cm6, cm7, cm8, cm9, cm10, cm11);

        // ===== Yêu cầu liên hệ =====
        db.ContactRequests.AddRange(
            new ContactRequest
            {
                ProjectId = agrisense.ProjectId, FullName = "Nguyễn Văn Hòa", Email = "hoa.nguyen@example.com", Phone = "0909000111",
                Organization = "HTX Nông nghiệp Long Hòa (mẫu)", RequestType = RequestType.SuDungSanPham,
                Message = "Hợp tác xã muốn lắp thử 2 trạm đo trong mùa khô năm nay, nhờ nhóm báo giá và thời gian lắp đặt.",
                Status = ContactStatus.Moi, CreatedAt = new DateTime(2026, 10, 24, 9, 30, 0)
            },
            new ContactRequest
            {
                ProjectId = traMangCau.ProjectId, FullName = "Trần Thị Mai", Email = "mai.tran@example.com", Phone = "0912345678",
                Organization = "Cửa hàng Đặc sản Miền Tây (mẫu)", RequestType = RequestType.HopTac,
                Message = "Cửa hàng muốn nhập 200 hộp trà mỗi tháng để phân phối tại TP.HCM.",
                Status = ContactStatus.DaLienHe, HandledBy = han.AccountId, CreatedAt = new DateTime(2026, 9, 2, 14, 0, 0)
            },
            new ContactRequest
            {
                ProjectId = zalord.ProjectId, FullName = "Lê Quang Vinh", Email = "vinh.le@example.com",
                RequestType = RequestType.SuDungSanPham,
                Message = "Mình muốn dùng Zalord cho máy tính ở văn phòng, có hướng dẫn cài đặt chi tiết không?",
                Status = ContactStatus.Moi, CreatedAt = new DateTime(2026, 10, 26, 20, 15, 0)
            },
            new ContactRequest
            {
                FullName = "Phạm Đức Toàn", Email = "toan.pham@example.com", Organization = "Quỹ Đầu tư Khởi nghiệp ABC (mẫu)",
                RequestType = RequestType.DauTu,
                Message = "Quỹ quan tâm các dự án nông nghiệp công nghệ cao của sinh viên, mong được kết nối với Ban quản trị.",
                Status = ContactStatus.Moi, CreatedAt = new DateTime(2026, 10, 27, 8, 0, 0)
            });

        // ===== Tin tức / sự kiện =====
        db.News.AddRange(
            new News
            {
                Title = "Phát động Cuộc thi Ý tưởng khởi nghiệp Sinh viên TVU 2026", Slug = "phat-dong-cuoc-thi-y-tuong-khoi-nghiep-2026",
                Thumbnail = "/uploads/sample/news/cuoc-thi-y-tuong.svg", NewsType = NewsType.CuocThi,
                Summary = "Cuộc thi dành cho sinh viên tất cả các khoa, nhận hồ sơ đến hết tháng 12/2026.",
                Content = "<p>Cuộc thi nhằm tìm kiếm, ươm tạo các ý tưởng khởi nghiệp sáng tạo của sinh viên. Các đội thi gồm 1–5 sinh viên, nộp bản mô tả ý tưởng và video giới thiệu.</p><p>Các dự án đạt giải được hỗ trợ đăng giới thiệu nổi bật trên website TVU Startup.</p><p><em>(Bài viết dữ liệu mẫu)</em></p>",
                EventDate = new DateTime(2026, 12, 20), Location = "Trường Đại học Trà Vinh",
                AuthorId = admin.AccountId, IsPublished = true, ViewCount = 420, CreatedAt = new DateTime(2026, 10, 15)
            },
            new News
            {
                Title = "Ngày hội Khởi nghiệp Sinh viên 2026: hơn 30 gian hàng trưng bày sản phẩm", Slug = "ngay-hoi-khoi-nghiep-sinh-vien-2026",
                Thumbnail = "/uploads/sample/news/ngay-hoi.svg", NewsType = NewsType.SuKien,
                Summary = "Ngày hội là dịp sinh viên giới thiệu sản phẩm, gặp gỡ doanh nghiệp và nhà đầu tư.",
                Content = "<p>Ngày hội có khu trưng bày sản phẩm, tọa đàm chia sẻ kinh nghiệm khởi nghiệp và phiên kết nối với doanh nghiệp.</p><p><em>(Bài viết dữ liệu mẫu)</em></p>",
                EventDate = new DateTime(2026, 11, 22, 7, 30, 0), Location = "Hội trường lớn – Trường Đại học Trà Vinh",
                AuthorId = admin.AccountId, IsPublished = true, ViewCount = 310, CreatedAt = new DateTime(2026, 10, 10)
            },
            new News
            {
                Title = "Tập huấn kỹ năng gọi vốn cho dự án khởi nghiệp", Slug = "tap-huan-ky-nang-goi-von",
                Thumbnail = "/uploads/sample/news/tap-huan.svg", NewsType = NewsType.SuKien,
                Summary = "Hướng dẫn xây dựng bài thuyết trình gọi vốn (pitch deck) và trả lời câu hỏi của nhà đầu tư.",
                Content = "<p>Buổi tập huấn hướng dẫn cách trình bày vấn đề, giải pháp, thị trường, mô hình kinh doanh và kế hoạch tài chính trong 5 phút.</p><p><em>(Bài viết dữ liệu mẫu)</em></p>",
                EventDate = new DateTime(2026, 11, 8, 14, 0, 0), Location = "Phòng hội thảo D5",
                AuthorId = admin.AccountId, IsPublished = true, ViewCount = 185, CreatedAt = new DateTime(2026, 10, 5)
            },
            new News
            {
                Title = "Kết nối doanh nghiệp với dự án sinh viên", Slug = "ket-noi-doanh-nghiep-voi-du-an-sinh-vien",
                Thumbnail = "/uploads/sample/news/ket-noi-doanh-nghiep.svg", NewsType = NewsType.TinTuc,
                Summary = "Doanh nghiệp có thể gửi yêu cầu hợp tác trực tiếp tới từng dự án trên website.",
                Content = "<p>Mỗi trang dự án có nút <strong>Liên hệ hợp tác</strong>; yêu cầu được chuyển tới nhóm sinh viên và Ban quản trị để theo dõi, phản hồi.</p><p><em>(Bài viết dữ liệu mẫu)</em></p>",
                AuthorId = admin.AccountId, IsPublished = false, CreatedAt = new DateTime(2026, 10, 27)
            });

        // ===== Banner trang chủ =====
        db.Banners.AddRange(
            new Banner { Title = "Ngày hội Khởi nghiệp Sinh viên TVU 2026", ImageUrl = "/uploads/sample/banners/ngay-hoi-khoi-nghiep.svg", LinkUrl = "/tin-tuc/ngay-hoi-khoi-nghiep-sinh-vien-2026", DisplayOrder = 1 },
            new Banner { Title = "Đăng dự án của bạn lên TVU Startup", ImageUrl = "/uploads/sample/banners/dang-du-an.svg", LinkUrl = "/dang-ky", DisplayOrder = 2 });

        await db.SaveChangesAsync();
    }
}
