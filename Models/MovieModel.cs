namespace Huskar.Models
{
    public class MovieModel
    {
        public int id { get; set; }
        public string title { get; set; }

        public string overview { get; set; }

        public string backdrop_path { get; set; }
        public string poster_path { get; set; }

        public DateTime release_date { get; set; }
    }
}
