using System.Numerics;
using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

public readonly struct LDTKEntity {
    public readonly string Name;
    public readonly int ID;
    public readonly Guid InstanceID;
    public readonly Color Color;
    public readonly Point2 Position;
    public readonly Vector2 Pivot;
    public readonly Point2 Size;
    public readonly string[] Tags;
    public readonly FieldContainer Fields;

    internal LDTKEntity(JsonElementEntityInstance data, JsonElementLayerInstance layer) {
        Name = data.Name;
        ID = data.ID;
        InstanceID = new(data.InstanceID);
        Color = data.Color;
        Position = FromArray(data.LevelPosition);
        Pivot = FromArray(data.Pivot);
        Size = new(data.Width, data.Height);
        Tags = data.Tags;
        Fields = data.Fields;
    }

    static Point2 FromArray(int[] values) => new(values[0], values[1]);
    static Vector2 FromArray(float[] values) => new(values[0], values[1]);
}