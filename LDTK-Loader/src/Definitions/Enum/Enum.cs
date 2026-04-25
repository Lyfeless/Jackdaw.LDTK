namespace Jackdaw.Loader.LDTK;

public readonly struct LDTKEnum {
    public readonly int ID;

    public readonly string Name;
    public readonly string[] Values;

    public readonly string[] Tags;

    internal LDTKEnum(JsonElementEnumDefinition data) {
        ID = data.ID;
        Name = data.Name;
        Values = [.. data.Values.Select(e => e.Value)];
        Tags = data.Tags;
    }
}