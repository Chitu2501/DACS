using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NHOM10_DACS_QLVIPHAM.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace NHOM10_DACS_QLVIPHAM.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        // Check if user is authenticated and is in CanBo or Admin role
        if (User.Identity?.IsAuthenticated == true && (User.IsInRole("CanBo") || User.IsInRole("Admin")))
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.UserName = user?.HoTen ?? user?.UserName;
            
            
            // Thống kê tổng quát
            ViewBag.TongViPham = await _context.ViPhams.CountAsync();
            ViewBag.TongLoaiViPham = await _context.LoaiViPhams.CountAsync();
            ViewBag.TongBienBan = await _context.BienBanVPhams.CountAsync();
            ViewBag.TongCongDan = await _context.CongDans.CountAsync();
            
            // Biên bản vi phạm gần đây
            var bienBanGanDay = await _context.BienBanVPhams
                .Include(b => b.ViPham)
                .Include(b => b.CongDan)
                .OrderByDescending(b => b.NgayLapBienBan)
                .Take(5)
                .ToListAsync();
            
            ViewBag.IsCanBo = true;
            return View("~/Views/Home/Index.cshtml", bienBanGanDay);
        }
        
        // Return standard view for regular users
        ViewBag.IsCanBo = false;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    // Trang tra cứu thông tin luật pháp
    public async Task<IActionResult> LuatPhap()
    {
        var viPhams = await _context.ViPhams
            .Include(v => v.LoaiViPham)
            .OrderBy(v => v.LoaiViPham != null ? v.LoaiViPham.TenLoaiVPham : "")
            .ThenBy(v => v.TenViPham)
            .ToListAsync();
            
        return View(viPhams);
    }

    // Trang hướng dẫn sử dụng
    public IActionResult HuongDan()
    {
        return View();
    }

    // Trang thông tin liên hệ
    public IActionResult LienHe()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Notification()
    {
        return ViewComponent("Notification");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
