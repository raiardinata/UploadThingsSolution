using UploadThings.Data;
using UploadThings.Models;
using UploadThings.Repositories;
using UploadThings.Repositories.SettingRepositories;

namespace UploadThings.UnitofWork
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Product> Products { get; }
        IRepository<User> Users { get; }
        IRepository<Transaction> Transactions { get; }
        Task CompleteAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly MariaDBContext _mariaDBContext;
        private readonly MSSQLContext _mssqlContext;
        private readonly PostgresContext _postgresContext;

        public IRepository<Product> Products { get; }
        public IRepository<User> Users { get; }
        public IRepository<Transaction> Transactions { get; }

        public UnitOfWork(MariaDBContext mariaDBContext, MSSQLContext mssqlContext, PostgresContext postgresContext)
        {
            _mariaDBContext = mariaDBContext;
            _mssqlContext = mssqlContext;
            _postgresContext = postgresContext;
            Products = new ProductRepository(mariaDBContext);
            Users = new UserRepository(mssqlContext);
            Transactions = new TransactionRepository(postgresContext);
        }

        public async Task CompleteAsync()
        {
            await _mariaDBContext.SaveChangesAsync();
            await _mssqlContext.SaveChangesAsync();
            await _postgresContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _mariaDBContext.Dispose();
            _mssqlContext.Dispose();
            _postgresContext.Dispose();
        }
    }

}
