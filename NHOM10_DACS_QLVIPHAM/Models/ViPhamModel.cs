using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHOM10_DACS_QLVIPHAM.Models
{
    public class ViPhamModel
    {
        [Key]
        public int MaViPham { get; set; }
        
        public string? TenViPham { get; set; }
        
        [ForeignKey("LoaiViPham")]
        public int MaLoaiVPham { get; set; }
        
        // Navigation property
        public virtual LoaiViPham? LoaiViPham { get; set; }
    }
} 