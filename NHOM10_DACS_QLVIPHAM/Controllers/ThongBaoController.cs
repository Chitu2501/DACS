using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NHOM10_DACS_QLVIPHAM.Models;
using NHOM10_DACS_QLVIPHAM.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NHOM10_DACS_QLVIPHAM.Controllers
{
    [Authorize]
    public class ThongBaoController : Controller
    {
        private readonly IThongBaoService _thongBaoService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ThongBaoController(IThongBaoService thongBaoService, 
                               UserManager<ApplicationUser> userManager,
                               ApplicationDbContext context)
        {
            _thongBaoService = thongBaoService;
            _userManager = userManager;
            _context = context;
        }

        // Hiển thị tất cả thông báo
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Use the service instead of direct DB query
            var thongBaos = await _thongBaoService.LayThongBaoNguoiDungAsync(userId, 100); // Get more notifications for the index page

            return View(thongBaos);
        }

        // Xem chi tiết thông báo
        public async Task<IActionResult> ChiTiet(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Find user's CongDan
            var congDan = await _context.CongDans
                .FirstOrDefaultAsync(c => c.MaNguoiDung == userId);
                
            if (congDan == null)
                return NotFound();
                
            // Find notification by ID and CongDan
            var thongBao = await _context.ThongBaos
                .FirstOrDefaultAsync(t => t.MaThongBao == id && t.MaCongDan == congDan.MaCongDan);

            if (thongBao == null)
                return NotFound();
                
            // Set properties for application use
            thongBao.UserId = userId;

            // Đánh dấu đã đọc
            await _thongBaoService.DanhDauDaDocAsync(id);

            // Nếu có link chi tiết, chuyển hướng đến trang chi tiết
            if (!string.IsNullOrEmpty(thongBao.LinkChiTiet))
            {
                return Redirect(thongBao.LinkChiTiet);
            }

            return View(thongBao);
        }

        // API lấy thông báo mới nhất
        [HttpGet]
        public async Task<IActionResult> GetThongBaoMoi()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { thongBaos = new object[0], soLuongChuaDoc = 0 });
            }
            
            var thongBaos = await _thongBaoService.LayThongBaoNguoiDungAsync(userId, 5);
            var soLuongChuaDoc = await _thongBaoService.DemThongBaoChuaDocAsync(userId);

            return Json(new
            {
                thongBaos = thongBaos.Select(t => new
                {
                    t.MaThongBao,
                    t.TieuDe,
                    t.NoiDung,
                    NgayTao = t.NgayTao.ToString("dd/MM/yyyy HH:mm"),
                    t.DaDoc,
                    t.LoaiThongBao,
                    t.LinkChiTiet
                }),
                soLuongChuaDoc
            });
        }

        // Đánh dấu đã đọc
        [HttpPost]
        public async Task<IActionResult> DanhDauDaDoc(int id)
        {
            await _thongBaoService.DanhDauDaDocAsync(id);
            return Ok();
        }

        // Đánh dấu tất cả đã đọc
        [HttpPost]
        public async Task<IActionResult> DanhDauTatCaDaDoc()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Find user's CongDan
            var congDan = await _context.CongDans
                .FirstOrDefaultAsync(c => c.MaNguoiDung == userId);
                
            if (congDan == null)
                return Ok(); // Nothing to mark as read
                
            // Get all unread notifications for this CongDan
            var thongBaos = await _context.ThongBaos
                .Where(t => t.MaCongDan == congDan.MaCongDan && t.TrangThai != "DADOC")
                .ToListAsync();

            foreach (var thongBao in thongBaos)
            {
                thongBao.TrangThai = "DADOC";
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
} 