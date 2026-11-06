using Nop.Core;

namespace NopPlus.Mcp.Core.Domain
{
    public partial class McpApiKeyScope : BaseEntity
    {
        public int ApiKeyId { get; set; }

        /// <summary>
        /// Tool name or pattern (e.g. orders.search or orders.*)
        /// </summary>
        public string ToolName { get; set; }
    }
}
