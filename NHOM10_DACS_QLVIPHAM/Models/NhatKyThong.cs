using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHOM10_DACS_QLVIPHAM.Models
{
    public class NhatKyThong
    {
        [Key]
        public int MaNhatKy { get; set; }
        
        public int MaNguoiDung { get; set; }
        
        public int MaHoSo { get; set; }
        
        public string? DoiTuongTacDong { get; set; }
        
        public DateTime ThoiGian { get; set; }
        
        public string? DiaChi { get; set; }
        
        public bool TinhBi { get; set; }
        
        public string? MoTa { get; set; }
        
        public string? NoiDungHoatDong { get; set; }
        
        public string? NguoiThucHien { get; set; }
    }
} 