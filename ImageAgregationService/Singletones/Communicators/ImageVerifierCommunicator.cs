using Grpc.Net.Client;
using Imagegenerator;
using Imageverifier;

namespace ImageAgregationService.Singletones.Communicators
{
    public class ImageVerifierCommunicator
    {
        private readonly ImageVerifier.ImageVerifierClient _imageVerifierClient;
        private readonly ILogger<ImageVerifierCommunicator> _logger;
        public ImageVerifierCommunicator(ILogger<ImageVerifierCommunicator> logger)
        {
            _logger = logger;
            GrpcChannel channel = GrpcChannel.ForAddress($"https://localhost:5001");
            _imageVerifierClient = new ImageVerifier.ImageVerifierClient(channel);
        }
        public async Task<VerifyImageResponse> VerifyImage(GenerateImageResponse imageResponse)
        {
            VerifyImageRequest request = new VerifyImageRequest
            {
                ImageByteArray = imageResponse.ImageByteArray,
                Template = imageResponse.Template
            };
            VerifyImageResponse response = await _imageVerifierClient.VerifyImageAsync(request);
            return response;
        }
    }
}