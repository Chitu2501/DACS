using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHOM10_DACS_QLVIPHAM.Models
{
    public class PhuongXa
    {
        [Key]
        public int MaPhuongXa { get; set; }
        
        [Required]
        public string TenPhuongXa { get; set; } = string.Empty;
        
        [ForeignKey("QuanHuyen")]
        public int MaQuanHuyen { get; set; }
        public QuanHuyen? QuanHuyen { get; set; }
        
        public string LoaiDonVi { get; set; } = string.Empty;
        public string? MoTa { get; set; }
    }
} 