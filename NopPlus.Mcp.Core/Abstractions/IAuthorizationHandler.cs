using System.Threading.Tasks;
using NopPlus.Mcp.Core.Models;
using NopPlus.Mcp.Core.Models.Auth;

namespace NopPlus.Mcp.Core.Abstractions
{
    public interface IAuthorizationHandler
    {
        Task<bool> AuthorizeAsync(McpPrincipal? principal, string toolName, object? request);
    }
}
