namespace Jackdaw.Loader.LDTK;

public static class AssetsExtensions {
    extension(Assets assets) {
        /// <summary>
        /// Get a loaded LDTK world from storage.
        /// </summary>
        /// <param name="name">The name id of the world asset to find.</param>
        /// <returns>The requested asset, or the fallback if the name id isn't present.</returns>
        public LDTKWorld GetLDTKWorld(string name) => assets.Get<LDTKWorld>(name);
    }
}