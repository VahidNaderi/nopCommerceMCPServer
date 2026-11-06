using NopPlus.Mcp.Core.Abstractions;
using NopPlus.Mcp.Core.Authorization;
using NopPlus.Mcp.Core.Models;

namespace NopPlus.Mcp.Core.Middlewares
{
    public class AuthorizationMiddleware : IMcpMiddleware
    {
        private readonly IMcpAuthorizationService _authorizationService;
        private readonly string _toolName;
        private readonly IMcpToolRegistry _registry;

        public AuthorizationMiddleware(IMcpAuthorizationService authorizationService, IMcpToolRegistry registry, string toolName)
        {
            _authorizationService = authorizationService;
            _registry = registry;
            _toolName = toolName;
        }

        public async Task InvokeAsync(McpContext context, McpMiddlewareDelegate next)
        {
            var tool = _registry.Get(_toolName);
            if (tool == null)
                throw new InvalidOperationException($"Tool '{_toolName}' not found for authorization middleware.");

            var result = await _authorizationService.AuthorizeAsync(tool, context);
            if (!result.IsAuthorized)
            {
                throw new UnauthorizedAccessException(result.FailureReason ?? "MCP authorization failed");
            }

            await next(context);
        }
    }
}
