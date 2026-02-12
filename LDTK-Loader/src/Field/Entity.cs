namespace Jackdaw.Loader.LDTK;

public readonly struct FieldEntity() {
    public readonly Guid EntityID { get; init; } = Guid.Empty;
    public readonly Guid LayerID { get; init; } = Guid.Empty;
    public readonly Guid LevelID { get; init; } = Guid.Empty;
    public readonly Guid WorldID { get; init; } = Guid.Empty;
}