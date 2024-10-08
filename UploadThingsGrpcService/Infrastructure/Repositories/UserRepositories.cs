﻿using UploadThingsGrpcService.Domain.Entities;
using UploadThingsGrpcService.Domain.Interfaces;
using UploadThingsGrpcService.Infrastructure.Data;

namespace UploadThingsGrpcService.Infrastructure.Repositories
{
    public class UserRepository(MSSQLContext context) : GeneralRepositories<User>(context), IUserRepository
    {

        // You can also add custom methods specific to the User entity here if needed
    }
}
