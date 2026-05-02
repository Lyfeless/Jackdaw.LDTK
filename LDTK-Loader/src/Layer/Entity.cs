namespace Jackdaw.Loader.LDTK;

/// <summary>
/// Layer instance data for an entity layer in a level.
/// </summary>
public readonly struct LDTKEntityLayer {
    /// <summary>
    /// Generic layer information.
    /// </summary>
    public readonly LDTKLayer Metadata;

    /// <summary>
    /// The information of every entity on the layer.
    /// </summary>
    public readonly LDTKEntity[] Entities;

    internal LDTKEntityLayer(JsonElementLayerInstance instance, JsonElementLayerDefinition definition) {
        Metadata = new(instance, definition);
        Entities = [.. instance.Entities.Select(e => new LDTKEntity(e, instance))];
    }
}