using Microsoft.AspNetCore.Http.Features;

namespace Labb2_ImagesServices
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Läser in inställningarna från appsettings.json
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            // Lägg till tjänster för Azure Computer Vision
            builder.Services.AddSingleton<MyComputerVisionService>();

            // Lägg till MVC-tjänster
            builder.Services.AddControllersWithViews();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // Lägg till standardrouten för kontrollern "Images"
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Images}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
