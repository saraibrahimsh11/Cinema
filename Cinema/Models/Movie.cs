using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Status { get; set; }
        public string MainImg { get; set; }
        public DateTime DateTime { get; set; }
        public int Quantity { get; set; }
        public int Rate { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }
        public int CinemaId { get; set; }
        [ForeignKey(nameof(CinemaId))]
        public Cinema Cinema { get; set; }
        public List<Actor> Actors { get; set; }
    }
}
