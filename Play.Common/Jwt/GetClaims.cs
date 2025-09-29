using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Play.Common.Jwt
{
    public class GetClaims
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetClaims(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid GetIdentityId()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userId is null ? Guid.Empty : Guid.Parse(userId);
        }
    }
}
