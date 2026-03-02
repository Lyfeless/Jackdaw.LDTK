using System.Numerics;
using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

/// <summary>
/// Loaded instance of an LDTK tile grid layer.
/// </summary>
public class LDTKTileLayer : Component, IStackableGrid<Point2?, LDTKTile?>, ISpatialGrid {
    readonly GridCollider colliderGrid;
    readonly CollisionComponent collider;
    readonly GridRenderComponent renderer;

    readonly Grid<LDTKTile?> Tiles;
    readonly LDTKTileset Tileset;

    Vector2 position;
    Vector2 tileSize;

    public Vector2 Position {
        get => position;
        set {
            position = value;
            colliderGrid.Position = value;
            renderer.Position = value;
        }
    }

    /// <summary>
    /// Loaded instance of an LDTK tile grid layer.
    /// </summary>
    /// <param name="game">The game instance.</param>
    /// <param name="tileset">The tileset the layer is using.</param>
    /// <param name="gridSize">The grid dimensions.</param>
    /// <param name="tileSize">The size of an individual grid tile.</param>
    /// <param name="position">The grid's position relative to the level position.</param>
    public LDTKTileLayer(Game game, LDTKTileset tileset, Point2 gridSize, int tileSize, Vector2 position) : base(game) {
        this.tileSize = new(tileSize);
        this.position = position;
        Tileset = tileset;
        Tiles = new(gridSize);

        colliderGrid = new GridCollider(gridSize, this.tileSize);
        collider = new(game, colliderGrid);
        renderer = new(game, (Point2)position, gridSize, (Point2)this.tileSize);
    }

    protected override void Added() {
        Actor.Components.AddAfter(collider, this);
        Actor.Components.AddAfter(renderer, this);
    }

    protected override void Removed() {
        if (!IsValid) { return; }
        Actor.Components.Remove(collider);
        Actor.Components.Remove(renderer);
    }

    public Vector2 TileSize => tileSize;

    public Point2 TileCount => Tiles.TileCount;

    public Rect Bounds => new(Position, TileCount * TileSize);

    public IGrid<Point2?, LDTKTile?> Set(Point2? tilesetCoord, int tileX, int tileY) => Set(tilesetCoord, new(tileX, tileY));

    public IGrid<Point2?, LDTKTile?> Set(Point2? tilesetCoord, Point2 tile) {
        LDTKTile? instance = CoordToTile(tilesetCoord);
        Tiles.Set(instance, tile);
        UpdateGrids(instance, tile);
        return this;
    }

    public LDTKTile? Get(int tileX, int tileY) => Get(new(tileX, tileY));
    public LDTKTile? Get(Point2 tile) => Tiles.Get(tile);

    public bool Contains(int tileX, int tileY) => Contains(new(tileX, tileY));
    public bool Contains(Point2 tile) => Tiles.Contains(tile);

    public IStackableGrid<Point2?, LDTKTile?> AddTileStackStart(Point2? element, int tileX, int tileY) => AddTileStackStart(element, new(tileX, tileY));
    public IStackableGrid<Point2?, LDTKTile?> AddTileStackStart(Point2? element, Point2 tile) => AddTileStackAt(element, tile, 0);

    public IStackableGrid<Point2?, LDTKTile?> AddTileStackEnd(Point2? element, int tileX, int tileY) => AddTileStackEnd(element, new(tileX, tileY));
    public IStackableGrid<Point2?, LDTKTile?> AddTileStackEnd(Point2? element, Point2 tile) => AddTileStackAt(element, tile, -1);
    public IStackableGrid<Point2?, LDTKTile?> AddTileStackAt(Point2? element, int tileX, int tileY, int index) => AddTileStackAt(element, new(tileX, tileY), index);
    public IStackableGrid<Point2?, LDTKTile?> AddTileStackAt(Point2? element, Point2 tile, int index) {
        LDTKTileElement? tileElement = CoordToTileElement(element);
        if (tileElement == null) { return this; }

        LDTKTile? instance = Tiles.Get(tile);
        if (instance == null) {
            instance = new(tileElement);
            Tiles.Set(instance, tile);
        }
        else {
            if (index == -1) { index = instance.ElementCount; }
            instance.Insert(tileElement, index);
        }

        UpdateGrids(instance, tile);

        return this;
    }

    public IStackableGrid<Point2?, LDTKTile?> RemoveTileStackStart(int tileX, int tileY) => RemoveTileStackStart(new(tileX, tileY));
    public IStackableGrid<Point2?, LDTKTile?> RemoveTileStackStart(Point2 tile) => RemoveTileStackAt(tile, 0);
    public IStackableGrid<Point2?, LDTKTile?> RemoveTileStackEnd(int tileX, int tileY) => RemoveTileStackEnd(new(tileX, tileY));
    public IStackableGrid<Point2?, LDTKTile?> RemoveTileStackEnd(Point2 tile) => RemoveTileStackAt(tile, -1);
    public IStackableGrid<Point2?, LDTKTile?> RemoveTileStackAt(int tileX, int tileY, int index) => RemoveTileStackAt(new(tileX, tileY), index);
    public IStackableGrid<Point2?, LDTKTile?> RemoveTileStackAt(Point2 tile, int index) {
        LDTKTile? instance = Tiles.Get(tile);
        if (instance == null) { return this; }

        if (instance.ElementCount > 1) {
            if (index == -1) { index = instance.ElementCount; }
            instance.RemoveAt(index);
        }
        else { Tiles.Set(null, tile); }

        UpdateGrids(instance, tile);

        return this;
    }

    void UpdateGrids(LDTKTile? tile, Point2 gridCoord) {
        colliderGrid.Set(tile?.Collider, gridCoord);
        renderer.Set(tile?.Sprite, gridCoord);
    }

    LDTKTile? CoordToTile(Point2? tilesetCoord) => ElementToTile(CoordToTileElement(tilesetCoord));
    LDTKTileElement? CoordToTileElement(Point2? tilesetCoord) => tilesetCoord == null ? null : Tileset.Get((Point2)tilesetCoord);
    static LDTKTile? ElementToTile(LDTKTileElement? element) => element == null ? null : new(element);
}