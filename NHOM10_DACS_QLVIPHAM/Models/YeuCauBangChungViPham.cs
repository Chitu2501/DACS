using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHOM10_DACS_QLVIPHAM.Models
{
    public class YeuCauBangChungViPham
    {
        [Key]
        public int MaYeuCau { get; set; }
        
        [ForeignKey("BienBanVPham")]
        public int MaBienBan { get; set; }
        
        [ForeignKey("CongDan")]
        public int MaCongDan { get; set; }
        
        public DateTime NgayYeuCau { get; set; }
        
        public string? LyDoYeuCau { get; set; }
        
        public string? TrangThai { get; set; } // "Chờ xử lý", "Đã cung cấp", "Từ chối"
        
        public DateTime? NgayXuLy { get; set; }
        
        public string? GhiChu { get; set; }
        
        public string? NguoiXuLy { get; set; }
        
        public string? DuongDanBangChung { get; set; } // Lưu đường dẫn đến bằng chứng (Base64 hoặc URL)
        
        // Navigation properties
        public virtual BienBanVPham? BienBanVPham { get; set; }
        public virtual CongDan? CongDan { get; set; }
    }
} 