using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHOM10_DACS_QLVIPHAM.Models
{
    // =============================================
    // Model: HoaDon (Invoice)
    // Represents an invoice for payment in the system.
    // Properties:
    //   - MaHoaDon: Invoice ID (Primary Key)
    //   - TenHoaDon: Invoice name
    //   - NgayLap: Creation date
    //   - SoTien: Amount (decimal, precision 18,2)
    //   - NoiDung: Content/description
    //   - MaCongDan: Linked citizen (Foreign Key)
    //   - MaNguoiDung: Linked user (string)
    // Navigation:
    //   - CongDan: Linked citizen entity
    //   - ChiTietHoaDons: List of invoice details (ChiTietHoaDon)
    // =============================================
    public class HoaDon
    {
        public HoaDon()
        {
            ChiTietHoaDons = new List<ChiTietHoaDon>();
        }
        
        [Key]
        public int MaHoaDon { get; set; }
        
        public string? TenHoaDon { get; set; }
        
        public DateTime NgayLap { get; set; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal SoTien { get; set; }
        
        public string? NoiDung { get; set; }
        
        [ForeignKey("CongDan")]
        public int MaCongDan { get; set; }
        
        public string MaNguoiDung { get; set; } = string.Empty;
        
        // Navigation properties
        public virtual CongDan? CongDan { get; set; }
        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; }
    }
} 