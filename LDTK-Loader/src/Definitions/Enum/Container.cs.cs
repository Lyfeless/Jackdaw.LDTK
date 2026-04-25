namespace Jackdaw.Loader.LDTK;

public readonly struct LDTKEnumContainer {
    public readonly Dictionary<string, LDTKEnum> Entries = [];

    internal LDTKEnumContainer(JsonElementEnumDefinition[] enums)
        => AddAll(enums);

    internal LDTKEnumContainer(JsonElementEnumDefinition[] enums, JsonElementEnumDefinition[] externalEnums) {
        AddAll(enums);
        AddAll(externalEnums);
    }

    void AddAll(JsonElementEnumDefinition[] enums) {
        foreach (JsonElementEnumDefinition data in enums) {
            Entries.Add(data.Name, new(data));
        }
    }

    public LDTKEnum[] AllWithTag(string tag) => [.. Entries.Values.Where(e => e.Tags.Contains(tag))];
    public LDTKEnum? ByName(string name) => Entries.GetValueOrDefault(name);
    public LDTKEnum? ByID(int id) => Entries.Values.FirstOrDefault(e => e.ID == id);
}