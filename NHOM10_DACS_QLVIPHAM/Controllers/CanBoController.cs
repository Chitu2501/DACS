using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NHOM10_DACS_QLVIPHAM.Models;
using NHOM10_DACS_QLVIPHAM.Services;
using NHOM10_DACS_QLVIPHAM.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace NHOM10_DACS_QLVIPHAM.Controllers
{
    [Authorize(Roles = "Admin,CanBo")]
    public class CanBoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IThongBaoService _thongBaoService;

        public CanBoController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IThongBaoService thongBaoService)
        {
            _context = context;
            _userManager = userManager;
            _thongBaoService = thongBaoService;
        }

        // GET: /CanBo/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Account");
            }

            // Get user information
            var user = await _userManager.FindByNameAsync(userName);
            ViewBag.UserName = user?.UserName;

            // Get statistics
            ViewBag.TongViPham = await _context.ViPhams.CountAsync();
            ViewBag.TongLoaiViPham = await _context.LoaiViPhams.CountAsync();
            ViewBag.TongBienBan = await _context.BienBanVPhams.CountAsync();
            ViewBag.TongCongDan = await _context.CongDans.CountAsync();

            // Get pending tasks counts
            ViewBag.SoKhieuNaiMoi = await _context.KhieuNaiVPhams
                .Where(k => k.TrangThai == "Chờ xử lý")
                .CountAsync();

            ViewBag.SoYeuCauCapNhat = await _context.YeuCauCapNhatThongTins
                .Where(y => y.TrangThai == "Chờ duyệt")
                .CountAsync();

            ViewBag.SoYeuCauCapCCCD = await _context.YeuCauCapLaiThes
                .Where(y => y.TrangThai == "Chờ duyệt")
                .CountAsync();

            ViewBag.SoThanhToanChoXuLy = await _context.BienBanVPhams
                .Where(t => t.KetQuaXuLy == "Chờ xử lý")
                .CountAsync();

            // Get recent violations
            var recentViolations = await _context.BienBanVPhams
                .Include(b => b.CongDan)
                .OrderByDescending(b => b.NgayLapBienBan)
                .Take(5)
                .ToListAsync();

            return View(recentViolations);
        }

        // GET: /CanBo/Index
        public IActionResult Index()
        {
            return View();
        }

        // GET: /CanBo/QuanLyViPham
        public async Task<IActionResult> QuanLyViPham()
        {
            var viPhams = await _context.ViPhams
                .Include(v => v.LoaiViPham)
                .ToListAsync();
            
            return View(viPhams);
        }

        // GET: /CanBo/QuanLyLoaiViPham
        public async Task<IActionResult> QuanLyLoaiViPham()
        {
            var loaiViPhams = await _context.LoaiViPhams.ToListAsync();
            return View(loaiViPhams);
        }

        // GET: /CanBo/QuanLyBienBanViPham
        public async Task<IActionResult> QuanLyBienBanViPham()
        {
            var bienBanViPhams = await _context.BienBanVPhams
                .Include(b => b.ViPham)
                .ThenInclude(v => v.LoaiViPham)
                .ToListAsync();
            return View(bienBanViPhams);
        }

        // GET: /CanBo/ThemViPham
        public async Task<IActionResult> ThemViPham()
        {
            var loaiViPhams = await _context.LoaiViPhams.ToListAsync();
            var viewModel = new ThemViPhamViewModel
            {
                LoaiViPhams = loaiViPhams.Select(l => new SelectListItem
                {
                    Value = l.MaLoaiVPham.ToString(),
                    Text = l.TenLoaiVPham
                }).ToList()
            };
            return View(viewModel);
        }

        // POST: /CanBo/ThemViPham
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemViPham(ThemViPhamViewModel model)
        {
            if (ModelState.IsValid)
            {
                var viPham = new ViPham
                {
                    TenViPham = model.TenViPham,
                    MaLoaiVPham = model.MaLoaiVPham
                };

                _context.ViPhams.Add(viPham);
                await _context.SaveChangesAsync();

                // Cập nhật nhật ký thông tin
                var nhatKyThong = new NhatKyThong
                {
                    NoiDungHoatDong = $"Thêm vi phạm mới: {model.TenViPham}",
                    ThoiGian = DateTime.Now,
                    NguoiThucHien = User.Identity?.Name ?? "Admin"
                };

                _context.NhatKyThongs.Add(nhatKyThong);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(QuanLyViPham));
            }

            // Nếu ModelState không hợp lệ, tải lại danh sách loại vi phạm
            var loaiViPhams = await _context.LoaiViPhams.ToListAsync();
            model.LoaiViPhams = loaiViPhams.Select(l => new SelectListItem
            {
                Value = l.MaLoaiVPham.ToString(),
                Text = l.TenLoaiVPham
            }).ToList();

            return View(model);
        }

        // GET: /CanBo/SuaViPham/5
        public async Task<IActionResult> SuaViPham(int id)
        {
            var viPham = await _context.ViPhams.FindAsync(id);
            if (viPham == null)
            {
                return NotFound();
            }

            var loaiViPhams = await _context.LoaiViPhams.ToListAsync();
            var viewModel = new ThemViPhamViewModel
            {
                TenViPham = viPham.TenViPham,
                MaLoaiVPham = viPham.MaLoaiVPham,
                LoaiViPhams = loaiViPhams.Select(l => new SelectListItem
                {
                    Value = l.MaLoaiVPham.ToString(),
                    Text = l.TenLoaiVPham
                }).ToList()
            };

            ViewBag.MaViPham = id;
            return View(viewModel);
        }

        // POST: /CanBo/SuaViPham/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaViPham(int id, ThemViPhamViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var viPham = await _context.ViPhams.FindAsync(id);
                    if (viPham == null)
                    {
                        return NotFound();
                    }

                    viPham.TenViPham = model.TenViPham;
                    viPham.MaLoaiVPham = model.MaLoaiVPham;

                    _context.Update(viPham);
                    await _context.SaveChangesAsync();

                    // Cập nhật nhật ký thông tin
                    var nhatKyThong = new NhatKyThong
                    {
                        NoiDungHoatDong = $"Cập nhật vi phạm: {model.TenViPham}",
                        ThoiGian = DateTime.Now,
                        NguoiThucHien = User.Identity?.Name ?? "Admin"
                    };

                    _context.NhatKyThongs.Add(nhatKyThong);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ViPhamExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(QuanLyViPham));
            }

            // Nếu ModelState không hợp lệ, tải lại danh sách loại vi phạm
            var loaiViPhams = await _context.LoaiViPhams.ToListAsync();
            model.LoaiViPhams = loaiViPhams.Select(l => new SelectListItem
            {
                Value = l.MaLoaiVPham.ToString(),
                Text = l.TenLoaiVPham
            }).ToList();

            ViewBag.MaViPham = id;
            return View(model);
        }

        private bool ViPhamExists(int id)
        {
            return _context.ViPhams.Any(e => e.MaViPham == id);
        }

        // GET: /CanBo/ChiTietBienBan/5
        public async Task<IActionResult> ChiTietBienBan(int id)
        {
            var bienBan = await _context.BienBanVPhams
                .Include(b => b.ViPham)
                    .ThenInclude(v => v.LoaiViPham)
                .Include(b => b.CongDan)
                .FirstOrDefaultAsync(b => b.MaBienBan == id);

            if (bienBan == null)
            {
                return NotFound();
            }

            // Ghi log xem chi tiết
            var nhatKyThong = new NhatKyThong
            {
                NoiDungHoatDong = $"Xem chi tiết biên bản vi phạm: {bienBan.TenBienBan} (Mã: {bienBan.MaBienBan})",
                ThoiGian = DateTime.Now,
                NguoiThucHien = User.Identity?.Name ?? "Admin"
            };

            _context.NhatKyThongs.Add(nhatKyThong);
            await _context.SaveChangesAsync();

            return View(bienBan);
        }

        // POST: /CanBo/XoaBienBan/5
        [HttpPost, ActionName("XoaBienBan")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaBienBanConfirmed(int id)
        {
            var bienBan = await _context.BienBanVPhams.FindAsync(id);
            if (bienBan == null)
            {
                return NotFound();
            }

            string tenBienBan = bienBan.TenBienBan;
            int maBienBan = bienBan.MaBienBan;

            _context.BienBanVPhams.Remove(bienBan);
            await _context.SaveChangesAsync();

            // Ghi log xóa biên bản
            var nhatKyThong = new NhatKyThong
            {
                NoiDungHoatDong = $"Xóa biên bản vi phạm: {tenBienBan} (Mã: {maBienBan})",
                ThoiGian = DateTime.Now,
                NguoiThucHien = User.Identity?.Name ?? "Admin"
            };

            _context.NhatKyThongs.Add(nhatKyThong);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Biên bản vi phạm đã được xóa thành công.";
            return RedirectToAction(nameof(QuanLyBienBanViPham));
        }

        // GET: /CanBo/QuanLyYeuCauCapLaiThe
        public async Task<IActionResult> QuanLyYeuCauCapLaiThe()
        {
            var yeuCaus = await _context.YeuCauCapLaiThes
                .Include(y => y.CongDan)
                .Where(y => y.TrangThai == "Chờ xét duyệt")
                .ToListAsync();
            return View(yeuCaus);
        }

        // GET: /CanBo/XemYeuCauCapLaiThe/5
        public async Task<IActionResult> XemYeuCauCapLaiThe(int id)
        {
            var yeuCau = await _context.YeuCauCapLaiThes
                .Include(y => y.CongDan)
                .FirstOrDefaultAsync(y => y.MaYeuCauCapLai == id);

            if (yeuCau == null)
            {
                return NotFound();
            }

            return View(yeuCau);
        }

        // GET: /CanBo/XetDuyetYeuCauCapLaiThe/5
        public async Task<IActionResult> XetDuyetYeuCauCapLaiThe(int id)
        {
            var yeuCau = await _context.YeuCauCapLaiThes
                .Include(y => y.CongDan)
                .FirstOrDefaultAsync(y => y.MaYeuCauCapLai == id);

            if (yeuCau == null)
            {
                return NotFound();
            }

            var viewModel = new XetDuyetCapLaiTheViewModel
            {
                MaYeuCauCapLai = yeuCau.MaYeuCauCapLai,
                LyDo = yeuCau.LyDo,
                NgayYeuCau = yeuCau.NgayYeuCau,
                MaCongDan = yeuCau.MaCongDan,
                TenCongDan = yeuCau.CongDan?.TenCongDan,
                MaTheCC = yeuCau.MaTheCC
            };

            return View(viewModel);
        }

        // POST: /CanBo/XetDuyetYeuCauCapLaiThe
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XetDuyetYeuCauCapLaiThe(XetDuyetCapLaiTheViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var yeuCau = await _context.YeuCauCapLaiThes.FindAsync(model.MaYeuCauCapLai);
            if (yeuCau == null)
            {
                return NotFound();
            }

            // Lấy thông tin người dùng hiện tại
            var userName = User.Identity.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            
            // Lấy cán bộ từ email người dùng
            var canBo = await _context.CanBos.FirstOrDefaultAsync(c => c.TenCanBo == user.HoTen);
            
            if (canBo == null)
            {
                // Nếu không tìm thấy cán bộ, tạo một cán bộ mới
                canBo = new CanBo
                {
                    TenCanBo = user.HoTen ?? "Admin",
                    ChucVu = "Quản trị viên",
                    MaPhongBan = 1 // Giả định
                };
                _context.CanBos.Add(canBo);
                await _context.SaveChangesAsync();
            }

            // Tạo xét duyệt
            var xetDuyet = new XetDuyetCapLaiThe
            {
                NgayXetDuyet = DateTime.Now,
                KetQua = model.KetQua,
                GhiChu = model.GhiChu,
                MaYeuCauCapLai = model.MaYeuCauCapLai,
                MaCanBo = canBo.MaCanBo
            };

            _context.XetDuyetCapLaiThes.Add(xetDuyet);
            await _context.SaveChangesAsync();

            // Cập nhật trạng thái yêu cầu
            yeuCau.TrangThai = model.KetQua;
            _context.Update(yeuCau);
            await _context.SaveChangesAsync();

            // Nếu đồng ý cấp lại, tạo thông tin cấp lại
            if (model.KetQua == "Đã duyệt")
            {
                var capLaiThe = new CapLaiTheCanCuoc
                {
                    NgayCapLai = DateTime.Now,
                    LyDo = yeuCau.LyDo,
                    TrangThai = "Đang xử lý",
                    MaCongDan = yeuCau.MaCongDan,
                    MaTheCC = yeuCau.MaTheCC
                };

                _context.CapLaiTheCanCuocs.Add(capLaiThe);
                await _context.SaveChangesAsync();
            }

            // Gửi thông báo cho công dân
            var thongBao = new ThongBao
            {
                MaCongDan = yeuCau.MaCongDan,
                TieuDe = "Kết quả xét duyệt yêu cầu cấp lại thẻ căn cước",
                NoiDung = $"Yêu cầu cấp lại thẻ căn cước của bạn đã được xét duyệt với kết quả: {model.KetQua}. {model.GhiChu}",
                NgayTao = DateTime.Now,
                DaDoc = false,
                LoaiThongBao = "Xét duyệt",
                UserId = _context.CongDans.FirstOrDefault(c => c.MaCongDan == yeuCau.MaCongDan)?.MaNguoiDung
            };

            _context.ThongBaos.Add(thongBao);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(QuanLyYeuCauCapLaiThe));
        }

        // GET: /CanBo/QuanLyKhieuNai
        public async Task<IActionResult> QuanLyKhieuNai(string trangThai = "DangXuLy", DateTime? tuNgay = null, DateTime? denNgay = null, string loai = "TatCa")
        {
            try
            {
                IQueryable<KhieuNaiVPham> khieuNaiQuery = _context.KhieuNaiVPhams
                    .Include(k => k.CongDan);
                
                // Lọc theo trạng thái
                if (!string.IsNullOrEmpty(trangThai) && trangThai != "TatCa")
                {
                    string trangThaiFilter = "";
                    switch (trangThai)
                    {
                        case "DangXuLy":
                            trangThaiFilter = "Đang xử lý";
                            break;
                        case "DaXuLy":
                            trangThaiFilter = "Đã xử lý";
                            break;
                        case "TuChoi":
                            trangThaiFilter = "Từ chối";
                            break;
                    }
                    
                    if (!string.IsNullOrEmpty(trangThaiFilter))
                    {
                        khieuNaiQuery = khieuNaiQuery.Where(k => k.TrangThai == trangThaiFilter);
                    }
                }
                
                // Lọc theo loại khiếu nại
                if (!string.IsNullOrEmpty(loai) && loai != "TatCa")
                {
                    khieuNaiQuery = khieuNaiQuery.Where(k => k.LoaiKhieuNai == loai);
                }
                
                // Lọc theo ngày
                if (tuNgay.HasValue)
                {
                    khieuNaiQuery = khieuNaiQuery.Where(k => k.NgayKhieuNai >= tuNgay.Value.Date);
                }
                
                if (denNgay.HasValue)
                {
                    khieuNaiQuery = khieuNaiQuery.Where(k => k.NgayKhieuNai <= denNgay.Value.Date.AddDays(1).AddSeconds(-1));
                }
                
                // Sắp xếp theo ngày khiếu nại mới nhất
                khieuNaiQuery = khieuNaiQuery.OrderByDescending(k => k.NgayKhieuNai);
                
                var khieuNais = await khieuNaiQuery.ToListAsync();

                // Sau khi lấy danh sách khiếu nại, thực hiện load thủ công biên bản vi phạm để tránh lỗi
                foreach (var khieuNai in khieuNais)
                {
                    if (khieuNai.MaBienBan.HasValue)
                    {
                        khieuNai.BienBanVPham = await _context.BienBanVPhams.FindAsync(khieuNai.MaBienBan.Value);
                    }
                }
                
                // Lưu các giá trị filter vào ViewBag để giữ lại giá trị trên UI
                ViewBag.TrangThai = trangThai;
                ViewBag.TuNgay = tuNgay?.ToString("yyyy-MM-dd");
                ViewBag.DenNgay = denNgay?.ToString("yyyy-MM-dd");
                ViewBag.Loai = loai;
                
                return View(khieuNais);
            }
            catch (Exception ex)
            {
                // Log lỗi
                Console.WriteLine(ex.ToString());
                ModelState.AddModelError("", "Có lỗi xảy ra khi tải dữ liệu. Vui lòng thử lại sau.");
                return View(new List<KhieuNaiVPham>());
            }
        }

        // GET: /CanBo/XemKhieuNai/5
        public async Task<IActionResult> XemKhieuNai(int id)
        {
            var khieuNai = await _context.KhieuNaiVPhams
                .Include(k => k.CongDan)
                .FirstOrDefaultAsync(k => k.MaKhieuNai == id);

            if (khieuNai == null)
            {
                return NotFound();
            }

            return View(khieuNai);
        }

        // GET: /CanBo/TraLoiKhieuNai/5
        [Authorize(Roles = "CanBo,Admin")]
        public async Task<IActionResult> TraLoiKhieuNai(int id)
        {
            var khieuNai = await _context.KhieuNaiVPhams
                .Include(k => k.CongDan)
                .Include(k => k.BienBanVPham)
                .FirstOrDefaultAsync(k => k.MaKhieuNai == id);

            if (khieuNai == null)
            {
                return NotFound();
            }

            // Nếu khiếu nại đã có phản hồi, chuyển hướng về trang chi tiết khiếu nại
            if (!string.IsNullOrEmpty(khieuNai.KetQuaXuLy))
            {
                return RedirectToAction("ChiTietKhieuNai", "CongDan", new { id = khieuNai.MaKhieuNai });
            }

            var viewModel = new TraLoiKhieuNaiViewModel
            {
                MaKhieuNai = khieuNai.MaKhieuNai,
                NoiDungKhieuNai = khieuNai.NoiDungKhieuNai,
                LyDoYeuCau = khieuNai.LyDoYeuCau,
                NgayKhieuNai = khieuNai.NgayKhieuNai,
                MaCongDan = khieuNai.MaCongDan,
                TenCongDan = khieuNai.CongDan?.TenCongDan,
                LoaiKhieuNai = khieuNai.LoaiKhieuNai,
                MaBienBan = khieuNai.MaBienBan,
                TenBienBan = khieuNai.BienBanVPham?.TenBienBan
            };

            return View(viewModel);
        }

        // GET: /CanBo/GetFilteredKhieuNai
        [HttpGet]
        public async Task<IActionResult> GetFilteredKhieuNai(string trangThai = "TatCa", DateTime? tuNgay = null, DateTime? denNgay = null, string loai = "TatCa")
        {
            try
            {
                IQueryable<KhieuNaiVPham> khieuNaiQuery = _context.KhieuNaiVPhams
                    .Include(k => k.CongDan);
                
                // Lọc theo trạng thái
                if (!string.IsNullOrEmpty(trangThai) && trangThai != "TatCa")
                {
                    string trangThaiFilter = "";
                    switch (trangThai)
                    {
                        case "DangXuLy":
                            trangThaiFilter = "Đang xử lý";
                            break;
                        case "DaXuLy":
                            trangThaiFilter = "Đã xử lý";
                            break;
                        case "TuChoi":
                            trangThaiFilter = "Từ chối";
                            break;
                    }
                    
                    if (!string.IsNullOrEmpty(trangThaiFilter))
                    {
                        khieuNaiQuery = khieuNaiQuery.Where(k => k.TrangThai == trangThaiFilter);
                    }
                }
                
                // Lọc theo loại khiếu nại
                if (!string.IsNullOrEmpty(loai) && loai != "TatCa")
                {
                    khieuNaiQuery = khieuNaiQuery.Where(k => k.LoaiKhieuNai == loai);
                }
                
                // Lọc theo ngày
                if (tuNgay.HasValue)
                {
                    khieuNaiQuery = khieuNaiQuery.Where(k => k.NgayKhieuNai >= tuNgay.Value.Date);
                }
                
                if (denNgay.HasValue)
                {
                    khieuNaiQuery = khieuNaiQuery.Where(k => k.NgayKhieuNai <= denNgay.Value.Date.AddDays(1).AddSeconds(-1));
                }
                
                // Sắp xếp theo ngày khiếu nại mới nhất
                khieuNaiQuery = khieuNaiQuery.OrderByDescending(k => k.NgayKhieuNai);
                
                var khieuNais = await khieuNaiQuery.ToListAsync();

                // Sau khi lấy danh sách khiếu nại, thực hiện load thủ công biên bản vi phạm để tránh lỗi
                foreach (var khieuNai in khieuNais)
                {
                    if (khieuNai.MaBienBan.HasValue)
                    {
                        khieuNai.BienBanVPham = await _context.BienBanVPhams.FindAsync(khieuNai.MaBienBan.Value);
                    }
                }
                
                // Tạo đối tượng kết quả để trả về cho client
                var result = new
                {
                    khieuNais = khieuNais.Select(k => new {
                        maKhieuNai = k.MaKhieuNai,
                        loaiKhieuNai = k.LoaiKhieuNai,
                        tenCongDan = k.CongDan?.TenCongDan,
                        ngayKhieuNai = k.NgayKhieuNai.ToString("dd/MM/yyyy HH:mm"),
                        maBienBan = k.MaBienBan,
                        trangThai = k.TrangThai
                    }),
                    summary = new
                    {
                        total = khieuNais.Count,
                        thanhToanCount = khieuNais.Count(k => k.LoaiKhieuNai == "ThanhToan"),
                        bangChungCount = khieuNais.Count(k => k.LoaiKhieuNai == "BangChung"),
                        pendingCount = khieuNais.Count(k => k.TrangThai == "Đang xử lý")
                    }
                };
                
                return Json(result);
            }
            catch (Exception ex)
            {
                // Log lỗi
                Console.WriteLine(ex.ToString());
                return Json(new { error = "Có lỗi xảy ra khi tải dữ liệu. Vui lòng thử lại sau." });
            }
        }

        // POST: /CanBo/TraLoiKhieuNai
        [HttpPost]
        [Authorize(Roles = "CanBo,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TraLoiKhieuNai(TraLoiKhieuNaiViewModel model, IFormFile bangChungFile)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
                    }
                    return View(model);
                }

                var khieuNai = await _context.KhieuNaiVPhams
                    .Include(k => k.CongDan)
                    .FirstOrDefaultAsync(k => k.MaKhieuNai == model.MaKhieuNai);

                if (khieuNai == null)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = "Không tìm thấy khiếu nại" });
                    }
                    return NotFound();
                }

                // Validate file if it's a BangChung type complaint
                if (khieuNai.LoaiKhieuNai == "BangChung" && bangChungFile != null && bangChungFile.Length > 0)
                {
                    // Check file size (limit to 5MB)
                    if (bangChungFile.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("bangChungFile", "Kích thước file không được vượt quá 5MB");
                        return View(model);
                    }

                    // Check file extension
                    var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };
                    var fileExtension = Path.GetExtension(bangChungFile.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("bangChungFile", "Chỉ chấp nhận file PDF, DOC, DOCX, JPG, PNG");
                        return View(model);
                    }
                }

                // Update complaint
                khieuNai.KetQuaXuLy = model.KetQuaXuLy;
                khieuNai.TrangThai = model.TrangThai;
                khieuNai.NgayTraLoi = DateTime.Now;
                khieuNai.NguyenXuLy = User.Identity.Name;

                // Handle file upload for BangChung type
                if (khieuNai.LoaiKhieuNai == "BangChung" && bangChungFile != null && bangChungFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await bangChungFile.CopyToAsync(memoryStream);
                        var fileBytes = memoryStream.ToArray();
                        khieuNai.DuongDanBangChung = Convert.ToBase64String(fileBytes);
                    }
                }

                _context.Update(khieuNai);
                await _context.SaveChangesAsync();

                // Create notification for citizen
                var thongBao = new ThongBao
                {
                    MaCongDan = khieuNai.MaCongDan,
                    TieuDe = khieuNai.LoaiKhieuNai == "ThanhToan" 
                            ? "Phản hồi khiếu nại thanh toán" 
                            : "Phản hồi yêu cầu bằng chứng",
                    NoiDung = khieuNai.LoaiKhieuNai == "ThanhToan"
                            ? $"Khiếu nại của bạn đã được xử lý. Kết quả: {model.KetQuaXuLy}"
                            : $"Yêu cầu bằng chứng của bạn đã được xử lý. Kết quả: {model.KetQuaXuLy}",
                    NgayTao = DateTime.Now,
                    DaDoc = false,
                    LoaiThongBao = khieuNai.LoaiKhieuNai == "ThanhToan" ? "KhieuNai" : "BangChung",
                    UserId = _context.CongDans.FirstOrDefault(c => c.MaCongDan == khieuNai.MaCongDan)?.MaNguoiDung
                };

                _context.ThongBaos.Add(thongBao);
                await _context.SaveChangesAsync();

                // Log the activity
                var nhatKyThong = new NhatKyThong
                {
                    NoiDungHoatDong = $"Đã trả lời khiếu nại #{khieuNai.MaKhieuNai}",
                    ThoiGian = DateTime.Now,
                    NguoiThucHien = User.Identity?.Name ?? "Admin"
                };
                _context.NhatKyThongs.Add(nhatKyThong);
                await _context.SaveChangesAsync();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "Phản hồi đã được gửi thành công" });
                }

                TempData["SuccessMessage"] = "Bạn đã trả lời khiếu nại thành công.";
                return RedirectToAction("QuanLyKhieuNai");
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error in TraLoiKhieuNai: {ex}");
                
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Có lỗi xảy ra khi xử lý yêu cầu. Vui lòng thử lại sau." });
                }

                ModelState.AddModelError("", "Có lỗi xảy ra khi xử lý yêu cầu. Vui lòng thử lại sau.");
                return View(model);
            }
        }

        // GET: /CanBo/TraCuuCongDanByCC
        [HttpGet]
        public async Task<IActionResult> TraCuuCongDanByCC(string maTheCC)
        {
            if (string.IsNullOrEmpty(maTheCC))
            {
                return Json(new { error = "Vui lòng nhập mã thẻ căn cước công dân" });
            }

            try
            {
                var congDan = await _context.CongDans
                    .FirstOrDefaultAsync(c => c.MaTheCC == maTheCC);

                if (congDan == null)
                {
                    return Json(new { error = "Không tìm thấy công dân với mã CCCD này" });
                }

                return Json(new { 
                    congDan = new { 
                        maCongDan = congDan.MaCongDan,
                        tenCongDan = congDan.TenCongDan,
                        maTheCC = congDan.MaTheCC,
                        ngaySinh = congDan.NgaySinh,
                        diaChi = congDan.DiaChi,
                        gioiTinh = congDan.GioiTinh
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = "Đã xảy ra lỗi khi tìm kiếm: " + ex.Message });
            }
        }

        // GET: /CanBo/BaoCaoYeuCauCapNhatThongTin
        public async Task<IActionResult> BaoCaoYeuCauCapNhatThongTin(DateTime? startDate = null, DateTime? endDate = null)
        {
            // Set default date range if not provided
            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);
            
            if (!endDate.HasValue)
                endDate = DateTime.Now;

            // Ensure endDate includes the entire day
            endDate = endDate.Value.Date.AddDays(1).AddTicks(-1);

            try 
            {
                // Get all requests within date range
                var yeuCaus = await _context.YeuCauCapNhatThongTins
                    .Where(y => y.NgayYeuCau >= startDate && y.NgayYeuCau <= endDate)
                    .OrderByDescending(y => y.NgayYeuCau)
                    .ToListAsync();

                int totalRequests = yeuCaus.Count;
                int approvedRequests = yeuCaus.Count(y => y.TrangThai == "Đã duyệt");
                int rejectedRequests = yeuCaus.Count(y => y.TrangThai == "Từ chối");
                int pendingRequests = yeuCaus.Count(y => y.TrangThai == "Chờ xử lý");

                // Calculate approval and rejection rates
                double approvalRate = totalRequests > 0 ? Math.Round((double)approvedRequests / totalRequests * 100, 1) : 0;
                double rejectionRate = totalRequests > 0 ? Math.Round((double)rejectedRequests / totalRequests * 100, 1) : 0;

                // Group requests by date for trend data
                var dailyData = yeuCaus
                    .GroupBy(y => y.NgayYeuCau.Date)
                    .OrderBy(g => g.Key)
                    .Select(g => new
                    {
                        Date = g.Key,
                        Total = g.Count(),
                        Approved = g.Count(y => y.TrangThai == "Đã duyệt"),
                        Rejected = g.Count(y => y.TrangThai == "Từ chối"),
                        Pending = g.Count(y => y.TrangThai == "Chờ xử lý")
                    })
                    .ToList();

                // Prepare trend chart data
                var trendLabels = dailyData.Select(d => d.Date.ToString("dd/MM/yyyy")).ToList();
                var trendApproved = dailyData.Select(d => d.Approved).ToList();
                var trendRejected = dailyData.Select(d => d.Rejected).ToList();
                var trendPending = dailyData.Select(d => d.Pending).ToList();

                // Set ViewBag data for the view
                ViewBag.TongYeuCau = totalRequests;
                ViewBag.SoYeuCauDaDuyet = approvedRequests;
                ViewBag.SoYeuCauDaTuChoi = rejectedRequests;
                ViewBag.SoYeuCauChoXuLy = pendingRequests;
                ViewBag.TyLeChapThuan = approvalRate;
                ViewBag.TyLeTuChoi = rejectionRate;
                ViewBag.DailyReports = dailyData;

                // Chart data
                ViewBag.TrendLabels = trendLabels;
                ViewBag.TrendApproved = trendApproved;
                ViewBag.TrendRejected = trendRejected;
                ViewBag.TrendPending = trendPending;

                // Date range for the UI
                ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
                ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");

                return View();
            }
            catch (Exception ex)
            {
                // Log the error
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi tạo báo cáo: " + ex.Message;
                return View();
            }
        }

        // GET: /CanBo/ChiTietLoaiViPham/5
        [HttpGet]
        public async Task<IActionResult> ChiTietLoaiViPham(int id)
        {
            try
            {
                var loaiViPham = await _context.LoaiViPhams
                    .FirstOrDefaultAsync(lv => lv.MaLoaiVPham == id);

                if (loaiViPham == null)
                {
                    return Json(new { error = "Không tìm thấy luật vi phạm với mã này" });
                }

                return Json(new { 
                    loaiViPham = new {
                        maLoaiVPham = loaiViPham.MaLoaiVPham,
                        tenLoaiVPham = loaiViPham.TenLoaiVPham,
                        moTa = loaiViPham.MoTa,
                        coquanQuanLy = loaiViPham.CoquanQuanLy,
                        mucPhatToiThieu = loaiViPham.MucPhatToiThieu,
                        mucPhatToiDa = loaiViPham.MucPhatToiDa
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = "Đã xảy ra lỗi khi tải thông tin: " + ex.Message });
            }
        }

        // GET: /CanBo/QuanLyYeuCauCapNhatThongTin
        public async Task<IActionResult> QuanLyYeuCauCapNhatThongTin()
        {
            try
            {
                var yeuCaus = await _context.YeuCauCapNhatThongTins
                    .Include(y => y.CongDan)
                    .OrderByDescending(y => y.NgayYeuCau)
                    .ToListAsync();

                return View(yeuCaus);
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine(ex.ToString());
                ModelState.AddModelError("", "Có lỗi xảy ra khi tải dữ liệu. Vui lòng thử lại sau.");
                return View(new List<YeuCauCapNhatThongTin>());
            }
        }

        // GET: /CanBo/QuanLyBienBanThanhToan
        public async Task<IActionResult> QuanLyBienBanThanhToan(DateTime? tuNgay, DateTime? denNgay, string trangThaiThanhToan = "TatCa")
        {
            try
            {
                var bienBans = _context.BienBanVPhams
                    .Include(b => b.CongDan)
                    .Include(b => b.ViPham)
                    .AsQueryable();

                if (tuNgay.HasValue)
                    bienBans = bienBans.Where(b => b.ThoiGianVPham >= tuNgay.Value);

                if (denNgay.HasValue)
                    bienBans = bienBans.Where(b => b.ThoiGianVPham <= denNgay.Value);

                if (!string.IsNullOrEmpty(trangThaiThanhToan) && trangThaiThanhToan != "TatCa")
                {
                    if (trangThaiThanhToan == "DaThanhToan")
                        bienBans = bienBans.Where(b => b.KetQuaXuLy == "Đã thanh toán");
                    else if (trangThaiThanhToan == "ChuaThanhToan")
                        bienBans = bienBans.Where(b => b.KetQuaXuLy != "Đã thanh toán");
                }

                var viewModel = new QuanLyBienBanThanhToanViewModel
                {
                    TuNgay = tuNgay,
                    DenNgay = denNgay,
                    TrangThaiThanhToan = trangThaiThanhToan,
                    DanhSachBienBan = await bienBans.OrderByDescending(b => b.ThoiGianVPham).ToListAsync()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                ModelState.AddModelError("", "Có lỗi xảy ra khi tải dữ liệu. Vui lòng thử lại sau.");
                return View(new QuanLyBienBanThanhToanViewModel());
            }
        }

        // POST: /CanBo/CapNhatTrangThaiThanhToan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThaiThanhToan(int maBienBan, bool daThanhtoan)
        {
            try
            {
                var bienBan = await _context.BienBanVPhams.FindAsync(maBienBan);
                if (bienBan == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy biên bản với mã này.";
                    return RedirectToAction(nameof(QuanLyBienBanThanhToan));
                }

                // Cập nhật trạng thái thanh toán
                bienBan.KetQuaXuLy = daThanhtoan ? "Đã thanh toán" : "Chưa thanh toán";
                
                _context.Update(bienBan);
                await _context.SaveChangesAsync();

                // Ghi log hoạt động
                var nhatKyThong = new NhatKyThong
                {
                    NoiDungHoatDong = daThanhtoan 
                        ? $"Đánh dấu biên bản {bienBan.TenBienBan} (Mã: {bienBan.MaBienBan}) đã thanh toán"
                        : $"Đánh dấu biên bản {bienBan.TenBienBan} (Mã: {bienBan.MaBienBan}) chưa thanh toán",
                    ThoiGian = DateTime.Now,
                    NguoiThucHien = User.Identity?.Name ?? "Admin"
                };

                _context.NhatKyThongs.Add(nhatKyThong);
                await _context.SaveChangesAsync();

                // Gửi thông báo cho công dân nếu đã thanh toán
                if (daThanhtoan)
                {
                    int congDanId = bienBan.MaCongDan;
                    var congDan = await _context.CongDans.FindAsync(congDanId);
                    
                    if (congDan != null)
                    {
                        var thongBao = new ThongBao
                        {
                            MaCongDan = congDanId,
                            TieuDe = "Biên bản vi phạm đã được thanh toán",
                            NoiDung = $"Biên bản vi phạm {bienBan.TenBienBan} đã được xác nhận thanh toán thành công.",
                            NgayTao = DateTime.Now,
                            DaDoc = false,
                            LoaiThongBao = "ThanhToan",
                            UserId = congDan.MaNguoiDung
                        };

                        _context.ThongBaos.Add(thongBao);
                        await _context.SaveChangesAsync();
                    }
                }

                TempData["SuccessMessage"] = daThanhtoan 
                    ? "Đã đánh dấu biên bản thanh toán thành công." 
                    : "Đã đánh dấu biên bản chưa thanh toán.";
                    
                return RedirectToAction(nameof(QuanLyBienBanThanhToan));
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine(ex.ToString());
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi cập nhật trạng thái thanh toán: " + ex.Message;
                return RedirectToAction(nameof(QuanLyBienBanThanhToan));
            }
        }

        // XoaLoaiViPham method for the QuanLyLuatViPham functionality
        [HttpGet]
        public async Task<IActionResult> XoaLoaiViPham(int id)
        {
            try
            {
                var loaiViPham = await _context.LoaiViPhams.FindAsync(id);
                if (loaiViPham == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy loại vi phạm với mã này.";
                    return RedirectToAction(nameof(QuanLyLoaiViPham));
                }

                // Check if the violation type is in use
                bool isInUse = await _context.ViPhams.AnyAsync(v => v.MaLoaiVPham == id);
                if (isInUse)
                {
                    TempData["ErrorMessage"] = "Không thể xóa loại vi phạm này vì đang được sử dụng trong hệ thống.";
                    return RedirectToAction(nameof(QuanLyLoaiViPham));
                }

                _context.LoaiViPhams.Remove(loaiViPham);
                await _context.SaveChangesAsync();

                // Ghi log hoạt động
                var nhatKyThong = new NhatKyThong
                {
                    NoiDungHoatDong = $"Xóa loại vi phạm: {loaiViPham.TenLoaiVPham} (Mã: {loaiViPham.MaLoaiVPham})",
                    ThoiGian = DateTime.Now,
                    NguoiThucHien = User.Identity?.Name ?? "Admin"
                };

                _context.NhatKyThongs.Add(nhatKyThong);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đã xóa loại vi phạm thành công.";
                return RedirectToAction(nameof(QuanLyLoaiViPham));
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine(ex.ToString());
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xóa loại vi phạm: " + ex.Message;
                return RedirectToAction(nameof(QuanLyLoaiViPham));
            }
        }

        // ThemLoaiViPham method for the QuanLyLuatViPham functionality
        public IActionResult ThemLoaiViPham()
        {
            return View();
        }

        // POST: /CanBo/ThemLoaiViPham
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemLoaiViPham(LoaiViPham loaiViPham)
        {
            if (ModelState.IsValid)
            {
                _context.LoaiViPhams.Add(loaiViPham);
                await _context.SaveChangesAsync();

                // Ghi log hoạt động
                var nhatKyThong = new NhatKyThong
                {
                    NoiDungHoatDong = $"Thêm loại vi phạm mới: {loaiViPham.TenLoaiVPham}",
                    ThoiGian = DateTime.Now,
                    NguoiThucHien = User.Identity?.Name ?? "Admin"
                };

                _context.NhatKyThongs.Add(nhatKyThong);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đã thêm loại vi phạm thành công.";
                return RedirectToAction(nameof(QuanLyLoaiViPham));
            }
            return View(loaiViPham);
        }

        // SuaLoaiViPham method for the QuanLyLuatViPham functionality
        public async Task<IActionResult> SuaLoaiViPham(int id)
        {
            var loaiViPham = await _context.LoaiViPhams.FindAsync(id);
            if (loaiViPham == null)
            {
                return NotFound();
            }
            return View(loaiViPham);
        }

        // POST: /CanBo/SuaLoaiViPham
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaLoaiViPham(int id, LoaiViPham loaiViPham)
        {
            if (id != loaiViPham.MaLoaiVPham)
            {
                return NotFound();
            }

            // Kiểm tra mức phạt tối thiểu phải nhỏ hơn hoặc bằng mức phạt tối đa
            if (loaiViPham.MucPhatToiThieu > loaiViPham.MucPhatToiDa)
            {
                ModelState.AddModelError("MucPhatToiThieu", "Mức phạt tối thiểu phải nhỏ hơn hoặc bằng mức phạt tối đa.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loaiViPham);
                    await _context.SaveChangesAsync();

                    // Ghi log hoạt động
                    var nhatKyThong = new NhatKyThong
                    {
                        NoiDungHoatDong = $"Cập nhật loại vi phạm: {loaiViPham.TenLoaiVPham} (Mã: {loaiViPham.MaLoaiVPham})",
                        ThoiGian = DateTime.Now,
                        NguoiThucHien = User.Identity?.Name ?? "Admin"
                    };

                    _context.NhatKyThongs.Add(nhatKyThong);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Đã cập nhật loại vi phạm thành công.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.LoaiViPhams.Any(e => e.MaLoaiVPham == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(QuanLyLoaiViPham));
            }
            return View(loaiViPham);
        }

        // GET: /CanBo/ThemBienBanViPham
        public async Task<IActionResult> ThemBienBanViPham()
        {
            // Get list of violations for dropdown
            var viPhams = await _context.ViPhams
                .Include(v => v.LoaiViPham)
                .ToListAsync();
            
            ViewBag.ViPhams = new SelectList(viPhams, "MaViPham", "TenViPham");
            
            return View();
        }
        
        // POST: /CanBo/ThemBienBanViPham
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemBienBanViPham(BienBanVPham model)
        {
            if (ModelState.IsValid)
            {
                // Set default values
                model.NgayLapBienBan = DateTime.Now;
                model.NguoiLap = User.Identity?.Name ?? "Admin";
                
                _context.BienBanVPhams.Add(model);
                await _context.SaveChangesAsync();
                
                // Log the activity
                var nhatKyThong = new NhatKyThong
                {
                    NoiDungHoatDong = $"Tạo biên bản vi phạm mới: {model.TenBienBan}",
                    ThoiGian = DateTime.Now,
                    NguoiThucHien = User.Identity?.Name ?? "Admin"
                };
                
                _context.NhatKyThongs.Add(nhatKyThong);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Tạo biên bản vi phạm mới thành công.";
                return RedirectToAction(nameof(QuanLyBienBanViPham));
            }
            
            // If we get here, something failed, redisplay form
            var viPhams = await _context.ViPhams
                .Include(v => v.LoaiViPham)
                .ToListAsync();
            
            ViewBag.ViPhams = new SelectList(viPhams, "MaViPham", "TenViPham");
            
            return View(model);
        }
        
        // GET: /CanBo/ThongKeThanhToan
        public async Task<IActionResult> ThongKeThanhToan(DateTime? tuNgay = null, DateTime? denNgay = null, string loaiThongKe = "Ngay")
        {
            var viewModel = new ThongKeThanhToanViewModel
            {
                TuNgay = tuNgay ?? DateTime.Now.AddMonths(-1),
                DenNgay = denNgay ?? DateTime.Now,
                LoaiThongKe = loaiThongKe
            };
            
            // Ensure denNgay includes the entire day
            var endDate = viewModel.DenNgay.Value.Date.AddDays(1).AddTicks(-1);
            
            try
            {
                // Get all violation reports within date range
                var bienBans = await _context.BienBanVPhams
                    .Where(b => b.NgayLapBienBan >= viewModel.TuNgay && b.NgayLapBienBan <= endDate)
                    .ToListAsync();
                
                // Calculate summary statistics
                viewModel.SoBienBanDaThanhToan = bienBans.Count(b => b.KetQuaXuLy == "Đã thanh toán");
                viewModel.SoBienBanChuaThanhToan = bienBans.Count(b => b.KetQuaXuLy != "Đã thanh toán");
                viewModel.TongTienDaThanhToan = bienBans
                    .Where(b => b.KetQuaXuLy == "Đã thanh toán")
                    .Sum(b => b.SoTienPhat);
                viewModel.TongTienChuaThanhToan = bienBans
                    .Where(b => b.KetQuaXuLy != "Đã thanh toán")
                    .Sum(b => b.SoTienPhat);
                
                // Group by time period based on loaiThongKe
                var groupedData = new List<ThongKeTheoThoiGian>();
                
                switch (loaiThongKe)
                {
                    case "Ngay":
                        groupedData = bienBans
                            .GroupBy(b => b.NgayLapBienBan.Date)
                            .OrderBy(g => g.Key)
                            .Select(g => new ThongKeTheoThoiGian
                            {
                                ThoiGian = g.Key.ToString("dd/MM/yyyy"),
                                SoBienBanDaThanhToan = g.Count(b => b.KetQuaXuLy == "Đã thanh toán"),
                                SoBienBanChuaThanhToan = g.Count(b => b.KetQuaXuLy != "Đã thanh toán"),
                                TongTienDaThanhToan = g.Where(b => b.KetQuaXuLy == "Đã thanh toán").Sum(b => b.SoTienPhat),
                                TongTienChuaThanhToan = g.Where(b => b.KetQuaXuLy != "Đã thanh toán").Sum(b => b.SoTienPhat)
                            })
                            .ToList();
                        break;
                    
                    case "Thang":
                        groupedData = bienBans
                            .GroupBy(b => new { b.NgayLapBienBan.Year, b.NgayLapBienBan.Month })
                            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                            .Select(g => new ThongKeTheoThoiGian
                            {
                                ThoiGian = $"{g.Key.Month}/{g.Key.Year}",
                                SoBienBanDaThanhToan = g.Count(b => b.KetQuaXuLy == "Đã thanh toán"),
                                SoBienBanChuaThanhToan = g.Count(b => b.KetQuaXuLy != "Đã thanh toán"),
                                TongTienDaThanhToan = g.Where(b => b.KetQuaXuLy == "Đã thanh toán").Sum(b => b.SoTienPhat),
                                TongTienChuaThanhToan = g.Where(b => b.KetQuaXuLy != "Đã thanh toán").Sum(b => b.SoTienPhat)
                            })
                            .ToList();
                        break;
                    
                    case "Nam":
                        groupedData = bienBans
                            .GroupBy(b => b.NgayLapBienBan.Year)
                            .OrderBy(g => g.Key)
                            .Select(g => new ThongKeTheoThoiGian
                            {
                                ThoiGian = g.Key.ToString(),
                                SoBienBanDaThanhToan = g.Count(b => b.KetQuaXuLy == "Đã thanh toán"),
                                SoBienBanChuaThanhToan = g.Count(b => b.KetQuaXuLy != "Đã thanh toán"),
                                TongTienDaThanhToan = g.Where(b => b.KetQuaXuLy == "Đã thanh toán").Sum(b => b.SoTienPhat),
                                TongTienChuaThanhToan = g.Where(b => b.KetQuaXuLy != "Đã thanh toán").Sum(b => b.SoTienPhat)
                            })
                            .ToList();
                        break;
                }
                
                viewModel.ThongKeTheoThoiGian = groupedData;
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine(ex.ToString());
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi tạo báo cáo: " + ex.Message;
                return View(viewModel);
            }
        }

        // GET: /CanBo/QuanLyLuatViPham
        public IActionResult QuanLyLuatViPham()
        {
            // Redirect to QuanLyLoaiViPham as that's the correct method name 
            // But we want to maintain compatibility with the view that uses QuanLyLuatViPham URL
            return RedirectToAction(nameof(QuanLyLoaiViPham));
        }

        // GET: /CanBo/XuLyThanhToanTrucTiep/5
        public async Task<IActionResult> XuLyThanhToanTrucTiep(int id)
        {
            var bienBan = await _context.BienBanVPhams
                .Include(b => b.ViPham)
                .Include(b => b.CongDan)
                .FirstOrDefaultAsync(b => b.MaBienBan == id);

            if (bienBan == null)
            {
                return NotFound();
            }

            return View(bienBan);
        }

        // POST: /CanBo/XuLyThanhToanTrucTiep/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XuLyThanhToanTrucTiep(int id, string ghiChu)
        {
            var bienBan = await _context.BienBanVPhams.FindAsync(id);
            if (bienBan == null)
            {
                return NotFound();
            }

            try
            {
                // Kiểm tra xem đã có lịch sử thanh toán chưa
                var existingPayment = await _context.LichSuThanhToans
                    .FirstOrDefaultAsync(l => l.MaBienBan == id);
                
                if (existingPayment != null)
                {
                    TempData["ErrorMessage"] = $"Biên bản này đã được thanh toán trước đó vào ngày {existingPayment.NgayThanhToan:dd/MM/yyyy}.";
                    return RedirectToAction(nameof(QuanLyBienBanThanhToan));
                }
                
                // Tạo bản ghi lịch sử thanh toán mới
                var lichSuThanhToan = new LichSuThanhToan
                {
                    MaBienBan = bienBan.MaBienBan,
                    SoTien = bienBan.SoTienPhat,
                    PhuongThucThanhToan = "Thanh toán trực tiếp",
                    NgayThanhToan = DateTime.Now,
                    MaGiaoDich = "TRUC-TIEP-" + DateTime.Now.ToString("yyyyMMddHHmmss")
                };
                
                _context.LichSuThanhToans.Add(lichSuThanhToan);
                
                // Cập nhật thông tin biên bản
                bienBan.KetQuaXuLy = "Đã thanh toán";
                bienBan.NgayThanhToan = DateTime.Now;
                bienBan.NgayLadDuBienBan = DateTime.Now;
                
                _context.Update(bienBan);
                await _context.SaveChangesAsync();

                // Tạo thông báo cho công dân
                var thongBao = new ThongBao
                {
                    MaCongDan = bienBan.MaCongDan,
                    TieuDe = "Thanh toán biên bản vi phạm thành công",
                    NoiDung = $"Biên bản vi phạm {bienBan.TenBienBan} đã được thanh toán thành công với số tiền {bienBan.SoTienPhat:N0} VNĐ.",
                    NgayTao = DateTime.Now,
                    DaDoc = false,
                    LoaiThongBao = "ThanhToan",
                    UserId = _context.CongDans.FirstOrDefault(c => c.MaCongDan == bienBan.MaCongDan)?.MaNguoiDung
                };
                
                _context.ThongBaos.Add(thongBao);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Đã xử lý thanh toán thành công cho biên bản.";
                return RedirectToAction(nameof(QuanLyBienBanThanhToan));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi xử lý thanh toán: {ex.Message}";
                return RedirectToAction(nameof(QuanLyBienBanThanhToan));
            }
        }

        // GET: /CanBo/TraCuuCongDan
        public IActionResult TraCuuCongDan()
        {
            return View(new List<CongDan>());
        }

        // POST: /CanBo/TraCuuCongDan
        [HttpPost]
        public async Task<IActionResult> TraCuuCongDan(string searchType, string searchTerm)
        {
            ViewBag.SearchType = searchType;
            ViewBag.SearchTerm = searchTerm;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return View(new List<CongDan>());
            }

            IQueryable<CongDan> query = _context.CongDans;

            switch (searchType)
            {
                case "MaCongDan":
                    if (int.TryParse(searchTerm, out int maCongDan))
                    {
                        query = query.Where(c => c.MaCongDan == maCongDan);
                    }
                    break;
                case "TenCongDan":
                    query = query.Where(c => c.TenCongDan.Contains(searchTerm));
                    break;
                case "MaTheCC":
                    query = query.Where(c => c.MaTheCC.Contains(searchTerm));
                    break;
                case "DiaChi":
                    query = query.Where(c => c.DiaChi.Contains(searchTerm));
                    break;
                case "SDT":
                    query = query.Where(c => c.SDT.Contains(searchTerm));
                    break;
                case "GMAIL":
                    query = query.Where(c => c.GMAIL.Contains(searchTerm));
                    break;
                case "GIOTTING":
                    query = query.Where(c => c.GioiTinh.Contains(searchTerm));
                    break;
                case "CONGVIEC":
                    query = query.Where(c => c.CONGVIEC.Contains(searchTerm));
                    break;
                case "GiayKhac":
                    query = query.Where(c => c.GiayKhac.Contains(searchTerm));
                    break;
                case "QuocTich":
                    query = query.Where(c => c.QuocTich.Contains(searchTerm));
                    break;
                case "DanToc":
                    query = query.Where(c => c.DanToc.Contains(searchTerm));
                    break;
                case "TonGiao":
                    query = query.Where(c => c.TonGiao.Contains(searchTerm));
                    break;
                case "QueQuan":
                    query = query.Where(c => c.QueQuan.Contains(searchTerm));
                    break;
                case "NoiThuongTru":
                    query = query.Where(c => c.NoiThuongTru.Contains(searchTerm));
                    break;
                case "DacDiemNhanDang":
                    query = query.Where(c => c.DacDiemNhanDang.Contains(searchTerm));
                    break;
                case "NgaySinh":
                    if (DateTime.TryParseExact(searchTerm, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime ngaySinh))
                    {
                        query = query.Where(c => c.NgaySinh.Date == ngaySinh.Date);
                    }
                    break;
                case "All":
                default:
                    query = query.Where(c => 
                        c.TenCongDan.Contains(searchTerm) || 
                        c.MaTheCC.Contains(searchTerm) ||
                        c.DiaChi.Contains(searchTerm) ||
                        c.SDT.Contains(searchTerm) ||
                        c.GMAIL.Contains(searchTerm) ||
                        c.GioiTinh.Contains(searchTerm) ||
                        c.QuocTich.Contains(searchTerm) ||
                        c.DanToc.Contains(searchTerm) ||
                        c.CONGVIEC.Contains(searchTerm)
                    );
                    break;
            }

            var results = await query.ToListAsync();
            return View(results);
        }

        [HttpGet]
        [Authorize(Roles = "CanBo")]
        public IActionResult TaoCongDan()
        {
            var viewModel = new TaoCongDanViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "CanBo")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoCongDan(TaoCongDanViewModel model)
        {
            if (ModelState.IsValid)
            {
                try 
                {
                    // Get the user manager and role manager from services
                    var userManager = HttpContext.RequestServices.GetService(typeof(UserManager<ApplicationUser>)) as UserManager<ApplicationUser>;
                    var roleManager = HttpContext.RequestServices.GetService(typeof(RoleManager<IdentityRole>)) as RoleManager<IdentityRole>;
                    
                    if (userManager == null || roleManager == null)
                    {
                        ModelState.AddModelError(string.Empty, "Không thể khởi tạo dịch vụ quản lý người dùng");
                        Console.WriteLine("Error: User manager or role manager is null");
                        return View(model);
                    }
                    
                    // Check if username already exists
                    var existingUser = await userManager.FindByNameAsync(model.UserName);
                    if (existingUser != null)
                    {
                        ModelState.AddModelError(string.Empty, "Tên đăng nhập đã tồn tại trong hệ thống");
                        return View(model);
                    }
                    
                    // Check if email already exists
                    var existingEmail = await userManager.FindByEmailAsync(model.Email);
                    if (existingEmail != null)
                    {
                        ModelState.AddModelError(string.Empty, "Email đã tồn tại trong hệ thống");
                        return View(model);
                    }
                    
                    // Create the user account
                    var user = new ApplicationUser { 
                        UserName = model.UserName, 
                        Email = model.Email,
                        HoTen = model.HoTen,
                        PhoneNumber = model.PhoneNumber ?? string.Empty,
                        EmailConfirmed = true // Set email as confirmed since this is created by an officer
                    };
                    
                    Console.WriteLine($"Creating user with username: {model.UserName}, email: {model.Email}");
                    
                    var result = await userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        Console.WriteLine($"User created successfully with ID: {user.Id}");
                        
                        // Create the CongDan role if it doesn't exist
                        if (!await roleManager.RoleExistsAsync("CongDan"))
                        {
                            Console.WriteLine("Creating CongDan role as it doesn't exist");
                            await roleManager.CreateAsync(new IdentityRole("CongDan"));
                        }
                        
                        // Assign the CongDan role
                        Console.WriteLine($"Assigning CongDan role to user {user.UserName}");
                        var roleResult = await userManager.AddToRoleAsync(user, "CongDan");
                        
                        if (!roleResult.Succeeded)
                        {
                            Console.WriteLine($"Failed to assign CongDan role. Errors: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                        }
                        else
                        {
                            Console.WriteLine("CongDan role assigned successfully");
                        }
                        
                        // Create a CongDan record
                        var congDan = new CongDan
                        {
                            TenCongDan = model.HoTen,
                            DiaChi = model.DiaChi,
                            SDT = model.PhoneNumber,
                            GMAIL = model.Email,
                            GIOTTING = model.GioiTinh,
                            CONGVIEC = model.CONGVIEC,
                            QuocTich = model.QuocTich,
                            NgaySinh = model.NgaySinh ?? DateTime.Now,
                            DanToc = model.DanToc,
                            TonGiao = model.TonGiao,
                            QueQuan = model.QueQuan,
                            NoiThuongTru = model.NoiThuongTru,
                            MaNguoiDung = user.Id
                        };
                        
                        _context.CongDans.Add(congDan);
                        await _context.SaveChangesAsync();
                        
                        Console.WriteLine($"CongDan record created with ID: {congDan.MaCongDan}");
                        
                        // Verify the role was assigned
                        var userRoles = await userManager.GetRolesAsync(user);
                        Console.WriteLine($"User roles after assignment: {string.Join(", ", userRoles)}");
                        
                        TempData["SuccessMessage"] = "Tạo tài khoản công dân thành công!";
                        return RedirectToAction("TraCuuCongDan");
                    }
                    
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Error creating user: {error.Description}");
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception when creating citizen account: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    ModelState.AddModelError(string.Empty, $"Lỗi hệ thống: {ex.Message}");
                }
            }
            else
            {
                // Log validation errors
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"Validation error: {error.ErrorMessage}");
                    }
                }
            }
            
            return View(model);
        }

        // GET: /CanBo/ChiTietCongDan/5
        public async Task<IActionResult> ChiTietCongDan(int id)
        {
            var congDan = await _context.CongDans
                .FirstOrDefaultAsync(c => c.MaCongDan == id);

            if (congDan == null)
            {
                return NotFound();
            }

            // Get user details if available
            if (!string.IsNullOrEmpty(congDan.MaNguoiDung))
            {
                var user = await _userManager.FindByIdAsync(congDan.MaNguoiDung);
                if (user != null)
                {
                    ViewBag.UserName = user.UserName;
                    ViewBag.Email = user.Email;
                    ViewBag.PhoneNumber = user.PhoneNumber;
                }
            }

            // Get biên bản vi phạm associated with this công dân
            var bienBanViPhams = await _context.BienBanVPhams
                .Include(b => b.ViPham)
                .ThenInclude(v => v.LoaiViPham)
                .Where(b => b.MaCongDan == id)
                .OrderByDescending(b => b.NgayLapBienBan)
                .ToListAsync();
            ViewBag.BienBanViPhams = bienBanViPhams;
            ViewBag.TongViPham = bienBanViPhams.Count;

            // Get khiếu nại associated with this công dân
            var khieuNais = await _context.KhieuNaiVPhams
                .Where(k => k.MaCongDan == id)
                .OrderByDescending(k => k.NgayKhieuNai)
                .ToListAsync();
            ViewBag.KhieuNais = khieuNais;
            ViewBag.TongKhieuNai = khieuNais.Count;

            // Get yêu cầu cập nhật associated with this công dân
            var yeuCauCapNhats = await _context.YeuCauCapNhatThongTins
                .Where(y => y.MaCongDan == id)
                .OrderByDescending(y => y.NgayYeuCau)
                .ToListAsync();
            ViewBag.YeuCauCapNhats = yeuCauCapNhats;
            ViewBag.TongYeuCau = yeuCauCapNhats.Count;

            return View(congDan);
        }

        // GET: /CanBo/ChinhSuaCongDan/5
        public async Task<IActionResult> ChinhSuaCongDan(int id)
        {
            var congDan = await _context.CongDans
                .FirstOrDefaultAsync(c => c.MaCongDan == id);

            if (congDan == null)
            {
                return NotFound();
            }

            var viewModel = new ChinhSuaCongDanViewModel
            {
                MaCongDan = congDan.MaCongDan,
                TenCongDan = congDan.TenCongDan,
                DiaChi = congDan.DiaChi,
                SDT = congDan.SDT,
                GMAIL = congDan.GMAIL,
                GIOTTING = congDan.GIOTTING,
                CONGVIEC = congDan.CONGVIEC,
                QuocTich = congDan.QuocTich,
                NgaySinh = congDan.NgaySinh,
                DanToc = congDan.DanToc,
                TonGiao = congDan.TonGiao,
                QueQuan = congDan.QueQuan,
                NoiThuongTru = congDan.NoiThuongTru,
                MaTheCC = congDan.MaTheCC,
                DacDiemNhanDang = congDan.DacDiemNhanDang,
                GiayKhac = congDan.GiayKhac,
                AnhChanDung = congDan.AnhChanDung
            };

            // Get user details if available
            if (!string.IsNullOrEmpty(congDan.MaNguoiDung))
            {
                var user = await _userManager.FindByIdAsync(congDan.MaNguoiDung);
                if (user != null)
                {
                    viewModel.UserName = user.UserName;
                    viewModel.Email = user.Email;
                    viewModel.PhoneNumber = user.PhoneNumber;
                }
            }

            return View(viewModel);
        }

        // POST: /CanBo/ChinhSuaCongDan/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChinhSuaCongDan(ChinhSuaCongDanViewModel model, IFormFile profilePicture)
        {
            Console.WriteLine("ChinhSuaCongDan POST started");
            Console.WriteLine($"Has profile picture: {profilePicture != null}, Model valid: {ModelState.IsValid}");
            
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    if (state.Value.Errors.Count > 0)
                    {
                        Console.WriteLine($"Error in {state.Key}: {state.Value.Errors[0].ErrorMessage}");
                    }
                }
                return View(model);
            }

            var congDan = await _context.CongDans
                .FirstOrDefaultAsync(c => c.MaCongDan == model.MaCongDan);

            if (congDan == null)
            {
                Console.WriteLine($"Citizen with ID {model.MaCongDan} not found");
                return NotFound();
            }

            // Handle profile picture upload
            if (profilePicture != null && profilePicture.Length > 0)
            {
                Console.WriteLine($"Processing image: {profilePicture.FileName}, Size: {profilePicture.Length / 1024} KB");
                
                // Check file size (limit to 5MB)
                if (profilePicture.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("profilePicture", "Kích thước ảnh không được vượt quá 5MB");
                    return View(model);
                }

                // Check file extension
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(profilePicture.FileName).ToLowerInvariant();
                
                Console.WriteLine($"File extension: {fileExtension}");
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("profilePicture", "Chỉ chấp nhận định dạng ảnh JPG, JPEG hoặc PNG");
                    return View(model);
                }

                try
                {
                    // Convert image to base64 string for storage
                    using (var memoryStream = new MemoryStream())
                    {
                        await profilePicture.CopyToAsync(memoryStream);
                        byte[] imageBytes = memoryStream.ToArray();
                        string base64String = Convert.ToBase64String(imageBytes);
                        
                        // Store just the base64 string without the data URI prefix, since it's added in the view
                        congDan.AnhChanDung = base64String;
                        
                        Console.WriteLine($"Image processed successfully: {profilePicture.FileName}, Size: {profilePicture.Length} bytes, Base64 length: {base64String.Length}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing image: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    ModelState.AddModelError("profilePicture", $"Lỗi khi xử lý ảnh: {ex.Message}");
                    return View(model);
                }
            }
            else if (model.AnhChanDung != null)
            {
                // Keep existing image if no new one is uploaded
                congDan.AnhChanDung = model.AnhChanDung;
                Console.WriteLine("Keeping existing image");
            }
            else
            {
                Console.WriteLine("No profile picture provided and no existing image in model");
            }

            // Update citizen information
            congDan.TenCongDan = model.TenCongDan;
            congDan.DiaChi = model.DiaChi;
            congDan.SDT = model.SDT;
            congDan.GMAIL = model.GMAIL;
            congDan.GIOTTING = model.GIOTTING;
            congDan.CONGVIEC = model.CONGVIEC;
            congDan.QuocTich = model.QuocTich;
            congDan.NgaySinh = model.NgaySinh;
            congDan.DanToc = model.DanToc;
            congDan.TonGiao = model.TonGiao;
            congDan.QueQuan = model.QueQuan;
            congDan.NoiThuongTru = model.NoiThuongTru;
            congDan.MaTheCC = model.MaTheCC;
            
            // Validate and sanitize DacDiemNhanDang field
            if (model.DacDiemNhanDang != null)
            {
                // Trim and sanitize the text
                string sanitizedText = model.DacDiemNhanDang.Trim();
                
                // Ensure it doesn't exceed the maximum length
                if (sanitizedText.Length > 500)
                {
                    sanitizedText = sanitizedText.Substring(0, 500);
                    Console.WriteLine("DacDiemNhanDang truncated to 500 characters");
                }
                
                // Log the final text
                Console.WriteLine($"DacDiemNhanDang final length: {sanitizedText.Length}");
                
                // Save the sanitized text
                congDan.DacDiemNhanDang = sanitizedText;
            }
            else
            {
                congDan.DacDiemNhanDang = null;
                Console.WriteLine("DacDiemNhanDang set to null");
            }
            
            congDan.GiayKhac = model.GiayKhac;

            // Update related user if exists
            if (!string.IsNullOrEmpty(congDan.MaNguoiDung))
            {
                var user = await _userManager.FindByIdAsync(congDan.MaNguoiDung);
                if (user != null)
                {
                    // Update user information
                    user.Email = model.Email;
                    user.PhoneNumber = model.SoDienThoai;
                    user.HoTen = model.TenCongDan;

                    // Update username if changed and not empty
                    if (!string.IsNullOrEmpty(model.UserName) && user.UserName != model.UserName)
                    {
                        // Check if username is available
                        var existingUser = await _userManager.FindByNameAsync(model.UserName);
                        if (existingUser != null && existingUser.Id != user.Id)
                        {
                            ModelState.AddModelError("UserName", "Tên đăng nhập đã tồn tại");
                            return View(model);
                        }
                        
                        user.UserName = model.UserName;
                    }

                    var result = await _userManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(model);
                    }
                }
            }

            try
            {
                _context.Update(congDan);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Successfully updated citizen ID: {congDan.MaCongDan}, Image length: {congDan.AnhChanDung?.Length ?? 0}");
                TempData["SuccessMessage"] = "Cập nhật thông tin công dân thành công";
                return RedirectToAction(nameof(ChiTietCongDan), new { id = congDan.MaCongDan });
            }
            catch (DbUpdateException dbEx)
            {
                // Log the detailed database exception
                Console.WriteLine($"Database error saving citizen: {dbEx.Message}");
                Console.WriteLine($"Inner Exception: {dbEx.InnerException?.Message}");
                
                // Check if it's related to the DacDiemNhanDang field
                string error = dbEx.InnerException?.Message ?? dbEx.Message;
                if (error.Contains("DacDiemNhanDang") || error.Contains("column name"))
                {
                    ModelState.AddModelError("DacDiemNhanDang", "Lỗi khi lưu đặc điểm nhận dạng: " + error);
                    Console.WriteLine("Error appears to be related to DacDiemNhanDang field");
                    
                    // Reset the field to ensure we can save
                    model.DacDiemNhanDang = "";
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Lỗi khi cập nhật database: " + error);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to database: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                ModelState.AddModelError(string.Empty, "Lỗi khi cập nhật: " + ex.Message);
                return View(model);
            }
        }

        // GET: /CanBo/SuaCongDan/5 - Redirects to ChinhSuaCongDan
        public IActionResult SuaCongDan(int id)
        {
            // Redirect to the correct action
            return RedirectToAction(nameof(ChinhSuaCongDan), new { id = id });
        }
        
        // POST: /CanBo/UploadProfileImage - Dedicated endpoint for image uploads
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProfileImage(int maCongDan, IFormFile profileImage)
        {
            Console.WriteLine($"UploadProfileImage called for citizen ID: {maCongDan}");
            
            if (profileImage == null || profileImage.Length == 0)
            {
                Console.WriteLine("No file was uploaded");
                return Json(new { success = false, message = "Không có file nào được tải lên" });
            }
            
            Console.WriteLine($"Received file: {profileImage.FileName}, Size: {profileImage.Length / 1024} KB");
            
            var congDan = await _context.CongDans.FindAsync(maCongDan);
            if (congDan == null)
            {
                Console.WriteLine($"Citizen with ID {maCongDan} not found");
                return Json(new { success = false, message = "Không tìm thấy công dân với mã này" });
            }
            
            try
            {
                // Check file size (limit to 5MB)
                if (profileImage.Length > 5 * 1024 * 1024)
                {
                    Console.WriteLine("File too large");
                    return Json(new { success = false, message = "Kích thước ảnh không được vượt quá 5MB" });
                }

                // Check file extension
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(profileImage.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    Console.WriteLine("Invalid file extension");
                    return Json(new { success = false, message = "Chỉ chấp nhận định dạng ảnh JPG, JPEG hoặc PNG" });
                }

                // Convert image to base64 string for storage
                using (var memoryStream = new MemoryStream())
                {
                    await profileImage.CopyToAsync(memoryStream);
                    byte[] imageBytes = memoryStream.ToArray();
                    string base64String = Convert.ToBase64String(imageBytes);
                    
                    // Store just the base64 string without the data URI prefix
                    congDan.AnhChanDung = base64String;
                    
                    Console.WriteLine($"Image processed successfully, Base64 length: {base64String.Length}");
                }
                
                _context.Update(congDan);
                await _context.SaveChangesAsync();
                
                Console.WriteLine("Image saved to database successfully");
                return Json(new { 
                    success = true, 
                    message = "Ảnh đã được cập nhật thành công",
                    redirectUrl = Url.Action("ChiTietCongDan", new { id = maCongDan })
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading image: {ex.Message}");
                return Json(new { success = false, message = $"Lỗi khi tải lên ảnh: {ex.Message}" });
            }
        }

        // GET: /CanBo/QuanLyTheCanCuoc
        public async Task<IActionResult> QuanLyTheCanCuoc(string search, string trangThai, string sortOrder, int? pageNumber)
        {
            var theCanCuocQuery = _context.TheCanCuocs.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                theCanCuocQuery = theCanCuocQuery.Where(t => 
                    t.MaTheCC.Contains(search) || 
                    t.TenCC.Contains(search));
            }

            // Apply status filter
            if (!string.IsNullOrEmpty(trangThai))
            {
                theCanCuocQuery = theCanCuocQuery.Where(t => t.TrangThai == trangThai);
            }

            // Apply sorting
            theCanCuocQuery = sortOrder switch
            {
                "maTheCC" => theCanCuocQuery.OrderBy(t => t.MaTheCC),
                "maTheCC_desc" => theCanCuocQuery.OrderByDescending(t => t.MaTheCC),
                "tenCC" => theCanCuocQuery.OrderBy(t => t.TenCC),
                "tenCC_desc" => theCanCuocQuery.OrderByDescending(t => t.TenCC),
                "ngayCap" => theCanCuocQuery.OrderBy(t => t.NgayCap),
                "ngayCap_desc" => theCanCuocQuery.OrderByDescending(t => t.NgayCap),
                _ => theCanCuocQuery.OrderBy(t => t.MaTheCC)
            };

            int pageSize = 10;
            int pageNumberValue = pageNumber ?? 1;

            var paginatedList = await PaginatedList<TheCanCuoc>.CreateAsync(
                theCanCuocQuery, pageNumberValue, pageSize);

            return View(paginatedList);
        }

        // GET: /CanBo/TaoTheCanCuoc
        public async Task<IActionResult> TaoTheCanCuoc(int? maCongDan)
        {
            if (!maCongDan.HasValue)
            {
                // Redirect to citizen management page if no maCongDan is provided
                return RedirectToAction("QuanLyCongDan");
            }

            var congDan = await _context.CongDans
                .FirstOrDefaultAsync(c => c.MaCongDan == maCongDan);
            
            if (congDan == null)
            {
                return NotFound();
            }
            
            // Check if ID card already exists
            if (!string.IsNullOrEmpty(congDan.MaTheCC))
            {
                var existingCard = await _context.TheCanCuocs
                    .FirstOrDefaultAsync(t => t.MaTheCC == congDan.MaTheCC);
                
                if (existingCard != null)
                {
                    // Redirect to edit page if card exists
                    return RedirectToAction("ChinhSuaTheCanCuoc", new { maTheCC = existingCard.MaTheCC });
                }
            }
            
            // Create a new ID card with prefilled info
            var theCanCuoc = new TheCanCuoc
            {
                NgayCap = DateTime.Now,
                NgayHetHan = DateTime.Now.AddYears(10),
                NoiCap = "Cục Cảnh sát quản lý hành chính về trật tự xã hội",
                TenCC = "Căn cước công dân"
            };
            
            ViewBag.CongDan = congDan;
            return View(theCanCuoc);
        }

        // POST: /CanBo/TaoTheCanCuoc
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoTheCanCuoc(int maCongDan, TheCanCuoc theCanCuoc, IFormFile anhMatTruoc, IFormFile anhMatSau)
        {
            var errorMessages = new List<string>();
            if (!ModelState.IsValid)
            {
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errorMessages.Add(error.ErrorMessage);
                    }
                }
            }
            if (string.IsNullOrWhiteSpace(theCanCuoc.MaTheCC))
                errorMessages.Add("Mã thẻ căn cước không được để trống.");
            if (string.IsNullOrWhiteSpace(theCanCuoc.TenCC))
                errorMessages.Add("Tên loại thẻ không được để trống.");
            if (!theCanCuoc.NgayCap.HasValue)
                errorMessages.Add("Ngày cấp không được để trống.");
            if (!theCanCuoc.NgayHetHan.HasValue)
                errorMessages.Add("Ngày hết hạn không được để trống.");
            if (string.IsNullOrWhiteSpace(theCanCuoc.NoiCap))
                errorMessages.Add("Nơi cấp không được để trống.");
            if (string.IsNullOrWhiteSpace(theCanCuoc.QuocTich))
                errorMessages.Add("Quốc tịch không được để trống.");
            if (string.IsNullOrWhiteSpace(theCanCuoc.TrangThai))
                errorMessages.Add("Trạng thái không được để trống.");
            if (anhMatTruoc == null || anhMatTruoc.Length == 0)
                errorMessages.Add("Vui lòng chọn ảnh mặt trước.");
            if (anhMatSau == null || anhMatSau.Length == 0)
                errorMessages.Add("Vui lòng chọn ảnh mặt sau.");

            var existingCongDan = await _context.CongDans.FirstOrDefaultAsync(c => c.MaCongDan == maCongDan);
            if (existingCongDan == null)
                errorMessages.Add("Không tìm thấy công dân để cấp thẻ.");

            var existingCard = await _context.TheCanCuocs.FirstOrDefaultAsync(t => t.MaTheCC == theCanCuoc.MaTheCC);
            if (existingCard != null)
                errorMessages.Add("Mã thẻ căn cước này đã tồn tại trong hệ thống.");

            if (errorMessages.Count > 0)
            {
                TempData["ErrorMessage"] = string.Join("<br>", errorMessages);
                ViewBag.CongDan = existingCongDan;
                return View(theCanCuoc);
            }

            // Xử lý upload ảnh mặt trước
            if (anhMatTruoc != null && anhMatTruoc.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "thecancouc");
                Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = $"{theCanCuoc.MaTheCC}_front_{Guid.NewGuid().ToString().Substring(0, 8)}{Path.GetExtension(anhMatTruoc.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await anhMatTruoc.CopyToAsync(fileStream);
                }
                theCanCuoc.AnhMatTruoc = $"/uploads/thecancouc/{uniqueFileName}";
            }
            // Xử lý upload ảnh mặt sau
            if (anhMatSau != null && anhMatSau.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "thecancouc");
                Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = $"{theCanCuoc.MaTheCC}_back_{Guid.NewGuid().ToString().Substring(0, 8)}{Path.GetExtension(anhMatSau.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await anhMatSau.CopyToAsync(fileStream);
                }
                theCanCuoc.AnhMatSau = $"/uploads/thecancouc/{uniqueFileName}";
            }

            // Thêm thẻ căn cước mới
            _context.TheCanCuocs.Add(theCanCuoc);
            // Gán mã thẻ cho công dân
            existingCongDan.MaTheCC = theCanCuoc.MaTheCC;
            _context.CongDans.Update(existingCongDan);

            // Gửi thông báo
            var thongBao = new ThongBao
            {
                MaCongDan = existingCongDan.MaCongDan,
                TieuDe = "Đã cấp thẻ căn cước mới",
                NoiDung = $"Bạn đã được cấp thẻ căn cước công dân mới với mã số {theCanCuoc.MaTheCC}",
                NgayTao = DateTime.Now,
                DaDoc = false,
                LoaiThongBao = "CCCD",
                UserId = existingCongDan.MaNguoiDung
            };
            _context.ThongBaos.Add(thongBao);

            // Nhật ký hoạt động
            var nhatKyThong = new NhatKyThong
            {
                NoiDungHoatDong = $"Cấp thẻ căn cước mới có mã {theCanCuoc.MaTheCC} cho công dân {existingCongDan.TenCongDan}",
                ThoiGian = DateTime.Now,
                NguoiThucHien = User.Identity?.Name ?? "Admin"
            };
            _context.NhatKyThongs.Add(nhatKyThong);

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã cấp thẻ căn cước mới thành công!";
            return RedirectToAction("ChiTietCongDan", new { id = maCongDan });
        }

        // GET: /CanBo/ChinhSuaTheCanCuoc/{maTheCC}
        public async Task<IActionResult> ChinhSuaTheCanCuoc(string maTheCC)
        {
            if (string.IsNullOrEmpty(maTheCC))
            {
                return BadRequest();
            }
            
            var theCanCuoc = await _context.Set<TheCanCuoc>().FindAsync(maTheCC);
                
            if (theCanCuoc == null)
            {
                return NotFound();
            }
            
            var congDan = await _context.CongDans
                .FirstOrDefaultAsync(c => c.MaTheCC == maTheCC);
            
            ViewBag.CongDan = congDan;
            return View(theCanCuoc);
        }

        // POST: /CanBo/ChinhSuaTheCanCuoc/{maTheCC}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChinhSuaTheCanCuoc(string maTheCC, TheCanCuoc theCanCuoc, IFormFile AnhMatTruoc, IFormFile AnhMatSau)
        {
            if (maTheCC != theCanCuoc.MaTheCC)
            {
                return BadRequest();
            }
            
            if (!ModelState.IsValid)
            {
                var citizenForView = await _context.CongDans
                    .FirstOrDefaultAsync(c => c.MaTheCC == maTheCC);
                ViewBag.CongDan = citizenForView;
                return View(theCanCuoc);
            }

            // Xử lý upload ảnh mặt trước
            if (AnhMatTruoc != null && AnhMatTruoc.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "thecancuoc");
                Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = $"{theCanCuoc.MaTheCC}_front_{Guid.NewGuid().ToString().Substring(0, 8)}{Path.GetExtension(AnhMatTruoc.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await AnhMatTruoc.CopyToAsync(fileStream);
                }
                theCanCuoc.AnhMatTruoc = $"/uploads/thecancuoc/{uniqueFileName}";
            }

            // Xử lý upload ảnh mặt sau
            if (AnhMatSau != null && AnhMatSau.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "thecancuoc");
                Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = $"{theCanCuoc.MaTheCC}_back_{Guid.NewGuid().ToString().Substring(0, 8)}{Path.GetExtension(AnhMatSau.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await AnhMatSau.CopyToAsync(fileStream);
                }
                theCanCuoc.AnhMatSau = $"/uploads/thecancuoc/{uniqueFileName}";
            }

            try
            {
                _context.Update(theCanCuoc);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TheCanCuocExists(theCanCuoc.MaTheCC))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            
            // Redirect to citizen details
            var citizenWithCard = await _context.CongDans
                .FirstOrDefaultAsync(c => c.MaTheCC == maTheCC);
                
            if (citizenWithCard != null)
            {
                return RedirectToAction("ChiTietCongDan", new { id = citizenWithCard.MaCongDan });
            }
            
            return RedirectToAction("QuanLyTheCanCuoc");
        }
        
        // Rename to 'citizenWithCard' to resolve variable name conflict
        private bool TheCanCuocExists(string maTheCC)
        {
            return _context.TheCanCuocs.Any(e => e.MaTheCC == maTheCC);
        }

        // GET: /CanBo/CapNhatTheCanCuoc/{maCongDan}
        public async Task<IActionResult> CapNhatTheCanCuoc(int maCongDan)
        {
            var congDan = await _context.CongDans
                .FirstOrDefaultAsync(c => c.MaCongDan == maCongDan);
            
            if (congDan == null)
            {
                return NotFound();
            }
            
            var theCanCuoc = await _context.Set<TheCanCuoc>().FindAsync(congDan.MaTheCC);
            
            var viewModel = new CapNhatTheCanCuocCanBoViewModel();
            
            if (theCanCuoc != null)
            {
                // Populate from existing ID card
                viewModel.MaTheCC = theCanCuoc.MaTheCC;
                viewModel.TenCC = theCanCuoc.TenCC;
                viewModel.NgayCap = theCanCuoc.NgayCap ?? DateTime.Now;
                viewModel.NgayHetHan = theCanCuoc.NgayHetHan ?? DateTime.Now.AddYears(10);
                viewModel.NoiCap = theCanCuoc.NoiCap;
                viewModel.QuocTich = theCanCuoc.QuocTich ?? "Việt Nam";
                viewModel.GhiChu = theCanCuoc.GhiChu;
                viewModel.TrangThai = theCanCuoc.TrangThai ?? "Đang sử dụng";
                viewModel.AnhMatTruocHienTai = theCanCuoc.AnhMatTruoc;
                viewModel.AnhMatSauHienTai = theCanCuoc.AnhMatSau;
            }
            else
            {
                // Suggest default values for new ID card
                viewModel.NgayCap = DateTime.Now;
                viewModel.NgayHetHan = DateTime.Now.AddYears(10);
                viewModel.NoiCap = "Cục Cảnh sát quản lý hành chính về trật tự xã hội";
                viewModel.TenCC = "Căn cước công dân";
                viewModel.QuocTich = "Việt Nam";
                viewModel.TrangThai = "Đang sử dụng";
            }
            
            ViewBag.CongDan = congDan;
            return View(viewModel);
        }

        // POST: /CanBo/CapNhatTheCanCuoc/{maCongDan}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTheCanCuoc(int maCongDan, CapNhatTheCanCuocCanBoViewModel model)
        {
            var congDan = await _context.CongDans
                .FirstOrDefaultAsync(c => c.MaCongDan == maCongDan);
            
            if (congDan == null)
            {
                return NotFound();
            }
            
            if (!ModelState.IsValid)
            {
                ViewBag.CongDan = congDan;
                return View(model);
            }
            
            var theCanCuoc = await _context.Set<TheCanCuoc>().FindAsync(model.MaTheCC);
            
            bool isNewCard = false;
            
            if (theCanCuoc == null)
            {
                // Create a new ID card
                theCanCuoc = new TheCanCuoc
                {
                    MaTheCC = model.MaTheCC,
                    TrangThai = model.TrangThai
                };
                _context.TheCanCuocs.Add(theCanCuoc);
                isNewCard = true;
            }
            
            // Update card information
            theCanCuoc.TenCC = model.TenCC;
            theCanCuoc.NgayCap = model.NgayCap;
            theCanCuoc.NgayHetHan = model.NgayHetHan;
            theCanCuoc.NoiCap = model.NoiCap;
            theCanCuoc.QuocTich = model.QuocTich;
            theCanCuoc.GhiChu = model.GhiChu;
            theCanCuoc.TrangThai = model.TrangThai;
            
            // Process front image upload
            if (model.AnhMatTruoc != null && model.AnhMatTruoc.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "thecancouc");
                Directory.CreateDirectory(uploadsFolder); // Ensure folder exists
                
                var uniqueFileName = $"{model.MaTheCC}_front_{Guid.NewGuid().ToString().Substring(0, 8)}{Path.GetExtension(model.AnhMatTruoc.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.AnhMatTruoc.CopyToAsync(fileStream);
                }
                
                // Delete old file if exists
                if (!string.IsNullOrEmpty(theCanCuoc.AnhMatTruoc))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", 
                        theCanCuoc.AnhMatTruoc.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
                
                theCanCuoc.AnhMatTruoc = $"/uploads/thecancouc/{uniqueFileName}";
            }
            
            // Process back image upload
            if (model.AnhMatSau != null && model.AnhMatSau.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "thecancouc");
                Directory.CreateDirectory(uploadsFolder); // Ensure folder exists
                
                var uniqueFileName = $"{model.MaTheCC}_back_{Guid.NewGuid().ToString().Substring(0, 8)}{Path.GetExtension(model.AnhMatSau.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.AnhMatSau.CopyToAsync(fileStream);
                }
                
                // Delete old file if exists
                if (!string.IsNullOrEmpty(theCanCuoc.AnhMatSau))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", 
                        theCanCuoc.AnhMatSau.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
                
                theCanCuoc.AnhMatSau = $"/uploads/thecancouc/{uniqueFileName}";
            }
            
            // Update the citizen's ID card reference
            if (congDan.MaTheCC != model.MaTheCC)
            {
                congDan.MaTheCC = model.MaTheCC;
                _context.Update(congDan);
            }
            
            // Create notification for the citizen
            var thongBao = new ThongBao
            {
                MaCongDan = congDan.MaCongDan,
                TieuDe = isNewCard ? "Cấp thẻ căn cước mới" : "Cập nhật thẻ căn cước",
                NoiDung = isNewCard 
                    ? $"Cán bộ đã cấp thẻ căn cước mới có mã {model.MaTheCC} cho bạn."
                    : $"Cán bộ đã cập nhật thông tin thẻ căn cước có mã {model.MaTheCC} của bạn.",
                NgayTao = DateTime.Now,
                DaDoc = false,
                LoaiThongBao = "CCCD",
                UserId = congDan.MaNguoiDung
            };
            
            _context.ThongBaos.Add(thongBao);
            
            // Create activity log
            var nhatKyThong = new NhatKyThong
            {
                NoiDungHoatDong = isNewCard 
                    ? $"Cấp thẻ căn cước mới có mã {model.MaTheCC} cho công dân {congDan.TenCongDan}" 
                    : $"Cập nhật thẻ căn cước có mã {model.MaTheCC} cho công dân {congDan.TenCongDan}",
                ThoiGian = DateTime.Now,
                NguoiThucHien = User.Identity?.Name ?? "Admin"
            };
            
            _context.NhatKyThongs.Add(nhatKyThong);
            
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = isNewCard 
                ? "Cấp thẻ căn cước mới thành công!" 
                : "Cập nhật thẻ căn cước thành công!";
            
            return RedirectToAction("ChiTietCongDan", new { id = maCongDan });
        }

        // GET: /CanBo/CongDanKhongCoTheCC
        public async Task<IActionResult> CongDanKhongCoTheCC(string tenCongDan, int page = 1, int pageSize = 10)
        {
            var query = _context.CongDans.AsQueryable();
            
            // Filter citizens without ID cards or with empty ID card numbers
            query = query.Where(c => c.MaTheCC == null || c.MaTheCC == "");
            
            // Additional filter by name if provided
            if (!string.IsNullOrWhiteSpace(tenCongDan))
            {
                query = query.Where(c => c.TenCongDan.Contains(tenCongDan));
            }
            
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            var congDans = await query
                .OrderBy(c => c.TenCongDan)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TenCongDan = tenCongDan;
            
            return View(congDans);
        }

        [Authorize(Roles = "CanBo,Admin")]
        public async Task<IActionResult> QuanLyCongDan(string search = "")
        {
            var congDans = string.IsNullOrWhiteSpace(search)
                ? await _context.CongDans.ToListAsync()
                : await _context.CongDans
                    .Where(c => c.TenCongDan.Contains(search) || c.MaTheCC.Contains(search) || c.SDT.Contains(search))
                    .ToListAsync();

            ViewBag.Search = search;
            return View(congDans);
        }

        [Authorize(Roles = "CanBo,Admin")]
        public async Task<IActionResult> CapNhatThongTinCanBo()
        {
            var userName = User.Identity.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null)
                return Content($"Không tìm thấy user: {userName}");

            var canBo = await _context.CanBos.FirstOrDefaultAsync(c => c.UserId == user.Id);
            if (canBo == null)
            {
                // Tự động tạo mới bản ghi CanBo nếu chưa có
                canBo = new CanBo
                {
                    UserId = user.Id,
                    TenCanBo = user.HoTen ?? user.UserName,
                    SoDienThoai = user.PhoneNumber,
                    // Có thể gán các trường mặc định khác nếu muốn
                };
                _context.CanBos.Add(canBo);
                await _context.SaveChangesAsync();
            }

            // Load phòng ban cho dropdown
            ViewBag.PhongBans = await _context.PhongBans
                .Select(p => new SelectListItem
                {
                    Value = p.MaPhongBan.ToString(),
                    Text = p.TenPhongBan
                })
                .ToListAsync();

            return View(canBo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CanBo,Admin")]
        public async Task<IActionResult> CapNhatThongTinCanBo(CanBo model)
        {
            if (!ModelState.IsValid)
            {
                // Reload phòng ban for dropdown if validation fails
                ViewBag.PhongBans = await _context.PhongBans
                    .Select(p => new SelectListItem
                    {
                        Value = p.MaPhongBan.ToString(),
                        Text = p.TenPhongBan
                    })
                    .ToListAsync();
                return View(model);
            }

            var canBo = await _context.CanBos.FindAsync(model.MaCanBo);
            if (canBo == null)
                return NotFound();

            // Update CanBo information
            canBo.TenCanBo = model.TenCanBo;
            canBo.ChucVu = model.ChucVu;
            canBo.MaPhongBan = model.MaPhongBan;
            canBo.SoDienThoai = model.SoDienThoai;
            canBo.DiaChi = model.DiaChi;
            canBo.NgaySinh = model.NgaySinh;
            canBo.GioiTinh = model.GioiTinh;

            // Update related ApplicationUser information
            var user = await _userManager.FindByIdAsync(canBo.UserId);
            if (user != null)
            {
                user.HoTen = model.TenCanBo;
                user.PhoneNumber = model.SoDienThoai;
                await _userManager.UpdateAsync(user);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [Authorize(Roles = "CanBo")]
        public async Task<IActionResult> CapNhatTheCanCuocCanBo(int maTheCC, CapNhatTheCanCuocViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var theCanCuoc = await _context.Set<TheCanCuoc>().FindAsync(maTheCC);
            if (theCanCuoc == null)
            {
                return NotFound();
            }

            theCanCuoc.TenCC = model.TenCC;
            theCanCuoc.NgayCap = model.NgayCap;
            theCanCuoc.NgayHetHan = model.NgayHetHan;
            theCanCuoc.NoiCap = model.NoiCap;
            theCanCuoc.GhiChu = model.GhiChu;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(QuanLyTheCanCuoc));
        }
    }
} 