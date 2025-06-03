using System;
using System.ComponentModel.DataAnnotations;

namespace NHOM10_DACS_QLVIPHAM.ViewModels
{
    public class YeuCauBangChungViewModel
    {
        public int MaBienBan { get; set; }
        
        public string? TenBienBan { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập lý do yêu cầu")]
        [Display(Name = "Lý do yêu cầu")]
        [StringLength(500, ErrorMessage = "Lý do không được quá 500 ký tự")]
        public string LyDoYeuCau { get; set; } = string.Empty;
        
        public DateTime NgayYeuCau { get; set; } = DateTime.Now;
    }
    
    public class QuanLyYeuCauBangChungViewModel
    {
        public int MaYeuCau { get; set; }
        
        public int MaBienBan { get; set; }
        
        public string? TenBienBan { get; set; }
        
        public string? TenCongDan { get; set; }
        
        public string? MaTheCC { get; set; }
        
        public DateTime NgayYeuCau { get; set; }
        
        public string? LyDoYeuCau { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập ghi chú")]
        [Display(Name = "Ghi chú")]
        public string GhiChu { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Chờ xử lý";
        
        public string? DuongDanBangChung { get; set; }
    }
} 