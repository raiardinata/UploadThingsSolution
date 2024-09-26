using Microsoft.EntityFrameworkCore;
using UploadThings.Models;
using UploadThingsGrpcService.Models;

namespace UploadThingsGrpcService.Data
{
    public class MSSQLContext : DbContext
    {
        public MSSQLContext(DbContextOptions<MSSQLContext> options) : base(options) { }
        public DbSet<ToDoItem> ToDoItems => Set<ToDoItem>();
        public DbSet<User> Users { get; set; }
    }
}
