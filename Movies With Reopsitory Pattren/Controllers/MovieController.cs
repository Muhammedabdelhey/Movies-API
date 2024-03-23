using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies_Core_Layer.Dtos;
using Movies_Core_Layer.Interfaces;
using Movies_Core_Layer.Models;
using Movies_Data_Access_Layer.EF;

namespace Movies_With_Reopsitory_Pattren.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IBaseRepository<Movie> _movieRepository;
        private readonly IBaseRepository<Genre> _genreRepository;
        private readonly List<string> _allowedExtenstions = new() { ".png", ".jpg" };
        private readonly long _maxPosterSize = 1048576;
        ApplicationDbContext _context;
        public MovieController(IBaseRepository<Movie> movieRepository, IBaseRepository<Genre> genreRepository, ApplicationDbContext context)
        {
            _movieRepository = movieRepository;
            _genreRepository = genreRepository;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var movies = await _movieRepository.GetAllAsync(new string[] { "Genre" });
            return Ok(movies);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id, new string[] { "Genre" });
            if (movie == null)
                return NotFound($"This MovieID: {id} Not Found");
            return Ok(movie);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromForm] MovieDto movieDto)
        {
            if (movieDto.Poster == null)
                return BadRequest("Poster Field Is Required");
            if (!_allowedExtenstions.Contains(Path.GetExtension(movieDto.Poster.FileName).ToLowerInvariant()))
                return BadRequest("png and jpg Images Only Allowed!");
            if (movieDto.Poster.Length > _maxPosterSize)
                return BadRequest("Max Allowed Size For Movie Poster is 1 MB!");
            var isValidGenre = await _genreRepository.GetByIdAsync(movieDto.GenreId);
            if (isValidGenre == null)
                return NotFound($"This GenreID: {movieDto.GenreId} Not Found");
            using var datastream = new MemoryStream();
            await movieDto.Poster.CopyToAsync(datastream);
            var movie = new Movie
            {
                Title = movieDto.Title,
                Year = movieDto.Year,
                Poster = datastream.ToArray(),
                GenreId = movieDto.GenreId,
                Rate = movieDto.Rate,
                Storeline = movieDto.Storeline,
            };
            await _movieRepository.AddAsync(movie);
            return Ok(movie);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] MovieDto movieDto)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null)
                return NotFound($"This MovieID: {id} Not Found");
            var isValidGenre = await _genreRepository.GetByIdAsync(movieDto.GenreId);
            if (isValidGenre == null)
                return NotFound($"This GenreID: {movieDto.GenreId} Not Found");
            movie.Title = movieDto.Title;
            movie.Year = movieDto.Year;
            movie.Storeline = movieDto.Storeline;
            movie.Rate = movieDto.Rate;
            movie.GenreId = movieDto.GenreId;
            if (movieDto.Poster != null)
            {
                if (!_allowedExtenstions.Contains(Path.GetExtension(movieDto.Poster.FileName).ToLowerInvariant()))
                    return BadRequest("png and jpg Images Only Allowed!");

                if (movieDto.Poster.Length > _maxPosterSize)
                    return BadRequest("Max Allowed Size For Movie Poster is 1 MB!");

                using var datastream = new MemoryStream();
                await movieDto.Poster.CopyToAsync(datastream);
                movie.Poster = datastream.ToArray();
            }
            await _movieRepository.UpdateAsync(movie);
            return Ok(movie);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null)
                return NotFound($"This MovieID: {id} Not Found");
            await _movieRepository.DeleteAsync(movie);
            return Ok(movie);
        }
    }
}
