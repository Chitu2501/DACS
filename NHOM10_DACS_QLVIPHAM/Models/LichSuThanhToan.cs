using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHOM10_DACS_QLVIPHAM.Models
{
    public class LichSuThanhToan
    {
        [Key]
        public int MaLichSu { get; set; }
        
        [ForeignKey("BienBanVPham")]
        public int MaBienBan { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal SoTien { get; set; }
        
        public string? PhuongThucThanhToan { get; set; }
        
        [Required]
        public DateTime NgayThanhToan { get; set; }
        
        public string? MaGiaoDich { get; set; }
        
        // Navigation property
        public virtual BienBanVPham? BienBanVPham { get; set; }
    }
} 