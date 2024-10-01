using Microsoft.EntityFrameworkCore;
using UploadThingsGrpcService.Domain.Interfaces;
using UploadThingsGrpcService.Infrastructure.Data;

namespace UploadThingsGrpcService.Infrastructure.Repositories
{
    // repository for basic CRUD
    public class GeneralRepositories<T> : IGeneralRepository<T> where T : class, IEntity
    {
        private readonly MSSQLContext _context;
        private readonly DbSet<T> _dbSet;
        public GeneralRepositories(MSSQLContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FirstOrDefaultAsync(entity => entity.Id == id);

        public async Task<Exception> AddAsync(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
                return new Exception(null);
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public async Task<Exception> UpdateAsync(T entity)
        {
            try
            {
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
                return new Exception(null);
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public async Task<Exception> DeleteAsync(int id)
        {
            try
            {
                var entity = await GetByIdAsync(id);
                if (entity == null)
                {
                    return new Exception($"Failed to get {_dbSet.GetType()} data with Id {id}.");
                }
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
                return new Exception(null);
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}
