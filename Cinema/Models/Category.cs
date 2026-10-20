using System.ComponentModel.DataAnnotations;

namespace Cinema.Models
{
    public class Category
    {
        public int Id { get; set; }
        [MinLength(3)]
        [MaxLength(30)]
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
