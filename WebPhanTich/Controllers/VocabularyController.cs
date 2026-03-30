using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using ExcelDataReader;
using WebPhanTich.Models;
using System.Data;
using System.Net.Http;

namespace WebPhanTich.Controllers
{
    [Authorize]
    public class VocabularyController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly HttpClient _httpClient;


        public VocabularyController(AppDbContext context, IWebHostEnvironment env, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _env = env;
            _httpClient = httpClientFactory.CreateClient();
        }

        // ===== Import Excel (Admin) =====
        [Authorize(Roles = "Admin")]
        public IActionResult Import()
        {
            string filePath = Path.Combine(_env.WebRootPath, "audios", "vocabulary", "vocabulary.xlsx");
            if (!System.IO.File.Exists(filePath))
                return Content($"❌ File Excel không tồn tại: {filePath}");

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var result = reader.AsDataSet();
                DataTable table = result.Tables[0];

                for (int i = 1; i < table.Rows.Count; i++)
                {
                    var kanji = table.Rows[i][0]?.ToString()?.Trim();
                    var hiragana = table.Rows[i][1]?.ToString()?.Trim();
                    var katakana = table.Rows[i][2]?.ToString()?.Trim();
                    var categoryName = table.Rows[i][3]?.ToString()?.Trim();
                    var meaning = table.Rows[i][4]?.ToString()?.Trim();
                    var audioFileName = table.Rows[i][5]?.ToString()?.Trim();

                    if (string.IsNullOrEmpty(kanji) || string.IsNullOrEmpty(categoryName))
                        continue;

                    var category = _context.Categories.FirstOrDefault(c => c.Name == categoryName);
                    if (category == null)
                    {
                        category = new Category { Name = categoryName };
                        _context.Categories.Add(category);
                        _context.SaveChanges();
                    }

                    if (_context.Vocabularies.Any(v => v.Word == kanji))
                        continue;

                    var audioPath = string.IsNullOrEmpty(audioFileName)
                        ? null
                        : "/audios/vocabulary/audio/" + audioFileName;

                    var vocab = new Vocabulary
                    {
                        Word = kanji,
                        Hiragana = hiragana,
                        Katakana = katakana,
                        Meaning = meaning,
                        AudioPath = audioPath,
                        CategoryId = category.Id
                    };

                    _context.Vocabularies.Add(vocab);
                }

                _context.SaveChanges();
            }

            return Content("✅ Import dữ liệu thành công!");
        }

        // ===== Index: Hiển thị danh sách chủ đề hoặc từ vựng =====
        public IActionResult Index(int? categoryId)
        {
            ViewBag.IsAdmin = User.IsInRole("Admin");

            if (categoryId.HasValue)
            {
                var vocabularies = _context.Vocabularies
                                           .Where(v => v.CategoryId == categoryId.Value)
                                           .Include(v => v.Category)
                                           .ToList();

                var category = _context.Categories.Find(categoryId.Value);
                ViewBag.CategoryName = category?.Name ?? "Chủ đề";

                return View(vocabularies);
            }
            else
            {
                var categories = _context.Categories.ToList();
                return View(categories);
            }
        }

        // ===== Details =====
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var vocabulary = await _context.Vocabularies
                .Include(v => v.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (vocabulary == null) return NotFound();

            return View(vocabulary);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Vocabulary vocabulary, IFormFile AudioFile)
        {
            if (ModelState.IsValid)
            {
                // Upload audio nếu có
                if (AudioFile != null && AudioFile.Length > 0)
                {
                    string uploadDir = Path.Combine(_env.WebRootPath, "audios", "vocabulary", "audio");
                    if (!Directory.Exists(uploadDir))
                        Directory.CreateDirectory(uploadDir);

                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(AudioFile.FileName)}";
                    string filePath = Path.Combine(uploadDir, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await AudioFile.CopyToAsync(stream);

                    vocabulary.AudioPath = "/audios/vocabulary/audio/" + fileName;
                }

                _context.Add(vocabulary);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { categoryId = vocabulary.CategoryId });
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", vocabulary.CategoryId);
            return View(vocabulary);
        }

        // ===== Edit =====
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var vocabulary = await _context.Vocabularies.FindAsync(id);
            if (vocabulary == null) return NotFound();

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", vocabulary.CategoryId);
            return View(vocabulary);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Vocabulary vocabulary, IFormFile AudioFile)
        {
            if (id != vocabulary.Id) return NotFound();

            if (ModelState.IsValid)
            {
                // Lấy bản ghi hiện tại từ DB
                var vocabFromDb = await _context.Vocabularies.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
                if (vocabFromDb == null) return NotFound();

                // Nếu có file mới, lưu file và cập nhật AudioPath
                if (AudioFile != null && AudioFile.Length > 0)
                {
                    string uploadDir = Path.Combine(_env.WebRootPath, "audios", "vocabulary", "audio");
                    if (!Directory.Exists(uploadDir))
                        Directory.CreateDirectory(uploadDir);

                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(AudioFile.FileName)}";
                    string filePath = Path.Combine(uploadDir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await AudioFile.CopyToAsync(stream);

                    vocabulary.AudioPath = "/audios/vocabulary/audio/" + fileName;
                }
                else
                {
                    // Giữ nguyên audio cũ nếu không upload file mới
                    vocabulary.AudioPath = vocabFromDb.AudioPath;
                }

                // Cập nhật các trường khác
                _context.Update(vocabulary);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index), new { categoryId = vocabulary.CategoryId });
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", vocabulary.CategoryId);
            return View(vocabulary);
        }


        // ===== Delete =====
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var vocabulary = await _context.Vocabularies
                .Include(v => v.Category)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vocabulary == null) return NotFound();

            return View(vocabulary);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vocabulary = await _context.Vocabularies.FindAsync(id);
            if (vocabulary != null)
            {
                // Xóa audio nếu có
                if (!string.IsNullOrEmpty(vocabulary.AudioPath))
                {
                    string filePath = Path.Combine(_env.WebRootPath, vocabulary.AudioPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                }

                _context.Vocabularies.Remove(vocabulary);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { categoryId = vocabulary?.CategoryId });
        }



        // ===== Upload Audio User =====
        [HttpPost]
        public async Task<IActionResult> UploadAudio(int sampleId, IFormFile userAudio)
        {
            if (userAudio == null || userAudio.Length == 0)
                return BadRequest("Không có file âm thanh.");

            string uploadDir = Path.Combine(_env.WebRootPath, "user_audios");
            Directory.CreateDirectory(uploadDir);

            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(userAudio.FileName)}";
            string filePath = Path.Combine(uploadDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await userAudio.CopyToAsync(stream);

            var result = new PronunciationResult
            {
                UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "",
                SampleId = sampleId,
                Score = 0,
                Feedback = "Chưa phân tích",
                Date = DateTime.Now,
                AudioPath = "/user_audios/" + fileName
            };

            _context.PronunciationResults.Add(result);
            await _context.SaveChangesAsync();

            return Ok("Upload thành công");
        }

        // ================= Analyze Audio (gọi API phân tích) =================
        [HttpPost]
        public async Task<IActionResult> AnalyzeAudio(int sampleId, IFormFile userAudio)
        {
            if (userAudio == null || userAudio.Length == 0)
                return BadRequest("Không có file âm thanh.");

            // Upload tạm thời
            string tempDir = Path.Combine(_env.WebRootPath, "temp_audios");
            Directory.CreateDirectory(tempDir);
            string tempPath = Path.Combine(tempDir, Guid.NewGuid() + Path.GetExtension(userAudio.FileName));

            using (var stream = new FileStream(tempPath, FileMode.Create))
                await userAudio.CopyToAsync(stream);

            try
            {
                using var content = new MultipartFormDataContent();
                using var fileStream = System.IO.File.OpenRead(tempPath);
                content.Add(new StreamContent(fileStream), "file", userAudio.FileName);

                // Gọi API Flask dev bằng HTTP
                var response = await _httpClient.PostAsync("http://127.0.0.1:5000/api/analyze", content);
                response.EnsureSuccessStatusCode();

                var jsonResult = await response.Content.ReadAsStringAsync();
                // Giả sử API trả về JSON { "score": 85, "feedback": "Good" }
                var result = System.Text.Json.JsonSerializer.Deserialize<PronunciationResult>(jsonResult);

                // Lưu kết quả vào DB
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";
                var pronResult = new PronunciationResult
                {
                    UserId = userId,
                    SampleId = sampleId,
                    Score = result?.Score ?? 0,
                    Feedback = result?.Feedback ?? "Chưa phân tích",
                    Date = DateTime.Now,
                    AudioPath = "/user_audios/" + Path.GetFileName(tempPath)
                };

                _context.PronunciationResults.Add(pronResult);
                await _context.SaveChangesAsync();

                // Trả về JSON cho JS hiển thị feedback
                return Json(new { score = pronResult.Score, feedback = pronResult.Feedback });
            }
            catch (Exception ex)
            {
                return BadRequest($"❌ Lỗi khi gọi API: {ex.Message}");
            }
        }


        // DTO trung gian để deserialize JSON API
        public class PronunciationResultDto
        {
            public double Score { get; set; }
            public string Feedback { get; set; } = string.Empty;
        }
    }
}


    

