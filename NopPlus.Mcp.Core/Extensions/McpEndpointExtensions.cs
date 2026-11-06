using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NopPlus.Mcp.Core.Abstractions;
using NopPlus.Mcp.Core.Models.Dto;


namespace NopPlus.Mcp.Core.Extensions
{
    public static class McpEndpointExtensions
    {
        public static IApplicationBuilder UseMcpEndpoint(this IApplicationBuilder app, string path)
        {
            app.Map(path, builder =>
            {
                builder.Run(async ctx =>
                {
                    var serializer = builder.ApplicationServices.GetRequiredService<IJsonSerializer>();
                    var executor = builder.ApplicationServices.GetRequiredService<IMcpToolExecutor>();
                    var logger = builder.ApplicationServices.GetService<ILoggerFactory>()?.CreateLogger("McpEndpoint");

                    using var sr = new StreamReader(ctx.Request.Body);
                    var body = await sr.ReadToEndAsync();
                    McpRequestDto? req = null;

                    try
                    {
                        req = serializer.Deserialize<McpRequestDto>(body);
                    }
                    catch (System.Exception ex)
                    {
                        logger?.LogError(ex, "MCP: Failed to deserialize request");
                        ctx.Response.StatusCode = 400;
                        var bad = new McpResponseDto { Success = false, Error = new McpError { Code = "invalid_request", Message = "Invalid request JSON", Details = ex.Message } };
                        await ctx.Response.WriteAsync(serializer.Serialize(bad));
                        return;
                    }

                    if (req == null || string.IsNullOrWhiteSpace(req.Tool))
                    {
                        ctx.Response.StatusCode = 400;
                        var bad = new McpResponseDto { Success = false, Error = new McpError { Code = "invalid_request", Message = "Tool name is required" } };
                        await ctx.Response.WriteAsync(serializer.Serialize(bad));
                        return;
                    }

                    var mctx = new Models.McpContext { Request = req.Payload };

                    try
                    {
                        await executor.ExecuteAsync(req.Tool, mctx);
                    }
                    catch (System.Exception ex)
                    {
                        logger?.LogError(ex, "MCP: Tool execution error for {Tool}", req.Tool);
                        ctx.Response.StatusCode = 500;
                        var err = new McpResponseDto { Success = false, Error = new McpError { Code = "tool_error", Message = ex.Message, Details = ex.StackTrace } };
                        await ctx.Response.WriteAsync(serializer.Serialize(err));
                        return;
                    }

                    var success = new McpResponseDto { Success = true, Payload = mctx.Response };
                    ctx.Response.ContentType = "application/json";
                    await ctx.Response.WriteAsync(serializer.Serialize(success));
                });
            });

            return app;
        }
    }
}
