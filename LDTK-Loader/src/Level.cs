using System.Numerics;
using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

public readonly struct LDTKLevel {
    public enum NeighborLocation {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        UP_LEFT,
        UP_RIGHT,
        DOWN_LEFT,
        DOWN_RIGHT,
        BELOW,
        ABOVE,
        SAME_LOCATION
    }
    public record struct Neighbor(string InstanceID, NeighborLocation Location);

    public readonly Actor ActorTree = Actor.Invalid;
    public readonly Point2 Size;
    public readonly Neighbor[] Neighbors;
    public readonly FieldContainer Fields = new();
    public readonly LDTKBackground Background = new();
    public readonly bool IsValid = false;

    internal LDTKLevel(Game game, LDTKWorld world, JsonElementLevel level) {
        IsValid = true;

        Size = new(level.Width, level.Height);
        Neighbors = [.. level.Neighbors.Select(e => new Neighbor(e.InstanceID, MapDirection(e.Direction)))];

        BoundsComponent bounds = new(game, new(Size));
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
                JsonEnumLayerType.Tiles => CreateTileLayer(game, world, new(world, instance, definition, instance.Tiles)),
                JsonEnumLayerType.AutoLayer => CreateTileLayer(game, world, new(world, instance, definition, instance.AutoLayerTiles)),
                _ => Actor.Invalid
            };

            if (!layer.IsValid) { continue; }

            layer.Match.Name = instance.Name;
            layer.Match.Guid = new(instance.InstanceID);
            layer.Position = Position(instance);
            layer.Visible = instance.Visible;
            ActorTree.Children.Add(layer);
        }

        if (world.Config.CustomElements.CanModifyLevelRoot) {
            ActorTree = world.Config.CustomElements.OnLevelRootCreate(new(ActorTree, ActorTree), this).Root;
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

            Actor actor = Actor.From(new LDTKEntityComponent(game, entity));
            actor.Match.Name = entity.Name;
            actor.Match.Guid = entity.InstanceID;
            actor.Position = entity.Position - data.Metadata.Offset;

            if (world.Config.CustomElements.CanModifyEntity) {
                actor = world.Config.CustomElements.OnEntityLoad(actor, entity);
            }

            func(actor, entity);
            layer.Children.Add(actor);
        }

        if (world.Config.CustomElements.CanModifyEntityLayer) {
            layer = world.Config.CustomElements.OnEntityLayerLoad(layer, data);
        }

        return layer;
    }

    static Actor CreateTileLayer(Game game, LDTKWorld world, LDTKTileLayer data) {
        GridRenderComponent spriteGrid = new(game, data.Metadata.Offset, data.GridSize, new(data.Tileset.TileSize, data.Tileset.TileSize));
        LDTKTileGridComponent tileGrid = new(game, data.GridSize, data.Tileset.TileSize);
        foreach (LDTKTile tile in data.TileElements) {
            Sprite sprite = new SpriteSingle(tile.TilesetTile.Texture) {
                FlipX = tile.Flip.FlipX,
                FlipY = tile.Flip.FlipY
            };

            if (world.Config.CustomElements.CanModifyTileSprite) {
                sprite = world.Config.CustomElements.OnTileSpriteLoad(sprite, game, tile, data);
            }

            spriteGrid.AddTileStackEnd(sprite, tile.GridPosition);
            tileGrid.AddTileStackEnd(tile, tile.GridPosition);
        }
        Actor layer = Actor.From(game, spriteGrid, tileGrid);

        if (world.Config.CustomElements.CanModifyTileLayer) {
            layer = world.Config.CustomElements.OnTileLayerLoad(layer, data);
        }

        return layer;
    }

    static Vector2 Position(JsonElementLevel level) => new(level.WorldX, level.WorldY);
    static Vector2 Position(JsonElementLayerInstance layer) => new(layer.TotalOffsetX, layer.TotalOffsetY);
    static NeighborLocation MapDirection(string direction) => direction switch {
        "n" => NeighborLocation.UP,
        "s" => NeighborLocation.DOWN,
        "e" => NeighborLocation.RIGHT,
        "w" => NeighborLocation.LEFT,
        "ne" => NeighborLocation.UP_RIGHT,
        "nw" => NeighborLocation.UP_LEFT,
        "se" => NeighborLocation.DOWN_LEFT,
        "sw" => NeighborLocation.DOWN_RIGHT,
        "<" => NeighborLocation.BELOW,
        ">" => NeighborLocation.ABOVE,
        "o" => NeighborLocation.SAME_LOCATION,
        _ => NeighborLocation.SAME_LOCATION
    };
}