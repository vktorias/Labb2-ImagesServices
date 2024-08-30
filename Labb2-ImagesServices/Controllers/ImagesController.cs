using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Linq.Expressions;

namespace Labb2_ImagesServices.Controllers
{
    public class ImagesController : Controller
    {
        private readonly MyComputerVisionService _computerVisionService;

        public ImagesController(MyComputerVisionService computerVisionService)
        {
            _computerVisionService = computerVisionService;
        }

        public IActionResult Index()
        {
            return View();
        }

        //Action-metod för att hantera formulärinlämningar via HttpPost för att analysera en bild
        [HttpPost]
        public async Task<IActionResult> Analyze(IFormFile imageFile, string imageUrl, int thumbnailWidth = 100, int thumbnailHeight = 100)
        {
            //Kontrollera om varken en bildfil eller bild-URL har valts
            if (imageFile == null && string.IsNullOrWhiteSpace(imageUrl))
            {
                ModelState.AddModelError(string.Empty, "Please upload an image or provide a valid image URL.");
                ViewBag.ThumbnailWidth = thumbnailWidth;
                ViewBag.ThumbnailHeight = thumbnailHeight;
                return View("Upload");
            }

            ImageAnalysis analysisResult = null;
            Stream thumbnailStream = null;

            try
            {
                // Kontrollera om en bildfil har valts och har innehåll
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Läser in strömmen från bildfilen
                    using (var stream = imageFile.OpenReadStream())
                    {
                        analysisResult = await _computerVisionService.AnalyzeImageAsync(stream);
                        thumbnailStream = await _computerVisionService.GenerateThumbnailAsync(imageFile.OpenReadStream(), thumbnailWidth, thumbnailHeight);
                    }
                }
                // Om en bild-URL har valts istället för en fil
                else if (!string.IsNullOrWhiteSpace(imageUrl))
                {
                    // Kontrollera om den valda URL:en är välformaterad
                    if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
                    {
                        ModelState.AddModelError(string.Empty, "The provided URL is not valid."); // Lägger till ett modellfel om URL:en inte är giltig
                        ViewBag.ThumbnailWidth = thumbnailWidth; // Lagrar miniatyrens bredd i ViewBag
                        ViewBag.ThumbnailHeight = thumbnailHeight; // Lagrar miniatyrens höjd i ViewBag
                        return View("Index"); // Returnerar till "Index"-vyn
                    }

                    // Array av giltiga bildfiler
                    string[] validImageExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
                    var uri = new Uri(imageUrl); // Skapar ett URI-objekt från bild-URL:en
                    // Kontrollera om URL:en slutar med en giltig bildfiländelse
                    if (!validImageExtensions.Any(ext => uri.AbsolutePath.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                    {
                        ModelState.AddModelError(string.Empty, "The provided URL must link to an image file (e.g. .jpg, .png, .gif).");
                        return View("Index");
                    }

                    // Analyserar bilden från URL:en med hjälp av datorseendetjänsten
                    analysisResult = await _computerVisionService.AnalyzeImageAsync(imageUrl);
                    // Genererar en miniatyrbild för bilden från URL:en
                    thumbnailStream = await _computerVisionService.GenerateThumbnailAsync(imageUrl, thumbnailWidth, thumbnailHeight);
                }

                // Lagrar analysresultatet och miniatyren i ViewBag för att skicka till vyn
                ViewBag.AnalysisResult = analysisResult;
                // Konverterar strömmen för miniatyren till en base64-sträng för att visa i vyn
                ViewBag.Thumbnail = ConvertToBase64(thumbnailStream);

                return View("Result");
            }
            // Hanterar fel och lägger till ett felmeddelande i modellens tillstånd
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while analyzing the image: " + ex.Message);
                ViewBag.ThumbnailWidth = thumbnailWidth;
                ViewBag.ThumbnailHeight = thumbnailHeight;
                return View("Index");
            }
        }

        // Privat hjälpfunktion för att konvertera en ström till en base64-sträng
        private string ConvertToBase64(Stream stream)
        {
            // Skapar en minnesström för att hålla bilddata
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream); // Kopierar innehållet i ingångsströmmen till minnesströmmen
                var bytes = memoryStream.ToArray(); // Konverterar minnesströmmen till en byte-array
                return Convert.ToBase64String(bytes); // Konverterar byte-arrayen till en base64-sträng och returnerar den
            }
        }
    }
}
