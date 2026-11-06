using System;
using System.Collections.Generic;

namespace NopPlus.Mcp.Core.Models
{
    public class McpContext
    {
        public Guid RequestId { get; set; } = Guid.NewGuid();

        public IDictionary<string, object> Items { get; } = new Dictionary<string, object>();

        public object? Request { get; set; }

        public object? Response { get; set; }

        public Models.Auth.McpPrincipal? Principal { get; set; }
    }
}
