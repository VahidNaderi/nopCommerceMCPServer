using Microsoft.Extensions.DependencyInjection;
using NopPlus.Mcp.Core.Abstractions;
using NopPlus.Mcp.Core.Services;
using NopPlus.Mcp.Core.Tools;

namespace NopPlus.Mcp.Core.Extensions
{
    public static class McpToolRegistrationExtensions
    {
        public static IServiceCollection AddDummyEchoTool(this IServiceCollection services)
        {
            services.AddSingleton<DummyEchoTool>();
            services.AddSingleton<IMcpTool>(sp => sp.GetRequiredService<DummyEchoTool>());
            // Also register it into the registry during startup via an IHostedService or consumer plugin init - this helper only ensures the tool is resolvable.
            return services;
        }
    }
}
