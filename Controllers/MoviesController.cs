using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models;

namespace MovieBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieRepository _movieRepository;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(IMovieRepository movieRepository, ILogger<MoviesController> logger)
        {
            _movieRepository = movieRepository;
            _logger = logger;
        }

        [Authorize(Roles = "Admin, Member")]
        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<List<Movie>>> GetAll()
        {
            _logger.LogInformation("Getting all movies");
            var movies = await _movieRepository.GetMovies();
            if(movies.Count == 0 )
            {
                return NoContent();
            }
            else
            {
                return Ok(movies);
            }
        }

        [Authorize(Roles = "Admin, Member")]
        [HttpGet]
        [Route("search/{moviename}")]
        public async Task<ActionResult<List<Movie>>> GetByName(string moviename)
        {
            _logger.LogInformation($"Getting this {moviename} movie");
            var movies = await _movieRepository.SearchMovie(moviename);
            if (movies.Count == 0)
            {
                return NoContent();
            }
            else
            {
                return Ok(movies);
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("{moviename}/delete/{theatrename}")]
        public async Task<ActionResult<string>> DeleteMovie(string moviename,string theatrename)
        {
            _logger.LogInformation($"Deleting {moviename} - movie and the {theatrename} - theatre");
            var response = await _movieRepository.DeleteMovie(moviename, theatrename);
            if (response)
            {
                return Ok(new
                {
                    message = $"{moviename} movie deleted successfully."
                });
            }
            else
            {
                return BadRequest($"{moviename} movie name or {theatrename} theatre name doesn't exists");
            }
        }
    }
}
