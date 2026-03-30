namespace WebPhanTich.Models
{
    public class PronunciationSample
    {
        public int Id { get; set; }
        public string Word { get; set; } = string.Empty;
        public string IPA { get; set; } = string.Empty;
        public string? AudioPath { get; set; }
        public string? Description { get; set; }
    }
}
