using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHOM10_DACS_QLVIPHAM.Models
{
    public class YeuCauCapNhatThongTin
    {
        [Key]
        public int MaYeuCau { get; set; }
        
        public int? MaCongDan { get; set; }
        
        [ForeignKey("User")]
        public string? MaNguoiDung { get; set; }
        
        [Required]
        public DateTime NgayYeuCau { get; set; }
        
        [Required]
        public string NoiDungCapNhat { get; set; } = string.Empty;
        
        // JSON string containing the updated fields and values
        [Required]
        public string ThongTinCapNhat { get; set; } = string.Empty;
        
        [Required]
        public string TrangThai { get; set; } = "Chờ xét duyệt";
        
        public DateTime? NgayXuLy { get; set; }
        
        public string? NguoiXuLy { get; set; }
        
        public string? GhiChu { get; set; }
        
        // Navigation properties
        [ForeignKey("MaCongDan")]
        public virtual CongDan? CongDan { get; set; }
        
        public virtual ApplicationUser? User { get; set; }
    }
} 