#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;

namespace Pragma.AutoUrlDependencyResolver
{
    [InitializeOnLoad]
    public static class AutoUrlDependencyResolver
    {
        static AutoUrlDependencyResolver()
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
                var dependencyEditor = DependencyEditorExtensions.OpenByPath(packageInfo.resolvedPath);
                
                var urlDependencies = dependencyEditor.GetUrlDependencies(DependencyType.Optional)
                    .Concat(dependencyEditor.GetUrlDependencies(DependencyType.Required))
                    .Select(x => x.Item2)
                    .Where(url => !string.IsNullOrEmpty(url));

                dependenciesPack.AddRange(urlDependencies);
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

#endif