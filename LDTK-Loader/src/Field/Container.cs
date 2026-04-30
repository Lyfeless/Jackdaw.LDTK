using System.Text.Json.Serialization;

namespace Jackdaw.Loader.LDTK;

/// <summary>
/// A container for an object's variable field values.
/// </summary>
[JsonConverter(typeof(FieldContainerJsonConverter))]
public class FieldContainer {
    /// <summary>
    /// All the object's fields.
    /// </summary>
    public readonly Dictionary<string, Field> Fields = [];

    public Field this[string field] => Fields[field];

    /// <summary>
    /// If the object has a field with the given name.
    /// </summary>
    /// <param name="field">The field name.</param>
    /// <returns>If the field exists.</returns>
    public bool Has(string field) => Fields.ContainsKey(field);

    /// <summary>
    /// If the object has a field with the given name.
    /// </summary>
    /// <param name="field">The field name.</param>
    /// <param name="fieldValue">The field, if it exists.</param>
    /// <returns>If the field exists.</returns>
    public bool Has(string field, out Field fieldValue) => Fields.TryGetValue(field, out fieldValue);
}