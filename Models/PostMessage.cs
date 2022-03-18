namespace Huskar.Models
{
    public class PostMessage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public string Profile { get; set; }
        public int MovieId { get; set; }
    }
}
