using ItineraryParser.API.Middleware;
using ItineraryParser.Core.Interfaces;
using ItineraryParser.Core.Settings;
using ItineraryParser.Infrastructure.Interfaces;
using ItineraryParser.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Serilog;

// --------------------
// Configure Serilog
// --------------------
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        "logs/log.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Use Serilog for logging
builder.Host.UseSerilog();

// --------------------
// Configuration
// --------------------
builder.Services.Configure<OpenAISettings>(
    builder.Configuration.GetSection("OpenAI"));

// --------------------
// CORS Configuration 
// --------------------
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://itineraryparser.web.app")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// --------------------
// Dependency Injection
// --------------------
builder.Services.AddSingleton<IPromptLoader, PromptLoader>();

builder.Services.AddSingleton<ILLMService>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<OpenAISettings>>().Value;
    var logger = sp.GetRequiredService<ILogger<OpenAIService>>();

    if (string.IsNullOrWhiteSpace(settings.ApiKey))
        throw new Exception("OpenAI API key is missing");

    return new OpenAIService(settings.ApiKey, logger);
});

builder.Services.AddScoped<ParserService>();

// --------------------
// MVC / Controllers
// --------------------
builder.Services.AddControllers();

// --------------------
// Hosting configuration
// --------------------
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://*:{port}");

var app = builder.Build();

// --------------------
// Middleware pipeline
// --------------------
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(); 

app.UseSerilogRequestLogging();

app.MapControllers();

app.Run();