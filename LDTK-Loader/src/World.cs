using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

/// <summary>
/// Instance of an LDTK world. Stores information and handles loading levels. <br/>
/// Currently LDTK projects will only have one world. The data is seperated to future-proof for updates
/// and to account for projects with experimental multi-world settings enabled.
/// </summary>
public class LDTKWorld {
    /// <summary>
    /// The world's layout format.
    /// </summary>
    public enum LDTKWorldLayout {
        /// <summary>
        /// No set position, levels can have any position.
        /// </summary>
        FREE,

        /// <summary>
        /// Levels are layed out in one contiguous world grid.
        /// </summary>
        GRIDVANIA,

        /// <summary>
        /// Levels are stored in linear order along the x axis.
        /// </summary>
        LINEAR_HORIZONTAL,

        /// <summary>
        /// Levels are stored in linear order along the y axis.
        /// </summary>
        LINEAR_VERTICAL
    }

    /// <summary>
    /// Loader configuration for converting level data into actors.
    /// </summary>
    public readonly LDTKConfig Config;

    /// <summary>
    /// The world's name id.
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// The GUID of the world instance.
    /// </summary>
    public readonly Guid InstanceID;

    /// <summary>
    /// The world's layout format.
    /// </summary>
    public readonly LDTKWorldLayout Layout;

    /// <summary>
    /// The size of the entire world grid.
    /// </summary>
    public readonly Point2 Size;

    readonly Dictionary<string, ILevelAccessor> Levels = [];

    readonly LDTKWorldDefinitions Definitions;

    /// <summary>
    /// The world's editor enums
    /// </summary>
    public LDTKEnumContainer Enums => Definitions.Enums;

    /// <summary>
    /// The world's tilesets.
    /// </summary>
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

    /// <summary>
    /// Load a level instance. <br/>
    /// The load times may vary depending if the project stores level data in seperate files.
    /// </summary>
    /// <param name="name">The level's name.</param>
    /// <returns>The level instance, or an empty level if a matching level isn't found.</returns>
    public LDTKLevel Load(string name) => new(Assets.Game, this, Levels.GetValueOrDefault(name)?.Get() ?? FailLoad(name));

    /// <summary>
    /// Load a level instance asynchronously. <br/>
    /// The load times may vary depending if the project stores level data in seperate files.
    /// </summary>
    /// <param name="name">The level's name.</param>
    /// <returns>The level instance, or an empty level if a matching level isn't found.</returns>
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