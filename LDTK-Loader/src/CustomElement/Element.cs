namespace Jackdaw.Loader.LDTK;

public abstract class LDTKCustomLoaderElement {
    public record struct LevelRoot(Actor Root, Actor Container);

    public virtual bool CanProcessTilesets { get; } = false;
    public virtual bool CanModifyLevelRoot { get; } = false;
    public virtual bool CanModifyEntity { get; } = false;
    public virtual bool CanModifyEntityLayer { get; } = false;
    public virtual bool CanModifyTileLayer { get; } = false;
    public virtual bool CanModifyTileSprite { get; } = false;

    public virtual void OnTilesetLoad(LDTKTileset tileset) { }
    public virtual Actor OnEntityLoad(Actor actor, LDTKEntity entity) => actor;
    public virtual Actor OnEntityLayerLoad(Actor actor, LDTKEntityLayer layer) => actor;
    public virtual Actor OnTileLayerLoad(Actor actor, LDTKTileLayer layer) => actor;
    public virtual Sprite OnTileSpriteLoad(Sprite sprite, Game game, LDTKTile tile, LDTKTileLayer layer) => sprite;
    public virtual LevelRoot OnLevelRootCreate(LevelRoot root, LDTKLevel level) => root;
}