namespace WebPhanTich.Models
{
    public class Listening
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AudioPath { get; set; }
        public string Transcript { get; set; }
        public string Level { get; set; }

        public virtual ICollection<ListeningQuestion> Questions { get; set; }
    }
}
