using Cinema.Models;

namespace Cinema.ViewModel
{
    public class DashboardVM
    {
        public int MoviesCount { get; set; }
        public int CategoriesCount { get; set; }
        public int CinemasCount { get; set; }
        public int ActorsCount { get; set; }
        public int NowShowing { get; set; }
        public int ComingSoon { get; set; }
        public int Ended { get; set; }
        public decimal AveragePrice { get; set; }
        public double AverageRate { get; set; }
        public int TotalQuantity { get; set; }
        public List<Movie> TopRated { get; set; } = new();
        public List<Movie> Latest { get; set; } = new();
    }
}
