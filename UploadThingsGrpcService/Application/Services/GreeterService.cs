using Grpc.Core;
using UploadThingsGrpcService.Greeter;

namespace UploadThingsGrpcService.Application.Services
{
    public class GreeterService(ILogger<GreeterService> logger) : Greeter.Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger = logger;

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }
    }
}
