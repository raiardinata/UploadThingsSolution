namespace UploadThingsGrpcService.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository ProductRepository { get; }
        IToDoRepository ToDoRepository { get; }
        IUserRepository UserRepository { get; }

        Task SaveAsync();  // This commits all changes in a transaction
    }
}
