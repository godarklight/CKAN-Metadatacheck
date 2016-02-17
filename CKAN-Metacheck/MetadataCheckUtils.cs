using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace CKANMetacheck
{
    public static class MetadataCheckUtils
    {
        public static void CheckMod(string modName, ModInfo modInfo, MetadataCache metadataCache)
        {
            CacheInfo ci = new CacheInfo();
            ci.version = modInfo.version;
            ci.state = "fail-install";
            CleanKSP();
            if (!InstallMod(modName))
            {
                metadataCache.AddCacheData(modName, ci);
                return;
            }
            if (!FilesOK(modName, modInfo))
            {
                ci.state = "fail-files";
                metadataCache.AddCacheData(modName, ci);
                return;
            }
            ci.state = "ok";
            metadataCache.AddCacheData(modName, ci);
            if (File.Exists("errors/" + modName + ".txt"))
            {
                File.Delete("errors/" + modName + ".txt");
            }
        }

        public static void CleanKSP()
        {
            Console.WriteLine("Running clean!");
            Process process = new Process();
            process.StartInfo.FileName = "/bin/sh";
            process.StartInfo.Arguments = "run.sh clean";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            process.WaitForExit();
            Console.WriteLine("Clean done!");
        }

        public static bool InstallMod(string modName)
        {
            Console.WriteLine("Running install!");
            Process process = new Process();
            process.StartInfo.FileName = "mono";
            process.StartInfo.Arguments = "ckan.exe install " + modName + " --headless";
            process.StartInfo.UseShellExecute = false; 
            /*
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            */
            process.Start();
            process.WaitForExit();
            if (process.ExitCode == 0)
            {
                Console.WriteLine("Install done!");    
            }
            else
            {
                Console.WriteLine("Install failed!");
            }
            return process.ExitCode == 0;
        }

        public static bool FilesOK(string modName, ModInfo modInfo)
        {
            Console.WriteLine("Checking files!");
            string fileName = CkanUtils.GetCacheName(modName, modInfo);
            string fullName = "downloads/" + fileName; 
            string extractDirectoryName = "temp/extract/";
            if (!File.Exists(fullName))
            {
                Console.WriteLine("Checking files FAILED!");
                return false;
            }
            if (Directory.Exists(extractDirectoryName))
            {
                Directory.Delete(extractDirectoryName, true);
            }
            Directory.CreateDirectory(extractDirectoryName);
            ZipFile.ExtractToDirectory(fullName, extractDirectoryName);
            string gamedataDir = CkanUtils.GetGameDataDir(fullName, modInfo);
            if (gamedataDir == null)
            {
                Console.WriteLine("GameData Directory was not found!");
                return false;
            }
            //Ignore files
            HashSet<string> ignoreFiles = new HashSet<string>();
            ignoreFiles.Add("LICENCE");
            ignoreFiles.Add("LICENSE");
            ignoreFiles.Add("Thumbs.db");
            ignoreFiles.Add(".DS_Store");
            ignoreFiles.Add("Source");
            ignoreFiles.Add("PluginData");
            ignoreFiles.Add("Ships");
            HashSet<string> ignoreExtensions = new HashSet<string>();
            ignoreExtensions.Add("md");
            ignoreExtensions.Add("txt");
            ignoreExtensions.Add("cs");
            bool ok = CheckDirectory(modName, extractDirectoryName + gamedataDir, "KSP/KSP_linux_current/GameData", ignoreFiles, ignoreExtensions);
            Console.WriteLine("Checking done!");
            return ok;
        }

        private static bool CheckDirectory(string modName, string zipSource, string ckanSource, HashSet<string> ignoreFiles, HashSet<string> ignoreExtensions)
        {
            List<string> errorList = new List<string>();
            //Check files exist
            string[] zipFiles = Directory.GetFiles(zipSource);
            string[] zipFolders = Directory.GetDirectories(zipSource);
            bool ok = true;
            foreach (string zipFile in zipFiles)
            {
                string fileName = zipFile.Substring(zipFile.LastIndexOf("/") + 1);
                if (!File.Exists(ckanSource + "/" + fileName))
                {
                    if (fileName.ToLower().StartsWith("modulemanager"))
                    {
                        bool skip = false;
                        foreach (string ckanFile in Directory.GetFiles(ckanSource))
                        {
                            
                            string ckanFileName = ckanFile.Substring(ckanFile.LastIndexOf("/") + 1);
                            if (ckanFileName.ToLower().StartsWith("modulemanager"))
                            {
                                skip = true;
                                continue;
                            }
                        }

                        if (skip)
                        {
                            continue;
                        }
                    }
                    if (ignoreExtensions.Contains(fileName.Substring(fileName.LastIndexOf(".") + 1)))
                    {
                        continue;
                    }
                    if (ignoreFiles.Contains(fileName))
                    {
                        continue;
                    }
                    errorList.Add("FILE: " + fileName + " IS MISSING!");
                    Console.WriteLine("FILE: " + fileName + " IS MISSING!");
                    ok = false;
                }
            }
            foreach (string zipFolder in zipFolders)
            {
                string folderName = zipFolder.Substring(zipFolder.LastIndexOf("/") + 1);
                if (Directory.Exists(ckanSource + "/" + folderName))
                {
                    ok = CheckDirectory(modName, zipSource + "/" + folderName, ckanSource + "/" + folderName, ignoreFiles, ignoreExtensions) && ok;
                }
                else
                {
                    if (ignoreFiles.Contains(folderName))
                    {
                        continue;
                    }
                    errorList.Add("FOLDER: " + folderName + " IS MISSING!");
                    Console.WriteLine("FOLDER: " + folderName + " IS MISSING!");
                    ok = false;
                }
            }
            if (errorList.Count != 0)
            {
                string errorName = "errors/" + modName + ".txt";
                if (File.Exists(errorName))
                {
                    File.Delete(errorName);
                }
                StringBuilder sb = new StringBuilder();

                foreach (string error in errorList)
                {
                    sb.AppendLine(error);
                }
                File.WriteAllText(errorName, sb.ToString());
            }
            /*
            Directory.GetDirectories(source);
            Console.WriteLine("Checking ")
            */
            return ok;
        }
    }
}

