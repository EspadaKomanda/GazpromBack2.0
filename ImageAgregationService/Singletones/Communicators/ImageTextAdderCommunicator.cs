using Grpc.Net.Client;
using Imagetextadder;
using Imageverifier;

namespace ImageAgregationService.Singletones.Communicators
{
    public class ImageTextAdderCommunicator 
    {
        private readonly ImageTextAdder.ImageTextAdderClient _imageTextAdderClient;
        private readonly ILogger<ImageTextAdderCommunicator> _logger;
        public ImageTextAdderCommunicator(ILogger<ImageTextAdderCommunicator> logger)
        {
            _logger = logger;
            GrpcChannel channel = GrpcChannel.ForAddress($"https://localhost:5001");
            _imageTextAdderClient = new ImageTextAdder.ImageTextAdderClient(channel);
        }
        public async Task<AddTextToImageResponse> AddText(VerifyImageResponse imageResponse, string text)
        {
            AddTextToImageRequest request = new AddTextToImageRequest
            {
                ImageByteArray = imageResponse.ImageByteArray,
                Text = text
            };
            AddTextToImageResponse response = await _imageTextAdderClient.AddTextToImageAsync(request);
            return response;
        }
    }
}