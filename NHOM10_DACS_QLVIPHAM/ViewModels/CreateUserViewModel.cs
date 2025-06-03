using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NHOM10_DACS_QLVIPHAM.ViewModels
{
    public class CreateUserViewModel
    {
        [Required]
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Quyền (Role)")]
        public string Role { get; set; }

        [Required]
        [Display(Name = "Phòng ban")]
        public int? MaPhongBan { get; set; }

        public List<SelectListItem>? PhongBans { get; set; }
    }
} 