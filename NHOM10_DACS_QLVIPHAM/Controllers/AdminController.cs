using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NHOM10_DACS_QLVIPHAM.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace NHOM10_DACS_QLVIPHAM.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult QuanLyNguoiDung()
        {
            var users = _userManager.Users.ToList();
            // Lấy roles cho từng user
            var userRoles = new Dictionary<string, List<string>>();
            foreach (var user in users)
            {
                var roles = _userManager.GetRolesAsync(user).Result.ToList();
                userRoles[user.Id] = roles;
            }
            ViewBag.UserRoles = userRoles;
            return View(users);
        }

        public async Task<IActionResult> PhanQuyen(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var currentRole = roles.FirstOrDefault() ?? string.Empty;

            var model = new PhanQuyenViewModel
            {
                UserId = user.Id,
                UserName = user.UserName ?? "Unknown",
                SelectedRole = currentRole
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PhanQuyen(PhanQuyenViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            // Xóa tất cả vai trò hiện tại
            var userRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, userRoles);

            // Thêm vai trò mới
            if (!string.IsNullOrEmpty(model.SelectedRole))
                await _userManager.AddToRoleAsync(user, model.SelectedRole);

            TempData["SuccessMessage"] = "Cập nhật quyền thành công!";
            return RedirectToAction(nameof(QuanLyNguoiDung));
        }

        // GET: /Admin/CreateRoles
        public async Task<IActionResult> CreateRoles()
        {
            // Create Admin role if it doesn't exist
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Create CanBo role if it doesn't exist
            if (!await _roleManager.RoleExistsAsync("CanBo"))
            {
                await _roleManager.CreateAsync(new IdentityRole("CanBo"));
            }

            // Create CongDan role if it doesn't exist
            if (!await _roleManager.RoleExistsAsync("CongDan"))
            {
                await _roleManager.CreateAsync(new IdentityRole("CongDan"));
            }

            // Create User role if it doesn't exist
            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }

            return Content("Roles created successfully");
        }

        // GET: /Admin/CreateAdminUser
        public async Task<IActionResult> CreateAdminUser()
        {
            var adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@quanlyvipham.vn",
                HoTen = "Administrator",
                EmailConfirmed = true
            };

            // Check if the admin already exists
            var existingAdmin = await _userManager.FindByNameAsync("admin");
            if (existingAdmin != null)
            {
                return Content("Admin user already exists");
            }

            // Create the admin user with the specified password
            var result = await _userManager.CreateAsync(adminUser, "Admin@123456");

            if (result.Succeeded)
            {
                // Add the admin to the Admin role
                await _userManager.AddToRoleAsync(adminUser, "Admin");
                return Content("Admin user created successfully");
            }

            return Content("Failed to create admin user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KhoaTaiKhoan(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            // Khóa tài khoản trong 100 năm
            user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
            await _userManager.UpdateAsync(user);
            TempData["SuccessMessage"] = $"Đã khóa tài khoản {user.UserName}";
            return RedirectToAction("QuanLyNguoiDung");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoKhoaTaiKhoan(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            user.LockoutEnd = null;
            await _userManager.UpdateAsync(user);
            TempData["SuccessMessage"] = $"Đã mở khóa tài khoản {user.UserName}";
            return RedirectToAction("QuanLyNguoiDung");
        }

        [HttpGet]
        public async Task<IActionResult> TaoCanBo()
        {
            var viewModel = new TaoCanBoViewModel
            {
                PhongBans = await _context.PhongBans
                    .Select(p => new SelectListItem
                    {
                        Value = p.MaPhongBan.ToString(),
                        Text = p.TenPhongBan
                    })
                    .ToListAsync()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoCanBo(TaoCanBoViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if username already exists
                var existingUser = await _userManager.FindByNameAsync(model.UserName);
                if (existingUser != null)
                {
                    ModelState.AddModelError("UserName", "Tên đăng nhập đã tồn tại.");
                    model.PhongBans = await _context.PhongBans
                        .Select(p => new SelectListItem
                        {
                            Value = p.MaPhongBan.ToString(),
                            Text = p.TenPhongBan
                        })
                        .ToListAsync();
                    return View(model);
                }

                // Create new user
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    HoTen = model.HoTen,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Add user to CanBo role
                    await _userManager.AddToRoleAsync(user, "CanBo");

                    // Create CanBo record
                    var canBo = new CanBo
                    {
                        TenCanBo = model.HoTen,
                        ChucVu = model.ChucVu,
                        MaPhongBan = model.MaPhongBan,
                        UserId = user.Id // Link with the user account
                    };

                    _context.CanBos.Add(canBo);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Tạo tài khoản cán bộ thành công!";
                    return RedirectToAction(nameof(QuanLyNguoiDung));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we get here, something failed, redisplay form
            model.PhongBans = await _context.PhongBans
                .Select(p => new SelectListItem
                {
                    Value = p.MaPhongBan.ToString(),
                    Text = p.TenPhongBan
                })
                .ToListAsync();
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult TaoPhongBan()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> TaoPhongBan(PhongBan model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.PhongBans.Add(model);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Tạo phòng ban thành công!";
            return RedirectToAction("QuanLyNguoiDung"); // hoặc trang quản lý phòng ban nếu có
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> QuanLyPhongBan()
        {
            var phongBans = await _context.PhongBans.ToListAsync();
            return View(phongBans);
        }
    }

    public class PhanQuyenViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string SelectedRole { get; set; } = string.Empty;
    }

    public class TaoCanBoViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập chức vụ")]
        [Display(Name = "Chức vụ")]
        public string ChucVu { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn phòng ban")]
        [Display(Name = "Phòng ban")]
        public int MaPhongBan { get; set; }

        public List<SelectListItem> PhongBans { get; set; } = new List<SelectListItem>();
    }
} 