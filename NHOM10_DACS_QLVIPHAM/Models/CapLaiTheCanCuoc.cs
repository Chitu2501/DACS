using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHOM10_DACS_QLVIPHAM.Models
{
    public class CapLaiTheCanCuoc
    {
        [Key]
        public int MaCapLai { get; set; }
        public DateTime NgayCapLai { get; set; }
        public string? LyDo { get; set; }
        public string? TrangThai { get; set; }
        public int MaCongDan { get; set; }
        public string? MaTheCC { get; set; }
    }
} 