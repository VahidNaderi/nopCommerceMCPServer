using System;
using Nop.Core;
using NopPlus.Mcp.Core.Authorization.Enums;

namespace NopPlus.Mcp.Core.Domain
{
    public partial class McpAuditRecord : BaseEntity
    {
        public string ToolName { get; set; }
        public McpAuditSeverity Severity { get; set; }
        public int? UserId { get; set; }
        public int? ApiKeyId { get; set; }
        public string? RequestPayload { get; set; }
        public string? ResponsePayload { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime TimestampUtc { get; set; }
    }
}
