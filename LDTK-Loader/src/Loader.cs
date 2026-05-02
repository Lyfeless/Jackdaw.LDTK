using System.Text.Json;
using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

/// <summary>
/// Asset loader stage for importing LDTK project files.
/// </summary>
public class LDTKLoader : AssetLoaderStage {
    const string WORLD_EXTENSION = ".ldtk";
    const string BACKUP_FOLDER = "backups";

    /// <summary>
    /// Configuration data for loading worlds and levels.
    /// </summary>
    public readonly LDTKConfig Config;

    readonly Dictionary<string, string[]> ProjectWorldCache = [];

    public LDTKLoader(LDTKConfig config) {
        Config = config;
        SetAfter<PackerLoader>();
    }

    public override AssetProviderItem[] GetLoadOptions(Assets assets)
        => [.. assets.Provider.GetItemsInGroup(Config.Group, WORLD_EXTENSION).Where(e => !IsBackup(e.Name))];

    public override void RunLoad(Assets assets, AssetCollection collection) {
        foreach (AssetProviderItem item in Filter(collection)) {
            JsonElementProjectRoot project = GetProject(assets, item);
            CacheWorlds(project.InstanceID, LoadWorlds(assets, item.Name, project));
        }
    }

    public override void RunUnload(Assets assets, AssetCollection collection) {
        foreach (AssetProviderItem item in Filter(collection)) {
            if (!ProjectWorldCache.TryGetValue(item.Name, out string[]? worlds)) { return; }
            foreach (string world in worlds) {
                RemoveAsset<LDTKWorld>(assets, world);
            }
        }
    }

    string[] LoadWorlds(Assets assets, string name, JsonElementProjectRoot project) {
        LDTKWorldDefinitions defs = new(assets, Config, project.Definitions);
        return IsMultiWorld(project)
            ? [.. project.Worlds.Select(e => LoadWorld(assets, defs, e, project.ExternalLevels))]
            : [LoadWorld(assets, defs, AsWorld(name, project), project.ExternalLevels)];
    }

    string LoadWorld(Assets assets, LDTKWorldDefinitions defs, JsonElementWorld worldData, bool external) {
        LDTKWorld world = new(assets, defs, worldData, Config, external);
        AddAsset(assets, world.Name, world);
        return world.Name;
    }

    void CacheWorlds(string project, params string[] worlds)
        => ProjectWorldCache.TryAdd(project, worlds);

    static JsonElementProjectRoot GetProject(Assets assets, AssetProviderItem item) {
        try {
            using Stream stream = assets.Provider.GetItemStream(item);
            JsonElementProjectRoot? project = JsonSerializer.Deserialize(stream, LDTKSourceGenerationContext.Default.JsonElementProjectRoot);
            if (project != null) { return (JsonElementProjectRoot)project; }
        } catch {
            Log.Warning($"LDTKLoader: Unable to load level definition file. An error occured when loading data.");
        }
        return new();
    }

    AssetProviderItem[] Filter(AssetCollection collection)
        => collection.Filter(Config.Group, WORLD_EXTENSION);

    static bool IsMultiWorld(JsonElementProjectRoot project) => project.Worlds.Length > 0;

    static JsonElementWorld AsWorld(string name, JsonElementProjectRoot project) => new() {
        Name = name,
        InstanceID = project.InstanceID,
        Levels = project.Levels,
        WorldGridWidth = project.WorldGridWidth ?? 0,
        WorldGridHeight = project.WorldGridHeight ?? 0,
        WorldLayout = project.WorldLayout ?? JsonEnumWorldLayout.Free,
    };

    /// <summary>
    /// Remove backlinks from a file path to start at the highest root folder.
    /// </summary>
    /// <param name="path">The path to modify.</param>
    /// <returns>The path with all leading backlinks removed.</returns>
    public static string RemoveBacklinks(string path) {
        const string BACKLINK = "../";
        while (path.StartsWith(BACKLINK)) { path = path[BACKLINK.Length..]; }
        return path;
    }

    static bool IsBackup(string path) => path.Split('/').Contains(BACKUP_FOLDER);
}