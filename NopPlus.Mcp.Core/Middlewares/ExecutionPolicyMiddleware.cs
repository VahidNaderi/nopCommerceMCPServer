using System.Linq;
using System.Threading.Tasks;
using NopPlus.Mcp.Core.Abstractions;
using NopPlus.Mcp.Core.Authorization.Enums;
using NopPlus.Mcp.Core.Models;

namespace NopPlus.Mcp.Core.Middlewares
{
    /// <summary>
    /// Enforces execution policy for tools.
    /// Phase 1: Immediate executes; RequireConfirmation returns metadata indicating confirmation required.
    /// </summary>
    public class ExecutionPolicyMiddleware : IMcpMiddleware
    {
        private readonly IMcpToolRegistry _registry;

        public ExecutionPolicyMiddleware(IMcpToolRegistry registry)
        {
            _registry = registry;
        }

        public Task InvokeAsync(McpContext context, McpMiddlewareDelegate next)
        {
            if (!context.Items.TryGetValue("__toolName", out var tnObj) || tnObj is not string toolName)
            {
                // if tool name not present, allow pipeline to continue and let registry fail
                return next(context);
            }

            var tool = _registry.Get(toolName);
            if (tool == null) return next(context);

            var policies = tool.ExecutionPolicy?.ToList() ?? new System.Collections.Generic.List<McpExecutionPolicy> { McpExecutionPolicy.Immediate };

            // If any policy is RequireConfirmation, short-circuit with metadata response
            if (policies.Contains(McpExecutionPolicy.RequireConfirmation))
            {
                // populate response indicating confirmation required; the actual UI/confirmation flow is outside scope
                context.Response = new { confirmation_required = true, tool = toolName, policies = policies };
                return Task.CompletedTask;
            }

            // default: continue pipeline
            return next(context);
        }
    }
}
