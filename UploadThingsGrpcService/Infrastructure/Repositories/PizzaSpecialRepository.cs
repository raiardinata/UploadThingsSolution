using UploadThingsGrpcService.Domain.Entities;
using UploadThingsGrpcService.Domain.Interfaces;
using UploadThingsGrpcService.Infrastructure.Data;

namespace UploadThingsGrpcService.Infrastructure.Repositories
{
    public class PizzaSpecialRepository(MSSQLContext context) : GeneralRepositories<PizzaSpecial>(context), IPizzaSpecialRepository
    {
        // You can also add custom methods specific to the Pizza Special here if needed
    }
}
