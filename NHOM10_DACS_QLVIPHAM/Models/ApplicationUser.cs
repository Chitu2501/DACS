using Microsoft.AspNetCore.Identity;

namespace NHOM10_DACS_QLVIPHAM.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? HoTen { get; set; }
        public string? ChucVu { get; set; }
        public string? DonViCongTac { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime? NgayCapNhat { get; set; }
    }
} 