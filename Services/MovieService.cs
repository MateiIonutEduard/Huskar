using Huskar.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#pragma warning disable

namespace Huskar.Services
{
    public class MovieService
    {
        private string token;
        public MovieService(IConfiguration config)
        {
            token = config["AppSettings:api_key"];
        }

        public async Task<int> GetPageCount()
        {
            var client = new HttpClient();
            var url = $"https://api.themoviedb.org/3/movie/top_rated?api_key={token}&language=en-US";

            var response = await client.GetAsync(url);
            var buffer = await response.Content.ReadAsStringAsync();

            var obj = JObject.Parse(buffer);
            int n = int.Parse(obj["total_pages"].ToString());
            return n;
        }

        private List<string> GetFiles(Gallery mg, int index)
        {
            var list = new List<string>();

            if (index == 0)
            {
                if (mg.backdrops.Length > 0)
                    foreach (var backdrop in mg.backdrops)
                        list.Add(backdrop.file_path);
            }
            else
            {
                if (mg.posters.Length > 0)
                    foreach (var poster in mg.posters)
                        list.Add(poster.file_path);
            }

            return list;
        }

        public async Task<MovieBuffer> GetDetails(int id)
        {
            string str = $"https://api.themoviedb.org/3/movie/{id}?api_key={token}&language=en-US";
            var request = new HttpRequestMessage(HttpMethod.Get, str);
            var client = new HttpClient();
            var response = await client.SendAsync(request);
            var buffer = await response.Content.ReadAsStringAsync();
            var movie = JsonConvert.DeserializeObject<MovieModel>(buffer);

            str = $"https://api.themoviedb.org/3/movie/{id}/images?api_key={token}&language=en-US";
            request = new HttpRequestMessage(HttpMethod.Get, str);

            response = await client.SendAsync(request);
            buffer = await response.Content.ReadAsStringAsync();
            var mg = JsonConvert.DeserializeObject<Gallery>(buffer);

            var backdrops = GetFiles(mg, 0);
            var posters = GetFiles(mg, 1);

            if (backdrops.Count == 0) backdrops.Add(movie.backdrop_path);
            if (posters.Count == 0) posters.Add(movie.poster_path);

            str = $"https://api.themoviedb.org/3/movie/{id}/credits?api_key={token}&language=en-US";
            request = new HttpRequestMessage(HttpMethod.Get, str);

            response = await client.SendAsync(request);
            buffer = await response.Content.ReadAsStringAsync();

            var array = JObject.Parse(buffer);
            var persons = JsonConvert.DeserializeObject<Person[]>(array["cast"].ToString());

            var obj = new MovieBuffer
            {
                id = movie.id,
                title = movie.title,
                overview = movie.overview,
                backdrops = backdrops.ToArray(),
                posters = posters.ToArray(),
                people = persons,
                release_date = DateOnly.FromDateTime(movie.release_date)
            };

            return obj;
        }

        public async Task<int> UpcomingPages()
        {
            var client = new HttpClient();
            var url = $"https://api.themoviedb.org/3/movie/upcoming?api_key={token}&language=en-US";
            var response = await client.GetAsync(url);
            var buffer = await response.Content.ReadAsStringAsync();

            var obj = JObject.Parse(buffer);
            int n = int.Parse(obj["total_pages"].ToString());
            return n;
        }

        public async Task<MovieModel[]> GetUpcoming(int page)
        {
            var client = new HttpClient();
            string str = $"https://api.themoviedb.org/3/movie/upcoming?api_key={token}&language=en-US&page={page}";
            
            var response = await client.GetAsync(str);
            var buffer = await response.Content.ReadAsStringAsync();

            var list = new List<MovieModel>();
            var obj = JObject.Parse(buffer);

            var array = JsonConvert.DeserializeObject<MovieModel[]>(obj["results"].ToString());
            list.AddRange(array);

            return list.ToArray();
        }

        public async Task<MovieModel[]> GetRated(int page)
        {
            string str = $"https://api.themoviedb.org/3/movie/top_rated?api_key={token}&language=en-US&page={page}";
            var request = new HttpRequestMessage(HttpMethod.Get, str);
            var client = new HttpClient();
            var response = await client.SendAsync(request);
            var buffer = await response.Content.ReadAsStringAsync();

            List<MovieModel> list = new List<MovieModel>();
            var obj = JObject.Parse(buffer);

           var array = JsonConvert.DeserializeObject<MovieModel[]>(obj["results"].ToString());
            list.AddRange(array);

            return list.ToArray();
        }
    }
}
