using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using NopPlus.Mcp.Core.Authorization.Enums;
using NopPlus.Mcp.Core.Models;
using NopPlus.Mcp.Core.Services;
using NopPlus.Mcp.Core.Tools;

namespace NopPlus.Mcp.Core.Tests
{
    public class ToolRegistryTests
    {
        [Test]
        public void Register_ToolExists_GetDescriptorsReturnsToolMetadata()
        {
            // Arrange
            var registry = new InMemoryToolRegistry();
            var tool = new DummyEchoTool();

            var descriptor = new ToolDescriptor
            {
                Name = tool.Name,
                Description = "Custom description",
                Category = "UnitTestCategory",
                RequiredPermissions = new[] { "Perm1" },
                RequiredMcpPermissions = new[] { "McpPerm1" },
                OperationType = McpOperationType.Write,
                ExecutionPolicy = new[] { McpExecutionPolicy.Immediate },
                AuditSeverity = McpAuditSeverity.Important
            };

            // Act
            registry.Register(tool, descriptor);
            var descriptors = registry.GetDescriptors().ToList();

            // Assert
            Assert.That(descriptors, Has.Exactly(1).Matches<ToolDescriptor>(d =>
                d.Name == descriptor.Name
                && d.Description == descriptor.Description
                && d.Category == descriptor.Category
            ));
        }

        [Test]
        public void DisabledTool_NotExposed()
        {
            // Arrange
            var registry = new InMemoryToolRegistry();
            var tool = new DummyEchoTool();

            // Act - do NOT register the tool to simulate a disabled/unexposed tool

            // Assert
            Assert.That(registry.Get(tool.Name) == null);
            Assert.That(!registry.GetDescriptors().Any(d => d.Name == tool.Name));
        }

        [Test]
        public void UnknownTool_ReturnsNotFound()
        {
            var registry = new InMemoryToolRegistry();
            var result = registry.Get("this.tool.does.not.exist");
            Assert.That(result == null);
        }

        [Test]
        public void DuplicateToolNames_RegistrationFails()
        {
            // Arrange
            var registry = new InMemoryToolRegistry();
            var t1 = new DuplicateNameToolA();
            var t2 = new DuplicateNameToolB();

            // Register first tool
            registry.Register(t1, new ToolDescriptor { Name = t1.Name });

            // Act & Assert - expect registration of a second tool with the same name to fail during startup validation
            Assert.Throws<System.InvalidOperationException>(() => registry.Register(t2, new ToolDescriptor { Name = t2.Name }));
        }

        private class DuplicateNameToolA : NopPlus.Mcp.Core.Abstractions.IMcpTool
        {
            public string Name => "dup.tool";
            public string Description => "A";
            public string? InputSchema => null;
            public string? Category => null;
            public System.Collections.Generic.IEnumerable<string> RequiredPermissions => System.Array.Empty<string>();
            public System.Collections.Generic.IEnumerable<string> RequiredMcpPermissions => System.Array.Empty<string>();
            public McpOperationType OperationType => default;
            public System.Collections.Generic.IEnumerable<McpExecutionPolicy> ExecutionPolicy => System.Array.Empty<McpExecutionPolicy>();
            public McpAuditSeverity AuditSeverity => default;
            public Task ExecuteAsync(Models.McpContext context) => Task.CompletedTask;
        }

        private class DuplicateNameToolB : NopPlus.Mcp.Core.Abstractions.IMcpTool
        {
            public string Name => "dup.tool";
            public string Description => "B";
            public string? InputSchema => null;
            public string? Category => null;
            public System.Collections.Generic.IEnumerable<string> RequiredPermissions => System.Array.Empty<string>();
            public System.Collections.Generic.IEnumerable<string> RequiredMcpPermissions => System.Array.Empty<string>();
            public McpOperationType OperationType => default;
            public System.Collections.Generic.IEnumerable<McpExecutionPolicy> ExecutionPolicy => System.Array.Empty<McpExecutionPolicy>();
            public McpAuditSeverity AuditSeverity => default;
            public Task ExecuteAsync(Models.McpContext context) => Task.CompletedTask;
        }
    }
}
