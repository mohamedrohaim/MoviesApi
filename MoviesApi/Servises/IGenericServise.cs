namespace MoviesApi.Servises
{
    public interface IGenericServise<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetByIdAsync(int id);

        Task<int> CreateAsync(T item);
        
        Task<int> UpdateAsync(T item);

        Task<int> DeleteAsync(T item);



    }
}
