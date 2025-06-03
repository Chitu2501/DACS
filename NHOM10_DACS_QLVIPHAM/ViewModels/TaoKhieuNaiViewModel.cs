using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NHOM10_DACS_QLVIPHAM.ViewModels
{
    public class TaoKhieuNaiViewModel
    {
        public int MaBienBan { get; set; }
        
        public string? TenBienBan { get; set; }
        
        [Display(Name = "Ngày lập biên bản")]
        [DataType(DataType.Date)]
        public DateTime NgayLapBienBan { get; set; }
        
        [Display(Name = "Số tiền phạt")]
        [DataType(DataType.Currency)]
        public decimal SoTienPhat { get; set; }
        
        [Display(Name = "Nội dung vi phạm")]
        public string? NoiDungViPham { get; set; }
        
        [Display(Name = "Loại khiếu nại")]
        [Required(ErrorMessage = "Vui lòng chọn loại khiếu nại")]
        public string LoaiKhieuNai { get; set; } = "ThanhToan"; // "ThanhToan" hoặc "BangChung"
        
        [Display(Name = "Nội dung khiếu nại")]
        [MinLength(20, ErrorMessage = "Nội dung khiếu nại phải có ít nhất 20 ký tự")]
        [MaxLength(500, ErrorMessage = "Nội dung khiếu nại không được vượt quá 500 ký tự")]
        public string NoiDungKhieuNai { get; set; } = "";
        
        [Display(Name = "Lý do yêu cầu bằng chứng")]
        [StringLength(500, ErrorMessage = "Lý do không được quá 500 ký tự")]
        public string? LyDoYeuCau { get; set; }
        
        [Display(Name = "File bằng chứng (nếu có)")]
        public IFormFile? FileBangChung { get; set; }
    }
} 