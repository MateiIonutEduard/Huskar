using System.ComponentModel.DataAnnotations.Schema;

namespace Huskar.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Profile { get; set; }

        public int AuthModel { get; set; }

        [ForeignKey("UserId")]
        public ICollection<Post> Posts { get; set; }
    }
}
