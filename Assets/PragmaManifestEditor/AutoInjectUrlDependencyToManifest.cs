using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Pragma.ManifestEditor
{
    [InitializeOnLoad]
    public static class AutoInjectUrlDependencyToManifest
    {
        private const string PATH = "Packages/manifest.json";

        private static ManifestEditor _rootManifest;
        
        // [UnityEditor.Callbacks.DidReloadScripts]
        // private static void OnScriptsReloaded()
        // {
        //     
        // }

        static AutoInjectUrlDependencyToManifest()
        {
            _rootManifest = new ManifestEditor(PATH);

            AssetDatabase.importPackageStarted += OnImportPackageStarted;
            AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
            AssetDatabase.importPackageFailed += OnImportPackageFailed;
            AssetDatabase.importPackageCancelled += OnImportPackageCancelled;
            
            Debug.Log("Initialize AutoInjectUrlDependencyToManifest");
        }
        
        private static void OnImportPackageCancelled(string packageName)
        {
            Debug.Log($"Cancelled the import of package: {packageName}");
            
            _rootManifest.Reload();
        }

        private static void OnImportPackageCompleted(string packagename)
        {
            Debug.Log($"Imported package: {packagename}");
            
            _rootManifest.Save();
        }

        private static void OnImportPackageFailed(string packagename, string errormessage)
        {
            Debug.Log($"Failed importing package: {packagename} with error: {errormessage}");
            
            _rootManifest.Reload();
        }

        private static void OnImportPackageStarted(string packagename)
        {
            Debug.Log($"Started importing package: {packagename}");

            var packageManifest = ManifestEditorExtensions.OpenByName(packagename);
            var packageDependencies = packageManifest.GetUrlDependencies();

            foreach (var dependency in packageDependencies)
            {
                _rootManifest.AddDependency(dependency);
            }
        }
    }
}