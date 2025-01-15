using Blazor_WebApp.Application.Services;
using Blazor_WebApp.Components;
using UploadThingsGrpcService.PizzaSpecialProto;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpClient<PizzaServiceClient>();
builder.Services.AddGrpcClient<PizzaSpecialService.PizzaSpecialServiceClient>(options =>
{
    options.Address = new Uri("https://localhost:7102");
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
