using Cinema.Models;

namespace Cinema.ViewModel
{
    public class HomeVM
    {
        public List<Movie> NowShowing { get; set; }
        public List<Movie> ComingSoon { get; set; }
        public int MoviesCount { get; set; }
        public int ActorsCount { get; set; }
        public int CategoriesCount { get; set; }
        public int CinemasCount { get; set; }
    }
}
