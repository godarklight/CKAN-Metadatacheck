using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json.Linq;

namespace CKANMetacheck
{
    public class MainClass
    {
        public static void Main(string[] arguments)
        {
            string KSP_VERSION = arguments[0];
            bool singleModMode = false;
            string singleModName = null;
            if (arguments.Length >= 2)
            {
                singleModMode = true;
                singleModName = arguments[1];
            }
            if (!Directory.Exists("errors"))
            {
                Directory.CreateDirectory("errors");
            }
            JToken registry = CkanUtils.GetRegistry();
            MetadataCache metadataCache = new MetadataCache("cache.json");
            Dictionary<string, CacheInfo> cacheInfo = metadataCache.GetCacheData();
            Dictionary<string, ModInfo> modInfo = CkanUtils.GetModInfo(KSP_VERSION, registry);
            int skipCount = 0;
            foreach (KeyValuePair<string,ModInfo> kvp in modInfo)
            {
                string modName = kvp.Key;
                if (singleModMode && singleModName != modName)
                {
                    continue;
                }
                bool usesKS = CkanUtils.UsesKerbalStuff(modName, registry);
                if (usesKS)
                {
                    Console.WriteLine("Skipping " + modName + " as it uses kerbalstuff");
                    continue;
                }
                bool checkMod = false;
                if (!cacheInfo.ContainsKey(kvp.Key))
                {
                    Console.WriteLine("Processing new mod: " + modName);
                    checkMod = true;
                }
                else
                {
                    if (kvp.Value.version != cacheInfo[modName].version)
                    {
                        Console.WriteLine("Processing updated mod: " + modName);
                        checkMod = true;
                    }
                    else
                    {
                        //if (cacheInfo[modName].state != "ok")
                        if (cacheInfo[modName].state == "fail-files")
                        {
                            Console.WriteLine("Processing failed mod: " + modName);
                            checkMod = true;
                        }
                    }
                }
                if (checkMod)
                {
                    MetadataCheckUtils.CheckMod(modName, kvp.Value, metadataCache);
                }
                else
                {
                    skipCount++;
                }
           }
            Console.WriteLine("Skipped processing of " + skipCount + " mods in cache");
        }
    }
}

