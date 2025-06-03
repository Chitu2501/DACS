using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHOM10_DACS_QLVIPHAM.Models
{
    public class TheCanCuoc
    {
        [Key]
        public string MaTheCC { get; set; } = string.Empty;
        
        [Display(Name = "Tên loại thẻ căn cước")]
        public string TenCC { get; set; } = string.Empty;
        
        [Display(Name = "Ngày cấp")]
        [DataType(DataType.Date)]
        public DateTime? NgayCap { get; set; }
        
        [Display(Name = "Ngày hết hạn")]
        [DataType(DataType.Date)]
        public DateTime? NgayHetHan { get; set; }
        
        [Display(Name = "Nơi cấp")]
        public string? NoiCap { get; set; }
        
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }
        
        [Display(Name = "Ảnh mặt trước")]
        public string? AnhMatTruoc { get; set; }
        
        [Display(Name = "Ảnh mặt sau")]
        public string? AnhMatSau { get; set; }
        
        [Display(Name = "Quốc tịch")]
        public string? QuocTich { get; set; }
        
        [Display(Name = "Trang thái")]
        public string? TrangThai { get; set; }
        
        // Create a relationship with CongDan (optional)
        [NotMapped]
        public CongDan? CongDan { get; set; }
    }
} 