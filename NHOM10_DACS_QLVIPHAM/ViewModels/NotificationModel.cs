using System;

namespace NHOM10_DACS_QLVIPHAM.ViewModels
{
    public class NotificationModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public string LinkChiTiet { get; set; } = "#";
    }
} 