using System.Text.Json.Serialization;

namespace Jackdaw.Loader.LDTK;

[JsonConverter(typeof(FieldContainerJsonConverter))]
public class FieldContainer {
    public readonly Dictionary<string, Field> Fields = [];

    public Field this[string field] => Fields[field];
    public bool Has(string field) => Fields.ContainsKey(field);
    public bool Has(string field, out Field fieldValue) => Fields.TryGetValue(field, out fieldValue);
}