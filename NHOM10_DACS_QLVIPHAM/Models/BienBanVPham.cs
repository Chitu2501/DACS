using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHOM10_DACS_QLVIPHAM.Models
{
    public class BienBanVPham
    {
        [Key]
        public int MaBienBan { get; set; }
        
        [ForeignKey("ViPham")]
        public int MaViPham { get; set; }
        
        public string? TenBienBan { get; set; }
        public DateTime NgayLapBienBan { get; set; }
        public DateTime? NgayLadDuBienBan { get; set; }
        public string? NguoiLap { get; set; }
        public string? DiaChi { get; set; }
        public DateTime ThoiGianVPham { get; set; }
        public string? NoiDungVPham { get; set; }
        public string? KetQuaXuLy { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal SoTienPhat { get; set; }
        public DateTime? NgayThanhToan { get; set; }
        
        [ForeignKey("CongDan")]
        public int MaCongDan { get; set; }
        
        // Thêm số căn cước công dân để tìm kiếm
        public string? MaTheCC { get; set; }
        
        // Navigation properties
        public virtual ViPham? ViPham { get; set; }
        public virtual CongDan? CongDan { get; set; }
    }
} 