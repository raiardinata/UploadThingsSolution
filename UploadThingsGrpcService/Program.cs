using Microsoft.EntityFrameworkCore;
using UploadThingsGrpcService.Application.Services;
using UploadThingsGrpcService.Domain.Interfaces;
using UploadThingsGrpcService.Infrastructure;
using UploadThingsGrpcService.Infrastructure.Data;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc().AddJsonTranscoding();
builder.Services.AddDbContext<MSSQLContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQLToDoDatabaseConnection")));

// Register your Unit of Work pattern.
builder.Services.AddScoped<IUnitOfWork, UnitofWork>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<ToDoServices>();
app.MapGrpcService<UserServices>();
app.MapGrpcService<ProductServices>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
