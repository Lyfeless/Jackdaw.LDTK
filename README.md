## LDTK Loader
An extension for loading levels made in the [LDTK](ldtk.io) editor as nodes for use in Jackdaw.
This extension is still work-in-progress and may be missing integration with some LDTK features.

### Usage
The extension loads LDTK projects using a custom asset loader stage. In order to work an `LDTKLoader` instance needs to be added to `CustomAssetLoaders` in the game's content config. Levels can be loaded using the `Load` method insided a loaded `LDTKWorld`.

Additional loader logic can be registered in the `LDTKConfig`, either to define entity creation or create custom loading behavior.
Some common loader logic is pre-defined:
    - `LDTKBackgroundLoaderElement`: Automatically import a level's defined background information as display components.
    - `LDTKAnimationLoaderElement`: Use a tile's custom data in a tileset to load an animation instead of the default texture.
        Custom data syntax: `[ENTRY NAME]: [Name]` ex `anim: Tile/GroundIdle`
    - `LDTKCollisionLoaderElement`: Load custom per-tile collision from a tile's custom data in a tileset. Has options for stting collision tags using a custom data entry or a tileset's enum flags. Supports multiple collision shapes:
      - FULL: A rectangle collider the exact size of the tile. Syntax: `[ENTRY NAME]: FULL`
      - RECT: A rectangle collider of any size. Syntax: `[ENTRY NAME]: RECT [x] [y] [width] [height]` ex: `collider: RECT 4 4 8 8`
      - CIRCLE: A circular collider centered on its x/y position. Syntax: `[ENTRY NAME]: CIRCLE [x] [y] [radius]` ex `collider: CIRCLE 8 8 4`
      - POLY: A convex polygon collider made of an abritrary bumber of points. Syntax: `[ENTRY NAME]: POLY [x1] [y1] [x2] [y2] ...` ex `collider: POLY 0 8 16 8 8 16 0 8`
```cs
// Create the game instance with a basic configuration
Game game = new(new GameConfig() {
    // ... Other game configuration
    Content = new() {
        CustomAssetLoaders = [
            new LDTKLoader(new LDTKConfig("Levels")
                .RegisterEntity("Example", (actor, entity) => {
                    // ... Entity creation logic
                })
                // Load background and tile-based animations
                .RegisterCustomLoaderElement(new LDTKBackgroundLoaderElement())
                .RegisterCustomLoaderElement(new LDTKAnimationLoaderElement("anim"))
            )
        ]
    }
});

// Load Level
LDTKWorld world = game.Assets.GetLDTKWorld("Levels");
LDTKLevel level = world.Load(name);
game.Root = level.ActorTree;
```