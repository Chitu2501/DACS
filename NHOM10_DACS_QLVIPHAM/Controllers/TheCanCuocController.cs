using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NHOM10_DACS_QLVIPHAM.Models;
using NHOM10_DACS_QLVIPHAM.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace NHOM10_DACS_QLVIPHAM.Controllers
{
    public class TheCanCuocController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TheCanCuocController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: /TheCanCuoc/TraCuuTheCanCuoc
        public IActionResult TraCuuTheCanCuoc()
        {
            return View();
        }
    
        // GET: /TheCanCuoc/TraCuuTheCanCuocById?maTheCC=...
        [HttpGet]
        public async Task<IActionResult> TraCuuTheCanCuocById(string maTheCC)
        {
            if (string.IsNullOrEmpty(maTheCC))
            {
                return RedirectToAction("TraCuuTheCanCuoc");
            }

            // Log the request for debugging
            Console.WriteLine($"Tra cứu thẻ căn cước với mã: {maTheCC}");

            var theCanCuoc = await _context.TheCanCuocs.FirstOrDefaultAsync(t => t.MaTheCC == maTheCC);
            if (theCanCuoc == null)
            {
                // Log the error for debugging
                Console.WriteLine($"Không tìm thấy thẻ căn cước với mã: {maTheCC}");
                
                // Check if TheCanCuocs table exists and has data
                var theCanCuocsCount = await _context.TheCanCuocs.CountAsync();
                Console.WriteLine($"Số lượng thẻ căn cước trong hệ thống: {theCanCuocsCount}");

                ModelState.AddModelError("", $"Mã thẻ căn cước [{maTheCC}] không tồn tại trong hệ thống");
                ViewBag.KetQua = false;
                ViewBag.MaTheCC = maTheCC;
                return View("TraCuuTheCanCuoc");
            }

            var congDan = await _context.CongDans.FirstOrDefaultAsync(c => c.MaTheCC == maTheCC);
            if (congDan == null)
            {
                // Log the error for debugging
                Console.WriteLine($"Không tìm thấy công dân với mã CCCD: {maTheCC}");
                
                ModelState.AddModelError("", $"Không tìm thấy thông tin công dân với mã thẻ này: {maTheCC}");
                ViewBag.KetQua = false;
                ViewBag.MaTheCC = maTheCC;
                return View("TraCuuTheCanCuoc");
            }

            // Log success
            Console.WriteLine($"Đã tìm thấy thẻ căn cước và công dân với mã: {maTheCC}");
            
            ViewBag.CongDan = congDan;
            return View("ChiTietTheCanCuoc", theCanCuoc);
        }

        // POST: /TheCanCuoc/TraCuuTheCanCuoc
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TraCuuTheCanCuoc(string maTheCC)
        {
            if (string.IsNullOrEmpty(maTheCC))
            {
                ModelState.AddModelError("", "Vui lòng nhập mã thẻ căn cước");
                return View();
            }

            // Look up the ID card in the database
            var theCanCuoc = await _context.TheCanCuocs.FirstOrDefaultAsync(t => t.MaTheCC == maTheCC);
            
            if (theCanCuoc == null)
            {
                // Log the error for debugging
                Console.WriteLine($"Không tìm thấy thẻ căn cước với mã: {maTheCC}");
                
                // Check if TheCanCuocs table exists and has data
                var theCanCuocsCount = await _context.TheCanCuocs.CountAsync();
                Console.WriteLine($"Số lượng thẻ căn cước trong hệ thống: {theCanCuocsCount}");
                
                ModelState.AddModelError("", $"Mã thẻ căn cước [{maTheCC}] không tồn tại trong hệ thống");
                ViewBag.KetQua = false;
                ViewBag.MaTheCC = maTheCC; // Pass the ID to view for displaying
                return View();
            }

            // Look up the citizen associated with this ID card
            var congDan = await _context.CongDans.FirstOrDefaultAsync(c => c.MaTheCC == maTheCC);
            
            if (congDan == null)
            {
                // Log the error for debugging
                Console.WriteLine($"Không tìm thấy công dân với mã CCCD: {maTheCC}");
                
                ModelState.AddModelError("", $"Không tìm thấy thông tin công dân với mã thẻ này: {maTheCC}");
                ViewBag.KetQua = false;
                ViewBag.MaTheCC = maTheCC;
                return View();
            }

            ViewBag.CongDan = congDan;
            ViewBag.KetQua = true;
            return View("ChiTietTheCanCuoc", theCanCuoc);
        }

        // GET: /TheCanCuoc/YeuCauCapLaiThe
        [Authorize]
        public IActionResult YeuCauCapLaiThe()
        {
            var viewModel = new YeuCauCapLaiTheViewModel();
            return View(viewModel);
        }

        // POST: /TheCanCuoc/YeuCauCapLaiThe
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> YeuCauCapLaiThe(YeuCauCapLaiTheViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Lấy thông tin người dùng hiện tại
            var userName = User.Identity?.Name;
            if (userName == null)
            {
                ModelState.AddModelError("", "Không tìm thấy thông tin người dùng");
                return View(model);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null)
            {
                ModelState.AddModelError("", "Không tìm thấy thông tin tài khoản");
                return View(model);
            }
            
            // Lấy công dân dựa trên email người dùng
            var congDan = await _context.CongDans.FirstOrDefaultAsync(c => c.GMAIL == user.Email);
            
            if (congDan == null)
            {
                ModelState.AddModelError("", "Không tìm thấy thông tin công dân liên kết với tài khoản của bạn");
                return View(model);
            }

            // Tạo yêu cầu cấp lại thẻ
            var yeuCauCapLaiThe = new YeuCauCapLaiThe
            {
                NgayYeuCau = DateTime.Now,
                LyDo = model.LyDo,
                TrangThai = "Chờ xét duyệt",
                MaCongDan = congDan.MaCongDan,
                MaTheCC = congDan.MaTheCC
            };

            _context.YeuCauCapLaiThes.Add(yeuCauCapLaiThe);
            await _context.SaveChangesAsync();

            // Gửi thông báo
            var thongBao = new ThongBao
            {
                MaCongDan = congDan.MaCongDan,
                TieuDe = "Đã nhận yêu cầu cấp lại thẻ căn cước",
                NoiDung = "Chúng tôi đã nhận được yêu cầu cấp lại thẻ căn cước của bạn. Yêu cầu đang được xử lý.",
                NgayTao = DateTime.Now,
                DaDoc = false,
                LoaiThongBao = "Yêu cầu cấp lại thẻ",
                UserId = user?.Id
            };

            _context.ThongBaos.Add(thongBao);
            await _context.SaveChangesAsync();

            return RedirectToAction("XacNhanYeuCauCapLaiThe");
        }

        // GET: /TheCanCuoc/XacNhanYeuCauCapLaiThe
        public IActionResult XacNhanYeuCauCapLaiThe()
        {
            return View();
        }

        // GET: /TheCanCuoc/TrangThaiYeuCauCapLaiThe
        [Authorize]
        public async Task<IActionResult> TrangThaiYeuCauCapLaiThe()
        {
            var userName = User.Identity?.Name;
            if (userName == null)
            {
                return View("Error", new ErrorViewModel { RequestId = "Không tìm thấy thông tin người dùng" });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null)
            {
                return View("Error", new ErrorViewModel { RequestId = "Không tìm thấy thông tin tài khoản" });
            }
            
            // Lấy công dân dựa trên email người dùng
            var congDan = await _context.CongDans.FirstOrDefaultAsync(c => c.GMAIL == user.Email);
            
            if (congDan == null)
            {
                return View("Error", new ErrorViewModel { RequestId = "Không tìm thấy thông tin công dân" });
            }

            var yeuCauCapLaiThes = await _context.YeuCauCapLaiThes
                .Where(y => y.MaCongDan == congDan.MaCongDan)
                .OrderByDescending(y => y.NgayYeuCau)
                .ToListAsync();

            return View(yeuCauCapLaiThes);
        }

        // GET: /TheCanCuoc/ThongTinCapLaiThe
        [Authorize]
        public async Task<IActionResult> ThongTinCapLaiThe()
        {
            var userName = User.Identity?.Name;
            if (userName == null)
            {
                return View("Error", new ErrorViewModel { RequestId = "Không tìm thấy thông tin người dùng" });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null)
            {
                return View("Error", new ErrorViewModel { RequestId = "Không tìm thấy thông tin tài khoản" });
            }
            
            // Lấy công dân dựa trên email người dùng
            var congDan = await _context.CongDans.FirstOrDefaultAsync(c => c.GMAIL == user.Email);
            
            if (congDan == null)
            {
                return View("Error", new ErrorViewModel { RequestId = "Không tìm thấy thông tin công dân" });
            }

            var capLaiThes = await _context.CapLaiTheCanCuocs
                .Where(c => c.MaCongDan == congDan.MaCongDan)
                .OrderByDescending(c => c.NgayCapLai)
                .ToListAsync();

            return View(capLaiThes);
        }

        // GET: /TheCanCuoc/CapNhatTheCanCuoc
        [Authorize]
        public async Task<IActionResult> CapNhatTheCanCuoc()
        {
            // Nếu là cán bộ thì không cho phép cập nhật thẻ căn cước của chính mình
            if (User.IsInRole("CanBo"))
            {
                // Cho phép cán bộ cập nhật thẻ căn cước của chính mình
                var userNameCanBo = User.Identity?.Name;
                if (userNameCanBo == null)
                {
                    return View("Error", new ErrorViewModel { RequestId = "Không tìm thấy thông tin người dùng" });
                }

                var userCanBo = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userNameCanBo);
                if (userCanBo == null)
                {
                    return View("Error", new ErrorViewModel { RequestId = "Không tìm thấy thông tin tài khoản" });
                }
                
                // Lấy công dân dựa trên email người dùng
                var congDanCanBo = await _context.CongDans.FirstOrDefaultAsync(c => c.GMAIL == userCanBo.Email);
                
                if (congDanCanBo == null)
                {
                    return View("Error", new ErrorViewModel { RequestId = "Không tìm thấy thông tin công dân" });
                }

                var theCanCuocCanBo = await _context.TheCanCuocs.FirstOrDefaultAsync(t => t.MaTheCC == congDanCanBo.MaTheCC);
                
                if (theCanCuocCanBo == null)
                {
                    // Create a new ID card model if it doesn't exist
                    theCanCuocCanBo = new TheCanCuoc
                    {
                        MaTheCC = congDanCanBo.MaTheCC ?? "",
                        TenCC = "Căn cước công dân",
                        NgayCap = DateTime.Now,
                        NgayHetHan = DateTime.Now.AddYears(10),
                        NoiCap = "Cục Cảnh sát quản lý hành chính về trật tự xã hội",
                        QuocTich = "Việt Nam",
                        TrangThai = "Đang sử dụng"
                    };
                }
                
                // Create view model for the ID card update
                var viewModelCanBo = new CapNhatTheCanCuocViewModel
                {
                    MaTheCC = theCanCuocCanBo.MaTheCC,
                    TenCC = theCanCuocCanBo.TenCC,
                    NgayCap = theCanCuocCanBo.NgayCap,
                    NgayHetHan = theCanCuocCanBo.NgayHetHan,
                    NoiCap = theCanCuocCanBo.NoiCap,
                    QuocTich = theCanCuocCanBo.QuocTich,
                    GhiChu = theCanCuocCanBo.GhiChu,
                    AnhMatTruocHienTai = theCanCuocCanBo.AnhMatTruoc,
                    AnhMatSauHienTai = theCanCuocCanBo.AnhMatSau
                };
                
                ViewBag.CongDan = congDanCanBo;
                return View(viewModelCanBo);
            }

            var userName = User.Identity?.Name;
            if (userName == null)
            {
                return View("Error", new ErrorViewModel { RequestId = "Không tìm thấy thông tin người dùng" });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null)
            {
                return View("Error", new ErrorViewModel { RequestId = "Không tìm thấy thông tin tài khoản" });
            }
            
            // Lấy công dân dựa trên email người dùng
            var congDan = await _context.CongDans.FirstOrDefaultAsync(c => c.GMAIL == user.Email);
            
            if (congDan == null)
            {
                return View("Error", new ErrorViewModel { RequestId = "Không tìm thấy thông tin công dân" });
            }

            var theCanCuoc = await _context.TheCanCuocs.FirstOrDefaultAsync(t => t.MaTheCC == congDan.MaTheCC);
            
            if (theCanCuoc == null)
            {
                // Create a new ID card model if it doesn't exist
                theCanCuoc = new TheCanCuoc
                {
                    MaTheCC = congDan.MaTheCC ?? "",
                    TenCC = "Căn cước công dân",
                    NgayCap = DateTime.Now,
                    NgayHetHan = DateTime.Now.AddYears(10),
                    NoiCap = "Cục Cảnh sát quản lý hành chính về trật tự xã hội",
                    QuocTich = "Việt Nam",
                    TrangThai = "Đang sử dụng"
                };
            }
            
            // Create view model for the ID card update
            var viewModel = new CapNhatTheCanCuocViewModel
            {
                MaTheCC = theCanCuoc.MaTheCC,
                TenCC = theCanCuoc.TenCC,
                NgayCap = theCanCuoc.NgayCap,
                NgayHetHan = theCanCuoc.NgayHetHan,
                NoiCap = theCanCuoc.NoiCap,
                QuocTich = theCanCuoc.QuocTich,
                GhiChu = theCanCuoc.GhiChu,
                AnhMatTruocHienTai = theCanCuoc.AnhMatTruoc,
                AnhMatSauHienTai = theCanCuoc.AnhMatSau
            };
            
            ViewBag.CongDan = congDan;
            return View(viewModel);
        }

        // POST: /TheCanCuoc/CapNhatTheCanCuoc
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTheCanCuoc(CapNhatTheCanCuocViewModel model, IFormFile anhMatTruoc, IFormFile anhMatSau)
        {
            if (!ModelState.IsValid)
            {
                var congDan = await GetCurrentCongDan();
                if (congDan == null)
                {
                    return View("Error", new ErrorViewModel { RequestId = "Không tìm thấy thông tin công dân" });
                }
                ViewBag.CongDan = congDan;
                return View(model);
            }

            var currentUser = await GetCurrentCongDan();
            if (currentUser == null)
            {
                return View("Error", new ErrorViewModel { RequestId = "Không tìm thấy thông tin công dân" });
            }

            var theCanCuoc = await _context.TheCanCuocs.FirstOrDefaultAsync(t => t.MaTheCC == model.MaTheCC);
            bool isNewCard = false;
            
            if (theCanCuoc == null)
            {
                // Create a new ID card if it doesn't exist
                theCanCuoc = new TheCanCuoc
                {
                    MaTheCC = model.MaTheCC,
                    TrangThai = "Đang sử dụng"
                };
                _context.TheCanCuocs.Add(theCanCuoc);
                isNewCard = true;
            }

            // Update the ID card information
            theCanCuoc.TenCC = model.TenCC;
            theCanCuoc.NgayCap = model.NgayCap;
            theCanCuoc.NgayHetHan = model.NgayHetHan;
            theCanCuoc.NoiCap = model.NoiCap;
            theCanCuoc.QuocTich = model.QuocTich;
            theCanCuoc.GhiChu = model.GhiChu;

            // Process the front side image
            if (anhMatTruoc != null && anhMatTruoc.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "thecancouc");
                Directory.CreateDirectory(uploadsFolder); // Ensure the directory exists
                
                string uniqueFileName = $"{model.MaTheCC}_front_{Guid.NewGuid().ToString().Substring(0, 8)}{Path.GetExtension(anhMatTruoc.FileName)}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await anhMatTruoc.CopyToAsync(fileStream);
                }
                
                // Delete old image file if it exists
                if (!string.IsNullOrEmpty(theCanCuoc.AnhMatTruoc))
                {
                    var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, theCanCuoc.AnhMatTruoc.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
                
                theCanCuoc.AnhMatTruoc = $"/uploads/thecancouc/{uniqueFileName}";
            }
            
            // Process the back side image
            if (anhMatSau != null && anhMatSau.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "thecancouc");
                Directory.CreateDirectory(uploadsFolder); // Ensure the directory exists
                
                string uniqueFileName = $"{model.MaTheCC}_back_{Guid.NewGuid().ToString().Substring(0, 8)}{Path.GetExtension(anhMatSau.FileName)}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await anhMatSau.CopyToAsync(fileStream);
                }
                
                // Delete old image file if it exists
                if (!string.IsNullOrEmpty(theCanCuoc.AnhMatSau))
                {
                    var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, theCanCuoc.AnhMatSau.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
                
                theCanCuoc.AnhMatSau = $"/uploads/thecancouc/{uniqueFileName}";
            }

            // Update the relationship with the citizen
            if (currentUser.MaTheCC != model.MaTheCC)
            {
                currentUser.MaTheCC = model.MaTheCC;
                _context.Update(currentUser);
            }

            try
            {
                await _context.SaveChangesAsync();
                
                // Create notification
                var thongBao = new ThongBao
                {
                    MaCongDan = currentUser.MaCongDan,
                    TieuDe = isNewCard ? "Đã thêm thẻ căn cước mới" : "Đã cập nhật thẻ căn cước",
                    NoiDung = isNewCard 
                        ? $"Đã thêm thẻ căn cước mới có mã {model.MaTheCC} vào hồ sơ của bạn."
                        : $"Đã cập nhật thông tin thẻ căn cước có mã {model.MaTheCC}",
                    NgayTao = DateTime.Now,
                    DaDoc = false,
                    LoaiThongBao = "Cập nhật thẻ căn cước",
                    UserId = currentUser.MaNguoiDung
                };
                
                _context.ThongBaos.Add(thongBao);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = isNewCard 
                    ? "Thêm thẻ căn cước thành công" 
                    : "Cập nhật thẻ căn cước thành công";
                    
                return RedirectToAction("ChiTietTheCanCuoc", new { maTheCC = model.MaTheCC });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi khi lưu thông tin thẻ căn cước: {ex.Message}");
                ViewBag.CongDan = currentUser;
                return View(model);
            }
        }

        // Helper method to get the current citizen
        private async Task<CongDan?> GetCurrentCongDan()
        {
            var userName = User.Identity?.Name;
            if (userName == null)
            {
                return null;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null)
            {
                return null;
            }
            
            return await _context.CongDans.FirstOrDefaultAsync(c => c.GMAIL == user.Email);
        }
    }
} 