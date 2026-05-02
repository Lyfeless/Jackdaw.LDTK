using System.Text.Json.Serialization;

namespace Jackdaw.Loader.LDTK;

[JsonSourceGenerationOptions(
    AllowTrailingCommas = true,
    ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip,
    PropertyNameCaseInsensitive = true
)]
[JsonSerializable(typeof(JsonElementProjectRoot))]
[JsonSerializable(typeof(JsonElementLevel))]
internal partial class LDTKSourceGenerationContext : JsonSerializerContext { }