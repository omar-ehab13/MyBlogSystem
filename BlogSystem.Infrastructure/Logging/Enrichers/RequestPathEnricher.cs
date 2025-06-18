using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;

namespace BlogSystem.Infrastructure.Logging.Enrichers;

public class RequestPathEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RequestPathEnricher() : this(new HttpContextAccessor())
    {
    }

    public RequestPathEnricher(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("RequestPath", httpContext.Request.Path));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("RequestMethod", httpContext.Request.Method));

            var userAgent = httpContext.Request.Headers["User-Agent"].FirstOrDefault();
            if (!string.IsNullOrEmpty(userAgent))
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserAgent", userAgent));
            }

            var clientIP = httpContext.Connection.RemoteIpAddress?.ToString();
            if (!string.IsNullOrEmpty(clientIP))
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ClientIP", clientIP));
            }
        }
    }
}
