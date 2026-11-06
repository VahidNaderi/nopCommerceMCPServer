using System.Text.Json;
using NopPlus.Mcp.Core.Abstractions;

namespace NopPlus.Mcp.Core.Services
{
    public class SystemTextJsonSerializer : IJsonSerializer
    {
        private readonly JsonSerializerOptions _options;

        public SystemTextJsonSerializer(JsonSerializerOptions? options = null)
        {
            _options = options ?? new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public T? Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, _options);
        }

        public string Serialize(object? obj)
        {
            return JsonSerializer.Serialize(obj, _options);
        }
    }
}
