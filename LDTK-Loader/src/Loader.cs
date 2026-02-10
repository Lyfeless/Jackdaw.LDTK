using System.Text.Json;
using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

public class LDTKLoader : AssetLoaderStage {
    readonly Game Game;
    readonly string FilePath;
    readonly Func<string, int?> CollisionTagFunc;

    readonly LDTKStorage Storage;

    public LDTKLoader(Game game, string path, Func<string, int?> collisionTagFunc) {
        Game = game;
        FilePath = path;
        CollisionTagFunc = collisionTagFunc;
        Storage = new(game, GetFolderPath(path));

        SetAfter<PackerLoader>();
    }

    public override void Run(Assets assets) {
        if (!File.Exists(FilePath)) { return; }
        WorldSaveData? data;
        try {
            data = JsonSerializer.Deserialize(File.ReadAllText(FilePath), LDTKSourceGenerationContext.Default.WorldSaveData);
        } catch { return; }
        if (data == null) { return; }

        foreach (TilesetSaveDefinition tileset in data.Definitions.Tilesets) {
            Storage.Tilesets.Add(
                tileset.DefinitionID,
                new(
                    Game,
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
            Storage.LayerDefinitions.Add(layer.DefinitionID, layer);
        }

        foreach (LevelSaveReference levelRef in data.Levels) {
            Storage.Add(levelRef.NameID, levelRef);
        }

        assets.RegisterCustomAssetStorage<LDTKLevelInstance>(Storage);
    }

    static string GetFolderPath(string path)
        => Path.Join(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));

    static Subtexture GetTilesetTexture(Assets assets, TilesetSaveDefinition tileset) {
        string path = Path.Join(
                Path.GetDirectoryName(tileset.TexturePath[(assets.Config.TextureFolder.Length + 1)..]),
                Path.GetFileNameWithoutExtension(tileset.TexturePath)
            ).Replace("\\", "/");

        return assets.GetSubtexture(path);
    }

    public LDTKLoader RegisterActor(string id, Action<Actor, EntitySaveData> func) {
        Storage.RegisterActor(id, func);
        return this;
    }
}