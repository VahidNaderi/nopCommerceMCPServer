using NopPlus.Mcp.Core.Abstractions;
using NopPlus.Mcp.Core.Authorization.Enums;
using NopPlus.Mcp.Core.Models;

namespace NopPlus.Mcp.Core.Tools
{
    public class DummyEchoTool : IMcpTool
    {
        public string Name => "dummy.echo";

        // Implemented IMcpTool members with sensible defaults
        public string Description => "Simple echo tool";
        public string? InputSchema => null;
        public string? Category => null;
        public IEnumerable<string> RequiredPermissions => Array.Empty<string>();
        public IEnumerable<string> RequiredMcpPermissions => Array.Empty<string>();
        public McpOperationType OperationType => default(McpOperationType);
        public IEnumerable<McpExecutionPolicy> ExecutionPolicy => Array.Empty<McpExecutionPolicy>();
        public McpAuditSeverity AuditSeverity => default(McpAuditSeverity);

        public Task ExecuteAsync(McpContext context)
        {
            // Simple echo: set response equal to request
            context.Response = new { Echo = context.Request };
            return Task.CompletedTask;
        }
    }
}
