using Microsoft.Extensions.DependencyInjection;
using NopPlus.Mcp.Core.Abstractions;
using NopPlus.Mcp.Core.Authorization;
using NopPlus.Mcp.Core.Services;
using System.Text.Json;

namespace NopPlus.Mcp.Core.Extensions
{
    public static class McpServiceCollectionExtensions
    {
        public static IServiceCollection AddMcpCore(this IServiceCollection services)
        {
            services.AddSingleton<IMcpToolRegistry, InMemoryToolRegistry>();
            services.AddSingleton<IMcpToolExecutor, Services.McpToolExecutor>();
            services.AddScoped<IMcpAuthorizationService, Authorization.McpAuthorizationService>();
            services.AddScoped<IMcpApiKeyService, Services.McpApiKeyService>();
            services.AddSingleton<IJsonSerializer>(sp => new SystemTextJsonSerializer(new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));
            services.AddSingleton<IAuditLogger, SimpleAuditLogger>();
            // Default audit logger is registered above; replace with a custom implementation if required by the application.
            services.AddSingleton<IBasicAuthenticationHandler, Services.NoOpBasicAuthenticationHandler>();
            services.AddSingleton<IAuthenticationProvider, Services.BasicAuthenticationProvider>();
            services.AddHttpContextAccessor();
            // Pipeline and middlewares are added as transient so consumers can register their own implementations
            services.AddTransient<Pipeline.McpPipeline>();
            // register execution policy middleware by default
            services.AddTransient<Middlewares.ExecutionPolicyMiddleware>();

            return services;
        }
    }
}
