
namespace Jackdaw.Loader.LDTK;

/// <summary>
/// Custom loader behavior for replacing a tile's default sprite with an animation. <br/>
/// Animations are defined using a line in the tile's custom data. <br/>
/// Format: <br/>
///     <c>[entryName]: [animationName]</c> <br/>
/// Example: <br/>
///     <c>anim: Tile/Idle</c>
/// </summary>
/// <param name="entryName">The id used to signify an animation in tile's custom data.</param>
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