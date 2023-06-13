using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Dtos.Genres;
using MoviesApi.Models;
using MoviesApi.Servises;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {

        private readonly GenreServise _servise;
        public GenresController(GenreServise servise)
        {
            _servise = servise;

        }




        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var genres = await _servise.GetAllAsync();
            return Ok(genres);

        }
        [HttpGet(template:"{id}")]
        public async Task<IActionResult> GetAllAsyncById(int id) { 
        var movie= await _servise.GetByIdAsync(id);
            if(movie==null)
                return NotFound(new { status="fauled",message=$"No Movie with id {id}"});
            return Ok(movie);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateGenreDto genreDto)
        {
            var genre = new Genre() {
                Name = genreDto.Name,
            };

            int status=await _servise.CreateAsync(genre);
            if (status > 0)
            {
                var genreData=_servise.GetByIdAsync(genre.Id);
                return Ok(genreData);
            }
            return BadRequest(new { status = "failed", message="failed to create Genre"});
            


            

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(byte id,[FromBody] CreateGenreDto genreDto) {
            var genre = await _servise.GetByIdAsync(id);
            if (genre == null) {
                var message = new { status = "failed" ,
                    
                message=$"genre not found" };
            return NotFound (message);
            }
            genre.Name = genreDto.Name;
            int status=await _servise.UpdateAsync(genre);
            if (status > 0)
            {
                var genreData = _servise.GetByIdAsync(genre.Id);
             
                return Ok(new {status="success",genreData});
            }
            return BadRequest(new { status = "failed", message = "failed to update Genre" });

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(byte id)
        {
            var genre = await _servise.GetByIdAsync(id);
            if (genre == null)
            {
                return NotFound(value: $"No genre was found with ID :{id}");
            }
           int status= await _servise.DeleteAsync(genre); 
            if (status > 0)
                 return Ok(new { status = "Success", message = "Genre Deleted Successfully" });
            return BadRequest(new { status = "failed", message = "failed to Delete Genre" });
        }



    }
}
