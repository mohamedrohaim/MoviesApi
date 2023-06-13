using Microsoft.EntityFrameworkCore;
using MoviesApi.Models;

namespace MoviesApi.Servises
{
    public class GenericServise<T> : IGenericServise<T> where T : class
    {
        private readonly ApplicationDBContext _context;
        public GenericServise(ApplicationDBContext context) {
            _context = context;
        }
        public async Task<int> CreateAsync(T item)
        {
            await _context.Set<T>().AddAsync(item);
            return await _context.SaveChangesAsync();
             
        }

        public async Task<int> DeleteAsync(T item)
        {
              _context.Set<T>().Remove(item);
            return  await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
            =>await _context.Set<T>().ToListAsync();


        public async Task<T> GetByIdAsync(int id)
            => await _context.Set<T>().FindAsync(id);




        public async Task<int> UpdateAsync(T item)
        {
            _context.Set<T>().Update(item);
            return await _context.SaveChangesAsync();
        }
    
        }
}
