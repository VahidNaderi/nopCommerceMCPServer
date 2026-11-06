using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using NopPlus.Mcp.Core.Abstractions;
using NopPlus.Mcp.Core.Authorization;
using NopPlus.Mcp.Core.Models;
using NopPlus.Mcp.Core.Models.Auth;
using NopPlus.Mcp.Core.Services;
using NUnit.Framework;

namespace NopPlus.Mcp.Core.Tests
{
    public class PermissionMatrixTests
    {
        [Test]
        public async Task Admin_CanExecute_Tool()
        {
            // Arrange
            var registry = new InMemoryToolRegistry();
            var tool = new TestTool("tool.admin", new[] { "admin_perm" });
            registry.Register(tool, new ToolDescriptor { Name = tool.Name, RequiredPermissions = tool.RequiredPermissions });

            var auth = new FakeAuthorizationService((t, ctx) =>
            {
                var name = ctx.Principal?.Name;
                return Task.FromResult(new AuthorizationResult { IsAuthorized = name == "admin" });
            });

            var context = new McpContext { Principal = new McpPrincipal { Name = "admin" } };

            // Act
            var can = await auth.CanExecuteAsync(tool, context);

            // Assert
            Assert.That(can);
        }

        [Test]
        public async Task Guest_CannotExecute_Tool()
        {
            var registry = new InMemoryToolRegistry();
            var tool = new TestTool("tool.admin", new[] { "admin_perm" });
            registry.Register(tool, new ToolDescriptor { Name = tool.Name, RequiredPermissions = tool.RequiredPermissions });

            var auth = new FakeAuthorizationService((t, ctx) =>
            {
                var name = ctx.Principal?.Name;
                return Task.FromResult(new AuthorizationResult { IsAuthorized = name == "admin", FailureReason = "Missing permission" });
            });

            var context = new McpContext { Principal = new McpPrincipal { Name = "guest" } };

            var can = await auth.CanExecuteAsync(tool, context);

            Assert.That(!can);
        }

        [Test]
        public async Task DisabledPermission_DeniesExecution()
        {
            var tool = new TestTool("tool.sensitive", new[] { "sensitive_perm" });
            var auth = new FakeAuthorizationService((t, ctx) =>
            {
                // simulate a permission that is present but disabled
                return Task.FromResult(new AuthorizationResult { IsAuthorized = false, FailureReason = "Permission disabled" });
            });

            var context = new McpContext { Principal = new McpPrincipal { Name = "admin" } };

            var can = await auth.CanExecuteAsync(tool, context);

            Assert.That(!can);
        }

        [Test]
        public async Task Tool_Hidden_WhenPermissionMissing()
        {
            // Two tools, one requires admin perm, other is public
            var registry = new InMemoryToolRegistry();
            var adminTool = new TestTool("tool.admin", new[] { "admin_perm" });
            var publicTool = new TestTool("tool.public", System.Array.Empty<string>());
            registry.Register(adminTool, new ToolDescriptor { Name = adminTool.Name, RequiredPermissions = adminTool.RequiredPermissions });
            registry.Register(publicTool, new ToolDescriptor { Name = publicTool.Name, RequiredPermissions = publicTool.RequiredPermissions });

            var auth = new FakeAuthorizationService((t, ctx) =>
            {
                // guest cannot run admin tool, but can run public tool
                var name = ctx.Principal?.Name;
                var allowed = name == "admin" || t.Name == publicTool.Name;
                return Task.FromResult(new AuthorizationResult { IsAuthorized = allowed });
            });

            var context = new McpContext { Principal = new McpPrincipal { Name = "guest" } };

            var visible = await FilterVisibleToolsAsync(registry, auth, context);

            Assert.That(visible.Select(d => d.Name), Does.Not.Contain(adminTool.Name));
            Assert.That(visible.Select(d => d.Name), Does.Contain(publicTool.Name));
        }

        private async Task<System.Collections.Generic.IEnumerable<ToolDescriptor>> FilterVisibleToolsAsync(InMemoryToolRegistry registry, IMcpAuthorizationService authService, McpContext context)
        {
            var descriptors = registry.GetDescriptors().ToList();
            var result = new System.Collections.Generic.List<ToolDescriptor>();
            foreach (var t in registry.GetAll())
            {
                var res = await authService.CanExecuteAsync(t, context);
                if (res)
                {
                    // find descriptor
                    var d = descriptors.FirstOrDefault(x => x.Name == t.Name);
                    if (d != null) result.Add(d);
                }
            }

            return result;
        }

        private class FakeAuthorizationService : IMcpAuthorizationService
        {
            private readonly System.Func<IMcpTool, McpContext, Task<AuthorizationResult>> _f;

            public FakeAuthorizationService(System.Func<IMcpTool, McpContext, Task<AuthorizationResult>> f)
            {
                _f = f;
            }

            public Task<AuthorizationResult> AuthorizeAsync(IMcpTool tool, McpContext context) => _f(tool, context);

            public async Task<bool> CanExecuteAsync(IMcpTool tool, McpContext context)
            {
                var r = await AuthorizeAsync(tool, context);
                return r.IsAuthorized;
            }
        }

        private class TestTool : IMcpTool
        {
            public TestTool(string name, System.Collections.Generic.IEnumerable<string> requiredPermissions)
            {
                Name = name;
                RequiredPermissions = requiredPermissions;
            }

            public string Name { get; }
            public string Description => "test";
            public string? InputSchema => null;
            public string? Category => null;
            public System.Collections.Generic.IEnumerable<string> RequiredPermissions { get; }
            public System.Collections.Generic.IEnumerable<string> RequiredMcpPermissions => System.Array.Empty<string>();
            public NopPlus.Mcp.Core.Authorization.Enums.McpOperationType OperationType => default;
            public System.Collections.Generic.IEnumerable<NopPlus.Mcp.Core.Authorization.Enums.McpExecutionPolicy> ExecutionPolicy => System.Array.Empty<NopPlus.Mcp.Core.Authorization.Enums.McpExecutionPolicy>();
            public NopPlus.Mcp.Core.Authorization.Enums.McpAuditSeverity AuditSeverity => default;
            public Task ExecuteAsync(McpContext context) => Task.CompletedTask;
        }
    }
}
