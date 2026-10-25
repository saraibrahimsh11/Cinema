namespace Cinema.Repositories
{
    public interface IMovieSubImageRepositry : IRepositry<MovieSubImage>
    {
        void RemoveRange(IEnumerable<MovieSubImage> movieSubImages);
    }
}
