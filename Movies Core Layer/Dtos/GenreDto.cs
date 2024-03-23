using System.ComponentModel.DataAnnotations;

namespace Movies_Core_Layer.Dtos
{
    public class GenreDto
    {
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
