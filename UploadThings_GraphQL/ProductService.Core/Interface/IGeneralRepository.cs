namespace ProductService.Core.Interface
{
    public interface IGeneralRepository<TEntity, TId> where TEntity : class
    {
        // interface for basic CRUD repository
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(TId id);
        Task<OperationResult> AddAsync(TEntity entity);
        Task<OperationResult> UpdateAsync(TEntity entity);
        Task<OperationResult> DeleteAsync(TId id);
    }
}
