using UploadThingsGrpcService.Domain.Entities;
using UploadThingsGrpcService.Domain.Interfaces;
using UploadThingsGrpcService.Infrastructure.Data;

namespace UploadThingsGrpcService.Infrastructure.Repositories
{
    public class ProductRepository(MSSQLContext context) : GeneralRepositories<Product>(context), IProductRepository
    {
        public async Task<string> Test()
        {
            return await Task.FromResult("Hai");
        }
        // You can also add custom methods specific to the User entity here if needed
    }
}
