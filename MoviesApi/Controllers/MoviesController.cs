using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.VisualBasic;
using MoviesApi.Dtos.Movie;
using MoviesApi.Models;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private List<string> stauts = new List<string>
        {
            "Success",
            "Failed",
        };

        private List<string> _allowedExtentions = new List<string> { ".jpg", ".png" };
        private long _maxPosterSize = 1048576;
        private readonly ApplicationDBContext _context;
        public MoviesController(ApplicationDBContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllMovies()
        {
            var movies = await _context.Movies
                   .OrderByDescending(x => x.Rate)
                   .Include(g => g.Genre)
                   .ToListAsync();
            
            var mappedMovies = mappingMovieToMovieDetaisl(movies);
            return Ok(new { stauts = stauts[0], movies=mappedMovies });
        }
        [HttpGet(template:"{id}")]
        public async Task<IActionResult> GetMovie(int id)
        {
            var movie = await _context.Movies.Include(g => g.Genre).FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
                return NotFound(new { stauts = stauts[1], message = "Movie Not Found" });
            var mappedMovie= mappingMovie(movie);
            return Ok(new { stauts = stauts[0], movie=mappedMovie });
        }




        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] MovieDto dto)
        {
            if (!await isValidGenre(dto.GenreId))
                return BadRequest(new { stauts = stauts[1], message = "Genre not found" });
            if (!checkExtention(dto.Poster))
            {
                return BadRequest(new { stauts = stauts[1], message = "only .png and jpg are allowed" });
            }
            if (dto.Poster.Length > _maxPosterSize)
                return BadRequest(new { stauts = stauts[1], message = "Max photo size is 1 MB" });
            var movie = new Movie()
            {
                Title = dto.Title,
                Rate = dto.Rate,
                StoreLine = dto.StoreLine,
                Year = dto.Year,
                GenreId = dto.GenreId,
                Poster = await GetByteArrayFromStreamAsync(dto.Poster),

            };
            try
            {
                await _context.AddAsync(movie);
                _context.SaveChanges();
                return Ok(new { status = stauts[0], movie });
            }
            catch (Exception ex)
            {
                return BadRequest(new { stauts = stauts[1], ex.Message });
            }



        }



        public static async Task<byte[]> GetByteArrayFromStreamAsync(IFormFile file)
        {
            using (var dataStream = new MemoryStream())
            {
                await file.CopyToAsync(dataStream);
                return dataStream.ToArray();
            }
        }

        private bool checkSize(long size)
        {
            return size > _maxPosterSize;
        }

        private bool checkExtention(IFormFile file)
        {
            return _allowedExtentions.Contains(Path.GetExtension(file.FileName.ToLower()));

        }
        private async Task<bool> isValidGenre(byte id)
            => await _context.Genres.AnyAsync(g => g.Id == id);

        private List<MovieDetails> mappingMovieToMovieDetaisl(List<Movie> movies)
        {
            var movieDetails = new MovieDetails();
            List<MovieDetails> moviesDetails = new List<MovieDetails>();
            foreach (var movie in movies)
            {
                movieDetails = mappingMovie(movie);
                moviesDetails.Add(movieDetails);

            }

            return moviesDetails;
        }

        private MovieDetails mappingMovie(Movie movie) {
            MovieDetails movieDetails = new MovieDetails();
            movieDetails.Id = movie.Id;
            movieDetails.Title = movie.Title;
            movieDetails.Poster = movie.Poster;
            movieDetails.StoreLine = movie.StoreLine;
            movieDetails.Year = movie.Year;
            movieDetails.GenreId = movie.GenreId;
            movieDetails.GenreName = movie.Genre.Name;
            movieDetails.Rate = movie.Rate;
            return movieDetails;
            
        }
    }
}
