namespace Jackdaw.Loader.LDTK;

readonly struct LDTKLayerDefinitionContainer {
    public readonly Dictionary<string, JsonElementLayerDefinition> Entries = [];

    internal LDTKLayerDefinitionContainer(JsonElementLayerDefinition[] layers) {
        foreach (JsonElementLayerDefinition layer in layers) {
            Entries.Add(layer.Name, layer);
        }
    }

    public readonly JsonElementLayerDefinition? ByName(string name) => Entries.GetValueOrDefault(name);
    public readonly JsonElementLayerDefinition? ByID(int id) => Entries.Values.FirstOrDefault(e => e.ID == id);
}