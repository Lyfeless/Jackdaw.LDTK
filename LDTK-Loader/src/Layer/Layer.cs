using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

public readonly struct LDTKLayer {
    public readonly string Name;
    public readonly int ID;
    public readonly Guid InstanceID;
    public readonly int LevelID;
    public readonly LDTKLayerParallax Parallax;
    public readonly Point2 Offset;
    public readonly float Opacity;
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

public interface ILayer {
    public LDTKLayer Metadata { get; internal init; }
}