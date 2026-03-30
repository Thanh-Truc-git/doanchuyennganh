using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebPhanTich.Models;

namespace WebPhanTich.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: /Admin/Index
        public IActionResult Index()
        {
            var samples = _context.PronunciationSamples.ToList();
            return View(samples);
        }

        // GET: /Admin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PronunciationSample model, IFormFile? audioFile)
        {
            if (ModelState.IsValid)
            {
                if (audioFile != null && audioFile.Length > 0)
                {
                    string uploadDir = Path.Combine(_env.WebRootPath, "audios");
                    if (!Directory.Exists(uploadDir))
                        Directory.CreateDirectory(uploadDir);

                    string filePath = Path.Combine(uploadDir, Guid.NewGuid() + Path.GetExtension(audioFile.FileName));
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        audioFile.CopyTo(stream);
                    }

                    model.AudioPath = "/audios/" + Path.GetFileName(filePath);
                }

                _context.PronunciationSamples.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: /Admin/Edit/5
        public IActionResult Edit(int id)
        {
            var sample = _context.PronunciationSamples.Find(id);
            if (sample == null) return NotFound();
            return View(sample);
        }

        // POST: /Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, PronunciationSample model, IFormFile? audioFile)
        {
            var sample = _context.PronunciationSamples.Find(id);
            if (sample == null) return NotFound();

            if (ModelState.IsValid)
            {
                sample.Word = model.Word;
                sample.IPA = model.IPA;
                sample.Description = model.Description;

                if (audioFile != null && audioFile.Length > 0)
                {
                    string uploadDir = Path.Combine(_env.WebRootPath, "audios");
                    string filePath = Path.Combine(uploadDir, Guid.NewGuid() + Path.GetExtension(audioFile.FileName));
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        audioFile.CopyTo(stream);
                    }
                    sample.AudioPath = "/audios/" + Path.GetFileName(filePath);
                }

                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: /Admin/Delete/5
        public IActionResult Delete(int id)
        {
            var sample = _context.PronunciationSamples.Find(id);
            if (sample == null) return NotFound();
            return View(sample);
        }

        // POST: /Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var sample = _context.PronunciationSamples.Find(id);
            if (sample != null)
            {
                _context.PronunciationSamples.Remove(sample);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
