using System.Threading.Tasks;

namespace NopPlus.Mcp.Core.Abstractions
{
    public interface IJsonSerializer
    {
        T? Deserialize<T>(string json);

        string Serialize(object? obj);
    }
}
