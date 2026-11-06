using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NopPlus.Mcp.Core.Abstractions;
using NopPlus.Mcp.Core.Models;

namespace NopPlus.Mcp.Core.Services
{
    public class McpToolExecutor : IMcpToolExecutor
    {
        private readonly IMcpToolRegistry _registry;
        private readonly Pipeline.McpPipeline _pipeline;
        private readonly ILogger<McpToolExecutor> _logger;

        public McpToolExecutor(IMcpToolRegistry registry, Pipeline.McpPipeline pipeline, ILogger<McpToolExecutor> logger)
        {
            _registry = registry;
            _pipeline = pipeline;
            _logger = logger;
        }

        public async Task ExecuteAsync(string toolName, McpContext context)
        {
            _logger.LogInformation("MCP: Executing tool {Tool} for request {RequestId}", toolName, context.RequestId);

            try
            {
                await _pipeline.ExecuteAsync(toolName, context);
                _logger.LogInformation("MCP: Tool {Tool} executed successfully for request {RequestId}", toolName, context.RequestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MCP: Tool {Tool} execution failed for request {RequestId}", toolName, context.RequestId);
                throw;
            }
        }
    }
}
