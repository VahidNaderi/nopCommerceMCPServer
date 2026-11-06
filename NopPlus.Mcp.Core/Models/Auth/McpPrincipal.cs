namespace NopPlus.Mcp.Core.Models.Auth
{
    public class McpPrincipal
    {
        public string Name { get; set; } = string.Empty;
        public int? UserId { get; set; }

        public IEnumerable<string> Roles { get; set; } = Array.Empty<string>();
    }
}
