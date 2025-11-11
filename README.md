# DACS
DOANCOSO_HTTT
# Hệ thống Quản lý Vi phạm giao thông (NHOM10_DACS_QLVIPHAM)

Hệ thống hỗ trợ quản lý biên bản, xử phạt và theo dõi các yêu cầu liên quan đến vi phạm hành chính của công dân. Ứng dụng được xây dựng trên nền tảng ASP.NET Core MVC (NET 9.0) với cơ sở dữ liệu SQL Server, tích hợp thanh toán PayPal và trợ lý AI (Gemini) để tự động hóa nhiều quy trình nghiệp vụ cho cán bộ quản lý lẫn công dân.

## Kiến trúc & công nghệ

- **Framework**: ASP.NET Core MVC 9.0, Entity Framework Core, ASP.NET Identity.
- **Cơ sở dữ liệu**: SQL Server (migrations đã được kèm sẵn trong thư mục `Migrations`).
- **Giao diện**: Tailwind CSS (sử dụng CDN) & Alpine.js (xem thêm `NHOM10_DACS_QLVIPHAM/README-TAILWIND.md`).
- **Thanh toán**: PayPal SDK (`PayPal`, `PayPalCoreSDK`, `PayPalMerchantSDK`).
- **Thông báo**: dịch vụ `IThongBaoService` gửi thông báo tới công dân và cán bộ khi có thanh toán, khiếu nại, yêu cầu cập nhật…
- **Trợ lý ảo**: `GeminiAIService` gọi API Google Gemini để cung cấp chức năng chatbot tư vấn.

## Chức năng chính

### Tổng quan dashboard
- Tổng hợp số lượng vi phạm, loại vi phạm, công dân, biên bản mới nhất.
- Biểu đồ thống kê doanh thu, lịch sử thanh toán theo thời gian (Model `THONGKEDOANHTHU`).
- Nhật ký thao tác (`NhatKyThong`) ghi nhận mọi hành động quản trị.

### Quản trị viên (`Admin`)
- Quản lý tài khoản hệ thống (`AdminUserController`, `AdminController`).
- Phân quyền theo vai trò: `Admin`, `CanBo`, `CongDan`, `User`.
- Quản lý phòng ban, cán bộ, cấu hình hệ thống và giám sát các yêu cầu xử lý.

### Cán bộ (`CanBo`)
- Quản lý danh mục **loại vi phạm** và **mức phạt** (`CanBoController.QuanLyLoaiViPham`).
- Lập, chỉnh sửa và theo dõi **biên bản vi phạm** (`BienBanVPham`, `BangChungViPham`).
- Quản lý **bằng chứng** kèm biên bản, xử lý yêu cầu bổ sung bằng chứng (`BangChungController`).
- Duyệt **khiếu nại vi phạm**, **yêu cầu cấp lại CCCD**, **yêu cầu cập nhật thông tin**.
- Theo dõi thanh toán, xác nhận hóa đơn, cập nhật trạng thái xử lý biên bản.

### Công dân (`CongDan`/`User`)
- Đăng ký/đăng nhập, cập nhật hồ sơ cá nhân (`AccountController`, `UserController`).
- Tra cứu vi phạm theo mã CCCD, xem chi tiết biên bản, tải bằng chứng (`ViPhamController`).
- Thực hiện thanh toán tiền phạt trực tuyến thông qua PayPal, nhận hóa đơn điện tử và lịch sử thanh toán (`ThanhToanController`, `HOADON`, `ChiTietHoaDon`).
- Gửi khiếu nại, yêu cầu cung cấp bằng chứng, yêu cầu cấp lại CCCD, yêu cầu cập nhật thông tin cư trú (`KhieuNaiVPham`, `YeuCauBangChungViPham`, `YeuCauCapLaiThe`, `YeuCauCapNhatThongTin`).
- Nhận thông báo trạng thái xử lý, lịch hẹn và thông tin mới nhất từ cơ quan quản lý (`ThongBaoController`).
- Sử dụng chatbot Gemini để được hướng dẫn nhanh các quy trình.

### Dịch vụ hỗ trợ khác
- **Quản lý căn cước**: theo dõi thẻ CCCD, lịch sử cấp lại (`TheCanCuocController`, `XetDuyetCapLaiThe`).
- **Quản lý cư trú**: lưu thông tin đăng ký cư trú, phường/xã/quận huyện (`DangKyCuTru`, `PhuongXa`, `QuanHuyen`).
- **Tự động hóa schema**: `Program.cs` kiểm tra và tạo các bảng/column quan trọng khi khởi động lần đầu (ví dụ `SoTienPhat`, `YeuCauCapNhatThongTins`).

## Cấu trúc thư mục

```
NHOM10_DACS_QLVIPHAM/
├── Controllers/         // Các controller MVC cho từng phân hệ
├── Models/              // Entity, DbContext, cấu hình PayPal & Gemini
├── ViewModels/          // ViewModel phục vụ UI và form nhập liệu
├── Views/               // Razor Views (đã chuyển sang Tailwind CSS)
├── Services/            // Dịch vụ thông báo & chatbot
├── Migrations/          // Bộ migrations Entity Framework Core
├── wwwroot/             // Static files (CSS, JS, hình ảnh)
├── README-TAILWIND.md   // Ghi chú sử dụng Tailwind qua CDN
└── PayPal-Readme.md     // Hướng dẫn thiết lập PayPal Sandbox/Live
```

## Chuẩn bị môi trường

1. Cài đặt **.NET SDK 9.0** và **SQL Server** (Express hoặc Developer).
2. Tạo cơ sở dữ liệu SQL Server trống, cập nhật chuỗi kết nối trong `appsettings.json` hoặc `appsettings.Development.json` (khuyến nghị dùng [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) cho môi trường phát triển).
3. Cập nhật khóa PayPal (`PayPal:ClientId`, `PayPal:ClientSecret`) và Gemini (`GeminiAI:ApiKey`, `GeminiAI:ApiUrl`). Không commit khóa thật vào Git.
4. Tùy chọn: điều chỉnh `builder.WebHost.UseUrls` trong `Program.cs` nếu muốn chạy ở port khác.

## Khởi tạo cơ sở dữ liệu

```bash
cd NHOM10_DACS_QLVIPHAM
# Khôi phục package và build dự án
 dotnet restore
 dotnet build

# Áp dụng migrations (Program.cs cũng tự động migrate khi chạy lần đầu)
 dotnet ef database update
```

Khi ứng dụng chạy lần đầu, hệ thống sẽ:
- Tự tạo các vai trò (`Admin`, `CanBo`, `CongDan`, `User`).
- Sinh tài khoản quản trị mặc định `admin@quanlyvipham.com` / mật khẩu `Admin@123`.
- Kiểm tra và tạo các bảng, cột bổ sung nếu còn thiếu.

> ⚠️ Hãy đổi mật khẩu tài khoản mặc định ngay khi triển khai thực tế.

## Chạy ứng dụng

```bash
cd NHOM10_DACS_QLVIPHAM
 dotnet run
```

Ứng dụng sẽ phục vụ tại `http://localhost:7895` (HTTP) và `https://localhost:8905` (HTTPS). Đăng nhập bằng tài khoản admin hoặc tự đăng ký tài khoản công dân để trải nghiệm hệ thống.

## Kiểm thử chức năng chính

1. **Tra cứu vi phạm**: truy cập `/ViPham/TraCuuViPham`, nhập mã CCCD mẫu (`079123456789` hoặc `079198765432`) để xem dữ liệu seed.
2. **Thanh toán**: tạo biên bản mẫu, thực hiện thanh toán PayPal sandbox, kiểm tra hóa đơn và lịch sử.
3. **Thông báo & khiếu nại**: gửi khiếu nại, theo dõi thông báo phản hồi trong trang cá nhân.
4. **Chatbot Gemini**: truy cập `/Chatbot` để đặt câu hỏi và nhận phản hồi từ AI.

## Ghi chú triển khai

- Luôn sử dụng môi trường `sandbox` của PayPal để thử nghiệm trước khi chuyển sang `live` (tham khảo `PayPal-Readme.md`).
- Xem thêm `README-TAILWIND.md` nếu cần chỉnh sửa giao diện.
- Nếu tùy biến schema trực tiếp trên SQL Server, hãy đồng bộ lại migrations để tránh xung đột.

## Đóng góp & phát triển

1. Tạo branch mới từ `main`.
2. Thực hiện chỉnh sửa và bổ sung unit test (nếu có).
3. Chạy `dotnet build` và kiểm thử thủ công trước khi gửi pull request.
4. Mô tả rõ thay đổi, ảnh chụp màn hình đối với các cập nhật UI.

---

Mọi thắc mắc hoặc đề xuất cải tiến vui lòng mở issue hoặc liên hệ nhóm phát triển.
