namespace Jackdaw.Loader.LDTK;

/// <summary>
/// Loader configuration for converting level data into actors.
/// </summary>
public class LDTKConfig(string group) {
    /// <summary>
    /// The provider group to search for LDTK projects.
    /// </summary>
    public readonly string Group = group;

    readonly Dictionary<string, Action<Actor, LDTKEntity>> Entities = [];
    internal readonly CustomLoaderElementContainer CustomElements = new();

    /// <summary>
    /// Get an entity constructor.
    /// </summary>
    /// <param name="name">The entity's name.</param>
    /// <returns>The entity constructor function, null if no matching entity is registered.</returns>
    public Action<Actor, LDTKEntity>? GetEntity(string name) => Entities.GetValueOrDefault(name);

    /// <summary>
    /// Register an entity constructor function for loading into levels. <br/>
    /// Constructor functions pass in an actor initialized with the entity's genric data
    /// and expect the constuctor to add any entity-specific components to it.
    /// </summary>
    /// <param name="name">The entity's name.</param>
    /// <param name="func">The constructor function for converting an entity into an actor.</param>
    /// <returns>The LDTK config.</returns>
    public LDTKConfig RegisterEntity(string name, Action<Actor, LDTKEntity> func) {
        Entities.Add(name, func);
        return this;
    }

    /// <summary>
    /// Register custom loader behavior. <br/>
    /// This is primarily for defining project-specific loader actions when initializing levels. <br/> <br/>
    /// Some common loader cases are provided by the project. <br/>
    /// - <see cref="LDTKAnimationLoaderElement"/> <br/>
    /// - <see cref="LDTKBackgroundLoaderElement"/> <br/>
    /// - <see cref="LDTKCollisionLoaderElement"/> <br/>
    /// </summary>
    /// <param name="element">The custom loader element.</param>
    /// <returns>The LDTK config.</returns>
    public LDTKConfig RegisterCustomLoaderElement(LDTKCustomLoaderElement element) {
        CustomElements.Add(element);
        return this;
    }
}