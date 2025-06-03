using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace NHOM10_DACS_QLVIPHAM.ViewModels
{
    public class YeuCauCapNhatThongTinViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập nội dung cập nhật")]
        [Display(Name = "Nội dung cập nhật")]
        public string NoiDungCapNhat { get; set; } = string.Empty;
        
        [Display(Name = "Họ và tên")]
        public string? HoTen { get; set; }
        
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }
        
        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? SDT { get; set; }
        
        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }
        
        [Display(Name = "Số thẻ căn cước")]
        public string? MaTheCC { get; set; }
        
        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }
        
        [Display(Name = "Giới tính")]
        public string? GioiTinh { get; set; }
        
        [Display(Name = "Nghề nghiệp")]
        public string? CongViec { get; set; }
    }
    
    public class XetDuyetCapNhatThongTinViewModel
    {
        public int MaYeuCau { get; set; }
        
        [Display(Name = "Nội dung cập nhật")]
        public string NoiDungCapNhat { get; set; } = string.Empty;
        
        [Display(Name = "Thông tin cập nhật")]
        public string ThongTinCapNhat { get; set; } = string.Empty;
        
        [Display(Name = "Ngày yêu cầu")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime NgayYeuCau { get; set; }
        
        [Display(Name = "Tên người dùng")]
        public string? TenNguoiDung { get; set; }
        
        [Display(Name = "Email")]
        public string? Email { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Đã duyệt";
        
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }
    }
} 