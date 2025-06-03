using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHOM10_DACS_QLVIPHAM.Models
{
    public class CanBo
    {
        [Key]
        public int MaCanBo { get; set; }
        public int MaPhongBan { get; set; }
        public string? TenCanBo { get; set; }
        public string? ChucVu { get; set; }
        public string? UserId { get; set; }  // Link with ApplicationUser
        public string? SoDienThoai { get; set; }
        public string? DiaChi { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string? GioiTinh { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
    }
} 