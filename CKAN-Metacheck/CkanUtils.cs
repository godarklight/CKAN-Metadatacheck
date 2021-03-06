﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CKANMetacheck
{
    public static class CkanUtils
    {
        private static int REGISTRY_VERSION = 3;

        public static JObject GetRegistry()
        {
            string registryData = File.ReadAllText("temp/registry.json");
            JObject token = JObject.Parse(registryData);
            int version = Int32.Parse(token.SelectToken("registry_version").ToString());
            if (version != REGISTRY_VERSION)
            {
                throw new InvalidDataException("Unsupported registry format, expected: " + REGISTRY_VERSION + ", got: " + version);
            }
            return token;
        }

        public static Dictionary<string, ModInfo> GetModInfo(string KSP_VERSION, JToken registry)
        {
            Dictionary<string, ModInfo> modInfo = new Dictionary<string, ModInfo>();
            foreach (JProperty mod in registry["available_modules"])
            {
                string currentModName = mod.Name;
                bool versionOk = false;

                foreach (JProperty versionProperty in mod.Value["module_version"])
                {
                    JObject versionKeys = (JObject)versionProperty.Value;
                    ModInfo mi = new ModInfo();
                    mi.version = (string)versionKeys["version"];
                    mi.downloadUrl = (string)versionKeys["download"];
                    JToken installs = versionKeys["install"];
                    if (installs.Type != JTokenType.Null)
                    {
                        foreach (JObject jobj in (JArray)installs)
                        {
                            string file = (string)jobj["file"];
                            string installs_to = (string)jobj["install_to"];
                            mi.installs.Add(new KeyValuePair<string,string>(file, installs_to));
                        }
                    }
                    string ksp_version = (string)versionKeys["ksp_version"];
                    string ksp_version_min = (string)versionKeys["ksp_version"];
                    string ksp_version_max = (string)versionKeys["ksp_version"];
                    versionOk = (ksp_version == KSP_VERSION) || VersionCompatible(KSP_VERSION, ksp_version_min, ksp_version_max);
                    if (versionOk)
                    {
                        modInfo.Add(currentModName, mi);
                        break;
                    }
                }
                if (versionOk)
                {
                    continue;
                }
            }
            return modInfo;
        }

        public static Dictionary<string, ModInfo> GetSingleModInfo(string KSP_VERSION, JToken registry, string modName, string modVersion)
        {
            Dictionary<string, ModInfo> modInfo = new Dictionary<string, ModInfo>();
            JObject mod = (JObject)registry["available_modules"][modName];
            if (mod == null)
            {
                return modInfo;
            }
            bool versionOk = false;
            foreach (JProperty versionProperty in mod["module_version"])
            {
                JObject versionKeys = (JObject)versionProperty.Value;
                ModInfo mi = new ModInfo();
                mi.version = (string)versionKeys["version"];
                mi.downloadUrl = (string)versionKeys["download"];
                JToken installs = versionKeys["install"];
                if (installs.Type != JTokenType.Null)
                {
                    foreach (JObject jobj in (JArray)installs)
                    {
                        string file = (string)jobj["file"];
                        string installs_to = (string)jobj["install_to"];
                        mi.installs.Add(new KeyValuePair<string,string>(file, installs_to));
                    }
                }
                string ksp_version = (string)versionKeys["ksp_version"];
                string ksp_version_min = (string)versionKeys["ksp_version"];
                string ksp_version_max = (string)versionKeys["ksp_version"];
                versionOk = (ksp_version == KSP_VERSION) || VersionCompatible(KSP_VERSION, ksp_version_min, ksp_version_max);
                if (versionOk && (modVersion == null || mi.version == modVersion))
                {
                    modInfo.Add(modName, mi);
                    return modInfo;
                }
            }
            return modInfo;
        }

        public static string GetGameDataDir(string modName, ModInfo modInfo)
        {
            string[] dirs = Directory.GetDirectories("temp/extract", "*", SearchOption.AllDirectories);
            foreach (string dirName in dirs)
            {
                if (Path.GetFileName(dirName).ToLower() == "gamedata")
                {
                    return dirName.Substring("temp/extract/".Length);
                }
            }
            return "";
        }

        public static bool VersionCompatible(string version, string min, string max)
        {
            if (String.IsNullOrEmpty(version) || String.IsNullOrEmpty(min) || String.IsNullOrEmpty(max))
            {
                return false;
            }
            return VersionLessThanOrEqualTo(min, version) && VersionLessThanOrEqualTo(version, max);
        }

        public static bool VersionLessThanOrEqualTo(string lhs, string rhs)
        {
            string[] lhsSplit = lhs.Split('.');
            string[] rhsSplit = rhs.Split('.');
            int[] lhsInt = new int[3];
            int[] rhsInt = new int[3];
            for (int i = 0; i < lhsSplit.Length && i < 3; i++)
            {
                lhsInt[i] = Int32.Parse(lhsSplit[i]);
            }
            for (int i = 0; i < rhsSplit.Length && i < 3; i++)
            {
                rhsInt[i] = Int32.Parse(rhsSplit[i]);
            }
            for (int i = 0; i < 3; i++)
            {
                if (lhsInt[i] < rhsInt[i])
                {
                    //Version is smaller
                    return true;
                }
                if (lhsInt[i] > rhsInt[i])
                {
                    //Version is bigger
                    return false;
                }
            }
            //Versions are the same
            return true;
        }

        public static string GetModHash(string modName, ModInfo modInfo)
        {
            string url = Uri.UnescapeDataString(modInfo.downloadUrl);
            //Differences?
            url = url.Replace("&", "%26");
            using (var sha1 = new SHA1Managed())
            {
                byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(url));
                string retVal = BitConverter.ToString(hash).Replace("-", "").Substring(0, 8);
                return retVal;
            }
        }

        public static bool UsesKerbalStuff(string modName, string modVersion, JToken registry, bool printKerbalstuffErrors, int depth)
        {
            if (depth == 0)
            {
                Console.WriteLine("Max depth limit reached for " + modName);
                //Defend against the infinite!
                return false;
            }
            if (registry["available_modules"][modName] == null)
            {
                return false;
            }
            JObject mod = (JObject)registry["available_modules"][modName].First.First.First.First;
            if (modVersion != null)
            {
                JObject exactVersion = (JObject)((JObject)registry["available_modules"][modName]["module_version"]).GetValue(modVersion);
                if (exactVersion != null)
                {
                    Console.WriteLine("Found version " + modVersion + " for " + modName);
                    mod = exactVersion;
                }
            }
            if (((string)mod["download"]).Contains("kerbalstuff.com"))
            {
                Console.WriteLine(modName + " uses kerbalstuff!");
                return true;
            }
            if (mod["depends"].Type != JTokenType.Null)
            {
                JArray deps = (JArray)mod["depends"];
                foreach (JObject depObject in deps)
                {
                    if (UsesKerbalStuff((string)depObject["name"], null, registry, printKerbalstuffErrors, depth - 1))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}

