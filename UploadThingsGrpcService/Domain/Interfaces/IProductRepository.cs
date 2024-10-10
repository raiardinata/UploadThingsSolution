using UploadThingsGrpcService.Domain.Entities;

namespace UploadThingsGrpcService.Domain.Interfaces
{
    public interface IProductRepository : IGeneralRepository<Product>
    {
        Task<string> Test();
        // Add product-specific methods if needed
    }
}
