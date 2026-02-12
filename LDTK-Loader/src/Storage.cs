using System.Text.Json;
using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

public class LDTKStorage(Game game, string folderPath) : IAssetStorage {
    const string EntityLayerDescriptor = "Entities";
    const string IntGridLayerDescriptor = "IntGrid";
    const string TileLayerDescriptor = "Tiles";
    const string AutoLayerDescriptor = "AutoLayer";

    readonly Game Game = game;
    readonly string FolderPath = folderPath;

    internal readonly Dictionary<int, LDTKTileset> Tilesets = [];
    internal readonly Dictionary<int, LayerSaveDefinition> LayerDefinitions = [];
    internal readonly Dictionary<string, LevelSaveReference> Levels = [];

    readonly Dictionary<string, Action<Actor, EntitySaveData>> ActorRegistry = [];

    LDTKLevelInstance FallbackInstance = new() {
        ActorTree = Actor.Invalid,
        Fields = new()
    };

    public void Add(string name, object asset) { Levels.Add(name, (LevelSaveReference)asset); }

    public object Get(string name) {
        if (!Levels.TryGetValue(name, out LevelSaveReference? levelRef)) {
            return FailWithFallback($"LDTKLoader: Level name {name} not found, load unsuccessful");
        }

        LevelSaveData? levelData;
        try {
            levelData = JsonSerializer.Deserialize(File.ReadAllText(Path.Join(FolderPath, $"{levelRef.NameID}.ldtkl")), LDTKSourceGenerationContext.Default.LevelSaveData);
        } catch {
            return LevelLoadFail(name);
        }

        if (levelData == null) {
            return LevelLoadFail(name);
        }

        Actor levelRoot = new(Game);
        levelRoot.Match.Name = levelRef.NameID;
        levelRoot.Position = new(levelRef.X, levelRef.Y);

        BoundsComponent bounds = new(Game, new(levelRef.Width, levelRef.Height));
        bounds.Match.Name = "LevelBounds";
        levelRoot.Components.Add(bounds);

        for (int i = levelData.Layers.Length - 1; i >= 0; --i) {
            LayerSaveData layerData = levelData.Layers[i];
            LayerSaveDefinition layerDefinition = LayerDefinitions[layerData.LayerDefinitionID];

            Actor newLayer = new(Game);
            newLayer.Match.Name = layerDefinition.Identifier ?? layerData.InstanceID;
            newLayer.Match.Guid = new Guid(layerData.InstanceID);
            newLayer.Position = new(layerData.OffsetX, layerData.OffsetY);
            if (!layerData.Visible) {
                newLayer.Ticking = false;
                newLayer.Visible = false;
            }

            switch (layerData.Type) {
                case EntityLayerDescriptor: {
                        foreach (EntitySaveData entityData in layerData.Entities) {
                            if (!ActorRegistry.ContainsKey(entityData.NameID)) {
                                Log.Warning($"LDTKLoader: Unhandled entity creation for {entityData.NameID}, no generator defined");
                                continue;
                            }
                            Actor newEntity = new(Game);
                            newEntity.Match.Name = entityData.InstanceID;
                            newEntity.Match.Guid = new Guid(entityData.InstanceID);
                            newEntity.Position = new(entityData.Position[0], entityData.Position[1]);
                            ActorRegistry[entityData.NameID](newEntity, entityData);

                            newLayer.Children.Add(newEntity);
                        }
                    }
                    break;
                case IntGridLayerDescriptor:
                case TileLayerDescriptor:
                case AutoLayerDescriptor: {
                        if (layerData.Tileset == null) {
                            Log.Warning($"LDTKLoader: Unable to load layer {newLayer.Match.Name}, no tileset assigned");
                            continue;
                        }
                        LDTKTileset tileset = Tilesets[(int)layerData.Tileset];
                        LDTKTileLayer tiles = new(Game, tileset, new(layerData.Width, layerData.Height), layerData.TileSize, new(layerData.OffsetX, layerData.OffsetY));
                        foreach (TileSaveData tile in layerData.Tiles) {
                            tiles.AddTileStackLocal(tileset.GetTileCoord(new Point2(tile.Source[0], tile.Source[1])), new(tile.Position[0], tile.Position[1]));
                        }
                        newLayer.Components.Add(tiles);
                    }
                    break;
                default:
                    Log.Warning($"LDTKLoader: Attempting to load invalid layer type {layerData.Type}, skipping.");
                    continue;
            }

            levelRoot.Children.Add(newLayer);
        }

        return new LDTKLevelInstance {
            ActorTree = levelRoot,
            Fields = levelRef.Fields
        };
    }

    public void RegisterActor(string id, Action<Actor, EntitySaveData> func) {
        if (ActorRegistry.ContainsKey(id)) { Log.Warning($"LDTKLoader: Attempting to re-define actor id {id}, skipping."); }
        ActorRegistry.Add(id, func);
    }

    public string[] GetAssetNames() {
        throw new NotImplementedException();
    }

    public object GetFallback() => FallbackInstance;

    public void SetFallback(object asset) {
        FallbackInstance = (LDTKLevelInstance)asset;
    }

    object LevelLoadFail(string name)
        => FailWithFallback($"LDTKLoader: An error occured while trying to load {name}, load unsuccessful");

    object FailWithFallback(string message) {
        Log.Warning(message);
        return GetFallback();
    }
}