namespace Jackdaw.Loader.LDTK;

public class LDTKWorldDefinitions {
    public readonly LDTKEnumContainer Enums;
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