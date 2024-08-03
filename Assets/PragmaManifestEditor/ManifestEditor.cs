using System;
using System.Collections.Generic;
using System.IO;
using Pragma.ManifestEditor.SimpleJSON;

namespace Pragma.ManifestEditor
{
    public class ManifestEditor
    {
        private const string KEY_DEPENDENCY = "dependencies";
        private const string KEY_OPTIONAL_DEPENDENCY = "optionalDependencies";
        private const string URL_DEPENDENCY_PREFIX = "https://";
        
        private readonly string _path;

        private JSONObject _root;

        public ManifestEditor(string path)
        {
            _path = path;
            Reload();
        }

        private string GetDependencyPath(DependencyType dependencyType)
        {
            return dependencyType switch
            {
                DependencyType.Required => KEY_DEPENDENCY,
                DependencyType.Optional => KEY_OPTIONAL_DEPENDENCY,
                _ => throw new ArgumentOutOfRangeException(nameof(dependencyType), dependencyType, null)
            };
        }

        public List<(string, string)> GetDependencies(DependencyType type)
        {
            var path = GetDependencyPath(type);
            var dependencies = new List<(string, string)>();

            if (!_root.HasKey(path))
            {
                return dependencies;
            }
            
            var dependenciesObj = _root[path].AsObject;
            
            foreach (var kvp in dependenciesObj)
            {
                dependencies.Add(new ValueTuple<string, string>(kvp.Key, kvp.Value));
            }

            return dependencies;
        }

        public List<(string, string)>GetUrlDependencies(DependencyType type)
        {
            var dependencies = GetDependencies(type);

            dependencies.RemoveAll(dependency => !dependency.Item2.StartsWith(URL_DEPENDENCY_PREFIX));

            return dependencies;
        }

        public void AddDependency(DependencyType type, ValueTuple<string, string> dependency)
        {
            AddDependency(type, dependency.Item1, dependency.Item2);
        }
        
        public void AddDependency(DependencyType type, string key, string value)
        {
            var path = GetDependencyPath(type);
            
            if (!_root.HasKey(path))
            {
                _root.Add(path, new JSONObject());
            }
            
            var dependencies = _root[path].AsObject;

            if (dependencies.HasKey(key))
            {
                return;
            }
            
            dependencies.Add(key, new JSONString(value));
        }
        
        public void RemoveDependency(DependencyType type, string key)
        {
            var path = GetDependencyPath(type);
            
            if (!_root.HasKey(path))
            {
                return;
            }

            var dependencies = _root[path].AsObject;
            dependencies.Remove(key);
        }
        
        public void Reload()
        {
            var json = File.ReadAllText(_path);
            _root = JSON.Parse(json).AsObject;
        }
        
        public void Save()
        {
            var json = _root.ToString(2);
            File.WriteAllText(_path, json);
        }
    }
}