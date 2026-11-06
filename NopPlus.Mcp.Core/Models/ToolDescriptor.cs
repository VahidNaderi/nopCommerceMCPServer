using System.Collections.Generic;
using NopPlus.Mcp.Core.Authorization.Enums;

namespace NopPlus.Mcp.Core.Models
{
    public class ToolDescriptor
    {
        public string Name { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? Description { get; set; }
        public IEnumerable<string> RequiredPermissions { get; set; } = new List<string>();
        public IEnumerable<string> RequiredMcpPermissions { get; set; } = new List<string>();
        public McpOperationType OperationType { get; set; } = McpOperationType.Read;
        public IEnumerable<McpExecutionPolicy> ExecutionPolicy { get; set; } = new List<McpExecutionPolicy> { McpExecutionPolicy.Immediate };
        public McpAuditSeverity AuditSeverity { get; set; } = McpAuditSeverity.Informational;
    }
}
