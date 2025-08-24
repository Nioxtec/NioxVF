namespace NioxVF.Api.Middleware;

/// <summary>
/// Middleware para autenticación por API Key
/// </summary>
public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApiKeyMiddleware> _logger;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<ApiKeyMiddleware> logger)
    {
        _next = next;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Permitir endpoints de salud y swagger sin autenticación
        if (context.Request.Path.StartsWithSegments("/health") ||
            context.Request.Path.StartsWithSegments("/swagger") ||
            context.Request.Path.StartsWithSegments("/metrics"))
        {
            await _next(context);
            return;
        }

        // Verificar API Key para rutas de API
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            if (!context.Request.Headers.TryGetValue("X-API-Key", out var apiKey))
            {
                _logger.LogWarning("Request without API Key from {RemoteIp}", context.Connection.RemoteIpAddress);
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key required");
                return;
            }

            var validApiKeys = _configuration.GetSection("ApiKeys").Get<string[]>() ?? Array.Empty<string>();
            
            if (!validApiKeys.Contains(apiKey.ToString()))
            {
                _logger.LogWarning("Invalid API Key attempted from {RemoteIp}", context.Connection.RemoteIpAddress);
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid API Key");
                return;
            }

            _logger.LogDebug("Valid API Key authenticated for request to {Path}", context.Request.Path);
        }

        await _next(context);
    }
}
