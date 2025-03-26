namespace ProductService.Core.Interface
{
    public interface IUnitofWork : IDisposable
    {
        IProductRepository ProductRepository { get; }

        Task<int> SaveChangesAsync();
    }
}
