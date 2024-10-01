using Microsoft.EntityFrameworkCore;
using UploadThingsGrpcService.Domain.Interfaces;
using UploadThingsGrpcService.Infrastructure.Data;
using UploadThingsGrpcService.Infrastructure.Repositories;
using UploadThingsGrpcService.Presentation.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc().AddJsonTranscoding();
builder.Services.AddDbContext<MSSQLContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQLToDoDatabaseConnection")));

// Register your repository with the DI container
builder.Services.AddScoped(typeof(IGeneralRepository<>), typeof(GeneralRepositories<>));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<ToDoServices>();
app.MapGrpcService<UserServices>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();