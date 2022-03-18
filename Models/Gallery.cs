namespace Huskar.Models
{
    public class Gallery
    {
        public int id { get; set; }
        public ImageModel[] backdrops { get; set; }
        public ImageModel[] posters { get; set; }
    }
}
