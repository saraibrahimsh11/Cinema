using Cinema.Data;
using Cinema.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cinema
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var connectionString =
                builder.Configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("Connection string"
                    + "'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString)
                );

            builder.Services.AddScoped(typeof(IRepositry<>), typeof(Repositry<>));

            builder.Services.AddScoped<IRepositry<Movie>, Repositry<Movie>>();
            builder.Services.AddScoped<IRepositry<Models.Cinema>, Repositry<Models.Cinema>>();
            builder.Services.AddScoped<IRepositry<Actor>, Repositry<Actor>>();
            builder.Services.AddScoped<IMovieSubImageRepositry, MovieSubImageRepositry>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
