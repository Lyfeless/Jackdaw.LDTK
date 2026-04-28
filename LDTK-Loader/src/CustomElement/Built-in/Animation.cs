
namespace Jackdaw.Loader.LDTK;

public class LDTKAnimationLoaderElement(string entryName) : LDTKCustomLoaderElement {
    readonly string Name = entryName;

    readonly Dictionary<(int, int), string> animatedTiles = [];

    public override bool CanProcessTilesets => true;
    public override bool CanModifyTileSprite => true;

    public override void OnTilesetLoad(LDTKTileset tileset) {
        int tilesetID = tileset.ID;
        foreach ((int tileID, LDTKTilesetTile tile) in tileset.Tiles) {
            if (!tile.CustomDataEntries.TryGetValue(Name, out string? data) || data == string.Empty) { continue; }
            animatedTiles.Add((tilesetID, tileID), data);
        }
    }

    public override Sprite OnTileSpriteLoad(Sprite sprite, Game game, LDTKTile tile, LDTKTileLayer layer) {
        if (!animatedTiles.TryGetValue((layer.Tileset.ID, tile.TilesetTile.ID), out string? animation)) { return sprite; }
        return new SpriteAnimated(game, animation);
    }
}