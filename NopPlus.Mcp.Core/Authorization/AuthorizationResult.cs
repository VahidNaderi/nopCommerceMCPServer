namespace NopPlus.Mcp.Core.Authorization
{
    public class AuthorizationResult
    {
        public bool IsAuthorized { get; set; }

        /// <summary>
        /// Optional human readable failure reason
        /// </summary>
        public string? FailureReason { get; set; }

        /// <summary>
        /// Optional machine friendly failure code
        /// </summary>
        public string? FailureCode { get; set; }
    }
}
