using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nop.Data;
using NopPlus.Mcp.Core.Domain;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NopPlus.Mcp.Core.Services
{
    public class McpApiKeyService : IMcpApiKeyService
    {
        private readonly IRepository<McpApiKey> _repo;
        private readonly IRepository<McpApiKeyScope> _scopeRepo;
        private readonly ILogger<McpApiKeyService> _logger;

        public McpApiKeyService(IRepository<McpApiKey> repo, IRepository<McpApiKeyScope> scopeRepo, ILogger<McpApiKeyService> logger)
        {
            _repo = repo;
            _scopeRepo = scopeRepo;
            _logger = logger;
        }

        public async Task<string> CreateKeyAsync(int userId, string name, TimeSpan? ttl = null)
        {
            // generate strong random key
            var plaintext = GenerateKey();
            var created = DateTime.UtcNow;
            var expires = ttl.HasValue ? created.Add(ttl.Value) : (DateTime?)null;

            var hash = HashKey(plaintext);

            var entity = new McpApiKey
            {
                Name = name,
                UserId = userId,
                KeyHash = hash,
                IsActive = true,
                CreatedOnUtc = created,
                ExpiresOnUtc = expires
            };

            _repo.Insert(entity);

            return await Task.FromResult(plaintext);
        }

        public async Task<McpApiKey> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<System.Collections.Generic.IEnumerable<McpApiKey>> GetKeysForUserAsync(int userId)
        {
            var q = _repo.Table.Where(x => x.UserId == userId).ToList();
            return await Task.FromResult(q);
        }

        public async Task<bool> RevokeKeyAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return false;
            e.IsActive = false;
            _repo.Update(e);
            return true;
        }

        public async Task<string> RotateKeyAsync(int id, TimeSpan? ttl = null)
        {
            var old = await _repo.GetByIdAsync(id);
            if (old == null) return null;
            old.IsActive = false;
            _repo.Update(old);

            return await CreateKeyAsync(old.UserId, old.Name, ttl);
        }

        public async Task<McpApiKey> ValidateKeyAsync(string plaintextKey)
        {
            // constant-time compare against stored hashes
            foreach (var e in _repo.Table.Where(x => x.IsActive))
            {
                if (e.ExpiresOnUtc.HasValue && e.ExpiresOnUtc.Value < DateTime.UtcNow) continue;
                if (VerifyHash(plaintextKey, e.KeyHash))
                {
                    // ensure scope exists; if no scopes defined, treat as allow-all
                    var scopes = _scopeRepo.Table.Where(s => s.ApiKeyId == e.Id).ToList();
                    // scopes are enforced at authorization level, here just identify key
                    // update last used
                    e.LastUsedOnUtc = DateTime.UtcNow;
                    _repo.Update(e);
                    return await Task.FromResult(e);
                }
            }

            return null;
        }

        public async Task UpdateLastUsedAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return;
            e.LastUsedOnUtc = DateTime.UtcNow;
            _repo.Update(e);
        }

        public async Task AddScopeAsync(int apiKeyId, string toolPattern)
        {
            var s = new McpApiKeyScope { ApiKeyId = apiKeyId, ToolName = toolPattern };
            _scopeRepo.Insert(s);
            await Task.CompletedTask;
        }

        public async Task RemoveScopeAsync(int scopeId)
        {
            var s = await _scopeRepo.GetByIdAsync(scopeId);
            if (s != null) _scopeRepo.Delete(s);
        }

        public async Task<IEnumerable<McpApiKeyScope>> GetScopesForKeyAsync(int apiKeyId)
        {
            var q = _scopeRepo.Table.Where(x => x.ApiKeyId == apiKeyId).ToList();
            return await Task.FromResult(q);
        }

        private static string GenerateKey()
        {
            var bytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        // PBKDF2 using HMACSHA256
        private static string HashKey(string plaintext)
        {
            var salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(salt);
            using var derive = new Rfc2898DeriveBytes(plaintext, salt, 100_000, HashAlgorithmName.SHA256);
            var subkey = derive.GetBytes(32);

            // store as: {iterations}.{salt-base64}.{subkey-base64}
            return $"{100000}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(subkey)}";
        }

        private static bool VerifyHash(string plaintext, string stored)
        {
            try
            {
                var parts = stored.Split('.', 3);
                if (parts.Length != 3) return false;
                var iterations = int.Parse(parts[0]);
                var salt = Convert.FromBase64String(parts[1]);
                var expected = Convert.FromBase64String(parts[2]);

                using var derive = new Rfc2898DeriveBytes(plaintext, salt, iterations, HashAlgorithmName.SHA256);
                var actual = derive.GetBytes(expected.Length);

                return CryptographicOperations.FixedTimeEquals(actual, expected);
            }
            catch (Exception ex)
            {
                // parsing failed
                return false;
            }
        }
    }
}
