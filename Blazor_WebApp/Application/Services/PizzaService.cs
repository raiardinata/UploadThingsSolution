using Blazor_WebApp.Data;
using System.Collections.ObjectModel;

namespace Blazor_WebApp.Application.Services
{
    public class PizzaService(HttpClient httpClient) // HttpClient httpClient, Constructor for dependency injection
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<IReadOnlyList<PizzaEntities>> GetPizzaAsync()
        {
            // Fetch the outer JSON and deserialize it into the PizzaSpecialResponse
            var response = await _httpClient.GetFromJsonAsync<PizzaSpecialResponse>("https://localhost:7102/v1/PizzaSpecial/GetAllList");

            // Convert the List<PizzaEntities> to IReadOnlyList<PizzaEntities>
            return new ReadOnlyCollection<PizzaEntities>(response?.PizzaSpecialData ?? []);
        }
    }
}
