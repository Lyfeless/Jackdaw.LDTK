using System.Numerics;
using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

/// <summary>
/// Information on the level's background. <br/>
/// Unused by the base loader, should be parsed by a custom loader element depending on camera behavior. <br/>
/// For a basic implementation, add an instance of <see cref="LDTKBackgroundLoaderElement"/> to the loader.
/// </summary>
public readonly struct LDTKBackground {
    /// <summary>
    /// The level's background color.
    /// </summary>
    public readonly Color Color;

    /// <summary>
    /// If the background has a defined texture.
    /// </summary>
    public readonly bool HasTexture;

    /// <summary>
    /// The background texture, if one exists. Clipped to the bounds contained within the level.
    /// </summary>
    public readonly Subtexture Texture = new();

    /// <summary>
    /// The texture's position, if one exists.
    /// </summary>
    public readonly Point2 Position = Point2.One;

    /// <summary>
    /// The texture's scale, if one exists.
    /// </summary>
    public readonly Vector2 Scale = Vector2.One;

    internal LDTKBackground(Assets assets, JsonElementLevel level) {
        Color = level.BackgroundColor;
        HasTexture = level.BackgroundPosition != null && level.BackgroundPath != null;
        if (HasTexture) {
            JsonElementBackgroundPosition data = (JsonElementBackgroundPosition)level.BackgroundPosition!;
            Rect cropRect = RectFromArray(data.CropBounds);
            string path = LDTKLoader.RemoveBacklinks(level.BackgroundPath!);
            path = AssetProviderItem.FromString(path).Name;
            Texture = assets.GetSubtexture(path).GetClipSubtexture(cropRect);
            Position = Point2FromArray(data.Position);
            Scale = Vector2FromArray(data.Scale);
        }
    }

    static Rect RectFromArray(float[] values) => new(values[0], values[1], values[2], values[3]);
    static Vector2 Vector2FromArray(float[] values) => new(values[0], values[1]);
    static Point2 Point2FromArray(int[] values) => new(values[0], values[1]);
}