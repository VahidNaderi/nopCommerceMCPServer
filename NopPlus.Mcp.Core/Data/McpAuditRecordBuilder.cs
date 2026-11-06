using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Data.Mapping;
using NopPlus.Mcp.Core.Domain;

namespace NopPlus.Mcp.Core.Data
{
    public class McpAuditRecordBuilder : NopEntityBuilder<McpAuditRecord>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(McpAuditRecord.ToolName)).AsString(200).NotNullable();
            table.WithColumn(nameof(McpAuditRecord.Severity)).AsInt32().NotNullable();
            table.WithColumn(nameof(McpAuditRecord.UserId)).AsInt32().Nullable();
            table.WithColumn(nameof(McpAuditRecord.ApiKeyId)).AsInt32().Nullable();
            table.WithColumn(nameof(McpAuditRecord.RequestPayload)).AsString(int.MaxValue).Nullable();
            table.WithColumn(nameof(McpAuditRecord.ResponsePayload)).AsString(int.MaxValue).Nullable();
            table.WithColumn(nameof(McpAuditRecord.Success)).AsBoolean().NotNullable();
            table.WithColumn(nameof(McpAuditRecord.ErrorMessage)).AsString(1000).Nullable();
            table.WithColumn(nameof(McpAuditRecord.TimestampUtc)).AsDateTime().NotNullable();
        }
    }
}
