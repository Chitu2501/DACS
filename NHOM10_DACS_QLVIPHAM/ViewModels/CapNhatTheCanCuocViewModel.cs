using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace NHOM10_DACS_QLVIPHAM.ViewModels
{
    public class CapNhatTheCanCuocCanBoViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mã thẻ căn cước")]
        [Display(Name = "Mã thẻ căn cước")]
        public string MaTheCC { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập tên loại thẻ")]
        [Display(Name = "Tên loại thẻ")]
        public string TenCC { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn ngày cấp")]
        [Display(Name = "Ngày cấp")]
        [DataType(DataType.Date)]
        public DateTime NgayCap { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn ngày hết hạn")]
        [Display(Name = "Ngày hết hạn")]
        [DataType(DataType.Date)]
        public DateTime NgayHetHan { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập nơi cấp")]
        [Display(Name = "Nơi cấp")]
        public string NoiCap { get; set; }
        
        [Display(Name = "Ghi chú")]
        public string GhiChu { get; set; }
        
        [Display(Name = "Quốc tịch")]
        [Required(ErrorMessage = "Vui lòng nhập quốc tịch")]
        public string QuocTich { get; set; }
        
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; }
        
        [Display(Name = "Ảnh mặt trước thẻ")]
        public IFormFile AnhMatTruoc { get; set; }
        
        [Display(Name = "Ảnh mặt sau thẻ")]
        public IFormFile AnhMatSau { get; set; }
        
        // For displaying existing images
        public string AnhMatTruocHienTai { get; set; }
        public string AnhMatSauHienTai { get; set; }
    }
} 