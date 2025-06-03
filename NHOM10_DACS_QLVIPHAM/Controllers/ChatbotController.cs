using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NHOM10_DACS_QLVIPHAM.Models;
using NHOM10_DACS_QLVIPHAM.Services;
using System.Linq;
using System.Threading.Tasks;

namespace NHOM10_DACS_QLVIPHAM.Controllers
{
    public class ChatbotController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IGeminiAIService _geminiAIService;

        public ChatbotController(ApplicationDbContext context, IGeminiAIService geminiAIService)
        {
            _context = context;
            _geminiAIService = geminiAIService;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string message)
        {
            try
            {
                var viPhams = await _context.ViPhams.Include(v => v.LoaiViPham).ToListAsync();
                var loaiViPhams = await _context.LoaiViPhams.ToListAsync();

                string FormatCurrency(decimal amount)
                {
                    return string.Format(System.Globalization.CultureInfo.GetCultureInfo("vi-VN"), "{0:C0}", amount);
                }

                // Check for guidance-related queries
                if (message.ToLower().Contains("tra cứu vi phạm") || message.ToLower().Contains("tìm vi phạm"))
                {
                    return Json(new { success = true, message = @"Để tra cứu vi phạm, bạn cần:
1. Truy cập vào mục 'Tra cứu vi phạm' từ trang chủ
2. Nhập mã căn cước công dân của bạn vào ô tìm kiếm
3. Nhấn nút 'Tra cứu' để xem kết quả
4. Hệ thống sẽ hiển thị danh sách các vi phạm (nếu có)" });
                }
                else if (message.ToLower().Contains("tra cứu thẻ căn cước") || message.ToLower().Contains("tìm thẻ căn cước"))
                {
                    return Json(new { success = true, message = @"Để tra cứu thẻ căn cước, bạn cần:
1. Truy cập vào mục 'Tra cứu thẻ căn cước' từ trang chủ
2. Nhập mã căn cước công dân của bạn vào ô tìm kiếm
3. Nhấn nút 'Tra cứu' để xem kết quả
4. Hệ thống sẽ hiển thị thông tin thẻ căn cước của bạn" });
                }
                else if (message.ToLower().Contains("đăng nhập") || message.ToLower().Contains("login"))
                {
                    return Json(new { success = true, message = @"Để đăng nhập hệ thống, bạn cần:
1. Nhấn vào nút 'Đăng nhập' ở góc phải trên cùng của trang
2. Nhập tên đăng nhập và mật khẩu của bạn
3. Nhấn nút 'Đăng nhập' để truy cập vào hệ thống
4. Sau khi đăng nhập, bạn sẽ được chuyển đến trang bảng điều khiển tương ứng với vai trò của mình" });
                }
                else if (message.ToLower().Contains("cấp lại thẻ căn cước") || message.ToLower().Contains("làm lại thẻ căn cước"))
                {
                    return Json(new { success = true, message = @"Để yêu cầu cấp lại thẻ căn cước, bạn cần:
1. Đăng nhập vào hệ thống với tài khoản công dân
2. Truy cập vào mục 'Cấp lại CCCD' từ trang bảng điều khiển
3. Điền đầy đủ thông tin vào biểu mẫu yêu cầu
4. Nhấn nút 'Gửi yêu cầu' để hoàn tất
5. Theo dõi trạng thái xét duyệt của yêu cầu trong mục thông báo" });
                }

                var prompt = $@"Bạn là một trợ lý ảo chuyên về vi phạm giao thông. Chỉ trả lời dựa trên thông tin sau:

Danh sách loại vi phạm:
{string.Join("\n", loaiViPhams.Select(l => $"- {l.TenLoaiVPham}: {l.MoTa ?? "Không có mô tả"}. Mức phạt: {FormatCurrency(l.MucPhatToiThieu)} - {FormatCurrency(l.MucPhatToiDa)}"))}

Danh sách vi phạm:
{string.Join("\n", viPhams.Select(v => $"- {v.TenViPham} (Loại: {v.LoaiViPham?.TenLoaiVPham})"))}

Câu hỏi của người dùng: {message}

Chỉ trả lời dựa trên dữ liệu trên. Nếu không có thông tin, hãy trả lời: 'Tôi không có thông tin về vi phạm này trong hệ thống.' Trả lời ngắn gọn, dễ hiểu, bằng tiếng Việt.";

                var response = await _geminiAIService.GetResponseAsync(prompt);
                return Json(new { success = true, message = response });
            }
            catch
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xử lý yêu cầu." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDetailedViPhamInfo(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return Json(new { error = "Vui lòng nhập từ khóa tìm kiếm" });
            }

            // Tìm kiếm trong cả bảng ViPham và LoaiViPham
            var viPhams = await _context.ViPhams
                .Include(v => v.LoaiViPham)
                .Where(v => v.TenViPham.Contains(query) || 
                           v.LoaiViPham.TenLoaiVPham.Contains(query) ||
                           v.LoaiViPham.MoTa.Contains(query))
                .Select(v => new
                {
                    v.MaViPham,
                    v.TenViPham,
                    LoaiViPham = new
                    {
                        v.LoaiViPham.MaLoaiVPham,
                        v.LoaiViPham.TenLoaiVPham,
                        v.LoaiViPham.MoTa,
                        v.LoaiViPham.CoquanQuanLy,
                        v.LoaiViPham.MucPhatToiThieu,
                        v.LoaiViPham.MucPhatToiDa
                    }
                })
                .ToListAsync();

            if (!viPhams.Any())
            {
                return Json(new { message = "Không tìm thấy thông tin vi phạm phù hợp" });
            }

            return Json(new { data = viPhams });
        }

        [HttpGet]
        public async Task<IActionResult> GetLoaiViPhamInfo(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return Json(new { error = "Vui lòng nhập từ khóa tìm kiếm" });
            }

            var loaiViPhams = await _context.LoaiViPhams
                .Where(l => l.TenLoaiVPham.Contains(query) || l.MoTa.Contains(query))
                .Select(l => new
                {
                    l.MaLoaiVPham,
                    l.TenLoaiVPham,
                    l.MoTa,
                    l.CoquanQuanLy,
                    l.MucPhatToiThieu,
                    l.MucPhatToiDa
                })
                .ToListAsync();

            if (!loaiViPhams.Any())
            {
                return Json(new { message = "Không tìm thấy thông tin vi phạm phù hợp" });
            }

            return Json(new { data = loaiViPhams });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLoaiViPham()
        {
            var loaiViPhams = await _context.LoaiViPhams
                .Select(l => new
                {
                    l.MaLoaiVPham,
                    l.TenLoaiVPham,
                    l.MoTa,
                    l.CoquanQuanLy,
                    l.MucPhatToiThieu,
                    l.MucPhatToiDa
                })
                .ToListAsync();

            return Json(new { data = loaiViPhams });
        }
    }
} 