#! NHẬN XÉT CỦA GIÁO VIÊN HƯỚNG DẪN

%%DOTS 20

^^ Trà Vinh, ngày ….. tháng ….. năm 2026
^^ **Giáo viên hướng dẫn**
^^ *(Ký tên và ghi rõ họ tên)*

<<<PAGEBREAK>>>

#! LỜI CAM ĐOAN

Tôi xin cam đoan đồ án “Xây dựng website giới thiệu các sản phẩm khởi nghiệp của Sinh viên Trường Đại học Trà Vinh” là kết quả thực hiện của cá nhân tôi dưới sự hướng dẫn của TS. Nguyễn Nhứt Lam. Mã nguồn, cơ sở dữ liệu, các số liệu và kết quả kiểm thử trình bày trong báo cáo là trung thực, được tạo ra trong quá trình thực hiện đồ án. Các tài liệu, hình ảnh, đoạn mã tham khảo từ nguồn khác đều được trích dẫn đầy đủ trong phần Tài liệu tham khảo.

Trong quá trình thực hiện, tôi có sử dụng công cụ trí tuệ nhân tạo Claude (Anthropic) để hỗ trợ một số công việc như gợi ý cấu trúc mã nguồn, giao diện, dữ liệu mẫu, kịch bản kiểm thử, vẽ sơ đồ và soạn thảo bản nháp một số nội dung báo cáo; chi tiết được khai báo tại Phụ lục 4. Tôi đã đọc hiểu, kiểm tra, chỉnh sửa và chịu trách nhiệm đối với toàn bộ mã nguồn và nội dung báo cáo.

Nếu có bất kỳ sai phạm nào, tôi xin hoàn toàn chịu trách nhiệm trước Nhà trường.

^^ Trà Vinh, ngày ….. tháng ….. năm 2026
^^ **Sinh viên thực hiện**
^^ *(Ký và ghi rõ họ tên)*
^^ &nbsp;
^^ &nbsp;
^^ **Lê Văn Bình Phương**

<<<PAGEBREAK>>>

#! LỜI CẢM ƠN

Trước tiên, tôi xin gửi lời cảm ơn chân thành đến TS. Nguyễn Nhứt Lam – giảng viên hướng dẫn học phần Chuyên đề ASP.NET – đã tận tình định hướng đề tài, giải đáp thắc mắc và cung cấp tài liệu hướng dẫn chi tiết giúp tôi hoàn thành đồ án đúng yêu cầu.

Tôi xin cảm ơn quý thầy cô Trường Đại học Trà Vinh đã truyền đạt cho tôi những kiến thức nền tảng về lập trình, cơ sở dữ liệu và phân tích thiết kế hệ thống trong suốt quá trình học tập – đây là cơ sở quan trọng để tôi thực hiện đồ án này.

Tôi cũng xin cảm ơn cộng đồng người dùng đã sử dụng và góp ý cho sản phẩm Zalord; những phản hồi đó là nguồn động lực để tôi lựa chọn và thực hiện đề tài về giới thiệu sản phẩm khởi nghiệp của sinh viên. Cuối cùng, xin cảm ơn gia đình và bạn bè đã luôn động viên, hỗ trợ tôi trong quá trình học tập.

Mặc dù đã cố gắng hoàn thiện, đồ án khó tránh khỏi những thiếu sót. Tôi rất mong nhận được ý kiến đóng góp của quý thầy cô để đồ án được hoàn thiện hơn.

Xin chân thành cảm ơn!

^^ Trà Vinh, ngày ….. tháng ….. năm 2026
^^ **Sinh viên thực hiện**
^^ &nbsp;
^^ **Lê Văn Bình Phương**

<<<PAGEBREAK>>>

#! MỤC LỤC

[[TOC]]

<<<PAGEBREAK>>>

#! DANH MỤC BẢNG

[[LOT]]

<<<PAGEBREAK>>>

#! DANH MỤC HÌNH

[[LOF]]

<<<PAGEBREAK>>>

#! DANH MỤC TỪ VIẾT TẮT

| Từ viết tắt | Tiếng Anh | Ý nghĩa |
|---|---|---|
| AJAX | Asynchronous JavaScript and XML | Kỹ thuật gửi yêu cầu, cập nhật một phần trang không tải lại |
| API | Application Programming Interface | Giao diện lập trình ứng dụng |
| CSDL | – | Cơ sở dữ liệu |
| CRUD | Create, Read, Update, Delete | Các thao tác thêm, xem, sửa, xóa dữ liệu |
| CSRF | Cross-Site Request Forgery | Tấn công giả mạo yêu cầu |
| DI | Dependency Injection | Tiêm phụ thuộc |
| EF Core | Entity Framework Core | Thư viện ORM của .NET |
| ERD | Entity Relationship Diagram | Sơ đồ thực thể – liên kết |
| FK | Foreign Key | Khóa ngoại |
| GVHD | – | Giáo viên hướng dẫn |
| HTML | HyperText Markup Language | Ngôn ngữ đánh dấu siêu văn bản |
| HTTP(S) | HyperText Transfer Protocol (Secure) | Giao thức truyền siêu văn bản (bảo mật) |
| IIS | Internet Information Services | Máy chủ web của Microsoft |
| JWT | JSON Web Token | Mã thông báo xác thực dạng JSON |
| LINQ | Language Integrated Query | Truy vấn tích hợp trong ngôn ngữ |
| LTS | Long Term Support | Phiên bản hỗ trợ dài hạn |
| MVC | Model – View – Controller | Mô hình Model – View – Controller |
| ORM | Object – Relational Mapping | Ánh xạ đối tượng – quan hệ |
| PBKDF2 | Password-Based Key Derivation Function 2 | Thuật toán băm mật khẩu |
| PK | Primary Key | Khóa chính |
| SPA | Single Page Application | Ứng dụng web một trang |
| SQL | Structured Query Language | Ngôn ngữ truy vấn có cấu trúc |
| SV | – | Sinh viên |
| TVU | Tra Vinh University | Trường Đại học Trà Vinh |
| UC | Use Case | Ca sử dụng |
| UML | Unified Modeling Language | Ngôn ngữ mô hình hóa thống nhất |
| URL | Uniform Resource Locator | Địa chỉ trang web |
| XSS | Cross-Site Scripting | Tấn công chèn mã độc vào trang web |
