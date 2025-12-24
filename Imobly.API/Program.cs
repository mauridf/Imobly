using System.Text;
using Imobly.API.Middleware;
using Imobly.Application;
using Imobly.Application.Mappings;
using Imobly.Infrastructure;
using Imobly.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configurar banco de dados
builder.Services.AddInfrastructure(builder.Configuration);

// Configurar serviços da Application
builder.Services.AddApplication();

// Configurar AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

// Configurar autenticação JWT
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("A chave JWT (Jwt:Key) não está configurada.");
}
var key = Encoding.ASCII.GetBytes(jwtKey);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Configurar autorização
builder.Services.AddAuthorization();

// Configurar Swagger com autenticação PARA VERSÃO 10.1.0
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Imobly API",
        Version = "v1",
        Description = "API para gestão de locação de imóveis",
        Contact = new OpenApiContact
        {
            Name = "Imobly Team",
            Email = "mauridf@gmail.com"
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Informe o token JWT no formato: Bearer {token}"
    });

    c.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer"),
            new List<string>()
        }
    });
});

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Adicionar health checks
builder.Services.AddHealthChecks()
    .AddNpgSql(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
        name: "PostgreSQL",
        tags: new[] { "database", "ready" })
    .AddUrlGroup(
        new Uri("http://localhost:5001/api/healthcheck"),
        name: "API",
        tags: new[] { "service", "ready" });

// Configurar endpoint de health checks
builder.Services.Configure<HealthCheckPublisherOptions>(options =>
{
    options.Delay = TimeSpan.FromSeconds(5);
    options.Period = TimeSpan.FromSeconds(30);
});

var app = builder.Build();

// ========== CONFIGURAR MIDDLEWARES (NA ORDEM CORRETA) ==========

// 1. Exception Handling (primeiro para capturar todos os erros)
app.UseGlobalExceptionHandler();

// 2. Request Logging
app.UseRequestLogging();

// 3. Rate Limiting
app.UseRateLimiting();

// 4. API Key Validation (opcional - para integrações externas)
// app.UseApiKeyValidation();

// 5. HSTS (HTTP Strict Transport Security) - apenas em produção
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

// 6. HTTPS Redirection
app.UseHttpsRedirection();

// 7. CORS
app.UseCors("AllowAll");

// 8. Response Compression (comentado temporariamente - pode causar conflito)
// app.UseResponseCompressionCustom();

// 9. HTTP Cache
app.UseHttpCache();

// 10. Static Files (se houver)
app.UseStaticFiles();

// 11. Routing
app.UseRouting();

// 12. Authentication
app.UseAuthentication();

// 13. Authorization
app.UseAuthorization();

// 14. Endpoints
app.MapControllers();

// 15. Health Check endpoint
app.MapHealthChecks("/health");

// 16. Swagger UI (apenas desenvolvimento)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Imobly API v1");
        c.RoutePrefix = string.Empty; // Para acessar na raiz
    });
}

// ========== FIM DOS MIDDLEWARES ==========

app.Run();