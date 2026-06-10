using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Configurations;
using NzolaWebAPI.Data;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Repositories;
using NzolaWebAPI.Repositories;
using NzolaWebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// dotnet add package Microsoft.EntityFrameworkCore.InMemory
// using Microsoft.EntityFrameworkCore;
builder.Services.AddDbContext<ContextoBDNzola>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//Registra as configurações de e-mail a partir do appsettings.json
builder.Services.Configure<NzolaWebAPI.Configurations.EmailSettings>(
    builder.Configuration.GetSection("EmailSettings")
);

builder.Services.AddScoped<IBazeRepository, BazeRepository>();
builder.Services.AddScoped<IComentarioRepository, ComentarioRepository>();
builder.Services.AddScoped<IConteudoPublicacaoRepository, ConteudoPublicacaoRepository>();
builder.Services.AddScoped<IPublicacaoRepository, PublicacaoRepository>();

// Registra a implementação do serviço de e-mail
builder.Services.AddScoped<IBazeService, BazeService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IConteudoPublicacaoService, ConteudoPublicacaoService>();
builder.Services.AddScoped<IComentarioService, ComentarioService>();
builder.Services.AddScoped<IPublicacaoService, PublicacaoService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUtilizadorRepository, UtilizadorRepository>();
builder.Services.AddScoped<IUtilizadorService, UtilizadorService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing",
    "Bracing",
    "Chilly",
    "Cool",
    "Mild",
    "Warm",
    "Balmy",
    "Hot",
    "Sweltering",
    "Scorching",
};

app.MapGet(
        "/weatherforecast",
        () =>
        {
            var forecast = Enumerable
                .Range(1, 5)
                .Select(index => new WeatherForecast(
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            return forecast;
        }
    )
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
