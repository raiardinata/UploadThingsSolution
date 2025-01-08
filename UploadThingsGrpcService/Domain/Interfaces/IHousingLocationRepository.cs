using UploadThingsGrpcService.Domain.Entities;

namespace UploadThingsGrpcService.Domain.Interfaces
{
    public interface IHousingLocationRepository : IGeneralRepository<HousingLocation>
    {
        Task<string> Test();
        // Add housing location-specific methods if needed
    }
}
