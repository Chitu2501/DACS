using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHOM10_DACS_QLVIPHAM.Models
{
    // =============================================
    // Model: ThanhToanViPham (Violation Payment)
    // Represents a payment for a violation.
    // Properties:
    //   - MaThanhToan: Payment ID (Primary Key)
    //   - NgayThanhToan: Payment date
    //   - SoTien: Amount (decimal, precision 18,2)
    //   - PhuongThucThanhToan: Payment method
    //   - MaGiaoDich: Transaction code
    //   - NguoiThucHien: Executor
    //   - NganHang: Bank
    //   - MaNguoiDung: User ID (string)
    //   - MaCongDan: Citizen ID
    //   - MaTheCC: Citizen card ID
    // =============================================
    public class ThanhToanViPham
    {
        [Key]
        public int MaThanhToan { get; set; }
        
        public DateTime NgayThanhToan { get; set; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal SoTien { get; set; }
        
        public string? PhuongThucThanhToan { get; set; }
        
        public string? MaGiaoDich { get; set; }
        
        public string? NguoiThucHien { get; set; }
        
        public string? NganHang { get; set; }
        
        public string MaNguoiDung { get; set; } = string.Empty;
        
        public int MaCongDan { get; set; }
        
        public string? MaTheCC { get; set; }
    }
} 