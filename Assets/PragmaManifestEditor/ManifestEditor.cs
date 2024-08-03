using System;
using System.Collections.Generic;
using System.IO;
using Pragma.ManifestEditor.SimpleJSON;

namespace Pragma.ManifestEditor
{
    public class ManifestEditor
    {
        private const string KEY_DEPENDENCY = "dependencies";
        private const string URL_DEPENDENCY_PREFIX = "https://";
        
        private readonly string _path;

        private JSONObject _root;

        public ManifestEditor(string path)
        {
            _path = path;
            Reload();
        }

        public List<(string, string)> GetDependencies()
        {
            var dependencies = new List<(string, string)>();

            if (!_root.HasKey(KEY_DEPENDENCY))
            {
                return dependencies;
            }
            
            var dependenciesObj = _root[KEY_DEPENDENCY].AsObject;
            
            foreach (var kvp in dependenciesObj)
            {
                dependencies.Add(new ValueTuple<string, string>(kvp.Key, kvp.Value));
            }

            return dependencies;
        }

        public List<(string, string)>GetUrlDependencies()
        {
            var dependencies = GetDependencies();

            dependencies.RemoveAll(dependency => !dependency.Item2.StartsWith(URL_DEPENDENCY_PREFIX));

            return dependencies;
        }

        public void AddDependency(ValueTuple<string, string> dependency)
        {
            AddDependency(dependency.Item1, dependency.Item2);
        }
        
        public void AddDependency(string key, string value)
        {
            if (!_root.HasKey(KEY_DEPENDENCY))
            {
                _root.Add(KEY_DEPENDENCY, new JSONObject());
            }
            
            var dependencies = _root[KEY_DEPENDENCY].AsObject;

            if (dependencies.HasKey(key))
            {
                return;
            }
            
            dependencies.Add(key, new JSONString(value));
        }
        
        public void RemoveDependency(string key)
        {
            if (!_root.HasKey(KEY_DEPENDENCY))
            {
                return;
            }

            var dependencies = _root[KEY_DEPENDENCY].AsObject;
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