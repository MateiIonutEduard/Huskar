namespace Huskar.Models
{
    public class MovieModel
    {
        public long id { get; set; }
        public string title { get; set; }

        public string overview { get; set; }

        public string backdrop_path { get; set; }
        public string poster_path { get; set; }
        public int[] genre_ids { get; set; }

        public DateTime? release_date { get; set; }
    }
}
