using System.ComponentModel.DataAnnotations;

namespace MoviesApi.Dtos.Genres
{
    public class CreateGenreDto
    {

        [MaxLength(length: 100)]
        public string Name { get; set; }
    }
}
