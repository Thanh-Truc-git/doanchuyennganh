using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebPhanTich.Models;

namespace WebPhanTich.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class PronunciationController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PronunciationController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // Hiển thị danh sách từ vựng + phát âm mẫu
        public IActionResult Index()
        {
            var samples = _context.PronunciationSamples.ToList();
            return View(samples);
        }

        // Nhận file ghi âm từ người dùng
        [HttpPost]
        public async Task<IActionResult> UploadAudio(int sampleId, IFormFile userAudio)
        {
            if (userAudio == null || userAudio.Length == 0)
                return BadRequest("Không có file âm thanh.");

            // Tạo thư mục lưu file nếu chưa có
            string uploadDir = Path.Combine(_env.WebRootPath, "user_audios");
            if (!Directory.Exists(uploadDir))
                Directory.CreateDirectory(uploadDir);

            // Lưu file
            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(userAudio.FileName)}";
            string filePath = Path.Combine(uploadDir, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await userAudio.CopyToAsync(stream);
            }

            // Lưu thông tin vào DB 
            var result = new PronunciationResult
            {
                UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "",
                SampleId = sampleId,
                Score = 0,
                Feedback = "Chưa phân tích",
                Date = DateTime.Now
            };
            _context.PronunciationResults.Add(result);
            await _context.SaveChangesAsync();

            return Ok("Upload thành công");
        }
        [HttpPost]
        public async Task<IActionResult> Analyze(IFormFile audioFile)
        {
            if (audioFile == null || audioFile.Length == 0)
                return Json(new { error = "Chưa chọn file âm thanh" });

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", audioFile.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await audioFile.CopyToAsync(stream);
            }

            using (var client = new HttpClient())
            {
                var form = new MultipartFormDataContent();
                form.Add(new StreamContent(System.IO.File.OpenRead(filePath)), "audio", audioFile.FileName);

                var response = await client.PostAsync("http://127.0.0.1:5001/analyze", form);
                var json = await response.Content.ReadAsStringAsync();

                return Content(json, "application/json");
            }
        }
    }
}
