using System.Numerics;

namespace Jackdaw.Loader.LDTK;

/// <summary>
/// Information for how a layer should display relative to a camera. <br/>
/// Unused by the base loader, should be parsed by a custom loader element depending on camera behavior.
/// </summary>
public readonly struct LDTKLayerParallax {
    /// <summary>
    /// The amount the layer is affected by camera scrolling, ranging from -1 to 1.
    /// A value of 0 follows the camera exactly.
    /// </summary>
    public readonly Vector2 Scale;

    /// <summary>
    /// If the layer should be scaled visually.
    /// </summary>
    public readonly bool ShouldScale;

    internal LDTKLayerParallax(JsonElementLayerDefinition data) {
        Scale = new(data.ParallaxFactorX, data.ParallaxFactorY);
        ShouldScale = data.ParallaxScaling;
    }
}