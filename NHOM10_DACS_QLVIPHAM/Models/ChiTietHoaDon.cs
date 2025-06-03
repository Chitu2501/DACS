using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHOM10_DACS_QLVIPHAM.Models
{
    // =============================================
    // Model: ChiTietHoaDon (Invoice Detail)
    // Represents the details of an invoice, linking to a specific payment.
    // Properties:
    //   - MaChiTietHoaDon: Detail ID (Primary Key)
    //   - MaHoaDon: Invoice ID (Foreign Key)
    //   - MaThanhToan: Payment ID (Foreign Key)
    // Navigation:
    //   - HoaDon: Linked invoice entity
    //   - ThanhToanViPham: Linked payment record
    // =============================================
    public class ChiTietHoaDon
    {
        [Key]
        public int MaChiTietHoaDon { get; set; }
        
        [ForeignKey("HoaDon")]
        public int MaHoaDon { get; set; }
        
        [ForeignKey("ThanhToanViPham")]
        public int MaThanhToan { get; set; }
        
        // Navigation properties
        public virtual HoaDon? HoaDon { get; set; }
        public virtual ThanhToanViPham? ThanhToanViPham { get; set; }
    }
} 