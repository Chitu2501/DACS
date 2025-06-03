using System;
using System.ComponentModel.DataAnnotations;

namespace NHOM10_DACS_QLVIPHAM.ViewModels
{
    public class ThanhToanViPhamViewModel
    {
        public int MaBienBan { get; set; }
        
        [Display(Name = "Tên biên bản")]
        public string? TenBienBan { get; set; }
        
        [Display(Name = "Ngày lập biên bản")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime NgayLapBienBan { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập số tiền")]
        [Display(Name = "Số tiền")]
        [Range(1, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0")]
        public decimal SoTien { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        [Display(Name = "Phương thức thanh toán")]
        public string PhuongThucThanhToan { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Vui lòng nhập tên ngân hàng")]
        [Display(Name = "Ngân hàng")]
        public string NganHang { get; set; } = string.Empty;
    }

    public class KhieuNaiViPhamViewModel
    {
        public int MaBienBan { get; set; }
        
        [Display(Name = "Tên biên bản")]
        public string? TenBienBan { get; set; }
        
        [Display(Name = "Ngày lập biên bản")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime NgayLapBienBan { get; set; }
        
        [Display(Name = "Nội dung vi phạm")]
        public string? NoiDungViPham { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập nội dung khiếu nại")]
        [Display(Name = "Nội dung khiếu nại")]
        [StringLength(500, ErrorMessage = "Nội dung khiếu nại không được vượt quá 500 ký tự")]
        public string NoiDungKhieuNai { get; set; } = string.Empty;
    }

    public class TraCuuViPhamViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mã thẻ căn cước")]
        [Display(Name = "Mã thẻ căn cước")]
        public string MaTheCC { get; set; } = string.Empty;
    }
} 