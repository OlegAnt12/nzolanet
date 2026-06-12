using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http.Features;
using NzolaWebAPI.Configurations;
using NzolaWebAPI.Data;
using NzolaWebAPI.Helpers;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Repositories;
using NzolaWebAPI.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Permite uploads maiores do que o limite padrão do ASP.NET Core.
// Vídeos ultrapassam facilmente o default do multipart/form-data.
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 209_715_200; // 200 MB
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 209_715_200; // 200 MB
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Nzola Network API", Version = "v1" });
    options.OperationFilter<FormFileOperationFilter>();
    options.SchemaFilter<FormFileSchemaFilter>();
    options.MapType<IFormFile>(() => new OpenApiSchema { Type = "string", Format = "binary" });
});

// CORS: Política permissiva para permitir todas as origens, métodos e cabeçalhos
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

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
builder.Services.AddScoped<IPublicacaoRepository, PublicacaoRepository>();

// Registra a implementação do serviço de e-mail
builder.Services.AddScoped<IBazeService, BazeService>();
builder.Services.AddScoped<IEmailService, EmailService>();
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
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Nzola Network API v1");
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Habilita CORS globalmente com a política permissiva
app.UseCors("AllowAll");

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
