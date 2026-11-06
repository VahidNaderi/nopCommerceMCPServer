using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nop.Services.Security;
using NopPlus.Mcp.Core.Abstractions;
using NopPlus.Mcp.Core.Models;

namespace NopPlus.Mcp.Core.Authorization
{
    public class McpAuthorizationService : IMcpAuthorizationService
    {
        private readonly IPermissionService _permissionService;
        private readonly Services.IMcpApiKeyService _apiKeyService;
        private readonly ILogger<McpAuthorizationService> _logger;

        public McpAuthorizationService(IPermissionService permissionService, ILogger<McpAuthorizationService> logger, Services.IMcpApiKeyService apiKeyService)
        {
            _permissionService = permissionService;
            _apiKeyService = apiKeyService;
            _logger = logger;
        }

        public async Task<AuthorizationResult> AuthorizeAsync(IMcpTool tool, McpContext context)
        {
            // API key authentication: if an API key is provided, validate it and authorize based on scopes only.
            if (context.Items.TryGetValue("ApiKey", out var keyObj) && keyObj is string apiKey)
            {
                // validate against persisted keys
                var keyEntity = await _apiKeyService.ValidateKeyAsync(apiKey);
                if (keyEntity == null)
                {
                    return new AuthorizationResult { IsAuthorized = false, FailureReason = "Invalid API key", FailureCode = "ApiKeyInvalid" };
                }

                // attach api key context for downstream (audit, logging)
                context.Items["ApiKeyId"] = keyEntity.Id;
                context.Items["ApiKeyUserId"] = keyEntity.UserId;

                // scope check: load scopes from service and validate tool name against patterns
                var scopes = (await _apiKeyService.GetScopesForKeyAsync(keyEntity.Id)).ToList();

                // Security decision: require explicit scopes. If no scopes are configured, deny by default.
                if (!scopes.Any())
                {
                    _logger.LogWarning("MCP: API key {ApiKeyId} attempted to call {Tool} but has no scopes configured", keyEntity.Id, tool.Name);
                    return new AuthorizationResult { IsAuthorized = false, FailureReason = "API key has no scopes", FailureCode = "ApiKeyNoScopes" };
                }

                var allowedByScope = scopes.Any(s => IsMatchPattern(s.ToolName, tool.Name));
                // also allow explicit wildcard '*' scope
                var hasGlobal = scopes.Any(s => s.ToolName == "*");

                if (!allowedByScope && !hasGlobal)
                {
                    _logger.LogWarning("MCP: API key {ApiKeyId} not allowed for tool {Tool}", keyEntity.Id, tool.Name);
                    return new AuthorizationResult { IsAuthorized = false, FailureReason = "API key not allowed for this tool", FailureCode = "ApiKeyScopeDenied" };
                }

                // API keys do not inherit user permissions; authorization is based on scopes only.
                return new AuthorizationResult { IsAuthorized = true };
            }

            // Fallback: require a principal (interactive user)
            if (context.Principal == null)
            {
                return new AuthorizationResult { IsAuthorized = false, FailureReason = "Unauthenticated", FailureCode = "Unauthenticated" };
            }

            // nopCommerce permission checks for interactive principals
            var perms = tool.RequiredPermissions?.ToList() ?? new System.Collections.Generic.List<string>();
            foreach (var p in perms)
            {
                var ok = await _permissionService.AuthorizeAsync(p);
                if (!ok)
                {
                    _logger.LogWarning("MCP: Permission check failed for {Permission} on tool {Tool} for principal {Principal}", p, tool.Name, context.Principal.Name);
                    return new AuthorizationResult { IsAuthorized = false, FailureReason = $"Missing permission {p}", FailureCode = "PermissionDenied" };
                }
            }

            // MCP-specific permissions are just string tokens; map to permission service as well when available
            var mcpPerms = tool.RequiredMcpPermissions?.ToList() ?? new System.Collections.Generic.List<string>();
            foreach (var mp in mcpPerms)
            {
                var ok = await _permissionService.AuthorizeAsync(mp);
                if (!ok)
                {
                    _logger.LogWarning("MCP: MCP permission check failed for {Permission} on tool {Tool} for principal {Principal}", mp, tool.Name, context.Principal.Name);
                    return new AuthorizationResult { IsAuthorized = false, FailureReason = $"Missing MCP permission {mp}", FailureCode = "McpPermissionDenied" };
                }
            }

            return new AuthorizationResult { IsAuthorized = true };
        }

        public async Task<bool> CanExecuteAsync(IMcpTool tool, McpContext context)
        {
            var res = await AuthorizeAsync(tool, context);
            return res.IsAuthorized;
        }

        private static bool IsMatchPattern(string pattern, string toolName)
        {
            // support exact match and wildcard suffix like orders.*
            if (string.Equals(pattern, "*")) return true;
            if (pattern.EndsWith(".*"))
            {
                var prefix = pattern.Substring(0, pattern.Length - 2);
                return toolName.StartsWith(prefix + ".", StringComparison.OrdinalIgnoreCase) || string.Equals(toolName, prefix, StringComparison.OrdinalIgnoreCase);
            }

            return string.Equals(pattern, toolName, StringComparison.OrdinalIgnoreCase);
        }
    }
}
