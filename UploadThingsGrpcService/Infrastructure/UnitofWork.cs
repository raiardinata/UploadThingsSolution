using UploadThingsGrpcService.Domain.Interfaces;
using UploadThingsGrpcService.Infrastructure.Data;
using UploadThingsGrpcService.Infrastructure.Repositories;

namespace UploadThingsGrpcService.Infrastructure
{
    public class UnitofWork(MSSQLContext context) : IUnitOfWork
    {
        private readonly MSSQLContext _context = context;

        public IProductRepository ProductRepository { get; } = new ProductRepository(context);
        public IToDoRepository ToDoRepository { get; } = new ToDoRepository(context);
        public IUserRepository UserRepository { get; } = new UserRepository(context);
        public IHousingLocationRepository HousingLocationRepository { get; } = new HousingLocationRepository(context);
        public IPizzaSpecialRepository PizzaSpecialRepository { get; } = new PizzaSpecialRepository(context);

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
