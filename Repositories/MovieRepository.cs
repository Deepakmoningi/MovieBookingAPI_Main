using MongoDB.Bson;
using MongoDB.Driver;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models;

namespace MovieBookingAPI.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly IMongoCollection<Movie> _movies;
        private readonly IMongoCollection<Ticket> _tickets;
        private readonly ILogger<MovieRepository> _logger;

        public MovieRepository(IMongoDbConfig config,IMongoClient mongoClient,ILogger<MovieRepository> logger)
        {
            var database = mongoClient.GetDatabase(config.DatabaseName);
            _movies=database.GetCollection<Movie>(config.MovieCollectionName);
            _tickets=database.GetCollection<Ticket>(config.TicketCollectionName);
            _logger = logger;
        }

        public async Task<List<Movie>> GetMovies()
        {
            _logger.LogInformation("Getting all movies");
            var movies = await _movies.FindAsync(m=>true);
            return movies.ToList();
        }

        public async Task<List<Movie>> SearchMovie(string movieName)
        {
            _logger.LogInformation($"Searching for movie name that matches with this {movieName}");
            var movies = await _movies.FindAsync(Builders<Movie>.Filter.Regex("moviename", new BsonRegularExpression(movieName, "i")));
            return movies.ToList();
        }

        public async Task<bool> DeleteMovie(string moviename,string theatreName)
        {
            _logger.LogInformation($"Deleting a {moviename} movie");
            var result = await _movies.DeleteOneAsync(x=>x.MovieName.ToLower() == moviename.ToLower() && x.TheatreName.ToLower() == theatreName.ToLower());
            _logger.LogInformation($"Deleting ticket related data for this {moviename} movie");
            await _tickets.DeleteManyAsync(x => x.MovieName.ToLower() == moviename.ToLower() && x.TheatreName.ToLower() == theatreName.ToLower());
            if (result.DeletedCount ==0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
