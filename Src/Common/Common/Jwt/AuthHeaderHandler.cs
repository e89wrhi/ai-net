using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace AI.Common.Jwt;

/// <summary>
/// A helper for HttpClient that automatically passes the current user's JWT token 
/// to any outgoing HTTP requests. This is essential for inter-service communication.
/// </summary>
public class AuthHeaderHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContext;

    public AuthHeaderHandler(IHttpContextAccessor httpContext)
    {
        _httpContext = httpContext;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = (_httpContext?.HttpContext?.Request.Headers["Authorization"])?.ToString();

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token?.Replace("Bearer ", "", StringComparison.CurrentCulture));

        return base.SendAsync(request, cancellationToken);
    }
}