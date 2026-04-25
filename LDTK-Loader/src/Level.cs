using System.Data.Common;
using System.Numerics;
using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

public readonly struct LDTKLevel {
    public readonly Actor ActorTree = Actor.Invalid;
    public readonly FieldContainer Fields = new();
    public readonly LDTKBackground Background = new();
    public readonly bool IsValid = false;

    internal LDTKLevel(Game game, LDTKWorld world, JsonElementLevel level) {
        IsValid = true;

        BoundsComponent bounds = new(game, Bounds(level));
        bounds.Match.Name = "LevelBounds";

        Background = new(game.Assets, level);
        Fields = level.Fields;

        ActorTree = Actor.From(bounds);
        ActorTree.Match.Name = level.Name;
        ActorTree.Match.Guid = new Guid(level.InstanceID);
        ActorTree.Position = Position(level);

        for (int i = level.Layers.Length - 1; i >= 0; --i) {
            JsonElementLayerInstance instance = level.Layers[i];
            JsonElementLayerDefinition definition = (JsonElementLayerDefinition)world.LayerDefinitions.ByID(instance.ID)!;

            Actor layer = instance.Type switch {
                JsonEnumLayerType.Entities => CreateEntityLayer(game, world, new(instance, definition)),
                JsonEnumLayerType.Tiles => CreateTileLayer(game, new(world, instance, definition, instance.Tiles)),
                JsonEnumLayerType.AutoLayer => CreateTileLayer(game, new(world, instance, definition, instance.AutoLayerTiles)),
                _ => Actor.Invalid
            };

            if (!layer.IsValid) { continue; }

            layer.Match.Name = instance.Name;
            layer.Match.Guid = new(instance.InstanceID);
            layer.Position = Position(instance);
            layer.Visible = instance.Visible;
            ActorTree.Children.Add(layer);
        }
    }

    static Actor CreateEntityLayer(Game game, LDTKWorld world, LDTKEntityLayer data) {
        Actor layer = new(game);
        foreach (LDTKEntity entity in data.Entities) {
            Action<Actor, LDTKEntity>? func = world.Config.GetEntity(entity.Name);
            if (func == null) {
                Log.Warning($"LDTKLoader: Unhandled entity creation for {entity.Name}, no generator defined");
                continue;
            }

            Actor actor = new(game);
            actor.Match.Name = entity.Name;
            actor.Match.Guid = entity.InstanceID;
            actor.Position = entity.Position - data.Metadata.Offset;
            func(actor, entity);
            layer.Children.Add(actor);
        }
        return layer;
    }

    static Actor CreateTileLayer(Game game, LDTKTileLayer data) {
        GridRenderComponent spriteGrid = new(game, data.Metadata.Offset, data.GridSize, new(data.Tileset.TileSize, data.Tileset.TileSize));
        foreach (LDTKTile tile in data.TileElements) {
            spriteGrid.AddTileStackEnd(new SpriteSingle(tile.Tile.Texture) {
                FlipX = tile.Flip.FlipX,
                FlipY = tile.Flip.FlipY
            }, tile.GridPosition);
        }
        Actor layer = Actor.From(spriteGrid);
        return layer;
    }

    static Vector2 Position(JsonElementLevel level) => new(level.WorldX, level.WorldY);
    static Vector2 Position(JsonElementLayerInstance layer) => new(layer.TotalOffsetX, layer.TotalOffsetY);
    static Rect Bounds(JsonElementLevel level) => new(level.Width, level.Height);
}