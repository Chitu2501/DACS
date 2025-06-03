using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHOM10_DACS_QLVIPHAM.Models
{
    public class BangChungKhieuNai
    {
        [Key]
        public int MaBangChung { get; set; }
        
        [ForeignKey("KhieuNaiVPham")]
        public int MaKhieuNai { get; set; }
        
        public string DuongDanFile { get; set; }
        
        public string MoTa { get; set; }
        
        public DateTime NgayTao { get; set; }
        
        public string NguoiTao { get; set; }
        
        // Navigation property
        public virtual KhieuNaiVPham KhieuNaiVPham { get; set; }
    }
} 