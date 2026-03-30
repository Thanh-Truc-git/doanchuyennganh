using System.ComponentModel.DataAnnotations.Schema;

namespace WebPhanTich.Models
{
    public class PronunciationResult
    {
        public int Id { get; set; }

        // User liên kết với IdentityUser
        public string UserId { get; set; } = string.Empty;

        // Sample liên kết với từ vựng
        public int SampleId { get; set; }

        public double Score { get; set; }
        public string Feedback { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        [ForeignKey(nameof(SampleId))]
        public PronunciationSample? Sample { get; set; }

        // Đường dẫn file âm thanh (nếu cần)
        public string? AudioPath { get; set; }
    }
}
