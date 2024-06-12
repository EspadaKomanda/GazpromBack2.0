using Grpc.Net.Client;
using ImageAgregationService.Models.RequestModels;
using Imagegenerator;

namespace ImageAgregationService.Singletones.Communicators
{
    public class ImageGenerationCommunicator 
    { 
        private readonly ImageGenerator.ImageGeneratorClient _imageGeneratorClient;
        private readonly ILogger<ImageGenerationCommunicator> _logger;
        public ImageGenerationCommunicator(ILogger<ImageGenerationCommunicator> logger)
        {
            _logger = logger;
            GrpcChannel channel = GrpcChannel.ForAddress($"https://localhost:5001");
            _imageGeneratorClient = new ImageGenerator.ImageGeneratorClient(channel);
        }
        public async Task<GenerateImageResponse> GenerateImage(string prompt)
        {
            GenerateImageRequest request = new GenerateImageRequest
            {
                Prompt = prompt
            };
            GenerateImageResponse response = await _imageGeneratorClient.GenerateImageAsync(request);
            return response;
        }
    }
}