namespace Jackdaw.Loader.LDTK;

/// <summary>
/// A reference to an instance of an entity.
/// </summary>
public readonly struct FieldEntity() {
    /// <summary>
    /// The entity's GUID.
    /// </summary>
    public readonly Guid EntityID { get; init; } = Guid.Empty;

    /// <summary>
    /// The GUID of the layer containing the entity.
    /// </summary>
    public readonly Guid LayerID { get; init; } = Guid.Empty;

    /// <summary>
    /// The GUID of the level containing the entity.
    /// </summary>
    public readonly Guid LevelID { get; init; } = Guid.Empty;

    /// <summary>
    /// The GUID of the world containing the entity.
    /// </summary>
    public readonly Guid WorldID { get; init; } = Guid.Empty;
}