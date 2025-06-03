using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHOM10_DACS_QLVIPHAM.Models;
using NHOM10_DACS_QLVIPHAM.ViewModels;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace NHOM10_DACS_QLVIPHAM.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "")
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = "")
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // Add logging to see what credentials are being used
                Console.WriteLine($"Login attempt for user: {model.UserName}");
                
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: true);
                
                // Log the result of the sign-in attempt
                Console.WriteLine($"Login result - Succeeded: {result.Succeeded}, IsLockedOut: {result.IsLockedOut}, IsNotAllowed: {result.IsNotAllowed}, RequiresTwoFactor: {result.RequiresTwoFactor}");
                
                if (result.Succeeded)
                {
                    // Get current user
                    var user = await _userManager.FindByNameAsync(model.UserName);
                    if (user == null)
                    {
                        ModelState.AddModelError(string.Empty, "Không tìm thấy thông tin người dùng");
                        return View(model);
                    }
                    
                    // Log user information
                    Console.WriteLine($"User found - Id: {user.Id}, Email: {user.Email}, UserName: {user.UserName}");
                    
                    // Log user roles
                    var roles = await _userManager.GetRolesAsync(user);
                    Console.WriteLine($"User roles: {string.Join(", ", roles)}");
                    
                    // Check if specific return URL was provided
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    
                    // Check roles and redirect accordingly - ensure proper case sensitivity
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else if (await _userManager.IsInRoleAsync(user, "CanBo"))
                    {
                        return RedirectToAction("Dashboard", "CanBo");
                    }
                    else if (await _userManager.IsInRoleAsync(user, "CongDan"))
                    {
                        return RedirectToAction("Dashboard", "CongDan");
                    }
                    else if (await _userManager.IsInRoleAsync(user, "User"))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        // Default redirect if no specific role
                        return RedirectToAction("Index", "Home");
                    }
                }
                if (result.IsLockedOut)
                {
                    TempData["LockoutMessage"] = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên để được hỗ trợ.";
                    return RedirectToAction("Login");
                }
                else
                {
                    if (result.IsNotAllowed)
                    {
                        ModelState.AddModelError(string.Empty, "Tài khoản chưa được xác thực.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Đăng nhập không hợp lệ. Vui lòng kiểm tra tên đăng nhập và mật khẩu.");
                    }
                    return View(model);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            var viewModel = new RegisterViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { 
                    UserName = model.UserName, 
                    Email = model.Email,
                    HoTen = model.HoTen,
                    PhoneNumber = model.PhoneNumber ?? string.Empty
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Assign the selected role if it exists
                    var roleManager = HttpContext.RequestServices.GetService(typeof(RoleManager<IdentityRole>)) as RoleManager<IdentityRole>;
                    if (roleManager != null && !string.IsNullOrEmpty(model.SelectedRole))
                    {
                        string roleName = model.SelectedRole;
                        
                        // Create the role if it doesn't exist
                        if (!await roleManager.RoleExistsAsync(roleName))
                        {
                            await roleManager.CreateAsync(new IdentityRole(roleName));
                        }
                        
                        // Assign the selected role
                        await _userManager.AddToRoleAsync(user, roleName);
                    }
                    
                    // Create a CongDan record if the role is CongDan
                    if (model.SelectedRole == "CongDan")
                    {
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
                    }
                    
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    
                    // Redirect based on role
                    if (model.SelectedRole == "CongDan")
                    {
                        return RedirectToAction("Dashboard", "CongDan");
                    }
                    else if (model.SelectedRole == "CanBo")
                    {
                        return RedirectToAction("Dashboard", "CanBo");
                    }
                    else if (model.SelectedRole == "Admin")
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
} 