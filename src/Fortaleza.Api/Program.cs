using Fortaleza.Api.Games;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapGameEndpoints();
app.MapFallbackToFile("index.html");

app.Run();

public partial class Program;
