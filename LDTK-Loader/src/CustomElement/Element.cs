namespace Jackdaw.Loader.LDTK;

/// <summary>
/// An interface for creating custom level loader behavior.
/// </summary>
public abstract class LDTKCustomLoaderElement {
    /// <summary>
    /// A container for the root of a level tree. <br/>
    /// In most cases both values are the same, but can be split if additional children are needed in the level container.
    /// </summary>
    /// <param name="Root">The root actor caontaining all elements related to the level.</param>
    /// <param name="Container">The container object for all layers of the level. Should remain in the root's tree.</param>
    public record struct LevelRoot(Actor Root, Actor Container);

    /// <summary>
    /// If the loader element needs to process tilesets when they're loaded. <br/>
    /// Set to true in order for <see cref="OnTilesetLoad"/> to be run. <br/>
    /// Only processed when adding the element to the loader.
    /// </summary>
    public virtual bool CanProcessTilesets { get; } = false;

    /// <summary>
    /// If the loader element needs to process entity layers when they're created. <br/>
    /// Set to true in order for <see cref="OnEntityLoad"/> to be run. <br/>
    /// Only processed when adding the element to the loader.
    /// </summary>
    public virtual bool CanModifyEntity { get; } = false;

    /// <summary>
    /// If the loader element needs to process tile layers when they're created. <br/>
    /// Set to true in order for <see cref="OnEntityLayerLoad"/> to be run. <br/>
    /// Only processed when adding the element to the loader.
    /// </summary>
    public virtual bool CanModifyEntityLayer { get; } = false;

    /// <summary>
    /// If the loader element needs to process level tile sprites when they're created. <br/>
    /// Set to true in order for <see cref="OnTileLayerLoad"/> to be run. <br/>
    /// Only processed when adding the element to the loader.
    /// </summary>
    public virtual bool CanModifyTileLayer { get; } = false;

    /// <summary>
    /// If the loader element needs to process a level's root actor when it's created.. <br/>
    /// Set to true in order for <see cref="OnTileSpriteLoad"/> to be run. <br/>
    /// Only processed when adding the element to the loader.
    /// </summary>
    public virtual bool CanModifyTileSprite { get; } = false;

    /// <summary>
    /// If the loader element needs to process level entities when they're created. <br/>
    /// Set to true in order for <see cref="OnLevelRootCreate"/> to be run. <br/>
    /// Only processed when adding the element to the loader.
    /// </summary>
    public virtual bool CanModifyLevelRoot { get; } = false;

    /// <summary>
    /// Process a tileset when it's loaded by the main loader. <br/>
    /// Only called if <see cref="CanProcessTilesets"> is true when registered.
    /// </summary>
    /// <param name="tileset">The tileset being loaded.</param>
    public virtual void OnTilesetLoad(LDTKTileset tileset) { }

    /// <summary>
    /// Process or modify an entity while it's being created by the main loader <br/>
    /// Only called if <see cref="CanModifyEntity"> is true when registered.
    /// </summary>
    /// <param name="actor">The entity's actor.</param>
    /// <param name="entity">The entity's data.</param>
    /// <returns>The entity's actor.</returns>
    public virtual Actor OnEntityLoad(Actor actor, LDTKEntity entity) => actor;

    /// <summary>
    /// Process or modify an entity layer while it's being created by the main loader <br/>
    /// Only called if <see cref="CanModifyEntityLayer"> is true when registered.
    /// </summary>
    /// <param name="actor">The entity layer's actor.</param>
    /// <param name="layer">The entity layer's data.</param>
    /// <returns>The entity layer's actor.</returns>
    public virtual Actor OnEntityLayerLoad(Actor actor, LDTKEntityLayer layer) => actor;

    /// <summary>
    /// Process or modify a tile layer while it's being created by the main loader <br/>
    /// Only called if <see cref="CanModifyTileLayer"> is true when registered.
    /// </summary>
    /// <param name="actor">The tile layer's actor.</param>
    /// <param name="layer">The tile layer's data.</param>
    /// <returns>The tile layer's actor.</returns>
    public virtual Actor OnTileLayerLoad(Actor actor, LDTKTileLayer layer) => actor;

    /// <summary>
    /// Process or modify a tile's sprite while it's being created by the main loader <br/>
    /// Only called if <see cref="CanModifyTileSprite"> is true when registered.
    /// </summary>
    /// <param name="sprite">The tile's sprite.</param>
    /// <param name="game">The game instance.</param>
    /// <param name="tile">The tile data.</param>
    /// <param name="layer">The tile's layer data.</param>
    /// <returns></returns>
    public virtual Sprite OnTileSpriteLoad(Sprite sprite, Game game, LDTKTile tile, LDTKTileLayer layer) => sprite;

    /// <summary>
    /// Process or modify the level's root while it's being created by the main loader <br/>
    /// Only called if <see cref="CanModifyLevelRoot"> is true when registered.
    /// </summary>
    /// <param name="root">The level root element.</param>
    /// <param name="level">The level instance data.</param>
    /// <returns>The level root.</returns>
    public virtual LevelRoot OnLevelRootCreate(LevelRoot root, LDTKLevel level) => root;
}