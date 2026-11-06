using System.Threading.Tasks;
using NopPlus.Mcp.Core.Models;

namespace NopPlus.Mcp.Core.Abstractions
{
    public interface IMcpMiddleware
    {
        Task InvokeAsync(McpContext context, McpMiddlewareDelegate next);
    }

    public delegate System.Threading.Tasks.Task McpMiddlewareDelegate(McpContext context);
}
