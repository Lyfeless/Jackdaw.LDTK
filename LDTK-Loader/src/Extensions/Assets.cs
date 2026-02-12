namespace Jackdaw.Loader.LDTK;

public static class AssetsExtensions
{
    extension(Assets assets)
    {
        public LDTKLevelInstance GetLDTKLevelInstance(string name) => assets.Get<LDTKLevelInstance>(name);
    }
}