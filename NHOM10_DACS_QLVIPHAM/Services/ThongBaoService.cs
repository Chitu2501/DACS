using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NHOM10_DACS_QLVIPHAM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHOM10_DACS_QLVIPHAM.Services
{
    public class ThongBaoService : IThongBaoService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ThongBaoService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Tạo thông báo cho một người dùng cụ thể
        public async Task<ThongBao> TaoThongBaoChoNguoiDungAsync(string userId, string tieuDe, string noiDung, 
            string loaiThongBao = "HE_THONG", string linkChiTiet = "", int? maBienBan = null, int? maCongDan = null)
        {
            // Find CongDan for this user
            CongDan? congDan = null;
            if (maCongDan.HasValue)
            {
                congDan = await _context.CongDans.FindAsync(maCongDan.Value);
            }
            else
            {
                // Try to find CongDan related to this user
                congDan = await _context.CongDans
                    .FirstOrDefaultAsync(c => c.MaNguoiDung == userId);
            }

            if (congDan == null)
            {
                // Tìm thông tin người dùng
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new InvalidOperationException("Không thể tạo thông báo: Không tìm thấy người dùng");
                }

                // Nếu người dùng thuộc role CanBo, tạo một CongDan mới cho họ
                if (await _userManager.IsInRoleAsync(user, "CanBo"))
                {
                    congDan = new CongDan
                    {
                        TenCongDan = user.HoTen ?? user.UserName,
                        MaNguoiDung = userId,
                        DiaChi = "Chưa cập nhật",
                        GMAIL = user.Email,
                        SDT = user.PhoneNumber,
                        NgaySinh = DateTime.Now
                    };

                    _context.CongDans.Add(congDan);
                    await _context.SaveChangesAsync();
                    
                    Console.WriteLine($"Đã tạo CongDan mới cho cán bộ {user.UserName} với ID: {congDan.MaCongDan}");
                }
                else
                {
                    throw new InvalidOperationException("Không thể tạo thông báo: Người dùng không có thông tin công dân");
                }
            }

            var thongBao = new ThongBao
            {
                TieuDe = tieuDe,
                NoiDung = noiDung,
                NgayTao = DateTime.Now,
                DaDoc = false,
                LoaiThongBao = loaiThongBao,
                MaCongDan = congDan.MaCongDan,
                TrangThai = "CHUADOC"
            };

            // Store UserId and other properties for application usage
            thongBao.UserId = userId;
            thongBao.LinkChiTiet = linkChiTiet;
            thongBao.MaBienBan = maBienBan;

            await _context.ThongBaos.AddAsync(thongBao);
            await _context.SaveChangesAsync();

            Console.WriteLine($"Đã tạo thông báo mới cho người dùng {userId}, ID thông báo: {thongBao.MaThongBao}");

            return thongBao;
        }

        // Tạo thông báo cho một role cụ thể
        public async Task TaoThongBaoChoRoleAsync(string roleName, string tieuDe, string noiDung, 
            string loaiThongBao = "HE_THONG", string linkChiTiet = "", int? maBienBan = null)
        {
            Console.WriteLine($"Bắt đầu tạo thông báo cho role {roleName}");
            
            // Lấy tất cả user thuộc role
            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
            Console.WriteLine($"Tìm thấy {usersInRole.Count} người dùng thuộc role {roleName}");
            
            var congDanIds = new List<int?>();
            
            // Nếu có biên bản, lấy công dân liên quan
            if (maBienBan.HasValue)
            {
                var bienBan = await _context.BienBanVPhams
                    .Include(b => b.CongDan)
                    .FirstOrDefaultAsync(b => b.MaBienBan == maBienBan.Value);
                
                if (bienBan?.CongDan != null)
                {
                    congDanIds.Add(bienBan.CongDan.MaCongDan);
                }
            }

            // Tạo thông báo cho từng người dùng trong role
            foreach (var user in usersInRole)
            {
                try
                {
                    var congDan = await _context.CongDans
                        .FirstOrDefaultAsync(c => c.MaNguoiDung == user.Id);
                    
                    // Để debug
                    if (congDan == null)
                    {
                        Console.WriteLine($"Không tìm thấy CongDan cho user {user.UserName} (ID: {user.Id})");
                    }
                    else
                    {
                        Console.WriteLine($"Tìm thấy CongDan ID: {congDan.MaCongDan} cho user {user.UserName}");
                    }
                        
                    await TaoThongBaoChoNguoiDungAsync(
                        user.Id, 
                        tieuDe, 
                        noiDung, 
                        loaiThongBao, 
                        linkChiTiet, 
                        maBienBan,
                        congDan?.MaCongDan
                    );
                }
                catch (Exception ex)
                {
                    // Log the error but continue with other users
                    Console.WriteLine($"Lỗi khi tạo thông báo cho user {user.Id}: {ex.Message}");
                }
            }

            // Nếu có công dân liên quan, tạo thông báo cho đúng user của công dân đó
            foreach (var congDanId in congDanIds.Where(id => id.HasValue))
            {
                var congDan = await _context.CongDans
                    .FirstOrDefaultAsync(c => c.MaCongDan == congDanId);
                
                if (congDan != null && !string.IsNullOrEmpty(congDan.MaNguoiDung))
                {
                    try
                    {
                        await TaoThongBaoChoNguoiDungAsync(
                            congDan.MaNguoiDung,
                            tieuDe,
                            noiDung,
                            loaiThongBao,
                            linkChiTiet,
                            maBienBan,
                            congDan.MaCongDan
                        );
                    }
                    catch (Exception ex)
                    {
                        // Log the error but continue
                        Console.WriteLine($"Lỗi khi tạo thông báo cho công dân {congDan.MaCongDan}: {ex.Message}");
                    }
                }
            }
            
            Console.WriteLine($"Hoàn thành tạo thông báo cho role {roleName}");
        }

        // Tạo thông báo thanh toán
        public async Task TaoThongBaoThanhToanAsync(int maBienBan, string userId, bool isCanBo = false)
        {
            var bienBan = await _context.BienBanVPhams
                .Include(b => b.CongDan)
                .Include(b => b.ViPham)
                .FirstOrDefaultAsync(b => b.MaBienBan == maBienBan);

            if (bienBan == null)
                return;

            if (isCanBo)
            {
                // Tạo thông báo cho cán bộ xử lý
                await TaoThongBaoChoRoleAsync(
                    "CanBo",
                    "Có biên bản vi phạm cần xác nhận thanh toán",
                    $"Biên bản {bienBan.TenBienBan} của công dân {bienBan.CongDan?.TenCongDan} đã thanh toán và đang chờ xác nhận.",
                    "THANH_TOAN",
                    $"/CanBo/ChiTietBienBan/{maBienBan}",
                    maBienBan
                );
            }
            else
            {
                if (bienBan.CongDan != null)
                {
                    try 
                    {
                        // Tạo thông báo cho người dùng thanh toán
                        await TaoThongBaoChoNguoiDungAsync(
                            userId,
                            "Đã nhận thanh toán vi phạm",
                            $"Thanh toán cho biên bản {bienBan.TenBienBan} đã được ghi nhận và đang được xử lý. Vui lòng chờ xác nhận từ cán bộ.",
                            "THANH_TOAN",
                            $"/NguoiDung/ChiTietBienBan/{maBienBan}",
                            maBienBan,
                            bienBan.CongDan.MaCongDan
                        );
                    }
                    catch (Exception ex)
                    {
                        // Log the error
                        Console.WriteLine($"Error creating notification: {ex.Message}");
                    }
                }
            }
        }

        // Tạo thông báo xác nhận thanh toán thành công
        public async Task TaoThongBaoXacNhanThanhToanAsync(int maBienBan)
        {
            var bienBan = await _context.BienBanVPhams
                .Include(b => b.CongDan)
                .Include(b => b.ViPham)
                .FirstOrDefaultAsync(b => b.MaBienBan == maBienBan);

            if (bienBan == null || bienBan.CongDan == null)
                return;

            // Tìm user liên quan đến công dân
            var congDan = bienBan.CongDan;
            if (!string.IsNullOrEmpty(congDan.MaNguoiDung))
            {
                try
                {
                    // Thông báo cho người dùng
                    await TaoThongBaoChoNguoiDungAsync(
                        congDan.MaNguoiDung,
                        "Thanh toán vi phạm thành công",
                        $"Thanh toán cho biên bản {bienBan.TenBienBan} đã được xác nhận thành công. Cảm ơn bạn đã thanh toán đúng hạn.",
                        "XAC_NHAN",
                        $"/NguoiDung/ChiTietBienBan/{maBienBan}",
                        maBienBan,
                        congDan.MaCongDan
                    );
                }
                catch (Exception ex)
                {
                    // Log the error
                    Console.WriteLine($"Error creating confirmation notification: {ex.Message}");
                }
            }
        }

        // Tạo thông báo khi có bằng chứng mới cho khiếu nại
        public async Task TaoThongBaoBangChungKhieuNaiAsync(int maKhieuNai, string mota)
        {
            // Tìm thông tin khiếu nại
            var khieuNai = await _context.KhieuNaiVPhams
                .Include(k => k.CongDan)
                .FirstOrDefaultAsync(k => k.MaKhieuNai == maKhieuNai);

            if (khieuNai == null || khieuNai.CongDan == null)
                return;

            // Tìm user liên quan đến công dân
            var congDan = khieuNai.CongDan;
            if (!string.IsNullOrEmpty(congDan.MaNguoiDung))
            {
                try
                {
                    // Thông báo cho người dùng
                    await TaoThongBaoChoNguoiDungAsync(
                        congDan.MaNguoiDung,
                        "Có bằng chứng mới cho khiếu nại của bạn",
                        $"Cán bộ đã cung cấp bằng chứng mới cho khiếu nại #{maKhieuNai}: {mota}",
                        "BangChungKhieuNai",
                        $"/CongDan/ChiTietKhieuNai/{maKhieuNai}",
                        khieuNai.MaBienBan,
                        congDan.MaCongDan
                    );
                }
                catch (Exception ex)
                {
                    // Log the error
                    Console.WriteLine($"Error creating evidence notification: {ex.Message}");
                }
            }
        }

        // Đánh dấu thông báo đã đọc
        public async Task DanhDauDaDocAsync(int maThongBao)
        {
            var thongBao = await _context.ThongBaos.FindAsync(maThongBao);
            if (thongBao != null)
            {
                thongBao.DaDoc = true;
                thongBao.TrangThai = "DADOC";
                await _context.SaveChangesAsync();
            }
        }

        // Lấy danh sách thông báo của người dùng
        public async Task<List<ThongBao>> LayThongBaoNguoiDungAsync(string userId, int limit = 5)
        {
            // Find CongDan related to this user
            var congDan = await _context.CongDans
                .FirstOrDefaultAsync(c => c.MaNguoiDung == userId);

            if (congDan == null)
                return new List<ThongBao>();

            var thongBaos = await _context.ThongBaos
                .Where(t => t.MaCongDan == congDan.MaCongDan)
                .OrderByDescending(t => t.NgayTao)
                .Take(limit)
                .ToListAsync();

            // Set the UserId property for application use
            foreach (var tb in thongBaos)
            {
                tb.UserId = userId;
            }

            return thongBaos;
        }

        // Đếm số thông báo chưa đọc
        public async Task<int> DemThongBaoChuaDocAsync(string userId)
        {
            // Find CongDan related to this user
            var congDan = await _context.CongDans
                .FirstOrDefaultAsync(c => c.MaNguoiDung == userId);

            if (congDan == null)
                return 0;

            return await _context.ThongBaos
                .CountAsync(t => t.MaCongDan == congDan.MaCongDan && t.TrangThai != "DADOC");
        }

        // Implement the TaoThongBao method required by the interface
        public virtual async Task<ThongBao> TaoThongBao(
            int maCongDan,
            string tieuDe,
            string noiDung,
            string loaiThongBao,
            string userId)
        {
            // Find the CongDan to get their MaNguoiDung if not provided
            if (string.IsNullOrEmpty(userId))
            {
                var congDan = await _context.CongDans.FindAsync(maCongDan);
                if (congDan != null)
                {
                    userId = congDan.MaNguoiDung;
                }
            }

            // Default implementation maps directly to TaoThongBaoChoNguoiDungAsync
            return await TaoThongBaoChoNguoiDungAsync(
                userId,
                tieuDe,
                noiDung,
                loaiThongBao,
                "", // No link detail provided
                null, // No MaBienBan provided
                maCongDan
            );
        }

        public async Task<IEnumerable<ViewModels.NotificationModel>> GetUserNotificationsAsync(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null) return Enumerable.Empty<ViewModels.NotificationModel>();

            var congDan = await _context.CongDans.FirstOrDefaultAsync(c => c.MaNguoiDung == user.Id);
            if (congDan == null) return Enumerable.Empty<ViewModels.NotificationModel>();

            return await _context.ThongBaos
                .Where(n => n.MaCongDan == congDan.MaCongDan)
                .OrderByDescending(n => n.NgayTao)
                .Select(n => new ViewModels.NotificationModel
                {
                    Title = n.TieuDe,
                    Message = n.NoiDung,
                    CreatedAt = n.NgayTao,
                    LinkChiTiet = n.LinkChiTiet
                })
                .Take(10)
                .ToListAsync();
        }
    }
} 