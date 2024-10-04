using MovieBookingAPI.Models;

namespace MovieBookingAPI.Interfaces
{
    public interface IMovieRepository
    {
        public Task<List<Movie>> GetMovies();
        public Task<List<Movie>> SearchMovie(string movieName);
        public Task<bool> DeleteMovie(string movieName,string Id);
    }
}
