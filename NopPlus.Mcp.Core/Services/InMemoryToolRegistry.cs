using NopPlus.Mcp.Core.Abstractions;
using System.Collections.Concurrent;

namespace NopPlus.Mcp.Core.Services;

public class InMemoryToolRegistry : IMcpToolRegistry
{
    private readonly ConcurrentDictionary<string, IMcpTool> _tools = new();
    private readonly ConcurrentDictionary<string, Models.ToolDescriptor> _descriptors = new();

    public void Register(IMcpTool tool, Models.ToolDescriptor? descriptor = null)

    {
        if (_tools.Any(t => t.Key.Equals(tool.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"A tool with the name '{tool.Name}' is already registered.");
        }
        _tools[tool.Name] = tool;

        if (descriptor == null)
        {
            descriptor = new Models.ToolDescriptor
            {
                Name = tool.Name,
                Description = tool.Description,
                RequiredPermissions = tool.RequiredPermissions,
                RequiredMcpPermissions = tool.RequiredMcpPermissions,
                OperationType = tool.OperationType,
                ExecutionPolicy = tool.ExecutionPolicy,
                AuditSeverity = tool.AuditSeverity
            };
        }

        _descriptors[tool.Name] = descriptor;
    }

    public IMcpTool? Get(string name)
    {
        _tools.TryGetValue(name, out var tool);
        return tool;
    }

    public IEnumerable<IMcpTool> GetAll() => _tools.Values;

    public IEnumerable<Models.ToolDescriptor> GetDescriptors() => _descriptors.Values;
}
