namespace Huskar.Models
{
    public class ImageModel
    {
        public string file_path { get; set; }
        public int width { get; set; }

        public int height { get; set; }

        public double aspect_ratio
        {
            get { return (double)width / height; }
            set { aspect_ratio = value; }
        }
    }
}
