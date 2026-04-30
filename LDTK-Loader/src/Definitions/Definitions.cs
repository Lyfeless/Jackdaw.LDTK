namespace Jackdaw.Loader.LDTK;

/// <summary>
/// Metadata elements for an entire LDTK world.
/// </summary>
public class LDTKWorldDefinitions {
    /// <summary>
    /// Editor enum information.
    /// </summary>
    public readonly LDTKEnumContainer Enums;

    /// <summary>
    /// Tileset information.
    /// </summary>
    public readonly LDTKTilesetContainer Tilesets;

    // Layer definitions store a small amount of data not duplicated in the instance.
    //  Just keeping a stored reference for the loader to collect when creating an instance.
    internal readonly LDTKLayerDefinitionContainer Layers;

    internal LDTKWorldDefinitions(Assets assets, LDTKConfig config, JsonElementDefinitions data) {
        Enums = new(data.Enums, data.ExternalEnums);
        Tilesets = new(assets, config, data.Tilesets);
        Layers = new(data.Layers);
    }
}