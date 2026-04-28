namespace Jackdaw.Loader.LDTK;

public class LDTKBackgroundLoaderElement(LDTKBackgroundLoaderElement.State state = LDTKBackgroundLoaderElement.State.BOTH) : LDTKCustomLoaderElement {
    public enum State {
        NONE,
        BOTH,
        TEXTURE_FIRST,
        TEXTURE_ONLY,
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