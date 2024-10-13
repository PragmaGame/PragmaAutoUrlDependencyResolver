using System;
using System.IO;

namespace Pragma.AutoUrlDependencyResolver
{
    public static class DependencyEditorExtensions
    {
        public static DependencyEditor OpenByName(string name)
        {
            var path = $"Packages/{name}/package.json";
            return OpenByPath(path);
        }
        
        public static DependencyEditor OpenByPath(string path)
        {
            var isDirectory = Directory.Exists(path) && !File.Exists(path);

            if (isDirectory)
            {
                var combinedPath = Path.Combine(path, "package.json");
                if (File.Exists(combinedPath))
                {
                    return new DependencyEditor(combinedPath);
                }
                
                throw new Exception($"Given path is a directory {path}");
            }

            if (!File.Exists(path))
            {
                throw new Exception($"Package not found! {path}");
            }

            return new DependencyEditor(path);
        }

        public static DependencyEditor OpenPackagesManifest()
        {
            var path = "Packages/manifest.json";
            return OpenByPath(path);
        }
    }
}