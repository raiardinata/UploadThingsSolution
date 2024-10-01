using Grpc.Net.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UploadThingsGrpcService.Infrastructure.Data;
using UploadThingsGrpcService.Infrastructure.Repositories;
using UploadThingsGrpcService.Presentation.Services;

namespace UploadThingsTestProject
{
    [TestFixture]
    public class ToDoServiceTest
    {
        private MSSQLContext _context;
        private ToDoRepositrories _repository;
        private GrpcChannel _channel;
        private IWebHost _server;

        [OneTimeSetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<MSSQLContext>()
                .UseSqlServer<MSSQLContext>()
                .Options;

            _context = new MSSQLContext(options);
            _repository = new ToDoRepositrories(_context);

            // Start the test gRPC server
            _server = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("https://localhost:7102/")
                .ConfigureServices(services =>
                {
                    services.AddGrpc();
                })
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGrpcService<ToDoServices>();
                    });
                })
                .Build();

            _server.Start();

            // Create a gRPC channel to communicate with the server
            _channel = GrpcChannel.ForAddress("https://localhost:7102/", new GrpcChannelOptions
            {
                HttpHandler = new HttpClientHandler()
            });
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _server?.StopAsync().Wait();
            _server?.Dispose();
            _channel?.Dispose();
            _context?.Dispose();
        }

        [Test]
        public async Task ListAllToDo()
        {
            // Act: Call the gRPC method and get the response
            var response = await _context.ToDoItems.ToListAsync();

            // Assert: Check that the response is as expected
            Assert.IsNotNull(response);
        }
    }
}
