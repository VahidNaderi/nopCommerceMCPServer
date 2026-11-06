using NopPlus.Mcp.Core.Authorization.Enums;
using NopPlus.Mcp.Core.Models;

namespace NopPlus.Mcp.Core.Abstractions
{
    public abstract class McpToolBase : IMcpTool
    {
        public abstract string Name { get; }

        public virtual string? Category => null;

        public virtual string Description => string.Empty;

        public virtual string? InputSchema => null;

        public virtual IEnumerable<string> RequiredPermissions => Array.Empty<string>();

        public virtual IEnumerable<string> RequiredMcpPermissions => Array.Empty<string>();

        public virtual McpOperationType OperationType => McpOperationType.Read;

        public virtual IEnumerable<McpExecutionPolicy> ExecutionPolicy => new[] { McpExecutionPolicy.Immediate };

        public virtual McpAuditSeverity AuditSeverity => McpAuditSeverity.Informational;

        public abstract Task ExecuteAsync(McpContext context);
    }
}
