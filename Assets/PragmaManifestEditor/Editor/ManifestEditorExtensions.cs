using System;
using System.IO;

namespace Pragma.ManifestEditor
{
    public static class ManifestEditorExtensions
    {
        public static ManifestEditor OpenByName(string name)
        {
            var path = $"Packages/{name}/package.json";
            return OpenByPath(path);
        }
        
        public static ManifestEditor OpenByPath(string path)
        {
            var isDirectory = Directory.Exists(path) && !File.Exists(path);

            if (isDirectory)
            {
                var combinedPath = Path.Combine(path, "package.json");
                if (File.Exists(combinedPath))
                {
                    return new ManifestEditor(combinedPath);
                }
                
                throw new Exception($"Given path is a directory {path}");
            }

            if (!File.Exists(path))
            {
                throw new Exception($"Package not found! {path}");
            }

            return new ManifestEditor(path);
        }

        public static ManifestEditor OpenPackagesManifest()
        {
            var path = "Packages/manifest.json";
            return OpenByPath(path);
        }
    }
}