using System.Numerics;
using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

/// <summary>
/// An single entity's instance data.
/// </summary>
public readonly struct LDTKEntity {
    /// <summary>
    /// The entity's name.
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// The entity's internal numeric id.
    /// </summary>
    public readonly int ID;

    /// <summary>
    /// The entity's instance GUID.
    /// </summary>
    public readonly Guid InstanceID;

    /// <summary>
    /// The entity's editor color.
    /// </summary>
    public readonly Color Color;

    /// <summary>
    /// The entity's local level position.
    /// </summary>
    public readonly Point2 Position;

    /// <summary>
    /// The entity's pivot point. <br/>
    /// Unused by the base loader, any pivot information for actor instances needs to be manually set.
    /// </summary>
    public readonly Vector2 Pivot;

    /// <summary>
    /// The entity's editor size.
    /// </summary>
    public readonly Point2 Size;

    /// <summary>
    /// Editor tags for sorting the entity.
    /// </summary>
    public readonly string[] Tags;

    /// <summary>
    /// The entity's variable field values.
    /// </summary>
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

/// <summary>
/// A static actor component for storing entity data. Added to all entity actors created in a level load.
/// </summary>
public class LDTKEntityComponent : Component {
    /// <summary>
    /// The entity's level data.
    /// </summary>
    public LDTKEntity Entity;

    public LDTKEntityComponent(Game game, LDTKEntity entity) : base(game) {
        Active = false;
        Entity = entity;
    }
}