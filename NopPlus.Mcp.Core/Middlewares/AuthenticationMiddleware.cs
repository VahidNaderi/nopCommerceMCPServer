using System.Threading.Tasks;
using NopPlus.Mcp.Core.Abstractions;
using NopPlus.Mcp.Core.Models;

namespace NopPlus.Mcp.Core.Middlewares
{
    public class AuthenticationMiddleware : IMcpMiddleware
    {
        private readonly IAuthenticationProvider _authProvider;

        public AuthenticationMiddleware(IAuthenticationProvider authProvider)
        {
            _authProvider = authProvider;
        }

        public async Task InvokeAsync(McpContext context, McpMiddlewareDelegate next)
        {
            context.Principal = await _authProvider.AuthenticateAsync(context.Request);
            await next(context);
        }
    }
}
