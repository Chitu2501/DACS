using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHOM10_DACS_QLVIPHAM.Models
{
    public class QuanHuyen
    {
        [Key]
        public int MaQuanHuyen { get; set; }
        
        [Required]
        public string TenQuanHuyen { get; set; } = string.Empty;
        
        public string LoaiDonVi { get; set; } = string.Empty;
        public string? MoTa { get; set; }
    }
} 