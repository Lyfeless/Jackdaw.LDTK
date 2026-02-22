using System.Text.Json;
using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

public class LDTKLoader : AssetLoaderStage {
    const string WORLD_EXTENSION = ".ldtk";

    readonly string Group;
    readonly Func<string, int?> CollisionTagFunc;

    readonly Dictionary<string, Action<Actor, EntitySaveData>> TempActorRegistry = [];

    public LDTKLoader(string group, Func<string, int?> collisionTagFunc) {
        Group = group;
        CollisionTagFunc = collisionTagFunc;

        SetAfter<PackerLoader>();
    }

    public override void Run(Assets assets) {
        LDTKStorage storage = new(assets.Game, Group);

        AssetProviderItem world = new("", Group, WORLD_EXTENSION);
        if (!assets.Provider.HasItem(world)) {
            Log.Warning($"LDTKLoader: Unable to load level definition file. No level definition file found for {world}.");
            return;
        }

        WorldSaveData? data;
        try {
            using Stream stream = assets.Provider.GetItemStream(world);
            Span<byte> bytes = new byte[stream.Length];
            stream.ReadExactly(bytes);
            data = JsonSerializer.Deserialize(bytes, LDTKSourceGenerationContext.Default.WorldSaveData)
                ?? throw new Exception();
        } catch {
            Log.Warning($"LDTKLoader: Unable to load level definition file. An error occured when loading data.");
            return;
        }

        foreach (TilesetSaveDefinition tileset in data.Definitions.Tilesets) {
            storage.Tilesets.Add(
                tileset.DefinitionID,
                new(
                    assets.Game,
                    identifier: tileset.Identifier,
                    atlas: GetTilesetTexture(assets, tileset),
                    tileCount: new(tileset.TileCountX, tileset.TileCountY),
                    tileSize: tileset.GridSize,
                    enumTags: tileset.TileTypes,
                    customData: tileset.CustomData,
                    collisionTagFunc: CollisionTagFunc
                )
            );
        }

        foreach (LayerSaveDefinition layer in data.Definitions.Layers) {
            storage.LayerDefinitions.Add(layer.DefinitionID, layer);
        }

        foreach (LevelSaveReference levelRef in data.Levels) {
            storage.Add(levelRef.NameID, levelRef);
        }

        foreach ((string id, Action<Actor, EntitySaveData> func) in TempActorRegistry) {
            storage.RegisterActor(id, func);
        }

        assets.RegisterCustomAssetStorage<LDTKLevelInstance>(storage);
    }

    static Subtexture GetTilesetTexture(Assets assets, TilesetSaveDefinition tileset) {
        string path = Path.Join(
                Path.GetDirectoryName(tileset.TexturePath[(assets.Config.TextureGroup.Length + 1)..]),
                Path.GetFileNameWithoutExtension(tileset.TexturePath)
            ).Replace("\\", "/");

        return assets.GetSubtexture(path);
    }

    public LDTKLoader RegisterActor(string id, Action<Actor, EntitySaveData> func) {
        TempActorRegistry.Add(id, func);
        return this;
    }
}