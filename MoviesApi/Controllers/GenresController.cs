using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Dtos.Genres;
using MoviesApi.Models;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public GenresController(ApplicationDBContext context)
        {
            _context = context;

        }




        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
            return Ok(genres);

        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateGenreDto genreDto)
        {
            var genre = new Genre() {
                Name = genreDto.Name,
            };

            await _context.AddAsync(genre);
            _context.SaveChanges();


            return Ok(genre);

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id,[FromBody] CreateGenreDto genreDto) {
            var genre = await _context.Genres.SingleOrDefaultAsync(g => g.Id == id);
            if (genre == null) {
                var message = new { status = "failed" ,
                    
                message=$"client error مليش فيه مش شغلتي  " };
            return NotFound (message);
            }
            genre.Name = genreDto.Name;
            _context.SaveChanges();
            return Ok(new {status="success",genre});
            
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var genre = await _context.Genres.SingleOrDefaultAsync(g => g.Id == id);
            if (genre == null)
            {
                return NotFound(value: $"No genre was found with ID :{id}");
            }
            _context.Genres.Remove(genre);
            _context.SaveChanges();
            return Ok();

        }


    }
}
