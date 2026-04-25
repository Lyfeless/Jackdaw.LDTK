namespace Jackdaw.Loader.LDTK;

public static class AssetsExtensions {
    extension(Assets assets) {
        public LDTKWorld GetLDTKWorld(string name) => assets.Get<LDTKWorld>(name);
    }
}