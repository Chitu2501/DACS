using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHOM10_DACS_QLVIPHAM.Models
{
    public class DangKyCuTru
    {
        [Key]
        public int MaDangKy { get; set; }
        public string? LoaiCuTru { get; set; }
        public string? DiaChi { get; set; }
        public DateTime NgayDangKy { get; set; }
        public DateTime NgayHetHan { get; set; }
        public string? DiaChu { get; set; }
        public int MaCongDan { get; set; }
        public string? MaTheCC { get; set; }
    }
} 