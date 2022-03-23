using System.Linq;
using Huskar.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#pragma warning disable

namespace Huskar.Services
{
    public class MovieService : IMovieService
    {
        private string token;
        private HttpClient client;
        public MovieService(IConfiguration config)
        {
            token = config["AppSettings:api_key"];
            client = new HttpClient();
        }
        
        public async Task<Genre[]> AllGenres()
        {
            string url = $"https://api.themoviedb.org/3/genre/movie/list?api_key=67b8ec3a134e7dd2607506cc43b34695&language=en-US";
            var response = await client.GetAsync(url);

            var buffer = await response.Content.ReadAsStringAsync();
            var obj = JObject.Parse(buffer);

            var list = JsonConvert.DeserializeObject<Genre[]>(obj["genres"].ToString());
            return list;
        }

        public async Task<(int, int)> GetFilterPages(string genres)
        {
            string url = $"https://api.themoviedb.org/3/discover/movie?api_key={token}&language=en-US&with_genres={genres}";
            var response = await client.GetAsync(url);
            var buffer = await response.Content.ReadAsStringAsync();

            var obj = JObject.Parse(buffer);
            int pages = int.Parse(obj["total_pages"].ToString());
            int total = int.Parse(obj["total_results"].ToString());
            return (pages, total);
        }

        public async Task<MovieModel[]> GetFilterResults(int page, string genres)
        {
            string url = $"https://api.themoviedb.org/3/discover/movie?api_key={token}&language=en-US&page={page}&with_genres={genres}";
            var response = await client.GetAsync(url);

            var buffer = await response.Content.ReadAsStringAsync();
            var list = new List<MovieModel>();
            var obj = JObject.Parse(buffer);

            var array = JsonConvert.DeserializeObject<MovieModel[]>(obj["results"].ToString());
            list.AddRange(array);

            return list.ToArray();
        }

        public async Task<(int, int)> GetSearchPages(string name)
        {
            string url = $"https://api.themoviedb.org/3/search/movie?query={name}&api_key={token}&language=en-US";
            var response = await client.GetAsync(url);
            var buffer = await response.Content.ReadAsStringAsync();

            var obj = JObject.Parse(buffer);
            int pages = int.Parse(obj["total_pages"].ToString());
            int total = int.Parse(obj["total_results"].ToString());
            return (pages, total);
        }

        public async Task<MovieModel[]> GetSearchResults(int page, string name)
        {
            string url = $"https://api.themoviedb.org/3/search/movie?query={name}&api_key={token}&language=en-US";
            var response = await client.GetAsync(url);
            var buffer = await response.Content.ReadAsStringAsync();

            var list = new List<MovieModel>();
            var obj = JObject.Parse(buffer);

            var array = JsonConvert.DeserializeObject<MovieModel[]>(obj["results"].ToString());
            list.AddRange(array);

            return list.ToArray();
        }

        private bool HasFilters(int[] buf, string genres)
        {
            string[] all = genres.Split(", ");
            int len = all.Length;

            int size = buf.Length;
            int n = 0;

            if (len > buf.Length) return false;

            for (int j = 0; j < len; j++)
            {
                int filter = int.Parse(all[j]);

                for (int k = 0; k < size; k++)
                    if (filter == buf[k]) n++;
            }

            return n == len;
        }

        private bool Exists(List<MovieModel> list, MovieModel movie)
        {
            int j;
            int n = list.Count;
            long id = movie.id;

            for(j = 0; j < n; j++)
                if (id == list[j].id) return true;

            return false;
        }

        public async Task<(MovieModel[], int)> GetResults(int page, string name, string genres)
        {
           MovieModel[] list;
           var filter = await GetFilterPages(genres);

            var search_filter = await GetSearchPages(name);
            var total = new List<MovieModel>();
            int k = 1, n;

            if (search_filter.Item2 < filter.Item2)
            {
                // search results are lesser
                n = search_filter.Item1;

                while (k <= n)
                {
                    list = await GetSearchResults(k, name);
                    var array = list.Where(m => HasFilters(m.genre_ids, genres))
                            .ToArray();

                    var data = array.Where(m => !Exists(total, m)).ToList();
                    total.AddRange(data);
                    k++;
                }
            }
            else
            {
                // genre filter results are lesser
                n = filter.Item1;

                while (k <= n)
                {
                    list = await GetFilterResults(k, genres);
                    var array = list.Where(m => m.title.Contains(name))
                            .ToArray();

                    var data = array.Where(m => !Exists(total, m)).ToList();
                    total.AddRange(data);
                    k++;
                }
            }

            int pages = total.Count / 20;
            int rem = total.Count % 20;
            if (rem > 0) pages++;

            if(page <= pages)
            {
                int index = 20 * (page - 1);
                int next = index + 20;

                if (rem > 0 && page == pages)
                {
                    list = new MovieModel[rem];
                    total.CopyTo(index, list, 0, rem);
                    return (list, pages);
                }
                else
                {
                    list = new MovieModel[20];
                    total.CopyTo(index, list, 0, 20);
                    return (list, pages);
                }
            }
            else
            {
                list = new MovieModel[0];
                return (list, 0);
            }
        }
        
        #region WORK_DONE
        public async Task<int> GetPageCount()
        {
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
                release_date = DateOnly.FromDateTime(movie.release_date.Value)
            };

            return obj;
        }

        public async Task<int> UpcomingPages()
        {
            var url = $"https://api.themoviedb.org/3/movie/upcoming?api_key={token}&language=en-US";
            var response = await client.GetAsync(url);
            var buffer = await response.Content.ReadAsStringAsync();

            var obj = JObject.Parse(buffer);
            int n = int.Parse(obj["total_pages"].ToString());
            return n;
        }

        public async Task<MovieModel[]> GetUpcoming(int page)
        {
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
            
            var response = await client.SendAsync(request);
            var buffer = await response.Content.ReadAsStringAsync();

            List<MovieModel> list = new List<MovieModel>();
            var obj = JObject.Parse(buffer);

           var array = JsonConvert.DeserializeObject<MovieModel[]>(obj["results"].ToString());
            list.AddRange(array);

            return list.ToArray();
        }
        #endregion
    }
}
