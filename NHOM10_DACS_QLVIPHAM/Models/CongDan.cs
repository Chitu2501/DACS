using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHOM10_DACS_QLVIPHAM.Models
{
    public class CongDan
    {
        [Key]
        public int MaCongDan { get; set; }
        public string? TenCongDan { get; set; }
        public string? DiaChi { get; set; }
        public string? SDT { get; set; }
        public string? GMAIL { get; set; }
        public string? GIOTTING { get; set; }
        public string? CONGVIEC { get; set; }
        public string? GiayKhac { get; set; }
        public string? QuocTich { get; set; }
        public DateTime NgaySinh { get; set; }
        public string? DanToc { get; set; }
        public string? TonGiao { get; set; }
        public string? QueQuan { get; set; }
        public string? NoiThuongTru { get; set; }
        public string? DacDiemNhanDang { get; set; }
        public string? AnhChanDung { get; set; }
        public string? MaTheCC { get; set; }
        public string? MaNguoiDung { get; set; }

        [ForeignKey("MaNguoiDung")]
        public virtual ApplicationUser? ApplicationUser { get; set; }
        
        [NotMapped]
        public string? GioiTinh
        {
            get { return GIOTTING; }
            set { GIOTTING = value; }
        }

        [NotMapped]
        public string? UserId
        {
            get { return MaNguoiDung; }
            set { MaNguoiDung = value; }
        }
    }
} 