using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHOM10_DACS_QLVIPHAM.Models
{
    public class BangChungViPham
    {
        [Key]
        public int MaBangChung { get; set; }
        
        public int MaViPham { get; set; }
        
        public string? HinhThanhAnh { get; set; }
        
        public int MaBienBan { get; set; }
        
        public int MaThanhToan { get; set; }
    }
} 