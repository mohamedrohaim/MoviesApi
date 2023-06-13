using Microsoft.EntityFrameworkCore;
using MoviesApi.Models;
using System.Collections;

namespace MoviesApi.Servises
{
    public class MovieServise : GenericServise<Movie>
    {
        private readonly ApplicationDBContext _context;
        public MovieServise(ApplicationDBContext context) : base(context)
        {

            _context = context;
        }
        public new async Task<IEnumerable> GetAllAsync()
        {
            var movies = await _context.Movies
                   .OrderByDescending(x => x.Rate)
                   .Include(g => g.Genre)
                   .ToListAsync();
            return movies;

        }
        public new async Task<Movie> GetByIdAsync()
                           => await _context.Movies
                           .Include(g => g.Genre).FirstOrDefaultAsync();


        public async Task<IEnumerable<Movie>>GetMoviesByGenreId(byte id)
        {
            var movies = await _context.Movies
                .Where(g => g.GenreId == id)
                .OrderBy(g => g.Rate)
                .Include(g => g.Genre)
                .ToListAsync();
            return movies;
        }
    }
}
