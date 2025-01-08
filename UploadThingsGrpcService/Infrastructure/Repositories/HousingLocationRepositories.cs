using UploadThingsGrpcService.Domain.Entities;
using UploadThingsGrpcService.Domain.Interfaces;
using UploadThingsGrpcService.Infrastructure.Data;

namespace UploadThingsGrpcService.Infrastructure.Repositories
{
    public class HousingLocationRepository(MSSQLContext context) : GeneralRepositories<HousingLocation>(context), IHousingLocationRepository
    {
        public async Task<string> Test()
        {
            return await Task.FromResult("Hai");
        }
        // You can also add custom methods specific to the Housing Location here if needed
    }
}
