using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHOM10_DACS_QLVIPHAM.Models
{
    public class ThongBao
    {
        [Key]
        public int MaThongBao { get; set; }

        [Required]
        [StringLength(255)]
        public string TieuDe { get; set; } = string.Empty;

        [Required]
        public string NoiDung { get; set; } = string.Empty;

        // Database column is now called NgayTao
        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Map to the existing TrangThai column in the database
        [Column("TrangThai")]
        public string? TrangThai { get; set; } = "CHUADOC";

        // This property will be used in the application
        [NotMapped]
        public bool DaDoc
        {
            get => TrangThai == "DADOC";
            set => TrangThai = value ? "DADOC" : "CHUADOC";
        }

        [StringLength(50)]
        public string LoaiThongBao { get; set; } = "HE_THONG"; // HE_THONG, THANH_TOAN, XAC_NHAN

        // Liên kết với người dùng (null nếu thông báo chung hoặc cho tất cả)
        // This will be handled via CongDan's MaNguoiDung
        [NotMapped]
        public string? UserId { get; set; }
        
        [NotMapped]
        public ApplicationUser? User { get; set; }

        // Liên kết với công dân (sử dụng cột MaCongDan trong DB)
        // MaCongDan is required in the DB, so we'll use CongDan to get to User
        public int MaCongDan { get; set; }
        
        [ForeignKey("MaCongDan")]
        public CongDan? CongDan { get; set; }

        // Liên kết với biên bản vi phạm (null nếu thông báo không liên quan đến biên bản)
        [NotMapped]
        public int? MaBienBan { get; set; }
        
        [NotMapped]
        public BienBanVPham? BienBanVPham { get; set; }

        // URL để điều hướng khi click vào thông báo
        [NotMapped]
        public string LinkChiTiet { get; set; } = string.Empty;

        // Dành cho thông báo có role cụ thể
        [NotMapped]
        public string? Role { get; set; }
    }
} 