using UploadThingsGrpcService.Domain.Entities;

namespace UploadThingsGrpcService.Domain.Interfaces
{
    public interface IToDoRepository : IGeneralRepository<ToDoItem>
    {
        // Add ToDo-specific methods if needed
    }
}
