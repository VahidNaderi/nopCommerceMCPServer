using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NopPlus.Mcp.Core.Abstractions;
using NopPlus.Mcp.Core.Models;

namespace NopPlus.Mcp.Core.Pipeline
{
    public class McpPipeline
    {
        private readonly IList<IMcpMiddleware> _middlewares;
        private readonly IMcpToolRegistry _toolRegistry;

        public McpPipeline(IEnumerable<IMcpMiddleware> middlewares, IMcpToolRegistry toolRegistry)
        {
            _middlewares = middlewares?.ToList() ?? new List<IMcpMiddleware>();
            _toolRegistry = toolRegistry ?? throw new ArgumentNullException(nameof(toolRegistry));
        }

        public async Task ExecuteAsync(string toolName, McpContext context)
        {
            var tool = _toolRegistry.Get(toolName);
            if (tool == null)
                throw new InvalidOperationException($"Tool '{toolName}' not found in registry.");

            // expose tool name to middlewares that need it
            context.Items["__toolName"] = toolName;

            McpMiddlewareDelegate terminal = ctx => tool.ExecuteAsync(ctx);

            McpMiddlewareDelegate pipeline = _middlewares.Reverse()
                .Aggregate(terminal, (next, mw) => (ctx) => mw.InvokeAsync(ctx, next));

            await pipeline(context);
        }
    }
}
