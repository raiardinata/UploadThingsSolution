using System.Collections.ObjectModel;
using UploadThingsGrpcService.PizzaSpecialProto;

namespace Blazor_WebApp.Application.Services
{
    /// <summary>
    /// Service client for managing pizza data via RESTful API and gRPC.
    /// </summary>
    public class PizzaServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly PizzaSpecialService.PizzaSpecialServiceClient _pizzaSpecialServiceClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="PizzaServiceClient"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client for RESTful API calls.</param>
        /// <param name="pizzaSpecialServiceClient">The gRPC client for gRPC API calls.</param>
        public PizzaServiceClient(HttpClient httpClient, PizzaSpecialService.PizzaSpecialServiceClient pizzaSpecialServiceClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _pizzaSpecialServiceClient = pizzaSpecialServiceClient ?? throw new ArgumentNullException(nameof(pizzaSpecialServiceClient));
        }

        /// <summary>
        /// Represents the response for pizza specials in the RESTful API.
        /// </summary>
        private class PizzaSpecialResponse
        {
            public List<ReadPizzaSpecialResponse> PizzaSpecialData { get; set; } = [];
        }

        /// <summary>
        /// Fetches pizza specials using the RESTful API.
        /// </summary>
        /// <returns>A read-only list of pizza specials.</returns>
        public async Task<IReadOnlyList<ReadPizzaSpecialResponse>> GetPizzaRESTfulAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<PizzaSpecialResponse>("https://localhost:7102/v1/PizzaSpecial/GetAllList")
                ?? new PizzaSpecialResponse();

            return new ReadOnlyCollection<ReadPizzaSpecialResponse>(response.PizzaSpecialData);
        }

        /// <summary>
        /// Fetches pizza specials using the gRPC API.
        /// </summary>
        /// <returns>A read-only list of pizza specials.</returns>
        public async Task<IReadOnlyList<ReadPizzaSpecialResponse>> GetPizzaGRpcAsync()
        {
            var response = await _pizzaSpecialServiceClient.ListPizzaSpecialAsync(new GetAllRequest());
            return new ReadOnlyCollection<ReadPizzaSpecialResponse>([.. response.PizzaSpecialData]);
        }
    }
}
