using Cinema.Data;
using Cinema.Repositories;
using Cinema.Utilites;
using Cinema.Utilites.DbSeeding;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace Cinema
{
    public class Program
    {
        public static async Task Main(string[] args)
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

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                // Redirect paths
                options.LoginPath = "/Identity/Account/Login";               // Where to redirect unauthenticated users
                options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // Where to redirect forbidden users
            });

            builder.Services.AddScoped(typeof(IRepositry<>), typeof(Repositry<>));

            builder.Services.AddScoped<IRepositry<Movie>, Repositry<Movie>>();
            builder.Services.AddScoped<IRepositry<Models.Cinema>, Repositry<Models.Cinema>>();
            builder.Services.AddScoped<IRepositry<Actor>, Repositry<Actor>>();
            builder.Services.AddScoped<IMovieSubImageRepositry, MovieSubImageRepositry>();
            builder.Services.AddScoped<IRepositry<ApplicationUserOtp>, Repositry<ApplicationUserOtp>>();
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IDbInitializer, DbInitializer>();

            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                await dbInitializer.InitializeAsync();
            }
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
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
