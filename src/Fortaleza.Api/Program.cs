using Amazon.Lambda.AspNetCoreServer.Hosting;
using Fortaleza.Api;
using Fortaleza.Api.Games;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

var app = builder.Build();

app.UseOriginVerification(app.Configuration);

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapGameEndpoints();
app.MapFallbackToFile("index.html");

app.Run();

public partial class Program;
