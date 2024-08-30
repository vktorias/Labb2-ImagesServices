using Microsoft.AspNetCore.Http.Features;

namespace Labb2_ImagesServices
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // L�ser in inst�llningarna fr�n appsettings.json
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            // L�gg till tj�nster f�r Azure Computer Vision
            builder.Services.AddSingleton<MyComputerVisionService>();

            // L�gg till MVC-tj�nster
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

            // L�gg till standardrouten f�r kontrollern "Images"
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Images}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
