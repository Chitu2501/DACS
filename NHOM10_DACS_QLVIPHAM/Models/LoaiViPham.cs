using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHOM10_DACS_QLVIPHAM.Models
{
    public class LoaiViPham
    {
        [Key]
        public int MaLoaiVPham { get; set; }
        public string? TenLoaiVPham { get; set; }
        public string? MoTa { get; set; }
        public string? CoquanQuanLy { get; set; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal MucPhatToiThieu { get; set; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal MucPhatToiDa { get; set; }
    }
} 