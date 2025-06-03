using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHOM10_DACS_QLVIPHAM.Models;
using NHOM10_DACS_QLVIPHAM.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace NHOM10_DACS_QLVIPHAM.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminUserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminUserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // GET: /AdminUser/Create
        public IActionResult Create()
        {
            ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
            var model = new CreateUserViewModel();
            model.PhongBans = _context.PhongBans.Select(p => new SelectListItem { Value = p.MaPhongBan.ToString(), Text = p.TenPhongBan }).ToList();
            return View(model);
        }

        // POST: /AdminUser/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Gán role
                    if (!string.IsNullOrEmpty(model.Role) && await _roleManager.RoleExistsAsync(model.Role))
                    {
                        await _userManager.AddToRoleAsync(user, model.Role);
                    }
                    // Nếu chọn phòng ban, tạo bản ghi CanBo
                    if (model.MaPhongBan.HasValue)
                    {
                        var canBo = new CanBo
                        {
                            TenCanBo = model.UserName,
                            MaPhongBan = model.MaPhongBan.Value,
                            UserId = user.Id
                        };
                        _context.CanBos.Add(canBo);
                        await _context.SaveChangesAsync();
                    }
                    return RedirectToAction("Index", "AdminUser");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
            model.PhongBans = _context.PhongBans.Select(p => new SelectListItem { Value = p.MaPhongBan.ToString(), Text = p.TenPhongBan }).ToList();
            return View(model);
        }

        // GET: /AdminUser/Index
        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        // POST: /AdminUser/AssignRole
        [HttpPost]
        public async Task<IActionResult> AssignRole(string userName, string role)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return NotFound($"Không tìm thấy user: {userName}");
            }
            if (!await _roleManager.RoleExistsAsync(role))
            {
                return BadRequest($"Role {role} không tồn tại");
            }
            if (!await _userManager.IsInRoleAsync(user, role))
            {
                await _userManager.AddToRoleAsync(user, role);
            }
            TempData["SuccessMessage"] = $"Đã gán role {role} cho user {userName}";
            return RedirectToAction("Index");
        }
    }
} 