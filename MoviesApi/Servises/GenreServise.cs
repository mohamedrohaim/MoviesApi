using Microsoft.EntityFrameworkCore;
using MoviesApi.Models;

namespace MoviesApi.Servises
{
    public class GenreServise : GenericServise<Genre>
    {
        private readonly ApplicationDBContext _context;
        public GenreServise(ApplicationDBContext context) : base(context)
        {
            _context = context;

        }
        public new async Task<int> DeleteAsync(Genre genre) {
           _context.Genres.Remove(genre);
            return await _context.SaveChangesAsync();
        
        }
        public new  async Task<Genre> GetByIdAsync(byte id)
            => await _context.Genres.FindAsync(id);


    }
}
