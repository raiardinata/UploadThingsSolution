using Microsoft.EntityFrameworkCore;
using UploadThingsGrpcService.Data;
using UploadThingsGrpcService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc().AddJsonTranscoding();
builder.Services.AddDbContext<MSSQLContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQLToDoDatabaseConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<ToDoServices>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();