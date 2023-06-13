using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.VisualBasic;
using MoviesApi.Dtos.Movie;
using MoviesApi.Models;
using MoviesApi.Servises;

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
        private readonly MovieServise _servise;
        public MoviesController(MovieServise servise)
        {
            _servise = servise;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllMovies()
        {
            var movies =await _servise.GetAllAsync();
          
            var mappedMovies = mappingMovieToMovieDetaisl(movies);
            return Ok(new { stauts = stauts[0], movies=mappedMovies });
        }
        [HttpGet(template:"{id}")]
        public async Task<IActionResult> GetMovie(int id)
        {
            var movie = await _servise.GetByIdAsync(id);
            if (movie == null)
                return NotFound(new { stauts = stauts[1], message = "Movie Not Found" });
            var mappedMovie= mappingMovie(movie);
            return Ok(new { stauts = stauts[0], movie=mappedMovie });
        }

        [HttpGet("GetMoviesByGenreId")]
        public async Task<IActionResult> GetMoviesByGenreId(byte id)
        {
            var movieS = await _servise.GetMoviesByGenreIdAsync(id);
            var mappedMovie = mappingMovieToMovieDetaisl(movieS);
            return Ok(new { stauts = stauts[0], movies = mappedMovie });
        }



        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] MovieDto dto)
        {
           /* if (!await isValidGenre(dto.GenreId))
                return BadRequest(new { stauts = stauts[1], message = "Genre not found" });*/
            if(dto.Poster==null)
                return BadRequest(new { stauts = stauts[1], message = "Poster is required" });
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
                Poster =  GetByteArrayFromStreamAsync(dto.Poster),

            };
            try
            {
                await _servise.UpdateAsync(movie);
                return Ok(new { status = stauts[0], movie });
            }
            catch (Exception ex)
            {
                return BadRequest(new { stauts = stauts[1], ex.Message });
            }



        }
        [HttpDelete(template:"{id}")]
        public async Task<IActionResult> DeleteMovieAsync(int id) { 
            var movie=await _servise.GetByIdAsync(id);
            if (movie == null)
                return NotFound(new { stauts = stauts[1], message = $"Not Found Movie with id {id}" });

            try
            {
                await _servise.DeleteAsync(movie);
                return Ok(new { stauts = stauts[0], message = "Deleted Successfully" });
            }catch(Exception ex)
            {
                return BadRequest(new { stauts = stauts[0], message = ex.Message });
            }
            }
        [HttpPut(template:"{id}")]
        public async Task<IActionResult> updateMovie(int id, [FromForm] MovieDto movieDto) {
            var movie=await _servise.GetByIdAsync(id);
            if(movie==null)
                return NotFound(new { stauts = stauts[1], message = $"Not Found Movie with id {id}" });
            (bool isValid, string message) =await ValidateGenreAndPoster(movieDto);
            if (!isValid)
                return BadRequest(new { stauts = stauts[1] ,message=message });
            
            movie.Title = movieDto.Title;
            movie.Year = movieDto.Year;
            movie.StoreLine=movieDto.StoreLine;
            movie.Rate= movieDto.Rate;
            movie.GenreId = movieDto.GenreId;
            if (movieDto.Poster != null) {
                movie.Poster = GetByteArrayFromStreamAsync(movieDto.Poster);
            }
            try { 
                _servise.UpdateAsync(movie);
                return Ok(new { stauts = stauts[0],message="Movie Updated Successfulley",movie });
            }catch(Exception ex) { return BadRequest(new { stauts = stauts[1],message=ex.Message }); }


        }


        private  byte[] GetByteArrayFromStreamAsync(IFormFile file)
        {
            using (var dataStream = new MemoryStream())
            {
                 file.CopyToAsync(dataStream);
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
      /*  private async Task<bool> isValidGenre(byte id)
            => await _context.Genres.AnyAsync(g => g.Id == id);*/

        private List<MovieDetails> mappingMovieToMovieDetaisl(IEnumerable<Movie> movies)
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


        private async Task<(bool, string)> ValidateGenreAndPoster(MovieDto dto)
        {
           /* if (! await isValidGenre(dto.GenreId))
                return (false, "Genre not found");*/
            if(dto.Poster != null) { 
                if (!checkExtention(dto.Poster))
                return (false, "Only .png and .jpg extensions are allowed");

            if (dto.Poster.Length > _maxPosterSize)
                return (false, "Max photo size is 1 MB");
            }

            return (true, "Validation successful");
        }

    }
}
