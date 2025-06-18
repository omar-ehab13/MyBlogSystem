using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;
using System.Security.Claims;

namespace BlogSystem.Infrastructure.Logging.Enrichers;

public class UserIdEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserIdEnricher() : this(new HttpContextAccessor())
    {
    }

    public UserIdEnricher(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserId", userId));
            }

            var userName = httpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            if (!string.IsNullOrEmpty(userName))
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserName", userName));
            }
        }
    }
}
