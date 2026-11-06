using System.Threading.Tasks;
using System.Threading.Tasks;
using NopPlus.Mcp.Core.Domain;

namespace NopPlus.Mcp.Core.Abstractions
{
    public interface IAuditLogger
    {
        Task LogAsync(McpAuditRecord record);
    }
}
