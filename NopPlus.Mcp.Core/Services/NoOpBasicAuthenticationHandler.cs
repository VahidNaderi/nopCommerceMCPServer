using System.Threading.Tasks;
using NopPlus.Mcp.Core.Abstractions;
using NopPlus.Mcp.Core.Models.Auth;

namespace NopPlus.Mcp.Core.Services
{
    public class NoOpBasicAuthenticationHandler : IBasicAuthenticationHandler
    {
        public Task<McpPrincipal?> AuthenticateAsync(string? authorizationHeader)
        {
            // Stub: no authentication performed. Consumers should replace with real implementation.
            return Task.FromResult<McpPrincipal?>(null);
        }
    }
}
