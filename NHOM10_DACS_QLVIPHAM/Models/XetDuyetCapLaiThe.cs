using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHOM10_DACS_QLVIPHAM.Models
{
    public class XetDuyetCapLaiThe
    {
        [Key]
        public int MaXetDuyet { get; set; }
        public DateTime NgayXetDuyet { get; set; }
        public string? KetQua { get; set; }
        public string? GhiChu { get; set; }
        public int MaYeuCauCapLai { get; set; }
        public int MaCanBo { get; set; }
    }
} 