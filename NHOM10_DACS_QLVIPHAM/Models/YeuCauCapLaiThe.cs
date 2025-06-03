using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHOM10_DACS_QLVIPHAM.Models
{
    public class YeuCauCapLaiThe
    {
        [Key]
        public int MaYeuCauCapLai { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public string? LyDo { get; set; }
        public string? TrangThai { get; set; }
        [ForeignKey("CongDan")]
        public int MaCongDan { get; set; }
        public string? MaTheCC { get; set; }

        // Navigation properties
        public virtual CongDan? CongDan { get; set; }
    }
} 