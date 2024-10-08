using UploadThingsGrpcService.Domain.Entities;
using UploadThingsGrpcService.Domain.Interfaces;
using UploadThingsGrpcService.Infrastructure.Data;

namespace UploadThingsGrpcService.Infrastructure.Repositories
{
    public class ToDoRepository : GeneralRepositories<ToDoItem>, IToDoRepository
    {
        public ToDoRepository(MSSQLContext context) : base(context)
        {
        }

        // You can also add custom methods specific to the User entity here if needed
    }
}
