using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using MyApi;
using MyApi.Data;
using NJsonSchema.Generation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "v1";
    config.Title = "MyApi";
    config.Version = "v1";
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DbConnection"));

#if DEBUG
    options.EnableSensitiveDataLogging();
#endif
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi(p => p.Path = "/openapi/{documentName}.yaml");
    app.UseSwaggerUi(p => p.DocumentPath = "/openapi/{documentName}.yaml");
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("WeatherForecast_GetWeatherForecast")
.WithTags("WeatherForecast")
.WithOpenApi();

app.MapPost("/image/{id}", async (string id, IFormFile form) =>
{
    using (var stream = form.OpenReadStream())
    {
        var memory = new MemoryStream();
        await stream.CopyToAsync(memory);
        stream.Seek(0, SeekOrigin.Begin);
    }

    Console.WriteLine(id);

    // Persist file
})
.WithName("Image_UploadImage")
.WithTags("Image")
.WithOpenApi();

app.MapPersonsEndpoints();

try
{
    using (var scope = app.Services.CreateScope())
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        //await context.Database.MigrateAsync();

        //await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        if (context.Database.HasPendingModelChanges())
        {
            logger.LogWarning("The entity model has changed since the last migration.");
        }

        if (args.Contains("--seed"))
        {
            await SeedData(context, configuration, logger);
            return;
        }
    }
}
catch (Exception e)
{
    Console.WriteLine(e);
}

await app.RunAsync();

static async Task SeedData(ApplicationDbContext context, IConfiguration configuration, ILogger<Program> logger)
{
    try
    {
        //await Seed.SeedData(context, configuration);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred seeding the " +
            "database. Error: {Message}", ex.Message);
    }
}

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
