using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NzolaWebAPI.Configurations;
using NzolaWebAPI.Data;
using NzolaWebAPI.Helpers;
using NzolaWebAPI.Hubs;
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
builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Para MVC, a propriedade se chama JsonSerializerOptions (não SerializerOptions)
        // E NÃO precisa do conversor customizado!
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Nzola Network API",
        Version = "v1",
        Description = "API da rede social NzolaNet. Fornece endpoints para autenticação, gestão de utilizadores, publicações, comentários, bazes (likes), seguidores, denúncias, notificações, pesquisa e painel administrativo.",
        Contact = new OpenApiContact
        {
            Name = "Equipa NzolaNet",
            Email = "suporte@nzolanet.ao"
        },
        License = new OpenApiLicense
        {
            Name = "Uso interno"
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    options.OperationFilter<FormFileOperationFilter>();
    options.SchemaFilter<FormFileSchemaFilter>();
    options.MapType<IFormFile>(() => new OpenApiSchema { Type = "string", Format = "binary" });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        policy =>
        {
            policy.SetIsOriginAllowed(_ => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
        }
    );
});

// SignalR
builder.Services.AddSignalR();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"]!)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            MapInboundClaims = true,
            RoleClaimType = ClaimTypes.Role,
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                // Log do header Authorization para debug
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrEmpty(authHeader))
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogInformation("AuthHeader recebido: {AuthHeader}", authHeader[..Math.Min(authHeader.Length, 80)]);
                }

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/notifications"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogWarning("JWT Challenge (401): {Error}, {ErrorDescription}", context.Error, context.ErrorDescription);
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError(context.Exception, "JWT Authentication failed for: {Path}", context.HttpContext.Request.Path);
                return Task.CompletedTask;
            },
            OnForbidden = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogWarning("JWT Forbidden (403) for: {Path}", context.HttpContext.Request.Path);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                var claims = string.Join(", ", context.Principal!.Claims.Select(c => $"{c.Type}:{c.Value}"));
                logger.LogInformation("JWT Token validado. Claims: {Claims}", claims);
                return Task.CompletedTask;
            }
        };
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
builder.Services.AddScoped<ISeguidorRepository, SeguidorRepository>();
builder.Services.AddScoped<IUtilizadorRepository, UtilizadorRepository>();
builder.Services.AddScoped<IDenunciaRepository, DenunciaRepository>();
builder.Services.AddScoped<IPedidoSeguirRepository, PedidoSeguirRepository>();

// Registra a implementação do serviço de e-mail
builder.Services.AddScoped<IBazeService, BazeService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IComentarioService, ComentarioService>();
builder.Services.AddScoped<IPublicacaoService, PublicacaoService>();
builder.Services.AddScoped<ISeguidorService, SeguidorService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUtilizadorService, UtilizadorService>();
builder.Services.AddScoped<IDenunciaService, DenunciaService>();
builder.Services.AddScoped<IPedidoSeguirService, PedidoSeguirService>();
builder.Services.AddScoped<IAdminService, AdminService>();

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

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

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

// 1. Ativa o mapeamento padrão da pasta 'wwwroot'
// Com isto, qualquer ficheiro em 'wwwroot/uploads/foto.png' fica acessível via http://localhost:5043/uploads/foto.png
//app.UseStaticFiles();

// 2. Mapeia explicitamente a pasta de uploads para servir ficheiros via URL
var pastaUploads = Path.Combine(app.Environment.WebRootPath, "uploads");
if (!Directory.Exists(pastaUploads))
{
    Directory.CreateDirectory(pastaUploads);
}
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(pastaUploads),
    RequestPath = "/uploads"
});

using (var scope = app.Services.CreateScope())
{
    var contexto = scope.ServiceProvider.GetRequiredService<ContextoBDNzola>();
    await DbInitializer.SeedAsync(contexto);
}

app.MapControllers();

app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
