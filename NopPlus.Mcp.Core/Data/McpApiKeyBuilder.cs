using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping;
using Nop.Data.Mapping.Builders;
using NopPlus.Mcp.Core.Domain;

namespace NopPlus.Mcp.Core.Data
{
    public class McpApiKeyBuilder : NopEntityBuilder<McpApiKey>
    {
        public McpApiKeyBuilder()
        {
        }

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(McpApiKey.Name)).AsString(200).NotNullable();
            table.WithColumn(nameof(McpApiKey.UserId)).AsInt32().NotNullable();
            table.WithColumn(nameof(McpApiKey.KeyHash)).AsString(2000).NotNullable();
            table.WithColumn(nameof(McpApiKey.IsActive)).AsBoolean().NotNullable().WithDefaultValue(true);
            table.WithColumn(nameof(McpApiKey.ExpiresOnUtc)).AsDateTime().Nullable();
            table.WithColumn(nameof(McpApiKey.CreatedOnUtc)).AsDateTime().NotNullable();
            table.WithColumn(nameof(McpApiKey.LastUsedOnUtc)).AsDateTime().Nullable();
        }
    }
}
