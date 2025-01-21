using Microsoft.EntityFrameworkCore;
using UploadThingsGrpcService.Domain.Entities;
using UploadThingsGrpcService.Domain.Interfaces;
using UploadThingsGrpcService.Infrastructure.Data;

namespace UploadThingsGrpcService.Infrastructure.Repositories
{
    // repository for basic CRUD
    public class GeneralRepositories<T>(MSSQLContext context) : IGeneralRepository<T> where T : class, IEntity
    {
        private readonly MSSQLContext _context = context;
        private readonly DbSet<T> _dbSet = context.Set<T>();

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FirstOrDefaultAsync(entity => entity.Id == id);

        public async Task<OperationResult> AddAsync(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
                return new OperationResult { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new OperationResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        public async Task<OperationResult> UpdateAsync(T entity)
        {
            try
            {
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
                return new OperationResult { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new OperationResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        public async Task<OperationResult> DeleteAsync(int id)
        {
            try
            {
                T? entity = await GetByIdAsync(id);
                if (entity == null)
                {
                    return new OperationResult { IsSuccess = true, ErrorMessage = $"Failed to get {_dbSet.GetType()} data with Id {id}." };
                }
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
                return new OperationResult { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new OperationResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }
    }
}
