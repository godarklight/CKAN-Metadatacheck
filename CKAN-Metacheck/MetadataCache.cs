using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CKANMetacheck
{
    public class MetadataCache
    {
        private string fileName;
        private JToken rootNode;

        public MetadataCache(string fileName)
        {
            this.fileName = fileName;
            if (!File.Exists(fileName))
            {
                JObject jobj = new JObject(new JProperty("mods", new JObject()));
                File.WriteAllText(fileName, jobj.ToString());
            }
            rootNode = JObject.Parse(File.ReadAllText(fileName));
        }

        public void Save()
        {
            File.WriteAllText(fileName, rootNode.ToString());
        }

        public Dictionary<string, CacheInfo> GetCacheData()
        {
            Dictionary<string,CacheInfo> cacheData = new Dictionary<string, CacheInfo>();
            foreach (JProperty mod in ((JObject)rootNode["mods"]).Properties())
            {
                CacheInfo ci = new CacheInfo();
                ci.version = (string)mod.Value["version"];
                ci.state = (string)mod.Value["state"];
                cacheData.Add(mod.Name, ci);
            }
            return cacheData;
        }

        public void AddCacheData(string modName, CacheInfo cacheData)
        {
            JProperty stateProperty = new JProperty("state", cacheData.state);
            JProperty versionProperty = new JProperty("version", cacheData.version);
            JObject cacheDataObject = new JObject(stateProperty, versionProperty);
            if (rootNode["mods"][modName] == null)
            {
                ((JObject)rootNode["mods"]).Add(new JProperty(modName, cacheDataObject));
            }
            else
            {
                JObject modObject = (JObject)rootNode["mods"];
                JProperty modProperty = modObject.Property(modName);
                modProperty.Value = cacheDataObject;
            }
            Save();
        }
    }
}

