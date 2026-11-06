namespace NopPlus.Mcp.Core.Models.Dto
{
    public class McpError
    {
        public string Code { get; set; } = "error";

        public string Message { get; set; } = string.Empty;

        public string? Details { get; set; }
    }
}
