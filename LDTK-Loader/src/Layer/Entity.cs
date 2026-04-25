using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

public readonly struct LDTKEntityLayer : ILayer {
    public LDTKLayer Metadata { get; init; }
    public readonly LDTKEntity[] Entities;

    internal LDTKEntityLayer(JsonElementLayerInstance instance, JsonElementLayerDefinition definition) {
        Metadata = new(instance, definition);
        Entities = [.. instance.Entities.Select(e => new LDTKEntity(e, instance))];
    }
}