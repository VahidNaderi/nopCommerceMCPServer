using System;
using System.Text.Json;
using System.Threading.Tasks;
using NopPlus.Mcp.Core.Abstractions;
using NopPlus.Mcp.Core.Domain;

namespace NopPlus.Mcp.Core.Services
{
    public class SimpleAuditLogger : IAuditLogger
    {
        public Task LogAsync(McpAuditRecord record)
        {
            // Minimal implementation - persist to console. Replace with repository storage if needed.
            var json = JsonSerializer.Serialize(record);
            Console.WriteLine($"[MCP-AUDIT] {record.TimestampUtc:o} {json}");
            return Task.CompletedTask;
        }
    }
}
