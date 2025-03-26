using ProductService.Core.Entities;

namespace ProductService.Core.Interface
{
    public interface IProductRepository : IGeneralRepository<Product, Guid>
    {
        // Add Product specific methods in here if needed
    }
}
