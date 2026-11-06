using System.Collections.Generic;

namespace NopPlus.Mcp.Core.Abstractions
{
    public interface IMcpToolRegistry
    {
        void Register(IMcpTool tool, Models.ToolDescriptor? descriptor = null);

        IMcpTool? Get(string name);

        IEnumerable<IMcpTool> GetAll();

        /// <summary>
        /// Return descriptors for all registered tools
        /// </summary>
        IEnumerable<Models.ToolDescriptor> GetDescriptors();
    }
}
