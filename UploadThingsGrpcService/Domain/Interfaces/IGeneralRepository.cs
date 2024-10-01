namespace UploadThingsGrpcService.Domain.Interfaces
{
    public interface IGeneralRepository<T> where T : class, IEntity
    {
        // interface for basic CRUD repository
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<Exception> AddAsync(T entity);
        Task<Exception> UpdateAsync(T entity);
        Task<Exception> DeleteAsync(int id);
    }

}
