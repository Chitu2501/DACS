using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NHOM10_DACS_QLVIPHAM.Models;
using NHOM10_DACS_QLVIPHAM.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace NHOM10_DACS_QLVIPHAM.Controllers
{
    public class ViPhamController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ViPhamController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /ViPham/TraCuuViPham
        public IActionResult TraCuuViPham()
        {
            return View();
        }

        // POST: /ViPham/TraCuuViPham
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TraCuuViPham(string maTheCC)
        {
            if (string.IsNullOrEmpty(maTheCC))
            {
                ModelState.AddModelError("", "Vui lòng nhập mã thẻ căn cước");
                return View();
            }

            // Kiểm tra thẻ căn cước có tồn tại không
            var congDan = await _context.CongDans.FirstOrDefaultAsync(c => c.MaTheCC == maTheCC);
            if (congDan == null)
            {
                ModelState.AddModelError("", "Không tìm thấy thông tin công dân với mã thẻ này");
                return View();
            }

            // Lấy các biên bản vi phạm của công dân - tìm kiếm bằng cả MaCongDan và MaTheCC
            var bienBanViPhams = await _context.BienBanVPhams
                .Where(b => b.MaCongDan == congDan.MaCongDan || b.MaTheCC == maTheCC)
                .Include(b => b.ViPham)
                .ThenInclude(v => v.LoaiViPham)
                .ToListAsync();

            ViewBag.CongDan = congDan;
            return View("KetQuaTraCuuViPham", bienBanViPhams);
        }

        // GET: /ViPham/TraCuuViPhamTheoTheCC
        [HttpGet]
        public async Task<IActionResult> TraCuuViPhamTheoTheCC(string maTheCC)
        {
            if (string.IsNullOrEmpty(maTheCC))
            {
                return Json(new { success = false, message = "Vui lòng nhập mã thẻ căn cước" });
            }

            // Kiểm tra thẻ căn cước có tồn tại không
            var congDan = await _context.CongDans.FirstOrDefaultAsync(c => c.MaTheCC == maTheCC);
            if (congDan == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin công dân với mã thẻ này" });
            }

            // Lấy các biên bản vi phạm của công dân - tìm kiếm bằng cả MaCongDan và MaTheCC
            var bienBanViPhams = await _context.BienBanVPhams
                .Where(b => b.MaCongDan == congDan.MaCongDan || b.MaTheCC == maTheCC)
                .Include(b => b.ViPham)
                .ThenInclude(v => v.LoaiViPham)
                .OrderByDescending(b => b.NgayLapBienBan)
                .Select(b => new {
                    maBienBan = b.MaBienBan,
                    tenBienBan = b.TenBienBan,
                    tenViPham = b.ViPham.TenViPham,
                    loaiViPham = b.ViPham.LoaiViPham.TenLoaiVPham,
                    ngayVP = b.ThoiGianVPham.ToString("dd/MM/yyyy"),
                    trangThai = b.KetQuaXuLy,
                    soTienPhat = b.SoTienPhat
                })
                .ToListAsync();

            // Chuẩn bị dữ liệu công dân để trả về
            var congDanData = new {
                tenCongDan = congDan.TenCongDan,
                maTheCC = congDan.MaTheCC,
                diaChi = congDan.DiaChi,
                ngaySinh = congDan.NgaySinh.ToString("dd/MM/yyyy")
            };

            return Json(new { success = true, data = bienBanViPhams, congDan = congDanData });
        }

        // GET: /ViPham/ChiTietViPham/5
        public async Task<IActionResult> ChiTietViPham(int id)
        {
            var bienBan = await _context.BienBanVPhams
                .Include(b => b.ViPham)
                .ThenInclude(v => v.LoaiViPham)
                .FirstOrDefaultAsync(b => b.MaBienBan == id);

            if (bienBan == null)
            {
                return NotFound();
            }

            var congDan = await _context.CongDans.FindAsync(bienBan.MaCongDan);
            
            ViewBag.CongDan = congDan;
            return View(bienBan);
        }

        // GET: /ViPham/ThanhToanViPham/5
        [Authorize]
        public async Task<IActionResult> ThanhToanViPham(int id)
        {
            var bienBan = await _context.BienBanVPhams
                .Include(b => b.ViPham)
                .FirstOrDefaultAsync(b => b.MaBienBan == id);

            if (bienBan == null)
            {
                return NotFound();
            }

            // Chuyển hướng sang trang thanh toán PayPal
            return RedirectToAction("Index", "ThanhToan", new { maBienBan = id });
        }

        // GET: /ViPham/HoaDonThanhToan/5
        [Authorize]
        public async Task<IActionResult> HoaDonThanhToan(int id)
        {
            var hoaDon = await _context.HoaDons
                .Include(h => h.ChiTietHoaDons)
                .ThenInclude(c => c.ThanhToanViPham)
                .FirstOrDefaultAsync(h => h.MaHoaDon == id);

            if (hoaDon == null)
            {
                return NotFound();
            }

            var congDan = await _context.CongDans.FindAsync(hoaDon.MaCongDan);
            ViewBag.CongDan = congDan;

            return View(hoaDon);
        }

        // GET: /ViPham/KhieuNaiViPham/5
        [Authorize]
        public async Task<IActionResult> KhieuNaiViPham(int id)
        {
            var bienBan = await _context.BienBanVPhams
                .Include(b => b.ViPham)
                .FirstOrDefaultAsync(b => b.MaBienBan == id);

            if (bienBan == null)
            {
                return NotFound();
            }

            var khieuNaiViewModel = new KhieuNaiViPhamViewModel
            {
                MaBienBan = bienBan.MaBienBan,
                TenBienBan = bienBan.TenBienBan,
                NgayLapBienBan = bienBan.NgayLapBienBan,
                NoiDungViPham = bienBan.NoiDungVPham
            };

            return View(khieuNaiViewModel);
        }

        // POST: /ViPham/KhieuNaiViPham
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KhieuNaiViPham(KhieuNaiViPhamViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Lấy biên bản
            var bienBan = await _context.BienBanVPhams.FindAsync(model.MaBienBan);
            var congDan = await _context.CongDans.FindAsync(bienBan.MaCongDan);

            // Tạo khiếu nại
            var khieuNai = new KhieuNaiVPham
            {
                NgayKhieuNai = DateTime.Now,
                NoiDungKhieuNai = model.NoiDungKhieuNai,
                TrangThai = "Đang xử lý",
                MaCongDan = congDan?.MaCongDan ?? 0,
                MaTheCC = congDan?.MaTheCC ?? string.Empty
            };

            _context.KhieuNaiVPhams.Add(khieuNai);
            await _context.SaveChangesAsync();

            // Gửi thông báo
            var thongBao = new ThongBao
            {
                MaCongDan = congDan?.MaCongDan ?? 0,
                TieuDe = "Đã nhận khiếu nại vi phạm",
                NoiDung = $"Chúng tôi đã nhận được khiếu nại của bạn về biên bản {bienBan.TenBienBan}. Khiếu nại đang được xử lý.",
                NgayTao = DateTime.Now,
                DaDoc = false,
                LoaiThongBao = "Khiếu nại",
                UserId = User.Identity?.IsAuthenticated == true ? 
                    _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name)?.Id : null
            };

            _context.ThongBaos.Add(thongBao);
            await _context.SaveChangesAsync();

            return RedirectToAction("XacNhanKhieuNai");
        }

        // GET: /ViPham/XacNhanKhieuNai
        public IActionResult XacNhanKhieuNai()
        {
            return View();
        }

        // GET: /ViPham/DanhSachViPham
        [Authorize]
        public async Task<IActionResult> DanhSachViPham()
        {
            var userName = User.Identity.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            
            // Lấy công dân dựa trên email người dùng
            var congDan = await _context.CongDans.FirstOrDefaultAsync(c => c.GMAIL == user.Email);
            
            if (congDan == null)
            {
                return View("Error", new ErrorViewModel { RequestId = "Không tìm thấy thông tin công dân" });
            }

            var bienBanViPhams = await _context.BienBanVPhams
                .Where(b => b.MaCongDan == congDan.MaCongDan)
                .Include(b => b.ViPham)
                .ThenInclude(v => v.LoaiViPham)
                .ToListAsync();

            return View(bienBanViPhams);
        }

        // GET: /ViPham/LichSuThanhToan
        [Authorize]
        public async Task<IActionResult> LichSuThanhToan()
        {
            var userName = User.Identity.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            
            // Lấy công dân dựa trên email người dùng
            var congDan = await _context.CongDans.FirstOrDefaultAsync(c => c.GMAIL == user.Email);
            
            if (congDan == null)
            {
                return View("Error", new ErrorViewModel { RequestId = "Không tìm thấy thông tin công dân" });
            }

            var thanhToanViPhams = await _context.ThanhToanViPhams
                .Where(t => t.MaCongDan == congDan.MaCongDan)
                .OrderByDescending(t => t.NgayThanhToan)
                .ToListAsync();

            return View(thanhToanViPhams);
        }

        // GET: /ViPham/DanhSachLoaiViPham
        public async Task<IActionResult> DanhSachLoaiViPham()
        {
            var loaiViPhams = await _context.LoaiViPhams.ToListAsync();
            return View(loaiViPhams);
        }

        // GET: /ViPham/ChiTietLoaiViPham/5
        public async Task<IActionResult> ChiTietLoaiViPham(int id)
        {
            var loaiViPham = await _context.LoaiViPhams.FindAsync(id);
            
            if (loaiViPham == null)
            {
                return NotFound();
            }
            
            // Lấy danh sách các vi phạm thuộc loại này
            var viPhams = await _context.ViPhams
                .Where(v => v.MaLoaiVPham == id)
                .ToListAsync();
                
            ViewBag.ViPhams = viPhams;
            
            return View(loaiViPham);
        }
        
        // GET: /ViPham/PrintBienBan/5
        public async Task<IActionResult> PrintBienBan(int id)
        {
            var bienBan = await _context.BienBanVPhams
                .Include(b => b.ViPham)
                .ThenInclude(v => v.LoaiViPham)
                .FirstOrDefaultAsync(b => b.MaBienBan == id);

            if (bienBan == null)
            {
                return NotFound();
            }

            var congDan = await _context.CongDans.FindAsync(bienBan.MaCongDan);
            
            ViewBag.CongDan = congDan;
            return View(bienBan);
        }
    }
} 