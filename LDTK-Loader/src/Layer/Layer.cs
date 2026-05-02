using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

/// <summary>
/// Generic information for all types of level layer instance.
/// </summary>
public readonly struct LDTKLayer {
    /// <summary>
    /// The layer's internal numeric id.
    /// </summary>
    public readonly int ID;

    /// <summary>
    /// The layer's name id.
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// The layer's instance GUID.
    /// </summary>
    public readonly Guid InstanceID;

    /// <summary>
    /// The numeric id ov the level the layer is a part of.
    /// </summary>
    public readonly int LevelID;

    /// <summary>
    /// Information for how a layer should display relative to a camera.
    /// </summary>
    public readonly LDTKLayerParallax Parallax;

    /// <summary>
    /// The layer's relative offset from the level's position.
    /// </summary>
    public readonly Point2 Offset;

    /// <summary>
    /// The layer's editor transparency.
    /// </summary>
    public readonly float Opacity;

    /// <summary>
    /// If the layer is rendered.
    /// </summary>
    public readonly bool Visible;

    internal LDTKLayer(JsonElementLayerInstance instance, JsonElementLayerDefinition definition) {
        Name = instance.Name;
        ID = instance.ID;
        InstanceID = new(instance.InstanceID);
        LevelID = instance.LevelID;
        Parallax = new(definition);
        Offset = new(instance.TotalOffsetX, instance.TotalOffsetY);
        Opacity = instance.Opacity;
        Visible = instance.Visible;
    }
}