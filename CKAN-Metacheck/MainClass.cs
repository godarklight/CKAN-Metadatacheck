using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json.Linq;

namespace CKANMetacheck
{
    public class MainClass
    {
        //Return conditions:
        //0 - OK
        //1 - Failed install
        //2 - Failed files
        //3 - Uses kerbalstuff
        //4 - Failed to check single mod

        //Arguments program.exe ksp_version (single_mod_to_test)
        public static int Main(string[] arguments)
        {
            string KSP_VERSION = arguments[0];
            bool singleModMode = false;
            string singleModName = null;
            string singleModVersion = null;
            if (arguments.Length >= 2)
            {
                singleModMode = true;
                singleModName = arguments[1];
            }
            if (arguments.Length >= 3)
            {
                singleModVersion = arguments[2];
            }
            if (!Directory.Exists("errors"))
            {
                Directory.CreateDirectory("errors");
            }
            JToken registry = CkanUtils.GetRegistry();
            MetadataCache metadataCache = new MetadataCache("cache.json");
            Dictionary<string, CacheInfo> cacheInfo = null;
            Dictionary<string, ModInfo> modInfo = null;
            if (singleModMode)
            {
                modInfo = CkanUtils.GetSingleModInfo(KSP_VERSION, registry, singleModName, singleModVersion);
            }
            else
            {
                cacheInfo = metadataCache.GetCacheData();
                modInfo = CkanUtils.GetModInfo(KSP_VERSION, registry);
            }
             
            int skipCount = 0;
            if (!singleModMode)
            {
                foreach (KeyValuePair<string,ModInfo> kvp in modInfo)
                {
                    string modName = kvp.Key;
                    bool usesKS = CkanUtils.UsesKerbalStuff(modName, null, registry, false);
                    if (usesKS)
                    {
                        Console.WriteLine("Skipping " + modName + " as it uses kerbalstuff");
                        continue;
                    }
                    //Always check if we are in single mod mode.
                    bool checkMod = singleModMode;
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
                            //if (cacheInfo[modName].state == "fail-files")
                            if (cacheInfo[modName].state != "ok")
                            {
                                Console.WriteLine("Processing failed mod: " + modName);
                                checkMod = true;
                            }
                        }
                    }
                    if (checkMod)
                    {
                        MetadataCheckUtils.CheckMod(modName, null, kvp.Value, metadataCache);
                    }
                    else
                    {
                        skipCount++;
                    }
                } 
            }
            else
            {
                if (modInfo.ContainsKey(singleModName))
                {
                    bool usesKS = CkanUtils.UsesKerbalStuff(singleModName, singleModVersion, registry, true);
                    if (!usesKS)
                    {
                        return MetadataCheckUtils.CheckMod(singleModName, singleModVersion, modInfo[singleModName], metadataCache);
                    }
                    else
                    {
                        Console.WriteLine("Mod uses kerbalstuff!");
                        return -3;
                    }
                }
                else
                {
                    Console.WriteLine("Failed to find " + singleModName);
                    return -4;
                }
            }
            if (!singleModMode)
            {
                Console.WriteLine("Skipped processing of " + skipCount + " mods in cache");
            }
            return 0;
        }
    }
}

