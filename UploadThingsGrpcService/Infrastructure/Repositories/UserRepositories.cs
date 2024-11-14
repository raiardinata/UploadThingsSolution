using Microsoft.EntityFrameworkCore;
using UploadThingsGrpcService.Domain.Entities;
using UploadThingsGrpcService.Domain.Interfaces;
using UploadThingsGrpcService.Infrastructure.Data;

namespace UploadThingsGrpcService.Infrastructure.Repositories
{
    public class UserRepository(MSSQLContext context) : GeneralRepositories<User>(context), IUserRepository
    {
        private readonly DbSet<User> _dbSet = context.Set<User>();

        // You can also add custom methods specific to the User entity here if needed
        public async Task<bool> UserLogin(User user)
        {
            try
            {
                User? dbResponse = await _dbSet.FirstOrDefaultAsync(context => context.Email == user.Email && context.PasswordHashed == user.PasswordHashed);

                return await Task.FromResult(dbResponse != null);
            }
            catch
            {
                return await Task.FromResult(false);
            }
        }
    }
}
