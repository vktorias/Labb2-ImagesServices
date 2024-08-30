using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace Labb2_ImagesServices
{
    public class MyComputerVisionService
    {
        private readonly string _endpoint;
        private readonly string _apiKey;
        private readonly ComputerVisionClient _computerVisionClient;

        public MyComputerVisionService(IConfiguration configuration)
        {
            _endpoint = configuration["AzureComputerVision:Endpoint"];
            _apiKey = configuration["AzureComputerVision:ApiKey"];

            _computerVisionClient = new ComputerVisionClient(new ApiKeyServiceClientCredentials(_apiKey))
            {
                Endpoint = _endpoint
            };
        }

        // Metod för att analysera en bildströmsinnehåll
        public async Task<ImageAnalysis> AnalyzeImageAsync(Stream imageStream)
        {
            var visualFeatures = new List<VisualFeatureTypes?> { VisualFeatureTypes.Tags, VisualFeatureTypes.Objects };
            return await _computerVisionClient.AnalyzeImageInStreamAsync(imageStream, visualFeatures: visualFeatures);
        }

        // Metod för att analysera en bild via en URL
        public async Task<ImageAnalysis> AnalyzeImageAsync(string imageUrl)
        {
            // Skapar en lista över visuella funktioner som ska extraheras från bilden
            var features = new List<VisualFeatureTypes?> { VisualFeatureTypes.Tags, VisualFeatureTypes.Objects };

            // Anropar Azure Computer Vision API för att analysera bilden och returnerar analysresultatet
            return await _computerVisionClient.AnalyzeImageAsync(imageUrl, visualFeatures: features);
        }

        // Metod för att generera en miniatyrbild från en bildströmsinnehåll
        public async Task<Stream> GenerateThumbnailAsync(Stream imageStream, int width, int height)
        {
            return await _computerVisionClient.GenerateThumbnailInStreamAsync(width, height, imageStream, true);
        }
        public async Task<Stream> GenerateThumbnailAsync(string imageUrl, int width, int height)
        {
            return await _computerVisionClient.GenerateThumbnailAsync(width, height, imageUrl, true);
        }
    }
}
