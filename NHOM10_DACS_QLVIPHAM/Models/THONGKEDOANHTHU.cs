using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHOM10_DACS_QLVIPHAM.Models
{
    public class ThongKeDoanhThu
    {
        [Key]
        public int MaThongKe { get; set; }
        
        public DateTime NgayThang { get; set; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TongTien { get; set; }
        
        public int MaHoaDon { get; set; }
    }
} 