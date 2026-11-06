using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NopPlus.Mcp.Core.Abstractions;
using NopPlus.Mcp.Core.Models.Auth;

namespace NopPlus.Mcp.Core.Services
{
    public class BasicAuthenticationProvider : IAuthenticationProvider
    {
        private readonly IBasicAuthenticationHandler _handler;
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public BasicAuthenticationProvider(IBasicAuthenticationHandler handler, IHttpContextAccessor? httpContextAccessor = null)
        {
            _handler = handler;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<McpPrincipal?> AuthenticateAsync(object? request)
        {
            // Attempt to fetch Authorization header from HttpContext if available
            string? header = null;
            if (_httpContextAccessor?.HttpContext != null && _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var values))
            {
                header = values.ToString();
            }

            return await _handler.AuthenticateAsync(header);
        }
    }
}
