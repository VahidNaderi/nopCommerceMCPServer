using System.Collections.Generic;
using System.Threading.Tasks;
using NopPlus.Mcp.Core.Domain;

namespace NopPlus.Mcp.Core.Services
{
    public interface IMcpApiKeyService
    {
        /// <summary>
        /// Create a new API key for user. Returns the plaintext key only once.
        /// </summary>
        Task<string> CreateKeyAsync(int userId, string name, System.TimeSpan? ttl = null);

        Task<IEnumerable<McpApiKey>> GetKeysForUserAsync(int userId);

        Task<McpApiKey> GetByIdAsync(int id);

        Task<bool> RevokeKeyAsync(int id);

        /// <summary>
        /// Rotate key: disable old and create new one with same metadata. Returns plaintext for new key.
        /// </summary>
        Task<string> RotateKeyAsync(int id, System.TimeSpan? ttl = null);

        /// <summary>
        /// Validate an incoming plaintext key. Returns the matched McpApiKey if valid and active, otherwise null.
        /// </summary>
        Task<McpApiKey> ValidateKeyAsync(string plaintextKey);

        Task UpdateLastUsedAsync(int id);

        Task AddScopeAsync(int apiKeyId, string toolPattern);

        Task RemoveScopeAsync(int scopeId);

        Task<IEnumerable<McpApiKeyScope>> GetScopesForKeyAsync(int apiKeyId);
    }
}
