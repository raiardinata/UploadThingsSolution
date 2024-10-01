using UploadThingsGrpcService.Domain.Entities;
using UploadThingsGrpcService.Domain.Interfaces;
using UploadThingsGrpcService.Infrastructure.Data;

namespace UploadThingsGrpcService.Infrastructure.Repositories
{
    public class ToDoRepositrory : GeneralRepositories<ToDoItem>, IToDoRepository
    {
        public ToDoRepositrory(MSSQLContext context) : base(context)
        {
        }

        // You can also add custom methods specific to the User entity here if needed
    }
}
