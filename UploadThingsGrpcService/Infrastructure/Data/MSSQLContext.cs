using Microsoft.EntityFrameworkCore;
using UploadThingsGrpcService.Domain.Entities;

namespace UploadThingsGrpcService.Infrastructure.Data
{
    public class MSSQLContext(DbContextOptions<MSSQLContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<ToDoItem> ToDoItems => Set<ToDoItem>();
        public DbSet<Product> Product => Set<Product>();
        public DbSet<HousingLocation> HousingLocations => Set<HousingLocation>();
        public DbSet<PizzaSpecial> PizzaSpecial => Set<PizzaSpecial>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(static entity =>
            {
                entity.Property(static e => e.ProductPrice)
                    .HasPrecision(9, 4); // 9 total digits, 4 of which are after the decimal
            });
        }
    }
}
