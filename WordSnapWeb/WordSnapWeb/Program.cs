using Microsoft.EntityFrameworkCore;
using WordSnapWeb.Models;
using WordSnapWeb.Services;

namespace WordSnapWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var directory = Path.Combine(Directory.GetCurrentDirectory(), "bin/Debug/net9.0");
            var configuration = new ConfigurationBuilder()
                        .SetBasePath(directory)
                        .AddJsonFile("appsettings.json")
                        .Build();
            var connectionString = configuration.GetConnectionString("DatabaseConnection");
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<WordSnapDbContext>(options => options.UseNpgsql(connectionString));
            builder.Services.AddScoped<IWordSnapRepository, WordSnapRepository>();
            builder.Services.AddScoped<IValidationService, ValidationService>();
            builder.Services.AddScoped<AuthenticationService>();

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
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
