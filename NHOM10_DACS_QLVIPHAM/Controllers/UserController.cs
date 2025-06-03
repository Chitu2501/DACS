using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NHOM10_DACS_QLVIPHAM.Models;
using System.Threading.Tasks;

namespace NHOM10_DACS_QLVIPHAM.Controllers
{
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UserController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewBag.UserName = user.HoTen ?? user.UserName;
            }
            else
            {
                ViewBag.UserName = "User";
            }
            return View();
        }

        public async Task<IActionResult> ThongTinCaNhan()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ThongTinCaNhanViewModel
            {
                UserName = user.UserName ?? "",
                Email = user.Email ?? "",
                HoTen = user.HoTen ?? "",
                PhoneNumber = user.PhoneNumber ?? ""
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThongTinCaNhan(ThongTinCaNhanViewModel model)
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

            user.Email = model.Email;
            user.HoTen = model.HoTen;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                return RedirectToAction(nameof(Dashboard));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
    }

    public class ThongTinCaNhanViewModel
    {
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public string HoTen { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
    }
} 