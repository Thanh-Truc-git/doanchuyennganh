using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPhanTich.Models;

namespace WebPhanTich.Controllers
{
    public class ListeningController : Controller
    {
        private readonly AppDbContext _context;

        public ListeningController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var list = _context.Listenings.ToList();
            return View(list);
        }

        public IActionResult Details(int id)
        {
            var listening = _context.Listenings
                .Include(l => l.Questions)
                .FirstOrDefault(l => l.Id == id);

            if (listening == null) return NotFound();
            return View(listening);
        }

        [HttpPost]
        public IActionResult Submit(int listeningId, Dictionary<int, string> answers)
        {
            var questions = _context.ListeningQuestions
                .Where(q => q.ListeningId == listeningId)
                .ToList();

            int score = 0;
            foreach (var q in questions)
            {
                if (answers.ContainsKey(q.Id) && answers[q.Id] == q.CorrectOption.ToString())
                {
                    score++;
                }
            }

            ViewBag.Score = score;
            ViewBag.Total = questions.Count;
            return View("Result");
        }
    }

}
