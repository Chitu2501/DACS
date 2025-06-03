using NHOM10_DACS_QLVIPHAM.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NHOM10_DACS_QLVIPHAM.Services
{
    public interface IThongBaoService
    {
        Task<ThongBao> TaoThongBaoChoNguoiDungAsync(string userId, string tieuDe, string noiDung, 
            string loaiThongBao = "HE_THONG", string linkChiTiet = "", int? maBienBan = null, int? maCongDan = null);
            
        Task<ThongBao> TaoThongBao(int maCongDan, string tieuDe, string noiDung, string loaiThongBao, string userId);
            
        Task TaoThongBaoChoRoleAsync(string roleName, string tieuDe, string noiDung, 
            string loaiThongBao = "HE_THONG", string linkChiTiet = "", int? maBienBan = null);
            
        Task TaoThongBaoThanhToanAsync(int maBienBan, string userId, bool isCanBo = false);
        
        Task TaoThongBaoXacNhanThanhToanAsync(int maBienBan);
        
        Task TaoThongBaoBangChungKhieuNaiAsync(int maKhieuNai, string mota);
        
        Task DanhDauDaDocAsync(int maThongBao);
        
        Task<List<ThongBao>> LayThongBaoNguoiDungAsync(string userId, int limit = 5);
        
        Task<int> DemThongBaoChuaDocAsync(string userId);

        Task<IEnumerable<ViewModels.NotificationModel>> GetUserNotificationsAsync(string username);
    }
} 