using System.ComponentModel;
using System.Text.Json;

namespace Jackdaw.Loader.LDTK;

internal interface ILevelAccessor {
    public JsonElementLevel Get();
}

internal readonly struct LocalLevelAccessor(JsonElementLevel level) : ILevelAccessor {
    readonly JsonElementLevel Level = level;
    public readonly JsonElementLevel Get() => Level;
}

internal readonly struct FileLevelAccessor : ILevelAccessor {
    readonly Assets Assets;
    readonly AssetProviderItem Item;

    public FileLevelAccessor(Assets assets, LDTKWorld world, string path) {
        string extension = Path.GetExtension(path);
        string pathNoExtension = path[..^extension.Length];

        Assets = assets;
        Item = new(world.Config.Group, pathNoExtension, extension);
    }

    public readonly JsonElementLevel Get() {
        if (!Assets.Provider.HasItem(Item)) { return LDTKWorld.FailLoad(Item.Name); }

        try {
            using Stream stream = Assets.Provider.GetItemStream(Item);
            JsonElementLevel? levelData = JsonSerializer.Deserialize(stream, LDTKSourceGenerationContext.Default.JsonElementLevel);
            if (levelData != null) { return (JsonElementLevel)levelData; }
        } catch { }

        return LDTKWorld.FailLoad(Item.Name);
    }
}