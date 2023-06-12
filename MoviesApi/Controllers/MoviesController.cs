using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.VisualBasic;
using MoviesApi.Dtos.Genres;
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

        private List<string> _allowedExtentions=new List<string> { ".jpg",".png"};
        private long _maxPosterSize = 1048576;
        private readonly ApplicationDBContext _context;
        public MoviesController(ApplicationDBContext context)
        {
            _context = context;
        }

     


        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] MovieDto dto) {
            if(! await isValidGenre(dto.GenreId))
                return BadRequest(new { stauts = stauts[1], message = "Genre not found" });
            if (!checkExtention(dto.Poster)) { 
            return BadRequest(new { stauts= stauts[1],message="only .png and jpg are allowed" });
            }
            if(dto.Poster.Length>_maxPosterSize)
                return BadRequest(new { stauts = stauts[1], message = "Max photo size is 1 MB" });
            var movie=new Movie() { 
            Title=dto.Title,
            Rate=dto.Rate,
            StoreLine=dto.StoreLine,
            Year=dto.Year,
            GenreId=dto.GenreId,
            Poster =await GetByteArrayFromStreamAsync(dto.Poster),

            };
            try { 
             await _context.AddAsync(movie);
             _context.SaveChanges();
             return Ok(new { status = stauts[0], movie });
            }
            catch(Exception ex)
            {
                return BadRequest(new {stauts = stauts[1], ex.Message });
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

        private  bool checkSize(long size) {
            return size> _maxPosterSize;
        }

        private bool checkExtention(IFormFile file) {
            return _allowedExtentions.Contains(Path.GetExtension(file.FileName.ToLower()));
        
        }
        private async Task<bool> isValidGenre(byte id) 
            =>await _context.Genres.AnyAsync(g => g.Id == id);
        

    }
}
