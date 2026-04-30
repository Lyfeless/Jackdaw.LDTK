namespace Jackdaw.Loader.LDTK;

/// <summary>
/// Custom loader behavior for adding a background to a level based on it's background data.
/// </summary>
/// <param name="state">The method the loader should use for decided the look of the background.</param>
public class LDTKBackgroundLoaderElement(LDTKBackgroundLoaderElement.State state = LDTKBackgroundLoaderElement.State.BOTH) : LDTKCustomLoaderElement {
    /// <summary>
    /// The method the loader should use for decided the look of the background.
    /// </summary>
    public enum State {
        /// <summary>
        /// Don't load any background.
        /// </summary>
        NONE,

        /// <summary>
        /// Load both color and texture if possible.
        /// </summary>
        BOTH,

        /// <summary>
        /// Load the background texture, or the solid color if no texture is set.
        /// </summary>
        TEXTURE_FIRST,

        /// <summary>
        /// Load the background texture, or no background if no texture is set.
        /// </summary>
        TEXTURE_ONLY,

        /// <summary>
        /// Ignore texture information and always set the background to the solid color.
        /// </summary>
        COLOR_ONLY
    }

    readonly State state = state;

    public override bool CanModifyLevelRoot => true;

    public override LevelRoot OnLevelRootCreate(LevelRoot root, LDTKLevel level) {
        Actor? element = GetElement(root.Root.Game, level);
        if (element == null) { return root; }
        root.Root.Children.AddFirst(element);
        return root;
    }

    Actor? GetElement(Game game, LDTKLevel level) {
        if (state == State.NONE) { return null; }
        if (state == State.BOTH) { return Both(game, level); }
        if (CanUseTexture(level)) { return Texture(game, level); }
        if (CanUseColor(level)) { return Color(game, level); }
        return null;
    }

    Actor Both(Game game, LDTKLevel level) {
        Actor actor = Actor.From(Color(game, level));
        if (CanUseTexture(level)) { actor.Children.Add(Texture(game, level)); }
        return actor;
    }

    static Actor Texture(Game game, LDTKLevel level) {
        Actor actor = Actor.From(new DisplayObjectRenderComponent(game, new SpriteSingle(level.Background.Texture) {
            Offset = level.Background.Position
        }));
        actor.Scale = level.Background.Scale;
        actor.Position = level.Background.Position;
        return actor;
    }

    static Actor Color(Game game, LDTKLevel level) => Actor.From(new DisplayObjectRenderComponent(game, new DisplayRectangle(level.Size.X, level.Size.Y) {
        Color = level.Background.Color
    }));

    bool CanUseTexture(LDTKLevel level) => level.Background.HasTexture && state != State.COLOR_ONLY;
    bool CanUseColor(LDTKLevel level) => state != State.TEXTURE_ONLY;
}