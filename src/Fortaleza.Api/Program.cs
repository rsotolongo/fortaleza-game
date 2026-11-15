using Fortaleza.Api.Games;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();

WebApplication app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapGameEndpoints();
app.MapFallbackToFile("index.html");
await app.RunAsync();
