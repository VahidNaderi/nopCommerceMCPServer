using System.Threading.Tasks;
using NopPlus.Mcp.Core.Models;
using NopPlus.Mcp.Core.Abstractions;

namespace NopPlus.Mcp.Core.Authorization
{
    public interface IMcpAuthorizationService
    {
        /// <summary>
        /// Authorize execution of the given tool for the provided context.
        /// Performs: authenticated user validation, nopCommerce permissions, MCP permissions and API key scope checks.
        /// </summary>
        Task<AuthorizationResult> AuthorizeAsync(IMcpTool tool, McpContext context);

        /// <summary>
        /// Convenience boolean wrapper
        /// </summary>
        Task<bool> CanExecuteAsync(IMcpTool tool, McpContext context);
    }
}
