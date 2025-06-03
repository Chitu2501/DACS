using Microsoft.AspNetCore.Mvc;
using NHOM10_DACS_QLVIPHAM.Services;
using System.Threading.Tasks;

namespace NHOM10_DACS_QLVIPHAM.ViewComponents
{
    public class NotificationViewComponent : ViewComponent
    {
        private readonly IThongBaoService _thongBaoService;

        public NotificationViewComponent(IThongBaoService thongBaoService)
        {
            _thongBaoService = thongBaoService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var notifications = await _thongBaoService.GetUserNotificationsAsync(User.Identity.Name);
            return View(notifications);
        }
    }
} 