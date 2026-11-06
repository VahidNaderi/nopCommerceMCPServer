using System.Collections.Generic;
using System.Threading.Tasks;
using NopPlus.Mcp.Core.Authorization.Enums;

namespace NopPlus.Mcp.Core.Abstractions
{
    public interface IMcpTool
    {
        /// <summary>
        /// Unique name of the tool. Used by registry and routing.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Human friendly description
        /// </summary>
        string Description { get; }

        /// <summary>
        /// JSON schema or other machine-readable description of the input expected by the tool
        /// </summary>
        string? InputSchema { get; }

        /// <summary>
        /// Optional category for grouping tools in UIs
        /// </summary>
        string? Category { get; }

        /// <summary>
        /// Standard nopCommerce permission system permission names required to run this tool
        /// </summary>
        IEnumerable<string> RequiredPermissions { get; }

        /// <summary>
        /// MCP-specific permission identifiers required to run this tool
        /// </summary>
        IEnumerable<string> RequiredMcpPermissions { get; }

        /// <summary>
        /// Operation classification for this tool
        /// </summary>
        McpOperationType OperationType { get; }

        /// <summary>
        /// Execution policies that apply to this tool
        /// </summary>
        IEnumerable<McpExecutionPolicy> ExecutionPolicy { get; }

        /// <summary>
        /// Default audit severity classification for operations by this tool
        /// </summary>
        McpAuditSeverity AuditSeverity { get; }

        /// <summary>
        /// Execute the tool logic within the provided context.
        /// Implementations should not handle cross-cutting concerns (auth, audit) - pipeline does that.
        /// </summary>
        Task ExecuteAsync(Models.McpContext context);
    }
}
