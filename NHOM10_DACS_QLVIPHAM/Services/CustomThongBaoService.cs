using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NHOM10_DACS_QLVIPHAM.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace NHOM10_DACS_QLVIPHAM.Services
{
    // This class extends ThongBaoService to maintain backward compatibility
    // with older code that calls TaoThongBao instead of TaoThongBaoChoNguoiDungAsync
    public class CustomThongBaoService : ThongBaoService
    {
        private readonly ApplicationDbContext _dbContext;
        
        public CustomThongBaoService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
            : base(context, userManager)
        {
            _dbContext = context;
        }

        // Override the TaoThongBao method from base class
        public override async Task<ThongBao> TaoThongBao(
            int maCongDan,
            string tieuDe,
            string noiDung,
            string loaiThongBao,
            string userId)
        {
            // Find the CongDan to get their MaNguoiDung if not provided
            if (string.IsNullOrEmpty(userId))
            {
                var congDan = await _dbContext.CongDans.FindAsync(maCongDan);
                if (congDan != null)
                {
                    userId = congDan.MaNguoiDung;
                }
            }

            // Map the loaiThongBao string to the correct format expected by TaoThongBaoChoNguoiDungAsync
            string mappedLoaiThongBao = MapLoaiThongBao(loaiThongBao);

            // Call the actual implementation
            return await TaoThongBaoChoNguoiDungAsync(
                userId,
                tieuDe,
                noiDung,
                mappedLoaiThongBao,
                "", // No link detail provided
                null, // No MaBienBan provided
                maCongDan
            );
        }

        private string MapLoaiThongBao(string loaiThongBao)
        {
            // Map old string values to new enum-like string values
            switch (loaiThongBao.ToUpper())
            {
                case "YÊU CẦU":
                case "YEU CAU":
                    return "YEU_CAU";
                case "THANH TOÁN":
                case "THANH TOAN":
                    return "THANH_TOAN";
                case "XÁC NHẬN":
                case "XAC NHAN":
                    return "XAC_NHAN";
                default:
                    return "HE_THONG";
            }
        }

        public Task<IEnumerable<ViewModels.NotificationModel>> GetUserNotificationsAsync(string username)
        {
            // Nếu không dùng service này thì có thể throw NotImplementedException hoặc trả về rỗng
            return Task.FromResult(Enumerable.Empty<ViewModels.NotificationModel>());
        }
    }
} 