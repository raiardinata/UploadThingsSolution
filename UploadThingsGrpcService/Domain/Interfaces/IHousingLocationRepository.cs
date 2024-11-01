using UploadThingsGrpcService.Domain.Entities;

namespace UploadThingsGrpcService.Domain.Interfaces
{
    public interface IHousingLocationRepository : IGeneralRepository<HousingLocation>
    {
        Task<string> Test();
        // Add product-specific methods if needed
    }
}
