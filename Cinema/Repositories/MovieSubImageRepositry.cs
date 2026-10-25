using Cinema.Data;

namespace Cinema.Repositories
{
    public class MovieSubImageRepositry : Repositry<MovieSubImage> , IMovieSubImageRepositry
    {
        public MovieSubImageRepositry(ApplicationDbContext context) : base(context)
        {
        }

        public void RemoveRange(IEnumerable<MovieSubImage> movieSubImages)
        {
            _context.RemoveRange(movieSubImages);
        }
    }
}
