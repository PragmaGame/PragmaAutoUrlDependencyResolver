using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;

namespace Pragma.ManifestEditor
{
    [InitializeOnLoad]
    public static class AutoResolverOptionalDependency
    {
        static AutoResolverOptionalDependency()
        {
            Events.registeringPackages += OnRegisteringPackage;
            Events.registeredPackages += OnRegisteredPackage;
        }

        private static void OnRegisteringPackage(PackageRegistrationEventArgs args)
        {
            if (args.added.Count == 0)
            {
                return;
            }
            
            var dependenciesPack = new List<string>();

            foreach (var packageInfo in args.added)
            {
                var dependencies = ManifestEditorExtensions.OpenByPath(packageInfo.resolvedPath)
                    .GetUrlDependencies(DependencyType.Optional)
                    .Select(x => x.Item2);
                
                dependenciesPack.AddRange(dependencies);
            }
            
            if (dependenciesPack.Count > 0)
            {
                Client.AddAndRemove(dependenciesPack.ToArray());
            }
        }
        
        private static void OnRegisteredPackage(PackageRegistrationEventArgs args)
        {
        }
    }
}