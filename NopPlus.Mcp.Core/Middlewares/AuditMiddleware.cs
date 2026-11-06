using System;
using System.Text.Json;
using System.Threading.Tasks;
using NopPlus.Mcp.Core.Abstractions;
using NopPlus.Mcp.Core.Domain;
using NopPlus.Mcp.Core.Models;

namespace NopPlus.Mcp.Core.Middlewares
{
    public class AuditMiddleware : IMcpMiddleware
    {
        private readonly IAuditLogger _auditLogger;

        public AuditMiddleware(IAuditLogger auditLogger)
        {
            _auditLogger = auditLogger;
        }

        public async Task InvokeAsync(McpContext context, McpMiddlewareDelegate next)
        {
            var record = new McpAuditRecord
            {
                ToolName = context.Items.TryGetValue("__toolName", out var tn) ? tn as string ?? string.Empty : string.Empty,
                TimestampUtc = DateTime.UtcNow,
                RequestPayload = context.Request != null ? JsonSerializer.Serialize(context.Request) : null
            };

            try
            {
                await next(context);

                record.ResponsePayload = context.Response != null ? JsonSerializer.Serialize(context.Response) : null;
                record.Success = true;
            }
            catch (Exception ex)
            {
                record.Success = false;
                record.ErrorMessage = ex.Message;
                await _auditLogger.LogAsync(record);
                throw;
            }

            await _auditLogger.LogAsync(record);
        }
    }
}
