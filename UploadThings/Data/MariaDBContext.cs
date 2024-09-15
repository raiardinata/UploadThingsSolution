using Microsoft.EntityFrameworkCore;
using UploadThings.Models;

namespace UploadThings.Data
{
    public class MariaDBContext : DbContext
    {
        public MariaDBContext(DbContextOptions<MariaDBContext> options) : base(options) { }
        public DbSet<Product> Products { get; set; }
    }
}
