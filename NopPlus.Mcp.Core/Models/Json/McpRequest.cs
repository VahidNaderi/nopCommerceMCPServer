namespace NopPlus.Mcp.Core.Models.Json
{
    public class McpRequest
    {
        public string Tool { get; set; } = string.Empty;

        public object? Payload { get; set; }
    }
}
