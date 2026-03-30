using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebPhanTich.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<PronunciationSample> PronunciationSamples { get; set; }
        public DbSet<PronunciationResult> PronunciationResults { get; set; }

        public DbSet<Vocabulary> Vocabularies { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Listening> Listenings { get; set; }
        public DbSet<ListeningQuestion> ListeningQuestions { get; set; }

    }
}
