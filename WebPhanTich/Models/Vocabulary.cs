using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebPhanTich.Models
{
    public class Vocabulary
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Từ vựng tiếng Nhật")]
        public string Word { get; set; }

        [Display(Name = "Phiên âm / Nghĩa tiếng Việt")]
        public string? Hiragana { get; set; }
        public string? Katakana { get; set; }

        public string Meaning { get; set; }

        [Display(Name = "Đường dẫn âm thanh")]
        public string? AudioPath { get; set; }

        // 🔗 Liên kết Category
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
    }
}
