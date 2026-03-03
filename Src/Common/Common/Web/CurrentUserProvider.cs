using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AI.Common.Web;

/// <summary>
/// A helper to find out who the current user is.
/// It looks into the HTTP Context to extract the User ID from the JWT claims.
/// </summary>
public interface ICurrentUserProvider
{
    Guid? GetCurrentUserId();
}

public class CurrentUserProvider : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }


    public Guid? GetCurrentUserId()
    {
        var nameIdentifier = _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        Guid.TryParse(nameIdentifier, out var userId);

        return userId;
    }
}