using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NHOM10_DACS_QLVIPHAM.Models;
using NHOM10_DACS_QLVIPHAM.Services;
using NHOM10_DACS_QLVIPHAM.ViewModels;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

namespace NHOM10_DACS_QLVIPHAM.Controllers
{
    public class BangChungController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IThongBaoService _thongBaoService;

        public BangChungController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IThongBaoService thongBaoService)
        {
            _context = context;
            _userManager = userManager;
            _thongBaoService = thongBaoService;
        }

        // GET: BangChung/YeuCauBangChung/5
        [Authorize]
        public async Task<IActionResult> YeuCauBangChung(int id)
        {
            // Lấy thông tin user hiện tại
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Không tìm thấy thông tin người dùng.");
            }

            // Lấy thông tin công dân
            var congDan = await _context.CongDans.FirstOrDefaultAsync(c => c.MaNguoiDung == user.Id);
            if (congDan == null)
            {
                return NotFound("Không tìm thấy thông tin công dân.");
            }

            // Lấy thông tin biên bản vi phạm
            var bienBan = await _context.BienBanVPhams
                .FirstOrDefaultAsync(b => b.MaBienBan == id && b.MaCongDan == congDan.MaCongDan);
            if (bienBan == null)
            {
                return NotFound("Không tìm thấy biên bản vi phạm.");
            }

            // Kiểm tra xem công dân đã yêu cầu bằng chứng cho biên bản này chưa
            var yeuCauTonTai = await _context.YeuCauBangChungViPhams
                .AnyAsync(y => y.MaBienBan == id && y.MaCongDan == congDan.MaCongDan && (y.TrangThai == "Chờ xử lý" || y.TrangThai == "Đã cung cấp"));

            if (yeuCauTonTai)
            {
                TempData["ErrorMessage"] = "Bạn đã gửi yêu cầu cung cấp bằng chứng cho biên bản này rồi hoặc yêu cầu đã được xử lý.";
                return RedirectToAction("ChiTietViPham", "CongDan", new { id = id });
            }

            var viewModel = new YeuCauBangChungViewModel
            {
                MaBienBan = bienBan.MaBienBan,
                TenBienBan = bienBan.TenBienBan
            };

            return View(viewModel);
        }

        // POST: BangChung/YeuCauBangChung/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> YeuCauBangChung(YeuCauBangChungViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Lấy thông tin user hiện tại
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Không tìm thấy thông tin người dùng.");
            }

            // Lấy thông tin công dân
            var congDan = await _context.CongDans.FirstOrDefaultAsync(c => c.MaNguoiDung == user.Id);
            if (congDan == null)
            {
                return NotFound("Không tìm thấy thông tin công dân.");
            }

            // Lấy thông tin biên bản vi phạm
            var bienBan = await _context.BienBanVPhams
                .Include(b => b.ViPham)
                .FirstOrDefaultAsync(b => b.MaBienBan == model.MaBienBan && b.MaCongDan == congDan.MaCongDan);
            if (bienBan == null)
            {
                return NotFound("Không tìm thấy biên bản vi phạm.");
            }

            // Tạo yêu cầu bằng chứng mới
            var yeuCau = new YeuCauBangChungViPham
            {
                MaBienBan = model.MaBienBan,
                MaCongDan = congDan.MaCongDan,
                NgayYeuCau = DateTime.Now,
                LyDoYeuCau = model.LyDoYeuCau,
                TrangThai = "Chờ xử lý"
            };

            _context.YeuCauBangChungViPhams.Add(yeuCau);
            await _context.SaveChangesAsync();

            // Tạo thông báo cho cán bộ
            var thongBao = new NhatKyThong
            {
                NoiDungHoatDong = $"Công dân {congDan.TenCongDan} yêu cầu cung cấp bằng chứng cho biên bản {bienBan.TenBienBan}",
                ThoiGian = DateTime.Now,
                NguoiThucHien = congDan.TenCongDan
            };

            _context.NhatKyThongs.Add(thongBao);
            await _context.SaveChangesAsync();

            // Tạo thông báo cho người dùng
            await _thongBaoService.TaoThongBao(
                congDan.MaCongDan,
                "Đã nhận yêu cầu cung cấp bằng chứng",
                $"Chúng tôi đã nhận được yêu cầu cung cấp bằng chứng cho biên bản {bienBan.TenBienBan}. Yêu cầu đang được xử lý.",
                "Yêu cầu",
                user.Id
            );

            TempData["SuccessMessage"] = "Yêu cầu cung cấp bằng chứng đã được gửi thành công.";
            return RedirectToAction("ChiTietViPham", "CongDan", new { id = model.MaBienBan });
        }

        // GET: BangChung/DanhSachYeuCau
        [Authorize(Roles = "Admin,CanBo")]
        public async Task<IActionResult> DanhSachYeuCau()
        {
            var yeuCaus = await _context.YeuCauBangChungViPhams
                .Include(y => y.BienBanVPham)
                .Include(y => y.CongDan)
                .OrderByDescending(y => y.NgayYeuCau)
                .ToListAsync();

            return View(yeuCaus);
        }

        // GET: BangChung/XuLyYeuCau/5
        [Authorize(Roles = "Admin,CanBo")]
        public async Task<IActionResult> XuLyYeuCau(int id)
        {
            var yeuCau = await _context.YeuCauBangChungViPhams
                .Include(y => y.BienBanVPham)
                .Include(y => y.CongDan)
                .FirstOrDefaultAsync(y => y.MaYeuCau == id);

            if (yeuCau == null)
            {
                return NotFound("Không tìm thấy yêu cầu.");
            }

            var viewModel = new QuanLyYeuCauBangChungViewModel
            {
                MaYeuCau = yeuCau.MaYeuCau,
                MaBienBan = yeuCau.MaBienBan,
                TenBienBan = yeuCau.BienBanVPham?.TenBienBan,
                TenCongDan = yeuCau.CongDan?.TenCongDan,
                MaTheCC = yeuCau.CongDan?.MaTheCC,
                NgayYeuCau = yeuCau.NgayYeuCau,
                LyDoYeuCau = yeuCau.LyDoYeuCau,
                TrangThai = yeuCau.TrangThai ?? "Chờ xử lý",
                DuongDanBangChung = yeuCau.DuongDanBangChung
            };

            return View(viewModel);
        }

        // POST: BangChung/XuLyYeuCau/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,CanBo")]
        public async Task<IActionResult> XuLyYeuCau(QuanLyYeuCauBangChungViewModel model, IFormFile BangChungFile)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var yeuCau = await _context.YeuCauBangChungViPhams
                .Include(y => y.BienBanVPham)
                .Include(y => y.CongDan)
                .FirstOrDefaultAsync(y => y.MaYeuCau == model.MaYeuCau);

            if (yeuCau == null)
            {
                return NotFound("Không tìm thấy yêu cầu.");
            }

            // Cập nhật thông tin xử lý
            yeuCau.TrangThai = model.TrangThai;
            yeuCau.GhiChu = model.GhiChu;
            yeuCau.NgayXuLy = DateTime.Now;
            yeuCau.NguoiXuLy = User.Identity?.Name;

            // Xử lý tải lên bằng chứng nếu có
            if (BangChungFile != null && BangChungFile.Length > 0 && model.TrangThai == "Đã cung cấp")
            {
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await BangChungFile.CopyToAsync(memoryStream);
                        byte[] fileData = memoryStream.ToArray();
                        string base64String = Convert.ToBase64String(fileData);
                        
                        // Lưu bằng chứng dưới dạng base64 string
                        yeuCau.DuongDanBangChung = base64String;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi tải lên file: " + ex.Message);
                    return View(model);
                }
            }

            _context.Update(yeuCau);
            await _context.SaveChangesAsync();

            // Ghi log hành động
            var nhatKyThong = new NhatKyThong
            {
                NoiDungHoatDong = $"Xử lý yêu cầu cung cấp bằng chứng #{yeuCau.MaYeuCau} cho biên bản {yeuCau.BienBanVPham?.TenBienBan}, kết quả: {model.TrangThai}",
                ThoiGian = DateTime.Now,
                NguoiThucHien = User.Identity?.Name ?? "Admin"
            };

            _context.NhatKyThongs.Add(nhatKyThong);
            await _context.SaveChangesAsync();

            // Tạo thông báo cho công dân
            if (yeuCau.CongDan?.MaNguoiDung != null)
            {
                await _thongBaoService.TaoThongBao(
                    yeuCau.MaCongDan,
                    "Kết quả yêu cầu cung cấp bằng chứng",
                    $"Yêu cầu cung cấp bằng chứng cho biên bản {yeuCau.BienBanVPham?.TenBienBan} đã được xử lý. Kết quả: {model.TrangThai}. {model.GhiChu}",
                    "Yêu cầu",
                    yeuCau.CongDan.MaNguoiDung
                );
            }

            TempData["SuccessMessage"] = "Đã xử lý yêu cầu cung cấp bằng chứng thành công.";
            return RedirectToAction(nameof(DanhSachYeuCau));
        }

        // GET: BangChung/XemBangChung/5
        [Authorize]
        public async Task<IActionResult> XemBangChung(int id)
        {
            // Lấy thông tin user hiện tại
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Không tìm thấy thông tin người dùng.");
            }

            var yeuCau = await _context.YeuCauBangChungViPhams
                .Include(y => y.BienBanVPham)
                .Include(y => y.CongDan)
                .FirstOrDefaultAsync(y => y.MaYeuCau == id);

            if (yeuCau == null)
            {
                return NotFound("Không tìm thấy yêu cầu.");
            }

            // Kiểm tra quyền xem (công dân chỉ xem được yêu cầu của mình)
            bool isAdmin = User.IsInRole("Admin") || User.IsInRole("CanBo");
            bool isOwner = yeuCau.CongDan?.MaNguoiDung == user.Id;

            if (!isAdmin && !isOwner)
            {
                return Forbid();
            }

            // Kiểm tra trạng thái đã cung cấp
            if (yeuCau.TrangThai != "Đã cung cấp" || string.IsNullOrEmpty(yeuCau.DuongDanBangChung))
            {
                TempData["ErrorMessage"] = "Bằng chứng chưa được cung cấp.";
                if (isAdmin)
                {
                    return RedirectToAction(nameof(DanhSachYeuCau));
                }
                else
                {
                    return RedirectToAction("ChiTietViPham", "CongDan", new { id = yeuCau.MaBienBan });
                }
            }

            return View(yeuCau);
        }
    }
} 