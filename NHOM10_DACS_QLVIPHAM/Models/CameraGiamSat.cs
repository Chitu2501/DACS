using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHOM10_DACS_QLVIPHAM.Models
{
    public class CameraGiamSat
    {
        [Key]
        public int IDCam { get; set; }
        
        public string? ViTri { get; set; }
        
        public string? TrangThai { get; set; }
    }
} 