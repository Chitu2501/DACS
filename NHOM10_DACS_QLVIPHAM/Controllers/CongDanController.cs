using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NHOM10_DACS_QLVIPHAM.Models;
using NHOM10_DACS_QLVIPHAM.Services;
using NHOM10_DACS_QLVIPHAM.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NHOM10_DACS_QLVIPHAM.Controllers
{
    [Authorize(Roles = "CongDan")]
    public class CongDanController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IThongBaoService _thongBaoService;

        public CongDanController(
            UserManager<ApplicationUser> userManager, 
            ApplicationDbContext context,
            IThongBaoService thongBaoService)
        {
            _userManager = userManager;
            _context = context;
            _thongBaoService = thongBaoService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.UserName = user?.HoTen ?? user?.UserName;
            
            // Lấy thông tin công dân từ User hiện tại (giả định có quan hệ)
            var userId = user?.Id;
            var congDan = userId == null ? null : await _context.CongDans
                .FirstOrDefaultAsync(c => c.MaNguoiDung == userId);
            
            ViewBag.CongDan = congDan;
            ViewBag.HasCCCD = congDan != null && !string.IsNullOrEmpty(congDan.MaTheCC);
            
            // Lấy thông tin thẻ căn cước
            if (ViewBag.HasCCCD)
            {
                var theCanCuoc = await _context.TheCanCuocs
                    .FirstOrDefaultAsync(t => t.MaTheCC == congDan.MaTheCC);
                ViewBag.TheCanCuoc = theCanCuoc;
            }
            
            // Đếm số vi phạm của công dân
            var soViPham = 0;
            if (congDan != null)
            {
                soViPham = await _context.BienBanVPhams
                    .Where(b => b.MaCongDan == congDan.MaCongDan)
                    .CountAsync();
            }
            ViewBag.SoViPham = soViPham;
            
            return View();
        }

        public async Task<IActionResult> DanhSachViPham()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user?.Id;
            
            if (userId == null)
            {
                return NotFound("Không tìm thấy thông tin người dùng.");
            }
            
            var congDan = await _context.CongDans
                .FirstOrDefaultAsync(c => c.MaNguoiDung == userId);
                
            if (congDan == null)
            {
                return NotFound("Không tìm thấy thông tin công dân liên kết với tài khoản này.");
            }
            
            var danhSachViPham = await _context.BienBanVPhams
                .Include(b => b.ViPham)
                .ThenInclude(v => v.LoaiViPham)
                .Where(b => b.MaCongDan == congDan.MaCongDan)
                .ToListAsync();
                
            return View(danhSachViPham);
        }
        
        public async Task<IActionResult> ThongTinCaNhan()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var userId = user.Id;
            var congDan = await _context.CongDans
                .FirstOrDefaultAsync(c => c.MaNguoiDung == userId);

            var model = new ThongTinCongDanViewModel
            {
                UserName = user.UserName ?? "",
                Email = user.Email ?? "",
                HoTen = user.HoTen ?? "",
                PhoneNumber = user.PhoneNumber ?? "",
                MaTheCC = congDan?.MaTheCC,
                DiaChi = congDan?.DiaChi,
                GioiTinh = congDan?.GIOTTING,
                NgaySinh = congDan?.NgaySinh,
                CongViec = congDan?.CONGVIEC,
                CCCD_HienTai = congDan?.MaTheCC
            };

            // Lấy trạng thái yêu cầu cập nhật gần nhất (nếu có)
            var yeuCauCapNhat = await _context.YeuCauCapNhatThongTins
                .Where(y => y.MaNguoiDung == userId)
                .OrderByDescending(y => y.NgayYeuCau)
                .FirstOrDefaultAsync();

            ViewBag.CoYeuCauDangCho = yeuCauCapNhat != null && yeuCauCapNhat.TrangThai == "Chờ xét duyệt";
            ViewBag.TrangThaiYeuCau = yeuCauCapNhat?.TrangThai;
            ViewBag.NgayYeuCau = yeuCauCapNhat?.NgayYeuCau;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> YeuCauCapNhatThongTin()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var userId = user.Id;
            var congDan = await _context.CongDans
                .FirstOrDefaultAsync(c => c.MaNguoiDung == userId);

            // Kiểm tra nếu đã có yêu cầu cập nhật đang chờ xử lý
            var yeuCauDangCho = await _context.YeuCauCapNhatThongTins
                .Where(y => y.MaNguoiDung == userId && y.TrangThai == "Chờ xét duyệt")
                .AnyAsync();

            if (yeuCauDangCho)
            {
                TempData["ErrorMessage"] = "Bạn đã có một yêu cầu cập nhật thông tin đang chờ xét duyệt. Vui lòng đợi xét duyệt trước khi gửi yêu cầu mới.";
                return RedirectToAction(nameof(ThongTinCaNhan));
            }

            var model = new YeuCauCapNhatThongTinViewModel
            {
                HoTen = user.HoTen,
                Email = user.Email,
                SDT = user.PhoneNumber,
                DiaChi = congDan?.DiaChi,
                MaTheCC = congDan?.MaTheCC,
                NgaySinh = congDan?.NgaySinh,
                GioiTinh = congDan?.GIOTTING,
                CongViec = congDan?.CONGVIEC
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> YeuCauCapNhatThongTin(YeuCauCapNhatThongTinViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var userId = user.Id;
            var congDan = await _context.CongDans
                .FirstOrDefaultAsync(c => c.MaNguoiDung == userId);

            // Tạo object chứa thông tin cập nhật
            var thongTinCapNhat = new
            {
                HoTen = model.HoTen,
                Email = model.Email,
                SDT = model.SDT,
                DiaChi = model.DiaChi,
                MaTheCC = model.MaTheCC,
                NgaySinh = model.NgaySinh,
                GioiTinh = model.GioiTinh,
                CongViec = model.CongViec
            };

            // Chuyển thành JSON
            var jsonThongTin = JsonConvert.SerializeObject(thongTinCapNhat);

            // Tạo yêu cầu cập nhật
            var yeuCauCapNhat = new YeuCauCapNhatThongTin
            {
                MaNguoiDung = userId,
                MaCongDan = congDan?.MaCongDan,
                NgayYeuCau = DateTime.Now,
                NoiDungCapNhat = model.NoiDungCapNhat,
                ThongTinCapNhat = jsonThongTin,
                TrangThai = "Chờ xét duyệt"
            };

            _context.YeuCauCapNhatThongTins.Add(yeuCauCapNhat);
            await _context.SaveChangesAsync();

            // Sử dụng ThongBaoService để gửi thông báo cho tất cả cán bộ
            await _thongBaoService.TaoThongBaoChoRoleAsync(
                "CanBo",
                "Yêu cầu cập nhật thông tin mới",
                $"Công dân {user.HoTen} đã gửi yêu cầu cập nhật thông tin cá nhân.",
                "YeuCauCapNhat",
                $"/CanBo/XemYeuCauCapNhatThongTin/{yeuCauCapNhat.MaYeuCau}"
            );

            TempData["SuccessMessage"] = "Yêu cầu cập nhật thông tin đã được gửi thành công và đang chờ xét duyệt.";
            return RedirectToAction(nameof(ThongTinCaNhan));
        }

        [HttpGet]
        public async Task<IActionResult> LichSuYeuCauCapNhat()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var lichSuYeuCau = await _context.YeuCauCapNhatThongTins
                .Where(y => y.MaNguoiDung == user.Id)
                .OrderByDescending(y => y.NgayYeuCau)
                .ToListAsync();

            return View(lichSuYeuCau);
        }

        public async Task<IActionResult> ChiTietViPham(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Không tìm thấy thông tin người dùng.");
            }
            
            var userId = user.Id;
            var congDan = await _context.CongDans
                .FirstOrDefaultAsync(c => c.MaNguoiDung == userId);
                
            if (congDan == null)
            {
                return NotFound("Không tìm thấy thông tin công dân liên kết với tài khoản này.");
            }
            
            // Lấy thông tin biên bản vi phạm và đảm bảo nó thuộc về công dân hiện tại
            var bienBan = await _context.BienBanVPhams
                .Include(b => b.ViPham)
                .ThenInclude(v => v.LoaiViPham)
                .FirstOrDefaultAsync(b => b.MaBienBan == id && b.MaCongDan == congDan.MaCongDan);
                
            if (bienBan == null)
            {
                return NotFound("Không tìm thấy biên bản vi phạm hoặc biên bản không thuộc về bạn.");
            }
            
            ViewBag.CongDan = congDan;
            return View(bienBan);
        }
        
        // GET: /CongDan/TaoKhieuNai/5
        public async Task<IActionResult> TaoKhieuNai(int maBienBan, string loai = "ThanhToan")
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Không tìm thấy thông tin người dùng.");
            }
            
            var congDan = await _context.CongDans
                .FirstOrDefaultAsync(c => c.MaNguoiDung == user.Id);
                
            if (congDan == null)
            {
                return NotFound("Không tìm thấy thông tin công dân liên kết với tài khoản này.");
            }
            
            var bienBan = await _context.BienBanVPhams
                .Include(b => b.ViPham)
                .FirstOrDefaultAsync(b => b.MaBienBan == maBienBan && b.MaCongDan == congDan.MaCongDan);
                
            if (bienBan == null)
            {
                return NotFound("Không tìm thấy biên bản vi phạm.");
            }
            
            // Kiểm tra xem đã có khiếu nại cho biên bản này chưa
            var khieuNaiTonTai = await _context.KhieuNaiVPhams
                .AnyAsync(k => k.MaCongDan == congDan.MaCongDan && k.MaBienBan == maBienBan && k.LoaiKhieuNai == loai);
                
            if (khieuNaiTonTai)
            {
                TempData["ErrorMessage"] = "Bạn đã có một khiếu nại cho biên bản này.";
                return RedirectToAction("ChiTietViPham", new { id = maBienBan });
            }
            
            var model = new TaoKhieuNaiViewModel
            {
                MaBienBan = maBienBan,
                TenBienBan = bienBan.TenBienBan,
                NgayLapBienBan = bienBan.NgayLapBienBan,
                SoTienPhat = bienBan.SoTienPhat,
                NoiDungViPham = bienBan.NoiDungVPham,
                LoaiKhieuNai = loai
            };
            
            return View(model);
        }
        
        // POST: /CongDan/TaoKhieuNai
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoKhieuNai(TaoKhieuNaiViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var user = await _userManager.GetUserAsync(User);
            var congDan = await _context.CongDans
                .FirstOrDefaultAsync(c => c.MaNguoiDung == user.Id);
                
            if (congDan == null)
            {
                return NotFound("Không tìm thấy thông tin công dân.");
            }
            
            var bienBan = await _context.BienBanVPhams
                .FirstOrDefaultAsync(b => b.MaBienBan == model.MaBienBan && b.MaCongDan == congDan.MaCongDan);
                
            if (bienBan == null)
            {
                return NotFound("Không tìm thấy biên bản vi phạm.");
            }
            
            // Tạo khiếu nại mới
            var khieuNai = new KhieuNaiVPham
            {
                MaCongDan = congDan.MaCongDan,
                MaTheCC = congDan.MaTheCC,
                NgayKhieuNai = DateTime.Now,
                NoiDungKhieuNai = model.NoiDungKhieuNai,
                TrangThai = "Đang xử lý",
                MaBienBan = model.MaBienBan,
                LoaiKhieuNai = model.LoaiKhieuNai
            };
            
            // Thêm thông tin cho yêu cầu bằng chứng nếu là loại "BangChung"
            if (model.LoaiKhieuNai == "BangChung")
            {
                khieuNai.LyDoYeuCau = model.LyDoYeuCau;
            }
            
            _context.KhieuNaiVPhams.Add(khieuNai);
            await _context.SaveChangesAsync();
            
            // Nếu có file bằng chứng đính kèm, lưu lại
            if (model.FileBangChung != null && model.FileBangChung.Length > 0)
            {
                // Lưu file bằng chứng
                using (var memoryStream = new MemoryStream())
                {
                    await model.FileBangChung.CopyToAsync(memoryStream);
                    var fileBytes = memoryStream.ToArray();
                    
                    // Lưu bằng chứng
                    var bangChung = new BangChungKhieuNai
                    {
                        MaKhieuNai = khieuNai.MaKhieuNai,
                        DuongDanFile = Convert.ToBase64String(fileBytes),
                        MoTa = "File đính kèm từ công dân",
                        NgayTao = DateTime.Now,
                        NguoiTao = congDan.TenCongDan
                    };
                    
                    _context.BangChungKhieuNais.Add(bangChung);
                    await _context.SaveChangesAsync();
                }
            }
            
            // Tạo thông báo cho cán bộ
            var thongBao = new ThongBao
            {
                TieuDe = model.LoaiKhieuNai == "ThanhToan" 
                        ? "Có khiếu nại mới về biên bản thanh toán" 
                        : "Có yêu cầu bằng chứng mới",
                NoiDung = model.LoaiKhieuNai == "ThanhToan"
                        ? $"Công dân {congDan.TenCongDan} đã gửi khiếu nại về biên bản {bienBan.TenBienBan}."
                        : $"Công dân {congDan.TenCongDan} đã gửi yêu cầu bằng chứng cho biên bản {bienBan.TenBienBan}.",
                NgayTao = DateTime.Now,
                DaDoc = false,
                LoaiThongBao = "KhieuNai",
                MaCongDan = 1 // Mặc định là admin hoặc hệ thống
            };
            
            _context.ThongBaos.Add(thongBao);
            await _context.SaveChangesAsync();
            
            // Tạo thông báo cho công dân
            var thongBaoCongDan = new ThongBao
            {
                TieuDe = model.LoaiKhieuNai == "ThanhToan" 
                        ? "Đã gửi khiếu nại thành công" 
                        : "Đã gửi yêu cầu bằng chứng thành công",
                NoiDung = model.LoaiKhieuNai == "ThanhToan"
                        ? $"Khiếu nại của bạn về biên bản {bienBan.TenBienBan} đã được gửi thành công. Vui lòng chờ phản hồi từ hệ thống."
                        : $"Yêu cầu bằng chứng của bạn cho biên bản {bienBan.TenBienBan} đã được gửi thành công. Vui lòng chờ phản hồi từ hệ thống.",
                NgayTao = DateTime.Now,
                DaDoc = false,
                LoaiThongBao = "KhieuNai",
                MaCongDan = congDan.MaCongDan
            };
            
            _context.ThongBaos.Add(thongBaoCongDan);
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = model.LoaiKhieuNai == "ThanhToan" 
                                        ? "Khiếu nại đã được gửi thành công." 
                                        : "Yêu cầu bằng chứng đã được gửi thành công.";
            return RedirectToAction("DanhSachKhieuNai");
        }

        // GET: /CongDan/TaoKhieuNaiThanhToan/5 - Keep for backward compatibility
        public async Task<IActionResult> TaoKhieuNaiThanhToan(int maBienBan)
        {
            return await TaoKhieuNai(maBienBan, "ThanhToan");
        }

        // POST: /CongDan/TaoKhieuNaiThanhToan - Keep for backward compatibility
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoKhieuNaiThanhToan(TaoKhieuNaiViewModel model)
        {
            model.LoaiKhieuNai = "ThanhToan";
            return await TaoKhieuNai(model);
        }

        // GET: /CongDan/DanhSachKhieuNai
        public async Task<IActionResult> DanhSachKhieuNai()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Không tìm thấy thông tin người dùng.");
            }
            
            var congDan = await _context.CongDans
                .FirstOrDefaultAsync(c => c.MaNguoiDung == user.Id);
                
            if (congDan == null)
            {
                return NotFound("Không tìm thấy thông tin công dân liên kết với tài khoản này.");
            }
            
            var danhSachKhieuNai = await _context.KhieuNaiVPhams
                .Where(k => k.MaCongDan == congDan.MaCongDan)
                .OrderByDescending(k => k.NgayKhieuNai)
                .ToListAsync();
            
            return View(danhSachKhieuNai);
        }
        
        // GET: /CongDan/ChiTietKhieuNai/5
        public async Task<IActionResult> ChiTietKhieuNai(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Không tìm thấy thông tin người dùng.");
            }
            
            var congDan = await _context.CongDans
                .FirstOrDefaultAsync(c => c.MaNguoiDung == user.Id);
                
            if (congDan == null)
            {
                return NotFound("Không tìm thấy thông tin công dân liên kết với tài khoản này.");
            }
            
            var khieuNai = await _context.KhieuNaiVPhams
                .Include(k => k.BienBanVPham)
                .FirstOrDefaultAsync(k => k.MaKhieuNai == id && k.MaCongDan == congDan.MaCongDan);
                
            if (khieuNai == null)
            {
                return NotFound("Không tìm thấy khiếu nại.");
            }
            
            // Lấy danh sách bằng chứng khiếu nại
            var bangChungs = await _context.BangChungKhieuNais
                .Where(b => b.MaKhieuNai == id)
                .OrderByDescending(b => b.NgayTao)
                .ToListAsync();
                
            ViewBag.BangChungs = bangChungs;
            
            return View(khieuNai);
        }

        [HttpGet]
        [Authorize(Roles = "CanBo")]
        public async Task<IActionResult> CapNhatThongTinCaNhan()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            var userId = user.Id;
            var congDan = await _context.CongDans.FirstOrDefaultAsync(c => c.MaNguoiDung == userId);
            if (congDan == null)
            {
                return NotFound();
            }
            var model = new ThongTinCongDanViewModel
            {
                UserName = user.UserName ?? "",
                Email = user.Email ?? "",
                HoTen = user.HoTen ?? "",
                PhoneNumber = user.PhoneNumber ?? "",
                MaTheCC = congDan.MaTheCC,
                DiaChi = congDan.DiaChi,
                GioiTinh = congDan.GIOTTING,
                NgaySinh = congDan.NgaySinh,
                CongViec = congDan.CONGVIEC,
                CCCD_HienTai = congDan.MaTheCC
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "CanBo")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatThongTinCaNhan(ThongTinCongDanViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            var userId = user.Id;
            var congDan = await _context.CongDans.FirstOrDefaultAsync(c => c.MaNguoiDung == userId);
            if (congDan == null)
            {
                return NotFound();
            }
            // Cập nhật thông tin
            user.HoTen = model.HoTen;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            congDan.MaTheCC = model.MaTheCC;
            congDan.DiaChi = model.DiaChi;
            congDan.GIOTTING = model.GioiTinh;
            if (model.NgaySinh.HasValue)
            {
                congDan.NgaySinh = model.NgaySinh.Value;
            }
            congDan.CONGVIEC = model.CongViec;
            await _userManager.UpdateAsync(user);
            _context.CongDans.Update(congDan);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("ThongTinCaNhan");
        }

        [HttpGet]
        public async Task<IActionResult> ChiTietTheCanCuoc(string maTheCC)
        {
            if (string.IsNullOrEmpty(maTheCC))
                return RedirectToAction("TraCuuTheCanCuoc");

            var theCanCuoc = await _context.TheCanCuocs.FirstOrDefaultAsync(t => t.MaTheCC == maTheCC);
            if (theCanCuoc == null)
                return NotFound();

            var congDan = await _context.CongDans.FirstOrDefaultAsync(c => c.MaTheCC == maTheCC);
            ViewBag.CongDan = congDan;
            return View(theCanCuoc);
        }
    }

    public class ThongTinCongDanViewModel
    {
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public string HoTen { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string? MaTheCC { get; set; }
        public string? DiaChi { get; set; }
        public string? GioiTinh { get; set; }
        public System.DateTime? NgaySinh { get; set; }
        public string? CongViec { get; set; }
        public string? CCCD_HienTai { get; set; }
    }
} 