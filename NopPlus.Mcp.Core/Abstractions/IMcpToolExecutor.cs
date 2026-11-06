using System.Threading.Tasks;
using NopPlus.Mcp.Core.Models;

namespace NopPlus.Mcp.Core.Abstractions
{
    public interface IMcpToolExecutor
    {
        Task ExecuteAsync(string toolName, McpContext context);
    }
}
