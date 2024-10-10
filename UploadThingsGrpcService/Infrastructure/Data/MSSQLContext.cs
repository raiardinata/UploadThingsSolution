using Microsoft.EntityFrameworkCore;
using UploadThingsGrpcService.Domain.Entities;

namespace UploadThingsGrpcService.Infrastructure.Data
{
    public class MSSQLContext(DbContextOptions<MSSQLContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<ToDoItem> ToDoItems => Set<ToDoItem>();
        public DbSet<Product> Product => Set<Product>();
        public DbSet<CurrentIdentity> CurrentIdentity => Set<CurrentIdentity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.ProductPrice)
                      .HasPrecision(9, 4); // 9 total digits, 4 of which are after the decimal
            });
        }
    }
}
