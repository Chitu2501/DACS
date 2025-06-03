using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NHOM10_DACS_QLVIPHAM.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<NhatKyThong> NhatKyThongs { get; set; } = null!;
        public DbSet<ThongKeDoanhThu> ThongKeDoanhThus { get; set; } = null!;
        public DbSet<HoaDon> HoaDons { get; set; } = null!;
        public DbSet<ThanhToanViPham> ThanhToanViPhams { get; set; } = null!;
        public DbSet<BangChungViPham> BangChungViPhams { get; set; } = null!;
        public DbSet<CameraGiamSat> CameraGiamSats { get; set; } = null!;
        public DbSet<NguoiDung> NguoiDungs { get; set; } = null!;
        public DbSet<ThongBao> ThongBaos { get; set; } = null!;
        public DbSet<LoaiViPham> LoaiViPhams { get; set; } = null!;
        public DbSet<ViPham> ViPhams { get; set; } = null!;
        public DbSet<DangKyCuTru> DangKyCuTrus { get; set; } = null!;
        public DbSet<CongDan> CongDans { get; set; } = null!;
        public DbSet<BienBanVPham> BienBanVPhams { get; set; } = null!;
        public DbSet<TheCanCuoc> TheCanCuocs { get; set; } = null!;
        public DbSet<CapLaiTheCanCuoc> CapLaiTheCanCuocs { get; set; } = null!;
        public DbSet<XetDuyetCapLaiThe> XetDuyetCapLaiThes { get; set; } = null!;
        public DbSet<KhieuNaiVPham> KhieuNaiVPhams { get; set; } = null!;
        public DbSet<PhongBan> PhongBans { get; set; } = null!;
        public DbSet<CanBo> CanBos { get; set; } = null!;
        public DbSet<YeuCauCapLaiThe> YeuCauCapLaiThes { get; set; } = null!;
        public DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; } = null!;
        public DbSet<YeuCauCapNhatThongTin> YeuCauCapNhatThongTins { get; set; } = null!;
        public DbSet<LichSuThanhToan> LichSuThanhToans { get; set; } = null!;
        public DbSet<YeuCauBangChungViPham> YeuCauBangChungViPhams { get; set; } = null!;
        public DbSet<BangChungKhieuNai> BangChungKhieuNais { get; set; } = null!;
        public DbSet<PhuongXa> PhuongXas { get; set; } = null!;
        public DbSet<QuanHuyen> QuanHuyens { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define relationships here
            modelBuilder.Entity<ChiTietHoaDon>()
                .HasOne(c => c.HoaDon)
                .WithMany()
                .HasForeignKey(c => c.MaHoaDon)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ChiTietHoaDon>()
                .HasOne(c => c.ThanhToanViPham)
                .WithMany()
                .HasForeignKey(c => c.MaThanhToan)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ViPham>()
                .HasOne(v => v.LoaiViPham)
                .WithMany()
                .HasForeignKey(v => v.MaLoaiVPham)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure TheCanCuoc as having a string primary key
            modelBuilder.Entity<TheCanCuoc>()
                .HasKey(t => t.MaTheCC);
                
            // Cấu hình cho LichSuThanhToan
            modelBuilder.Entity<LichSuThanhToan>()
                .ToTable("LichSuThanhToans");
                
            modelBuilder.Entity<LichSuThanhToan>()
                .HasOne(l => l.BienBanVPham)
                .WithMany()
                .HasForeignKey(l => l.MaBienBan)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 