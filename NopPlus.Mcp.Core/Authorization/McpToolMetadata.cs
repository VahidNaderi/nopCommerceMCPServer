using NopPlus.Mcp.Core.Authorization.Enums;

namespace NopPlus.Mcp.Core.Authorization
{
    public class McpToolMetadata
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? InputSchema { get; set; }
        public IEnumerable<string> RequiredPermissions { get; set; } = new List<string>();
        public IEnumerable<string> RequiredMcpPermissions { get; set; } = new List<string>();
        public McpOperationType OperationType { get; set; } = McpOperationType.Read;
        public IEnumerable<McpExecutionPolicy> ExecutionPolicy { get; set; } = new List<McpExecutionPolicy> { McpExecutionPolicy.Immediate };
        public McpAuditSeverity AuditSeverity { get; set; } = McpAuditSeverity.Informational;
    }
}
