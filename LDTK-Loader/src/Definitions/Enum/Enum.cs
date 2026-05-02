namespace Jackdaw.Loader.LDTK;

/// <summary>
/// Information for a single editor enum.
/// </summary>
public readonly struct LDTKEnum {
    /// <summary>
    /// The enum's internal numeric id.
    /// </summary>
    public readonly int ID;

    /// <summary>
    /// The enum's name id.
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// All possible values the enum can be set to.
    /// </summary>
    public readonly string[] Values;

    /// <summary>
    /// Editor tags for sorting the enum.
    /// </summary>
    public readonly string[] Tags;

    internal LDTKEnum(JsonElementEnumDefinition data) {
        ID = data.ID;
        Name = data.Name;
        Values = [.. data.Values.Select(e => e.Value)];
        Tags = data.Tags;
    }
}