using System.Threading.Tasks;
using NopPlus.Mcp.Core.Models.Auth;

namespace NopPlus.Mcp.Core.Abstractions
{
    public interface IBasicAuthenticationHandler
    {
        Task<McpPrincipal?> AuthenticateAsync(string? authorizationHeader);
    }
}
