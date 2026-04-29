using System.Numerics;
using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

public class LDTKTileGridComponent : Component, IStackableGrid<LDTKTile, List<LDTKTile>>, ISpatialGrid {
    readonly Grid<List<LDTKTile>> Grid;
    Point2 tileSize;
    Vector2 position;

    public LDTKTileGridComponent(Game game, Point2 gridSize, int tileSize) : base(game) {
        Active = false;
        Grid = new(gridSize);
        this.tileSize = new(tileSize, tileSize);
    }

    public Point2 TileCount => Grid.TileCount;

    public Vector2 Position { get => position; set => position = value; }

    public Vector2 TileSize => tileSize;

    public Rect Bounds => new(position, tileSize * Grid.TileCount);

    public List<LDTKTile>? Get(int tileX, int tileY) => Grid.Get(tileX, tileY);
    public List<LDTKTile>? Get(Point2 tile) => Grid.Get(tile);

    public IGrid<LDTKTile, List<LDTKTile>> Set(LDTKTile element, int tileX, int tileY) { Grid.Set([element], tileX, tileY); return this; }
    public IGrid<LDTKTile, List<LDTKTile>> Set(LDTKTile element, Point2 tile) { Grid.Set([element], tile); return this; }

    public IStackableGrid<LDTKTile, List<LDTKTile>> AddTileStackStart(LDTKTile element, int tileX, int tileY) => AddTileStackStart(element, new(tileX, tileY));
    public IStackableGrid<LDTKTile, List<LDTKTile>> AddTileStackStart(LDTKTile element, Point2 tile) => AddTileStackAt(element, tile, 0);

    public IStackableGrid<LDTKTile, List<LDTKTile>> AddTileStackEnd(LDTKTile element, int tileX, int tileY) => AddTileStackEnd(element, new(tileX, tileY));
    public IStackableGrid<LDTKTile, List<LDTKTile>> AddTileStackEnd(LDTKTile element, Point2 tile) => AddTileStackAt(element, tile, -1);

    public IStackableGrid<LDTKTile, List<LDTKTile>> AddTileStackAt(LDTKTile element, int tileX, int tileY, int index) => AddTileStackAt(element, new(tileX, tileY), index);
    public IStackableGrid<LDTKTile, List<LDTKTile>> AddTileStackAt(LDTKTile element, Point2 tile, int index) {
        List<LDTKTile>? value = Grid.Get(tile);
        if (value == null) { Grid.Set([element], tile); }
        else {
            if (index == -1) { value.Add(element); }
            else { value.Insert(index, element); }
        }
        return this;
    }

    public bool Contains(int tileX, int tileY) => Grid.Contains(tileX, tileY);
    public bool Contains(Point2 tile) => Grid.Contains(tile);

    public IStackableGrid<LDTKTile, List<LDTKTile>> RemoveTileStackStart(int tileX, int tileY) => RemoveTileStackStart(new(tileX, tileY));
    public IStackableGrid<LDTKTile, List<LDTKTile>> RemoveTileStackStart(Point2 tile) => RemoveTileStackAt(tile, 0);

    public IStackableGrid<LDTKTile, List<LDTKTile>> RemoveTileStackEnd(int tileX, int tileY) => RemoveTileStackEnd(new(tileX, tileY));
    public IStackableGrid<LDTKTile, List<LDTKTile>> RemoveTileStackEnd(Point2 tile) => RemoveTileStackAt(tile, -1);

    public IStackableGrid<LDTKTile, List<LDTKTile>> RemoveTileStackAt(int tileX, int tileY, int index) => RemoveTileStackAt(new(tileX, tileY), index);
    public IStackableGrid<LDTKTile, List<LDTKTile>> RemoveTileStackAt(Point2 tile, int index) {
        List<LDTKTile>? value = Grid.Get(tile);
        if (value != null) {
            if (value.Count == 1) { Grid.Set(null, tile); }
            else { value?.RemoveAt(index == -1 ? value.Count - 1 : index); }
        }
        return this;
    }
}