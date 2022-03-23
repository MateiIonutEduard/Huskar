using Huskar.Models;

namespace Huskar.Services
{
    public interface IMovieService
    {
        public Task<Genre[]> AllGenres();
        public Task<(int, int)> GetFilterPages(string genres);
        public Task<MovieModel[]> GetFilterResults(int page, string genres);
        public Task<(int, int)> GetSearchPages(string name);
        public Task<MovieModel[]> GetSearchResults(int page, string name);
        public Task<(MovieModel[], int)> GetResults(int page, string name, string genres);
        public Task<int> GetPageCount();
        public Task<MovieBuffer> GetDetails(int id);
        public Task<int> UpcomingPages();
        public Task<MovieModel[]> GetUpcoming(int page);
        public Task<MovieModel[]> GetRated(int page);
    }
}
