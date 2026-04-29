using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

public class LDTKWorld {
    public enum LDTKWorldLayout {
        FREE,
        GRIDVANIA,
        LINEAR_HORIZONTAL,
        LINEAR_VERTICAL
    }

    public readonly LDTKConfig Config;

    public readonly string Name;
    public readonly Guid InstanceID;
    public readonly LDTKWorldLayout Layout;
    public readonly Point2 Size;

    readonly Dictionary<string, ILevelAccessor> Levels = [];

    readonly LDTKWorldDefinitions Definitions;
    public LDTKEnumContainer Enums => Definitions.Enums;
    public LDTKTilesetContainer Tilesets => Definitions.Tilesets;
    internal LDTKLayerDefinitionContainer LayerDefinitions => Definitions.Layers;

    readonly Assets Assets;

    internal LDTKWorld(Assets assets, LDTKWorldDefinitions definitions, JsonElementWorld data, LDTKConfig config, bool external) {
        Config = config;
        Assets = assets;

        Name = data.Name;
        InstanceID = new(data.InstanceID);
        Layout = MapLayout(data.WorldLayout);
        Size = new(data.WorldGridWidth, data.WorldGridHeight);

        Definitions = definitions;

        foreach (JsonElementLevel level in data.Levels) {
            Levels.Add(level.Name, CreateAccessor(assets, level, external));
        }
    }

    ILevelAccessor CreateAccessor(Assets assets, JsonElementLevel level, bool external)
        => external
            ? new FileLevelAccessor(assets, this, level.ExternalDataPath!)
            : new LocalLevelAccessor(level);

    public LDTKLevel Load(string name) => new(Assets.Game, this, Levels.GetValueOrDefault(name)?.Get() ?? FailLoad(name));
    public async Task<LDTKLevel> LoadAsync(string name) => Load(name);

    internal static LDTKWorldLayout MapLayout(JsonEnumWorldLayout layout) => layout switch {
        JsonEnumWorldLayout.GridVania => LDTKWorldLayout.GRIDVANIA,
        JsonEnumWorldLayout.LinearHorizontal => LDTKWorldLayout.LINEAR_HORIZONTAL,
        JsonEnumWorldLayout.LinearVertical => LDTKWorldLayout.LINEAR_VERTICAL,
        _ => LDTKWorldLayout.FREE,
    };

    internal static JsonElementLevel FailLoad(string name) {
        Log.Warning($"LDTK Loader: Failed to load level {name}, returning empty");
        return new();
    }
}