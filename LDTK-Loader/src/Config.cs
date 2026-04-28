namespace Jackdaw.Loader.LDTK;

public class LDTKConfig(string group) {
    public readonly string Group = group;

    readonly Dictionary<string, Action<Actor, LDTKEntity>> Entities = [];
    internal readonly CustomLoaderElementContainer CustomElements = new();

    public Action<Actor, LDTKEntity>? GetEntity(string name) => Entities.GetValueOrDefault(name);

    public LDTKConfig RegisterEntity(string name, Action<Actor, LDTKEntity> func) {
        Entities.Add(name, func);
        return this;
    }

    public LDTKConfig RegisterCustomLoaderElement(LDTKCustomLoaderElement element) {
        CustomElements.Add(element);
        return this;
    }
}