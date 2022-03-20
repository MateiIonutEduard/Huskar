namespace Huskar.Models
{
    public class MovieBuffer
    {
        public long id { get; set; }
        public string title { get; set; }
        public string overview { get; set; }
        public string[] backdrops { get; set; }
        public string[] posters { get; set; }

        public Person[] people { get; set; }
        public DateOnly? release_date { get; set; }
    }
}
