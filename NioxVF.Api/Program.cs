using Microsoft.EntityFrameworkCore;
using NioxVF.Api.Middleware;
using NioxVF.Api.Services;
using NioxVF.Domain.Interfaces;
using NioxVF.Domain.Services;
using NioxVF.Persistence.Sqlite.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "NioxVF API", 
        Version = "v1",
        Description = "API para el conector Veri*Factu NioxVF"
    });
    c.AddSecurityDefinition("ApiKey", new()
    {
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name = "X-API-Key",
        Description = "API Key para autenticación"
    });
    c.AddSecurityRequirement(new()
    {
        [new() { Reference = new() { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "ApiKey" } }] = new string[] { }
    });
});

// Register domain services
builder.Services.AddSingleton<IHashChain, HashChainService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();

// Add Entity Framework Core with SQLite
builder.Services.AddDbContext<SqliteDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add logging
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "NioxVF API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

// Add API Key middleware
app.UseMiddleware<ApiKeyMiddleware>();

app.UseRouting();
app.MapControllers();

app.Run();
