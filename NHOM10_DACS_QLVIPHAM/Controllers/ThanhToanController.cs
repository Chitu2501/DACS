using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NHOM10_DACS_QLVIPHAM.Models;
using NHOM10_DACS_QLVIPHAM.Services;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace NHOM10_DACS_QLVIPHAM.Controllers
{
    public class ThanhToanController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PayPalConfig _payPalConfig;
        private readonly PayPalHelper _payPalHelper;
        private readonly IThongBaoService _thongBaoService;

        public ThanhToanController(ApplicationDbContext context, IOptions<PayPalConfig> payPalConfig, IThongBaoService thongBaoService)
        {
            _context = context;
            _payPalConfig = payPalConfig.Value;
            _payPalHelper = new PayPalHelper(_payPalConfig);
            _thongBaoService = thongBaoService;
        }

        // Action tạo bảng LichSuThanhToans trực tiếp
        public IActionResult TaoBangLichSuThanhToan()
        {
            try
            {
                string sql = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LichSuThanhToans]') AND type in (N'U'))
                    BEGIN
                        -- Create the table
                        CREATE TABLE [dbo].[LichSuThanhToans](
                            [MaLichSu] [int] IDENTITY(1,1) NOT NULL,
                            [MaBienBan] [int] NOT NULL,
                            [SoTien] [decimal](18, 0) NOT NULL,
                            [PhuongThucThanhToan] [nvarchar](max) NULL,
                            [NgayThanhToan] [datetime2](7) NOT NULL,
                            [MaGiaoDich] [nvarchar](max) NULL,
                            CONSTRAINT [PK_LichSuThanhToans] PRIMARY KEY CLUSTERED ([MaLichSu] ASC)
                        );

                        -- Add foreign key if the BienBanVPhams table exists
                        IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BienBanVPhams]') AND type in (N'U'))
                        BEGIN
                            ALTER TABLE [dbo].[LichSuThanhToans] 
                            ADD CONSTRAINT [FK_LichSuThanhToans_BienBanVPhams_MaBienBan] 
                            FOREIGN KEY([MaBienBan]) 
                            REFERENCES [dbo].[BienBanVPhams] ([MaBienBan]);
                        END
                    END";

                _context.Database.ExecuteSqlRaw(sql);
                return Content("Bảng LichSuThanhToans đã được tạo hoặc đã tồn tại.");
            }
            catch (Exception ex)
            {
                return Content($"Lỗi khi tạo bảng: {ex.Message}");
            }
        }

        // Action để chỉ xác nhận thanh toán PayPal mà không thực hiện lưu vào database
        public IActionResult XacNhanThanhToanPayPal(string paymentId, string token, string PayerID)
        {
            if (string.IsNullOrEmpty(paymentId) || string.IsNullOrEmpty(PayerID))
            {
                return Content("Thông tin thanh toán không hợp lệ");
            }
            
            try
            {
                // Kiểm tra cấu hình PayPal
                if (string.IsNullOrEmpty(_payPalConfig.ClientId) || 
                    _payPalConfig.ClientId == "YOUR_PAYPAL_CLIENT_ID" ||
                    string.IsNullOrEmpty(_payPalConfig.ClientSecret) ||
                    _payPalConfig.ClientSecret == "YOUR_PAYPAL_CLIENT_SECRET")
                {
                    return Content("Cấu hình PayPal chưa được thiết lập đúng");
                }
                
                // Sử dụng helper để thực hiện thanh toán
                var executedPayment = _payPalHelper.ExecutePayment(paymentId, PayerID);
                
                return Content($"Thanh toán PayPal thành công! Trạng thái: {executedPayment.state}, ID: {paymentId}");
            }
            catch (Exception ex)
            {
                string errorDetails = $"Lỗi: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorDetails += $"\nInner Exception: {ex.InnerException.Message}";
                }
                
                return Content(errorDetails);
            }
        }

        // Action để thử tạo thanh toán và lưu vào cơ sở dữ liệu
        public IActionResult ThuThanhToan(int maBienBan)
        {
            var bienBan = _context.BienBanVPhams
                .Include(b => b.CongDan)
                .FirstOrDefault(b => b.MaBienBan == maBienBan);
                
            if (bienBan == null)
                return Content("Không tìm thấy biên bản");
                
            try
            {
                // Kiểm tra xem đã có lịch sử thanh toán chưa
                var existingPayment = _context.LichSuThanhToans
                    .FirstOrDefault(l => l.MaBienBan == maBienBan);
                
                if (existingPayment != null)
                {
                    return Content($"Biên bản này đã được thanh toán trước đó vào ngày {existingPayment.NgayThanhToan}");
                }
                
                // Tạo bản ghi mới
                try
                {
                    var lichSuThanhToan = new LichSuThanhToan
                    {
                        MaBienBan = bienBan.MaBienBan,
                        SoTien = bienBan.SoTienPhat,
                        PhuongThucThanhToan = "Test",
                        NgayThanhToan = DateTime.Now,
                        MaGiaoDich = "TEST-" + Guid.NewGuid().ToString()
                    };
                    
                    _context.LichSuThanhToans.Add(lichSuThanhToan);
                    _context.SaveChanges();
                    
                    // Cập nhật trạng thái biên bản
                    bienBan.KetQuaXuLy = "Đã thanh toán";
                    bienBan.NgayLadDuBienBan = DateTime.Now;
                    _context.SaveChanges();

                    // Tạo thông báo
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (!string.IsNullOrEmpty(userId))
                    {
                        // Thông báo cho người dùng
                        _thongBaoService.TaoThongBaoThanhToanAsync(bienBan.MaBienBan, userId, false).Wait();
                        
                        // Thông báo cho cán bộ
                        _thongBaoService.TaoThongBaoThanhToanAsync(bienBan.MaBienBan, userId, true).Wait();
                    }
                    
                    return Content("Tạo thanh toán thử nghiệm thành công!");
                }
                catch (DbUpdateException dbEx)
                {
                    return Content($"Lỗi khi lưu thanh toán: {dbEx.Message}");
                }
            }
            catch (Exception ex)
            {
                return Content($"Lỗi: {ex.Message}");
            }
        }

        // Action để kiểm tra lỗi database
        public IActionResult KiemTraLoi()
        {
            try
            {
                // Kiểm tra cấu trúc của bảng LichSuThanhToan
                var columns = _context.Model.FindEntityType(typeof(LichSuThanhToan)).GetProperties().Select(p => p.Name).ToList();
                
                // Kiểm tra bảng dữ liệu
                bool tableExists = false;
                try
                {
                    var result = _context.Database.ExecuteSqlRaw("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'LichSuThanhToans'");
                    tableExists = result > 0;
                }
                catch
                {
                    tableExists = false;
                }
                
                // Kiểm tra xem bảng BienBanVPham có tồn tại không
                var bienBanCount = _context.BienBanVPhams.Count();
                
                // Kiểm tra các mối quan hệ
                var relationships = _context.Model.FindEntityType(typeof(LichSuThanhToan)).GetForeignKeys().Select(fk => fk.PrincipalEntityType.Name).ToList();
                
                return Content($"Cấu trúc bảng LichSuThanhToan: {string.Join(", ", columns)}\n" +
                              $"Bảng LichSuThanhToans tồn tại: {tableExists}\n" +
                              $"Số lượng BienBanVPham: {bienBanCount}\n" +
                              $"Mối quan hệ của LichSuThanhToan: {string.Join(", ", relationships)}");
            }
            catch (Exception ex)
            {
                return Content($"Lỗi kiểm tra database: {ex.Message}");
            }
        }

        // Action để kiểm tra kết nối với PayPal
        public IActionResult KiemTraKetNoi()
        {
            try
            {
                if (string.IsNullOrEmpty(_payPalConfig.ClientId) || 
                    _payPalConfig.ClientId == "YOUR_PAYPAL_CLIENT_ID" ||
                    string.IsNullOrEmpty(_payPalConfig.ClientSecret) ||
                    _payPalConfig.ClientSecret == "YOUR_PAYPAL_CLIENT_SECRET")
                {
                    return Content("Lỗi: Cấu hình PayPal chưa được thiết lập đúng. Vui lòng kiểm tra appsettings.json");
                }

                // Sử dụng helper để lấy API context
                var apiContext = _payPalHelper.GetAPIContext();
                
                return Content($"Kết nối PayPal thành công! Mode: {_payPalConfig.Mode}, AccessToken: {apiContext.AccessToken.Substring(0, 20)}...");
            }
            catch (Exception ex)
            {
                return Content($"Lỗi kết nối PayPal: {ex.Message}");
            }
        }

        public IActionResult Index(int maBienBan)
        {
            var bienBan = _context.BienBanVPhams
                .Include(b => b.ViPham)
                .Include(b => b.CongDan)
                .FirstOrDefault(b => b.MaBienBan == maBienBan);

            if (bienBan == null)
            {
                return NotFound();
            }

            // Kiểm tra xem đã thanh toán chưa
            if (bienBan.KetQuaXuLy == "Đã thanh toán")
            {
                return RedirectToAction("ThongBao", new { message = "Biên bản này đã được thanh toán." });
            }

            ViewBag.BienBan = bienBan;
            return View();
        }

        public IActionResult ThongBao(string message)
        {
            ViewBag.Message = message;
            return View();
        }

        public IActionResult HuyThanhToan()
        {
            return RedirectToAction("ThongBao", new { message = "Bạn đã hủy thanh toán." });
        }

        [HttpGet]
        public IActionResult ThanhCong(string paymentId, string token, string PayerID, int maBienBan)
        {
            return RedirectToAction("XuLyThanhCong", new { paymentId, token, PayerID, maBienBan });
        }

        public IActionResult XuLyThanhCong(string paymentId, string token, string PayerID, int maBienBan)
        {
            var bienBan = _context.BienBanVPhams
                .Include(b => b.CongDan)
                .FirstOrDefault(b => b.MaBienBan == maBienBan);
                
            if (bienBan == null)
                return NotFound();

            try
            {
                var payment = _payPalHelper.ExecutePayment(paymentId, PayerID);
                
                if (payment.state.ToLower() == "approved")
                {
                    try
                    {
                        // Kiểm tra xem đã có lịch sử thanh toán chưa
                        var existingPayment = _context.LichSuThanhToans
                            .FirstOrDefault(l => l.MaGiaoDich == paymentId);
                        
                        if (existingPayment != null)
                        {
                            return RedirectToAction("ThongBao", new { message = "Thanh toán này đã được xử lý trước đó." });
                        }
                        
                        // Ghi lại thanh toán
                        try
                        {
                            var ngayThanhToan = DateTime.Now;
                            var soTien = bienBan.SoTienPhat;
                            
                            string insertSql = $@"
                                IF NOT EXISTS (SELECT * FROM [dbo].[LichSuThanhToans] WHERE [MaGiaoDich] = '{paymentId}')
                                BEGIN
                                    INSERT INTO [dbo].[LichSuThanhToans] 
                                    ([MaBienBan], [SoTien], [PhuongThucThanhToan], [NgayThanhToan], [MaGiaoDich])
                                    VALUES 
                                    ({maBienBan}, {soTien}, N'PayPal', '{ngayThanhToan:yyyy-MM-dd HH:mm:ss}', N'{paymentId}')
                                END";
                            
                            _context.Database.ExecuteSqlRaw(insertSql);
                            
                            // Cập nhật trạng thái biên bản riêng biệt
                            bienBan.KetQuaXuLy = "Đã thanh toán";
                            bienBan.NgayLadDuBienBan = DateTime.Now;
                            _context.SaveChanges();

                            // Tạo thông báo
                            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                            if (!string.IsNullOrEmpty(userId))
                            {
                                // Thông báo cho người dùng
                                _thongBaoService.TaoThongBaoThanhToanAsync(bienBan.MaBienBan, userId, false).Wait();
                                
                                // Thông báo cho cán bộ
                                _thongBaoService.TaoThongBaoThanhToanAsync(bienBan.MaBienBan, userId, true).Wait();
                            }
                            
                            return RedirectToAction("ThongBao", new { message = "Thanh toán thành công!" });
                        }
                        catch (Exception sqlEx)
                        {
                            Console.WriteLine($"SQL Error: {sqlEx.Message}");
                            return RedirectToAction("ThongBao", new { message = $"Lỗi khi lưu thanh toán: {sqlEx.Message}" });
                        }
                    }
                    catch (DbUpdateException dbEx)
                    {
                        return RedirectToAction("ThongBao", new { message = $"Lỗi khi cập nhật dữ liệu: {dbEx.Message}" });
                    }
                }
                else
                {
                    return RedirectToAction("ThongBao", new { message = "Thanh toán chưa được phê duyệt." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return RedirectToAction("ThongBao", new { message = $"Lỗi khi xử lý thanh toán: {ex.Message}" });
            }
        }
        
        public IActionResult ThanhToanPayPal(int maBienBan)
        {
            var bienBan = _context.BienBanVPhams.Find(maBienBan);
            if (bienBan == null)
                return NotFound();

            if (bienBan.KetQuaXuLy == "Đã thanh toán")
                return RedirectToAction("ThongBao", new { message = "Biên bản này đã được thanh toán." });

            try
            {
                // Kiểm tra cấu hình PayPal
                if (string.IsNullOrEmpty(_payPalConfig.ClientId) || 
                    _payPalConfig.ClientId == "YOUR_PAYPAL_CLIENT_ID" ||
                    string.IsNullOrEmpty(_payPalConfig.ClientSecret) ||
                    _payPalConfig.ClientSecret == "YOUR_PAYPAL_CLIENT_SECRET")
                {
                    return RedirectToAction("ThongBao", new { message = "Cấu hình PayPal chưa được thiết lập. Vui lòng liên hệ quản trị viên." });
                }

                // Chuyển đổi tiền VND sang USD cho PayPal (đơn giản hóa: 23000 VND = 1 USD)
                decimal usdAmount = bienBan.SoTienPhat / 23000;
                
                string returnUrl = Url.Action("ThanhCong", "ThanhToan", new { maBienBan }, Request.Scheme);
                string cancelUrl = Url.Action("HuyThanhToan", "ThanhToan", null, Request.Scheme);
                string description = $"Thanh toán vi phạm #{bienBan.MaBienBan} - {bienBan.TenBienBan}";

                // Sử dụng helper để tạo thanh toán
                string approvalUrl = _payPalHelper.CreatePayment(usdAmount, description, returnUrl, cancelUrl);

                return Redirect(approvalUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return RedirectToAction("ThongBao", new { message = $"Lỗi khi khởi tạo thanh toán: {ex.Message}" });
            }
        }
    }
} 