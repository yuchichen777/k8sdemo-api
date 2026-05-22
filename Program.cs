using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/api/ping", () =>
{
    return Results.Ok(new
    {
        Message = "Hello K8s V1",
        Time = DateTime.Now
    });
});

app.MapGet("/api/config", (IConfiguration config) =>
{
    return Results.Ok(new
    {
        AppName = config["AppSettings:AppName"],
        Env = config["AppSettings:Environment"]
    });
});

app.MapGet("/api/secret", (IConfiguration config) =>
{
    return Results.Ok(new
    {
        DbPassword = config["AppSettings:DbPassword"]
    });
});

app.MapGet("/api/dbtest", async () =>
{
    var connStr =
        "Server=mysql-service;" +
        "Port=3306;" +
        "Database=k8sdemo;" +
        "User=root;" +
        "Password=root123;";

    using var conn = new MySqlConnection(connStr);

    await conn.OpenAsync();

    return Results.Ok(new
    {
        Message = "DB Connected",
        ServerVersion = conn.ServerVersion
    });
});

app.MapGet("/api/health", () =>
{
    return Results.Ok(new
    {
        Status = "Healthy V9",
        Time = DateTime.Now
    });
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
