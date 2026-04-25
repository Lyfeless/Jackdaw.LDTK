namespace Jackdaw.Loader.LDTK;

public class LDTKConfig(string group) {
    public readonly string Group = group;

    readonly Dictionary<string, Action<Actor, LDTKEntity>> Entities = [];

    public Action<Actor, LDTKEntity>? GetEntity(string name) => Entities.GetValueOrDefault(name);

    public LDTKConfig RegisterEntity(string name, Action<Actor, LDTKEntity> func) {
        Entities.Add(name, func);
        return this;
    }

    public LDTKConfig RegisterLoaderComponent(ILoaderComponent component) {
        //! FIXME (Alex): Implement...
        return this;
    }
}