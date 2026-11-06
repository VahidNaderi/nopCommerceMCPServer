namespace NopPlus.Mcp.Core.Models.Dto
{
    public class McpRequestDto
    {
        public string Tool { get; set; } = string.Empty;

        public object? Payload { get; set; }
    }
}
