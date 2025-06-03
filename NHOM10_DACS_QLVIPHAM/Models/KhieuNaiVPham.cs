using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHOM10_DACS_QLVIPHAM.Models
{
    public class KhieuNaiVPham
    {
        [Key]
        public int MaKhieuNai { get; set; }
        public DateTime NgayKhieuNai { get; set; }
        public string? NoiDungKhieuNai { get; set; }
        public string? DanhGiaKetQua { get; set; }
        public string? TrangThai { get; set; }
        public string? KetQuaXuLy { get; set; }
        public DateTime? NgayTraLoi { get; set; }
        public string? NguyenXuLy { get; set; }
        [ForeignKey("CongDan")]
        public int MaCongDan { get; set; }
        public string? MaTheCC { get; set; }
        
        [ForeignKey("BienBanVPham")]
        public int? MaBienBan { get; set; }
        
        // New fields for combined functionality
        public string LoaiKhieuNai { get; set; } = "ThanhToan"; // "ThanhToan" hoặc "BangChung"
        public string? LyDoYeuCau { get; set; } // For evidence requests
        public string? GhiChu { get; set; } // For officer notes
        public string? DuongDanBangChung { get; set; } // Path to evidence files

        // Navigation properties
        public virtual CongDan? CongDan { get; set; }
        public virtual BienBanVPham? BienBanVPham { get; set; }
    }
} 