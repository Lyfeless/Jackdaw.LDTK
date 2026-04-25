using System.Numerics;
using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

public readonly struct LDTKBackground {
    public readonly Color Color;

    public readonly bool HasTexture;
    public readonly Subtexture Texture = new();
    public readonly Point2 Position = Point2.One;
    public readonly Vector2 Scale = Vector2.One;

    internal LDTKBackground(Assets assets, JsonElementLevel level) {
        Color = level.BackgroundColor;
        HasTexture = level.BackgroundPosition != null && level.BackgroundPath != null;
        if (HasTexture) {
            JsonElementBackgroundPosition data = (JsonElementBackgroundPosition)level.BackgroundPosition!;
            string path = LDTKLoader.RemoveBacklinks(level.BackgroundPath!);
            Rect cropRect = RectFromArray(data.Position);
            Texture = assets.GetSubtexture(LDTKLoader.RemoveBacklinks(path)).GetClipSubtexture(cropRect);
            Position = Point2FromArray(data.Position);
            Scale = Vector2FromArray(data.Scale);
        }
    }

    static Rect RectFromArray(int[] values) => new(values[0], values[1], values[2], values[3]);
    static Vector2 Vector2FromArray(float[] values) => new(values[0], values[1]);
    static Point2 Point2FromArray(int[] values) => new(values[0], values[1]);
}