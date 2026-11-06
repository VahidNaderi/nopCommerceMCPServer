namespace NopPlus.Mcp.Core.Models.Dto
{
    public class McpResponseDto
    {
        public bool Success { get; set; }

        public object? Payload { get; set; }

        public McpError? Error { get; set; }
    }
}
