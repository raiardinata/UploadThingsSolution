using UploadThingsGrpcService.Domain.Entities;

namespace UploadThingsGrpcService.Domain.Interfaces
{
    public interface IGeneralRepository<T> where T : class, IEntity
    {
        // interface for basic CRUD repository
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<OperationResult> AddAsync(T entity);
        Task<OperationResult> UpdateAsync(T entity);
        Task<OperationResult> DeleteAsync(int id);
    }

}
