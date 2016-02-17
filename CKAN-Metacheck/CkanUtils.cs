using System;
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
                JProperty versionProperty = (JProperty)mod.Value["module_version"].First;
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
                bool versionOk = (ksp_version == KSP_VERSION) || VersionCompatible(KSP_VERSION, ksp_version_min, ksp_version_max);
                if (versionOk)
                {
                    modInfo.Add(currentModName, mi);
                }
            }
            return modInfo;
        }

        public static string GetGameDataDir(string modName, ModInfo modInfo)
        {
            string[] dirs = Directory.GetDirectories("temp/extract", "GameData", SearchOption.AllDirectories);
            if (dirs.Length == 1 && dirs[0].EndsWith("GameData"))
            {
                string trimmed = dirs[0].Substring("temp/extract/".Length);
                return trimmed;
            }

            foreach (KeyValuePair<string,string> kvp in modInfo.installs)
            {
                string file = kvp.Key;
                string installs_to = kvp.Value;
                //Console.WriteLine(file + " :  " + installs_to);
                if (installs_to.StartsWith("GameData"))
                {
                    int countUp = 1;
                    foreach (char c in installs_to)
                    {
                        if (c == '/')
                        {
                            countUp++;
                        }
                    }
                    if (String.IsNullOrEmpty(file))
                    {
                        if (countUp == 1)
                        {
                            return "";
                        }
                        else
                        {
                            return null;
                        }
                    }
                    string gameDataDir = file;
                    for (int i = 0; i < countUp; i++)
                    {
                        if (gameDataDir == null || !gameDataDir.Contains("/"))
                        {
                            return null;
                        }
                        gameDataDir = gameDataDir.Substring(0, gameDataDir.LastIndexOf('/'));
                    }
                    return gameDataDir;
                }
            }
            return null;
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

        public static string GetCacheName(string modName, ModInfo modInfo)
        {
            string url = Uri.UnescapeDataString(modInfo.downloadUrl);
            string hashString = null;
            using (var sha1 = new SHA1Managed())
            {
                byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(url));
                hashString = BitConverter.ToString(hash).Replace("-", "").Substring(0, 8);
            }
            string version = Regex.Replace(modInfo.version, "[^A-Za-z0-9_.-]", "-");
            return hashString + "-" + modName + "-" + version + ".zip";
        }

        public static bool UsesKerbalStuff(string modName, JToken registry)
        {
            if (registry["available_modules"][modName] == null)
            {
                return false;
            }
            JObject mod = (JObject)registry["available_modules"][modName].First.First.First.First;
            if (((string)mod["download"]).Contains("kerbalstuff.com"))
            {
                return true;
            }
            if (mod["depends"].Type != JTokenType.Null)
            {
                JArray deps = (JArray)mod["depends"];
                foreach (JObject depObject in deps)
                {
                    if (UsesKerbalStuff((string)depObject["name"], registry))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}

