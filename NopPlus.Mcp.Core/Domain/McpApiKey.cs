using System;
using Nop.Core;

namespace NopPlus.Mcp.Core.Domain
{
    public partial class McpApiKey : BaseEntity
    {
        public string Name { get; set; }

        public int UserId { get; set; }

        /// <summary>
        /// Stored as salt:hash (base64) using PBKDF2
        /// </summary>
        public string KeyHash { get; set; }

        public bool IsActive { get; set; }

        public DateTime? ExpiresOnUtc { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public DateTime? LastUsedOnUtc { get; set; }
    }
}
