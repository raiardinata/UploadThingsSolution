using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using UploadThingsGrpcService.Application.Services;
using UploadThingsGrpcService.Domain.Interfaces;
using UploadThingsGrpcService.Infrastructure;
using UploadThingsGrpcService.Infrastructure.Data;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add CORS setting
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
        policy.WithOrigins("https://localhost:4430") // Update with your frontend's origin
              .AllowAnyMethod()
              .AllowAnyHeader());
});

builder.Services.AddHealthChecks();
builder.Services.AddGrpc().AddJsonTranscoding(); // Enabling gRpc and REST capability through JsonTranscoding
builder.Services.AddEndpointsApiExplorer(); // Enables OpenAPI (Swagger) documentation for Minimal APIs. Ensures that non-controller-based APIs (MapGet(), MapPost()) appear in Swagger.

builder.Services.AddGrpcSwagger(); // Enable Swagger to use JsonTranscoding metadata
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo { Title = "Scalar API Documentation", Version = "v1" });

    string filePath = Path.Combine(System.AppContext.BaseDirectory, "Server.xml");
    c.IncludeXmlComments(filePath);
    c.IncludeGrpcXmlComments(filePath, includeControllerXmlComments: true);
});

builder.Services.AddDbContext<MSSQLContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQLToDoDatabaseConnection")));

// Register your Unit of Work pattern.
builder.Services.AddScoped<IUnitOfWork, UnitofWork>();

builder.Services.AddOpenApi();

WebApplication app = builder.Build();
if (app.Environment.IsDevelopment())
{
    // Enable Swagger
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "/scalar/{documentName}.json";
    });

    // Map Scalar API Reference
    app.MapScalarApiReference(options =>
    {
        options.WithOpenApiRoutePattern("/scalar/{documentName}.json");
    });
}

app.UseCors("AllowSpecificOrigin");

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<ToDoServices>();
app.MapGrpcService<UserServices>();
app.MapGrpcService<ProductServices>();
app.MapGrpcService<HousingLocationServices>();
app.MapHealthChecks("/healthCheck");
app.UseHttpsRedirection();
app.Run();
