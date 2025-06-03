using System;
using System.ComponentModel.DataAnnotations;

namespace NHOM10_DACS_QLVIPHAM.ViewModels
{
    public class YeuCauCapLaiTheViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn lý do yêu cầu cấp lại thẻ")]
        [Display(Name = "Lý do yêu cầu cấp lại thẻ")]
        public string LyDo { get; set; } = string.Empty;
        
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn hình thức nhận thẻ")]
        [Display(Name = "Hình thức nhận thẻ")]
        public string HinhThucNhan { get; set; } = "TuDenLay"; // "TuDenLay" hoặc "GuiBuuDien"
    }

    public class TraCuuTheCanCuocViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mã thẻ căn cước")]
        [Display(Name = "Mã thẻ căn cước")]
        public string MaTheCC { get; set; } = string.Empty;
    }
    
    public class CapNhatTheCanCuocViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mã thẻ căn cước")]
        [Display(Name = "Mã thẻ căn cước")]
        [StringLength(12, MinimumLength = 9, ErrorMessage = "Mã thẻ căn cước phải từ 9-12 ký tự")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Mã thẻ căn cước chỉ được chứa số")]
        public string MaTheCC { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Vui lòng nhập tên loại thẻ căn cước")]
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
        
        [Display(Name = "Quốc tịch")]
        public string? QuocTich { get; set; }
        
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }
        
        [Display(Name = "Ảnh mặt trước hiện tại")]
        public string? AnhMatTruocHienTai { get; set; }
        
        [Display(Name = "Ảnh mặt sau hiện tại")]
        public string? AnhMatSauHienTai { get; set; }
    }
} 