using System.Threading.Tasks;
using NopPlus.Mcp.Core.Models;

namespace NopPlus.Mcp.Core.Abstractions
{
    public interface IAuthenticationProvider
    {
        Task<Models.Auth.McpPrincipal?> AuthenticateAsync(object? request);
    }
}
