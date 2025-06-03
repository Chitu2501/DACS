using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NHOM10_DACS_QLVIPHAM.ViewModels
{
    public class XetDuyetCapLaiTheViewModel
    {
        public int MaYeuCauCapLai { get; set; }
        
        [Display(Name = "Lý do yêu cầu")]
        public string? LyDo { get; set; }
        
        [Display(Name = "Ngày yêu cầu")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime NgayYeuCau { get; set; }
        
        public int MaCongDan { get; set; }
        
        [Display(Name = "Tên công dân")]
        public string? TenCongDan { get; set; }
        
        [Display(Name = "Mã thẻ căn cước")]
        public string? MaTheCC { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn kết quả xét duyệt")]
        [Display(Name = "Kết quả xét duyệt")]
        public string KetQua { get; set; } = "Đã duyệt";
        
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }
    }

    public class TraLoiKhieuNaiViewModel
    {
        public int MaKhieuNai { get; set; }
        public string NoiDungKhieuNai { get; set; } = string.Empty;
        [Required]
        public string NoiDungTraLoi { get; set; } = string.Empty;
        [Required]
        public string TrangThai { get; set; } = string.Empty;
        public string LyDoYeuCau { get; set; } = string.Empty;
        public DateTime NgayKhieuNai { get; set; }
        public int MaCongDan { get; set; }
        public string TenCongDan { get; set; } = string.Empty;
        public string LoaiKhieuNai { get; set; } = string.Empty;
        public int? MaBienBan { get; set; }
        public string TenBienBan { get; set; } = string.Empty;
        public string KetQuaXuLy { get; set; } = string.Empty;
    }

    public class XacNhanYeuCauCapLaiTheViewModel
    {
        public int MaYeuCau { get; set; }
        public string LyDo { get; set; } = string.Empty;
        [Required]
        public string TrangThai { get; set; } = string.Empty;
        public string? GhiChu { get; set; }
    }

    public class ThemViPhamViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên vi phạm")]
        [Display(Name = "Tên vi phạm")]
        public string TenViPham { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn loại vi phạm")]
        [Display(Name = "Loại vi phạm")]
        public int MaLoaiVPham { get; set; }

        public List<SelectListItem>? LoaiViPhams { get; set; }
    }

    public class ThemLoaiViPhamViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên loại vi phạm")]
        [Display(Name = "Tên loại vi phạm")]
        public string TenLoaiVPham { get; set; } = string.Empty;
        
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }
        
        [Display(Name = "Mức phạt tối thiểu")]
        [Range(0, double.MaxValue, ErrorMessage = "Mức phạt không được âm")]
        public decimal MucPhatToiThieu { get; set; }
        
        [Display(Name = "Mức phạt tối đa")]
        [Range(0, double.MaxValue, ErrorMessage = "Mức phạt không được âm")]
        public decimal MucPhatToiDa { get; set; }
    }

    public class ThemBienBanViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên biên bản")]
        [Display(Name = "Tên biên bản")]
        public string TenBienBan { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Vui lòng chọn vi phạm")]
        [Display(Name = "Vi phạm")]
        public int MaViPham { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn công dân")]
        [Display(Name = "Công dân")]
        public int MaCongDan { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập ngày lập biên bản")]
        [Display(Name = "Ngày lập biên bản")]
        [DataType(DataType.Date)]
        public DateTime NgayLapBienBan { get; set; } = DateTime.Now;
        
        [Display(Name = "Người lập")]
        public string NguoiLap { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [Display(Name = "Địa chỉ")]
        public string DiaChi { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Vui lòng nhập thời gian vi phạm")]
        [Display(Name = "Thời gian vi phạm")]
        [DataType(DataType.DateTime)]
        public DateTime ThoiGianVPham { get; set; } = DateTime.Now;
        
        [Required(ErrorMessage = "Vui lòng nhập nội dung vi phạm")]
        [Display(Name = "Nội dung vi phạm")]
        public string NoiDungVPham { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số tiền phạt")]
        [Display(Name = "Số tiền phạt")]
        [DataType(DataType.Currency)]
        [Range(0, 100000000, ErrorMessage = "Số tiền phạt phải lớn hơn 0 và nhỏ hơn 100,000,000")]
        public decimal SoTienPhat { get; set; }
        
        public List<SelectListItem>? ViPhams { get; set; }
        public List<SelectListItem>? CongDans { get; set; }
    }

    public class UploadBangChungKhieuNaiViewModel
    {
        public int MaKhieuNai { get; set; }
        
        [Display(Name = "Nội dung khiếu nại")]
        public string NoiDungKhieuNai { get; set; }
        
        [Display(Name = "Ngày khiếu nại")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime NgayKhieuNai { get; set; }
        
        public int MaCongDan { get; set; }
        
        [Display(Name = "Tên công dân")]
        public string TenCongDan { get; set; }
        
        public int? MaBienBan { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mô tả cho bằng chứng")]
        [Display(Name = "Mô tả bằng chứng")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string MoTa { get; set; }
        
        [Display(Name = "Cập nhật trạng thái khiếu nại")]
        public bool CapNhatTrangThai { get; set; } = true;
        
        [Display(Name = "Kết quả xử lý")]
        public string KetQuaXuLy { get; set; }
    }

    public class TaoCongDanViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} và tối đa {1} ký tự.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }

        [Display(Name = "Giới tính")]
        public string? GioiTinh { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [Display(Name = "Quốc tịch")]
        public string? QuocTich { get; set; }

        [Display(Name = "Dân tộc")]
        public string? DanToc { get; set; }

        [Display(Name = "Tôn giáo")]
        public string? TonGiao { get; set; }

        [Display(Name = "Quê quán")]
        public string? QueQuan { get; set; }

        [Display(Name = "Nơi thường trú")]
        public string? NoiThuongTru { get; set; }

        [Display(Name = "Công việc")]
        public string? CONGVIEC { get; set; }
    }

    public class ChinhSuaCongDanViewModel
    {
        public int MaCongDan { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ tên")]
        public string TenCongDan { get; set; }

        [Display(Name = "Địa chỉ")]
        public string DiaChi { get; set; }

        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SDT { get; set; }

        [Display(Name = "Số điện thoại tài khoản")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? SoDienThoai { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string GMAIL { get; set; }

        [Display(Name = "Giới tính")]
        public string GIOTTING { get; set; }

        [Display(Name = "Công việc")]
        public string CONGVIEC { get; set; }

        [Display(Name = "Quốc tịch")]
        public string QuocTich { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime NgaySinh { get; set; }

        [Display(Name = "Dân tộc")]
        public string DanToc { get; set; }

        [Display(Name = "Tôn giáo")]
        public string TonGiao { get; set; }

        [Display(Name = "Quê quán")]
        public string QueQuan { get; set; }

        [Display(Name = "Nơi thường trú")]
        public string NoiThuongTru { get; set; }

        [Display(Name = "Mã thẻ căn cước")]
        public string MaTheCC { get; set; }

        [Display(Name = "Đặc điểm nhận dạng")]
        [StringLength(500, ErrorMessage = "Đặc điểm nhận dạng không được vượt quá 500 ký tự")]
        public string DacDiemNhanDang { get; set; }

        [Display(Name = "Giấy tờ khác")]
        public string GiayKhac { get; set; }

        // Profile picture data
        [Display(Name = "Ảnh chân dung")]
        public string AnhChanDung { get; set; }

        // Account information if linked to an account
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; }
    }

    public class TaoCanBoViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập chức vụ")]
        [Display(Name = "Chức vụ")]
        public string ChucVu { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn phòng ban")]
        [Display(Name = "Phòng ban")]
        public int MaPhongBan { get; set; }

        public List<SelectListItem>? PhongBans { get; set; }
    }
} 