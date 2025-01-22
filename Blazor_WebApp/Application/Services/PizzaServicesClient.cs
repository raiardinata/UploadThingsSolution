using System.Collections.ObjectModel;
using UploadThingsGrpcService.PizzaSpecialProto;

namespace Blazor_WebApp.Application.Services
{
    /// <summary>
    /// Service client for managing pizza data via RESTful API and gRPC.
    /// Initializes a new instance of the <see cref="PizzaServiceClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client for RESTful API calls.</param>
    /// <param name="pizzaSpecialServiceClient">The gRPC client for gRPC API calls.</param>
    public class PizzaServiceClient(HttpClient httpClient, PizzaSpecialService.PizzaSpecialServiceClient pizzaSpecialServiceClient)
    {
        private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        private readonly PizzaSpecialService.PizzaSpecialServiceClient _pizzaSpecialServiceClient = pizzaSpecialServiceClient ?? throw new ArgumentNullException(nameof(pizzaSpecialServiceClient));

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
            PizzaSpecialResponse response = await _httpClient.GetFromJsonAsync<PizzaSpecialResponse>("https://localhost:7102/v1/PizzaSpecial/GetAllList") ?? new PizzaSpecialResponse();

            return new ReadOnlyCollection<ReadPizzaSpecialResponse>(response.PizzaSpecialData);
        }

        /// <summary>
        /// Fetches pizza specials using the gRPC API.
        /// </summary>
        /// <returns>A read-only list of pizza specials.</returns>
        public async Task<IReadOnlyList<ReadPizzaSpecialResponse>> GetPizzaGRpcAsync()
        {
            GetAllResponse response = await _pizzaSpecialServiceClient.ListPizzaSpecialAsync(new GetAllRequest());
            return new ReadOnlyCollection<ReadPizzaSpecialResponse>([.. response.PizzaSpecialData]);
        }
    }
}
