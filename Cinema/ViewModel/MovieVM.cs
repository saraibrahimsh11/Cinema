using Cinema.Models;

namespace Cinema.ViewModel
{
    public class MovieVM
    {
        public List<Movie> Movies { get; set; }
        public List<MovieSubImage> MovieSubImages { get; set; }
        public List<Actor> Actors { get; set; }
        public List<Category> Categories { get; set; }
        public List<Models.Cinema> Cinemas { get; set; }
    }
}
