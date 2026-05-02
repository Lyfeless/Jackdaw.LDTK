namespace Jackdaw.Loader.LDTK;

internal class CustomLoaderElementContainer {
    readonly List<LDTKCustomLoaderElement> TilesetProcessElements = [];
    readonly List<LDTKCustomLoaderElement> RootModifyElements = [];
    readonly List<LDTKCustomLoaderElement> EntityModifyElements = [];
    readonly List<LDTKCustomLoaderElement> EntityLayerModifyElements = [];
    readonly List<LDTKCustomLoaderElement> TileLayerModifyElements = [];
    readonly List<LDTKCustomLoaderElement> TileSpriteModifyElements = [];

    public bool CanProcessTilesets = false;
    public bool CanModifyLevelRoot = false;
    public bool CanModifyEntity = false;
    public bool CanModifyEntityLayer = false;
    public bool CanModifyTileLayer = false;
    public bool CanModifyTileSprite = false;

    public void Add(LDTKCustomLoaderElement element) {
        if (element.CanProcessTilesets) {
            TilesetProcessElements.Add(element);
            CanProcessTilesets = true;
        }

        if (element.CanModifyLevelRoot) {
            RootModifyElements.Add(element);
            CanModifyLevelRoot = true;
        }

        if (element.CanModifyEntity) {
            EntityModifyElements.Add(element);
            CanModifyEntity = true;
        }

        if (element.CanModifyEntityLayer) {
            EntityLayerModifyElements.Add(element);
            CanModifyEntityLayer = true;
        }

        if (element.CanModifyTileLayer) {
            TileLayerModifyElements.Add(element);
            CanModifyTileLayer = true;
        }

        if (element.CanModifyTileSprite) {
            TileSpriteModifyElements.Add(element);
            CanModifyTileSprite = true;
        }
    }

    public void OnTilesetLoad(LDTKTileset tileset) { foreach (LDTKCustomLoaderElement element in TilesetProcessElements) { element.OnTilesetLoad(tileset); } }

    public Actor OnEntityLoad(Actor actor, LDTKEntity entity) {
        foreach (LDTKCustomLoaderElement element in EntityModifyElements) { actor = element.OnEntityLoad(actor, entity); }
        return actor;
    }

    public Actor OnEntityLayerLoad(Actor actor, LDTKEntityLayer layer) {
        foreach (LDTKCustomLoaderElement element in EntityLayerModifyElements) { actor = element.OnEntityLayerLoad(actor, layer); }
        return actor;
    }

    public Sprite OnTileSpriteLoad(Sprite sprite, Game game, LDTKTile tile, LDTKTileLayer layer) {
        foreach (LDTKCustomLoaderElement element in TileSpriteModifyElements) { sprite = element.OnTileSpriteLoad(sprite, game, tile, layer); }
        return sprite;
    }

    public Actor OnTileLayerLoad(Actor actor, LDTKTileLayer layer) {
        foreach (LDTKCustomLoaderElement element in TileLayerModifyElements) { actor = element.OnTileLayerLoad(actor, layer); }
        return actor;
    }

    public LDTKCustomLoaderElement.LevelRoot OnLevelRootCreate(LDTKCustomLoaderElement.LevelRoot root, LDTKLevel level) {
        foreach (LDTKCustomLoaderElement element in RootModifyElements) { root = element.OnLevelRootCreate(root, level); }
        return root;
    }

}