using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHOM10_DACS_QLVIPHAM.Models
{
    public class NguoiDung
    {
        [Key]
        public int MaNguoiDung { get; set; }
        
        [Required]
        [StringLength(50)]
        public string TenDangNhap { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string MatKhau { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? HoTen { get; set; }
        
        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }
        
        [Phone]
        [StringLength(20)]
        public string? SoDienThoai { get; set; }
        
        [StringLength(50)]
        public string? ChucVu { get; set; }
        
        [StringLength(100)]
        public string? DonViCongTac { get; set; }
        
        public string? YeuTo { get; set; }
        
        public string? TrangThai { get; set; }
        
        public DateTime NgayTao { get; set; } = DateTime.Now;
        
        public DateTime? NgayCachNhat { get; set; }
        
        public int? MaThongBao { get; set; }
    }
} 