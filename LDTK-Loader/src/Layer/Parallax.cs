using System.Numerics;

namespace Jackdaw.Loader.LDTK;

public readonly struct LDTKLayerParallax {
    public readonly Vector2 Scale;
    public readonly bool ShouldScale;

    internal LDTKLayerParallax(JsonElementLayerDefinition data) {
        Scale = new(data.ParallaxFactorX, data.ParallaxFactorY);
        ShouldScale = data.ParallaxScaling;
    }
}