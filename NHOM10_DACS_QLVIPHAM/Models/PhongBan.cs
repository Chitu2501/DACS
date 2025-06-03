using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHOM10_DACS_QLVIPHAM.Models
{
    public class PhongBan
    {
        [Key]
        public int MaPhongBan { get; set; }
        public string? TenPhongBan { get; set; }
        public string? ChucVu { get; set; }
    }
} 