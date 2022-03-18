using System.ComponentModel.DataAnnotations;

namespace Huskar.Data
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Message { get; set; }

        public DateTime Date { get;set; }

        public int MovieId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
