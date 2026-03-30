namespace WebPhanTich.Models
{
    public class ListeningQuestion
    {
        public int Id { get; set; }
        public int ListeningId { get; set; }
        public string QuestionText { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        
        public char CorrectOption { get; set; }

        public virtual Listening Listening { get; set; }
    }
}
