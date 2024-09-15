using Microsoft.EntityFrameworkCore;
using UploadThings.Models;

namespace UploadThings.Data
{
    public class MSSQLContext : DbContext
    {
        public MSSQLContext(DbContextOptions<MSSQLContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
    }
}
