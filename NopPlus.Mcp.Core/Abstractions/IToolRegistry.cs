using System.Collections.Generic;
using System.Threading.Tasks;

namespace NopPlus.Mcp.Core.Abstractions
{
    public interface IToolRegistry
    {
        void Register(IMcpTool tool);

        IMcpTool? Get(string name);

        IEnumerable<IMcpTool> GetAll();
    }
}
