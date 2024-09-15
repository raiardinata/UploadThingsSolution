using Microsoft.EntityFrameworkCore;
using UploadThings.Data;
using UploadThings.Models.Factories;
using UploadThings.Services.Factories;
using UploadThings.UnitofWork;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add database checkers
builder.Services.AddSingleton<MSSQLDatabaseCheckerFactory>();
builder.Services.AddSingleton<MariaDBDatabaseCheckerFactory>();
builder.Services.AddSingleton<PostgresDatabaseCheckerFactory>();
builder.Services.AddSingleton<JsonOverideFactory>();

// Register the DbContext
builder.Services.AddDbContext<MariaDBContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("MariaProductDBDBConnection"), new MySqlServerVersion(new Version(8, 0, 21))));
builder.Services.AddDbContext<MSSQLContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQLUserDBConnection")));
builder.Services.AddDbContext<PostgresContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresTransactionConnection")));

// Register UnitOfWork and Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
