using Google.Protobuf.Collections;
using Grpc.Net.Client;
using ImageAgregationService.Models.RequestModels;
using Imagegenerator;
using ImageProcessor;

namespace ImageAgregationService.Singletones.Communicators
{
    public class ImageProcessorCommunicator
    {
        private readonly ImageProcessor.ImageProcessor.ImageProcessorClient _imageVerifierClient;
        private readonly ILogger<ImageProcessorCommunicator> _logger;
        public ImageProcessorCommunicator(ILogger<ImageProcessorCommunicator> logger)
        {
            _logger = logger;
            GrpcChannel channel = GrpcChannel.ForAddress($"https://localhost:5001");
            _imageVerifierClient = new ImageProcessor.ImageProcessor.ImageProcessorClient(channel);
        }
        public async Task<ImageResponse> VerifyImage(GenerateImageResponse imageResponse, GenerateImageKafkaRequest generateImageKafkaRequest)
        {
            ImageRequest request = new ImageRequest
            {
                ByteImage = imageResponse.ImageByteArray,
                Text = generateImageKafkaRequest.ImageText,
                Background = generateImageKafkaRequest.Background,
                Width = generateImageKafkaRequest.Resolution.Width,
                Height = generateImageKafkaRequest.Resolution.Height,
                ResolutionPos = generateImageKafkaRequest.Position.ToString(),
                Font = generateImageKafkaRequest.FontName,
                ShouldCheckColors = generateImageKafkaRequest.CheckColours
            };
            request.AllowedColorsStr.AddRange( generateImageKafkaRequest.AllowedColors);
            ImageResponse response = await _imageVerifierClient.VerifyImageAsync(request);
            return response;
        }
    }
}