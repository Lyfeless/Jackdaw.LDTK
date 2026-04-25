namespace Jackdaw.Loader.LDTK;

public readonly struct LDTKTilesetContainer {
    public readonly Dictionary<string, LDTKTileset> Entries = [];

    internal LDTKTilesetContainer(Assets assets, JsonElementTilesetDefinition[] tilesets) {
        foreach (JsonElementTilesetDefinition tileset in tilesets) {
            Entries.Add(tileset.Name, new(assets, tileset));
        }
    }

    public readonly LDTKTileset[] AllWithTag(string tag) => [.. Entries.Values.Where(e => e.Tags.Contains(tag))];
    public readonly LDTKTileset? ByName(string name) => Entries.GetValueOrDefault(name);
    public readonly LDTKTileset? ByID(int id) => Entries.Values.FirstOrDefault(e => e.ID == id);
}