using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NHOM10_DACS_QLVIPHAM.Models;
using System.Linq;

namespace NHOM10_DACS_QLVIPHAM.ViewModels
{
    public class ThongKeThanhToanViewModel
    {
        [Display(Name = "Từ ngày")]
        [DataType(DataType.Date)]
        public DateTime? TuNgay { get; set; }

        [Display(Name = "Đến ngày")]
        [DataType(DataType.Date)]
        public DateTime? DenNgay { get; set; }

        [Display(Name = "Loại thống kê")]
        public string LoaiThongKe { get; set; } = "Ngay"; // "Ngay", "Thang", "Nam"

        public decimal TongTienDaThanhToan { get; set; }
        public decimal TongTienChuaThanhToan { get; set; }
        public int SoBienBanDaThanhToan { get; set; }
        public int SoBienBanChuaThanhToan { get; set; }

        public List<ThongKeTheoThoiGian> ThongKeTheoThoiGian { get; set; } = new List<ThongKeTheoThoiGian>();
    }

    public class ThongKeTheoThoiGian
    {
        public required string ThoiGian { get; set; } // Ngày/Tháng/Năm tùy thuộc vào loại thống kê
        public decimal TongTienDaThanhToan { get; set; }
        public decimal TongTienChuaThanhToan { get; set; }
        public int SoBienBanDaThanhToan { get; set; }
        public int SoBienBanChuaThanhToan { get; set; }
    }

    public class QuanLyBienBanThanhToanViewModel
    {
        [Display(Name = "Từ ngày")]
        [DataType(DataType.Date)]
        public DateTime? TuNgay { get; set; }

        [Display(Name = "Đến ngày")]
        [DataType(DataType.Date)]
        public DateTime? DenNgay { get; set; }

        [Display(Name = "Trạng thái thanh toán")]
        public string TrangThaiThanhToan { get; set; } = "TatCa"; // "TatCa", "DaThanhToan", "ChuaThanhToan"

        public List<BienBanVPham> DanhSachBienBan { get; set; } = new List<BienBanVPham>();
        
        public ThongKeThanhToanViewModel ThongKe { get; set; } = new ThongKeThanhToanViewModel();
        
        public int TongSoBienBan => DanhSachBienBan.Count;
        public int SoDaThanhtoan => DanhSachBienBan.Count(b => b.KetQuaXuLy == "Đã thanh toán");
        public int SoChuaThanhToan => DanhSachBienBan.Count(b => b.KetQuaXuLy != "Đã thanh toán");
        public decimal TongTienDaThu => DanhSachBienBan.Where(b => b.KetQuaXuLy == "Đã thanh toán").Sum(b => b.SoTienPhat);
        public decimal TongTienChuaThu => DanhSachBienBan.Where(b => b.KetQuaXuLy != "Đã thanh toán").Sum(b => b.SoTienPhat);
    }
} 