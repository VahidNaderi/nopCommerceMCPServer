using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Data.Mapping;
using NopPlus.Mcp.Core.Domain;

namespace NopPlus.Mcp.Core.Data
{
    public class McpApiKeyScopeBuilder : NopEntityBuilder<McpApiKeyScope>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(McpApiKeyScope.ApiKeyId)).AsInt32().NotNullable();
            table.WithColumn(nameof(McpApiKeyScope.ToolName)).AsString(200).NotNullable();
        }
    }
}
