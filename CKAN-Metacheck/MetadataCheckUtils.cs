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
        public static Workarounds workarounds = new Workarounds();

        public static int CheckMod(string modName, string singleModVersion, ModInfo modInfo, MetadataCache metadataCache)
        {
            CacheInfo ci = new CacheInfo();
            ci.version = modInfo.version;
            ci.state = "fail-install";
            CleanKSP();
            if (!InstallMod(modName, singleModVersion))
            {
                metadataCache.AddCacheData(modName, ci);
                return -1;
            }
            if (!FilesOK(modName, modInfo))
            {
                ci.state = "fail-files";
                metadataCache.AddCacheData(modName, ci);
                return -2;
            }
            ci.state = "ok";
            metadataCache.AddCacheData(modName, ci);
            if (File.Exists("errors/" + modName + ".txt"))
            {
                File.Delete("errors/" + modName + ".txt");
            }
            return 0;
        }

        public static void CleanKSP()
        {
            Console.WriteLine("Running clean!");
            Process process = new Process();
            process.StartInfo.FileName = "/bin/sh";
            process.StartInfo.Arguments = "run.sh clean";
            process.StartInfo.UseShellExecute = false;
            /*
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            */
            process.Start();
            process.WaitForExit();
            Console.WriteLine("Clean done!");
        }

        public static bool InstallMod(string modName, string singleModVersion)
        {
            Console.WriteLine("Running install!");
            Process process = new Process();
            process.StartInfo.FileName = "mono";
            if (singleModVersion == null)
            {
                process.StartInfo.Arguments = "ckan.exe install " + modName + " --headless";
            }
            else
            {
                process.StartInfo.Arguments = "ckan.exe install " + modName + "=" + singleModVersion + " --headless";
            }
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
            //string fileName = CkanUtils.GetCacheName(modName, modInfo);
            string fullName = null; 
            string extractDirectoryName = "temp/extract/";
            string[] dirFiles = Directory.GetFiles("downloads");
            string fileHash = CkanUtils.GetModHash(modName, modInfo);
            foreach (string fileName in dirFiles)
            {
                if (Path.GetFileName(fileName).StartsWith(fileHash))
                {
                    fullName = fileName;
                    break;
                }                    
            }
            if (fullName == null)
            {
                Console.WriteLine("Checking files FAILED, No zip file!");
                return false;
            }
            if (Directory.Exists(extractDirectoryName))
            {
                Directory.Delete(extractDirectoryName, true);
            }
            Directory.CreateDirectory(extractDirectoryName);
            ZipFile.ExtractToDirectory(fullName, extractDirectoryName);
            string gamedataDir = workarounds.customRootFolders.ContainsKey(modName) ? workarounds.customRootFolders[modName] : CkanUtils.GetGameDataDir(fullName, modInfo);
            if (gamedataDir == null)
            {
                Console.WriteLine("GameData Directory was not found!");
                return false;
            }

            bool ok = CheckDirectory(modName, "temp/extract/" + gamedataDir, "/");
            Console.WriteLine("Checking done!");
            return ok;
        }

        private static bool CheckDirectory(string modName, string zipRoot, string zipSource)
        {
            string ckanRoot = "KSP/KSP_linux_current/GameData";
            List<string> errorList = new List<string>();
            //Check files exist
            string[] zipFiles = Directory.GetFiles(zipRoot + zipSource);
            string[] zipFolders = Directory.GetDirectories(zipRoot + zipSource);
            string relCkanPath = zipSource;
            if (workarounds.customFolderRedirects.ContainsKey(modName))
            {
                foreach (KeyValuePair<string,string> kvp in workarounds.customFolderRedirects[modName])
                {
                    if (zipSource.StartsWith(kvp.Key))
                    {
                        relCkanPath = kvp.Value + zipSource.Substring(kvp.Key.Length);
                    }
                }
            }
            string fullCkanPath = ckanRoot + relCkanPath;
            bool ok = true;
            foreach (string zipFile in zipFiles)
            {
                string fileName = Path.GetFileName(zipFile);
                string fullFilePath = fullCkanPath + fileName;
                if (workarounds.customFileRedirects.ContainsKey(modName) && workarounds.customFileRedirects[modName].ContainsKey(fileName))
                {
                    fullFilePath = ckanRoot + "/" + workarounds.customFileRedirects[modName][fileName];
                }
                if (!File.Exists(fullFilePath))
                {
                    if (fileName.ToLower().StartsWith("modulemanager"))
                    {
                        bool skip = false;
                        foreach (string ckanFile in Directory.GetFiles(fullCkanPath))
                        {
                            
                            string ckanFileName = Path.GetFileName(ckanFile);
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
                    if (workarounds.globalIgnoreExtensions.Contains(Path.GetExtension(fileName)))
                    {
                        continue;
                    }
                    if (workarounds.globalIgnoreFiles.Contains(fileName))
                    {
                        continue;
                    }
                    if (workarounds.customIgnoreFiles.ContainsKey(modName) && workarounds.customIgnoreFiles[modName].Contains(fileName))
                    {
                        continue;
                    }
                    errorList.Add("FILE: " + zipSource + fileName + " IS MISSING!");
                    Console.WriteLine("FILE: " + zipSource + fileName + " IS MISSING!");
                    ok = false;
                }
            }
            foreach (string zipFolder in zipFolders)
            {
                string folderName = Path.GetFileName(zipFolder);
                if (Directory.Exists(fullCkanPath + folderName))
                {
                    ok = CheckDirectory(modName, zipRoot, zipSource + folderName + "/") && ok;
                }
                else
                {
                    if (workarounds.globalIgnoreFiles.Contains(folderName))
                    {
                        continue;
                    }
                    if (workarounds.customIgnoreFiles.ContainsKey(modName) && workarounds.customIgnoreFiles[modName].Contains(folderName))
                    {
                        continue;
                    }
                    errorList.Add("FOLDER: " + zipSource + folderName + " IS MISSING!");
                    Console.WriteLine("FOLDER: " + zipSource + folderName + " IS MISSING!");
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
            return ok;
        }
    }
}

