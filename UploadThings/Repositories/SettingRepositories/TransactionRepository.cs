using Microsoft.EntityFrameworkCore;
using UploadThings.Data;
using UploadThings.Models;

namespace UploadThings.Repositories.SettingRepositories
{
    public class TransactionRepository : IRepository<Transaction>
    {
        private readonly PostgresContext _context;

        public TransactionRepository(PostgresContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Transaction>> GetAllAsync()
        {
            return await _context.transactions.ToListAsync();
        }

        public async Task<Transaction> GetByIdAsync(int id)
        {
            return await _context.transactions.FindAsync(id);
        }

        public async Task AddAsync(Transaction entity)
        {
            await _context.transactions.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Transaction entity)
        {
            _context.transactions.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var Transaction = await _context.transactions.FindAsync(id);
            if (Transaction != null)
            {
                _context.transactions.Remove(Transaction);
                await _context.SaveChangesAsync();
            }
        }
    }

}
