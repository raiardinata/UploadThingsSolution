using Microsoft.EntityFrameworkCore;
using UploadThings.Models;

namespace UploadThings.Data
{
    public class PostgresContext : DbContext
    {
        public PostgresContext(DbContextOptions<PostgresContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        }
        public DbSet<Transaction> transactions { get; set; }
    }
}
