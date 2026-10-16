using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.Models
{
    [PrimaryKey(nameof(Image), nameof(MovieId))]
    public class MovieSubImage
    {
        public string Image { get; set; }
        public int MovieId { get; set; }
        [ForeignKey(nameof(MovieId))]
        public Movie Movie { get; set; }
    }
}
