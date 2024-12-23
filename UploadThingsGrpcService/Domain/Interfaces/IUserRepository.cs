﻿using UploadThingsGrpcService.Domain.Entities;

namespace UploadThingsGrpcService.Domain.Interfaces
{
    public interface IUserRepository : IGeneralRepository<User>
    {
        // Add user-specific methods if needed
        Task<bool> UserLogin(User user);
    }
}
