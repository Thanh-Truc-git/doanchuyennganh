using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WebPhanTich.Models;

namespace WebPhanTich.Controllers
{
    public class PronunciationSamplesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PronunciationSamplesController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: PronunciationSamples
        public async Task<IActionResult> Index()
        {
            return View(await _context.PronunciationSamples.ToListAsync());
        }

        // GET: PronunciationSamples/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PronunciationSamples/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PronunciationSample model, IFormFile? audioFile)
        {
            if (ModelState.IsValid)
            {
                if (audioFile != null && audioFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "audio");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + audioFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await audioFile.CopyToAsync(stream);
                    }

                    model.AudioPath = "/audio/" + uniqueFileName;
                }

                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: PronunciationSamples/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _context.PronunciationSamples.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: PronunciationSamples/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PronunciationSample model, IFormFile? audioFile)
        {
            if (id != model.Id) return NotFound();

            var existingItem = await _context.PronunciationSamples.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (existingItem == null) return NotFound();

            if (ModelState.IsValid)
            {
                // Giữ nguyên AudioPath cũ nếu không có file mới
                string audioPath = existingItem.AudioPath;

                if (audioFile != null && audioFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "audio");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + audioFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await audioFile.CopyToAsync(stream);
                    }

                    audioPath = "/audio/" + uniqueFileName;
                }

                // Cập nhật chỉ những trường cần thiết
                existingItem.Word = model.Word;
                existingItem.IPA = model.IPA;
                existingItem.Description = model.Description;
                existingItem.AudioPath = audioPath;

                _context.Update(existingItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }


        // GET: PronunciationSamples/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.PronunciationSamples.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: PronunciationSamples/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.PronunciationSamples.FindAsync(id);
            if (item != null)
            {
                _context.PronunciationSamples.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
