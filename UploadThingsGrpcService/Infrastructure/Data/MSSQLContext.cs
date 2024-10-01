using Microsoft.EntityFrameworkCore;
using UploadThingsGrpcService.Domain.Entities;

namespace UploadThingsGrpcService.Infrastructure.Data
{
    public class MSSQLContext : DbContext
    {
        public MSSQLContext(DbContextOptions<MSSQLContext> options) : base(options) { }
        public DbSet<User> Users => Set<User>();
        public DbSet<ToDoItem> ToDoItems => Set<ToDoItem>();
    }
}
