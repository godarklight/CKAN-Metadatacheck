using System;
using System.Collections.Generic;

namespace CKANMetacheck
{
    public class Workarounds
    {
        public Dictionary<string, string> customRootFolders = GetCustomRootFolders();
        public Dictionary<string, HashSet<string>> customIgnoreFiles = GetCustomIgnoreFiles();
        public Dictionary<string, Dictionary<string,string>> customFolderRedirects = GetCustomFolderRedirects();
        public Dictionary<string, Dictionary<string,string>> customFileRedirects = GetCustomFileRedirects();
        public HashSet<string> globalIgnoreFiles = GetGlobalIgnoreFiles();
        public HashSet<string> globalIgnoreExtensions = GetGlobalIgnoreExtensions();
        public HashSet<string> skipMods = GetSkipMods();

        private static Dictionary<string, string> GetCustomRootFolders()
        {
            Dictionary<string, string> customRootFolders = new Dictionary<string, string>();
            customRootFolders.Add("BetterScienceLabsContinued", "BetterScienceLabsContinued");
            customRootFolders.Add("EBRCPartspack", "EBRC-PACKS_PACK-1");
            customRootFolders.Add("EF2000EuroFighter", "EF2000_Euro_Fighter-4.0");
            customRootFolders.Add("ExtrasolarPlanetsBeyondKerbol-RealisticScale", "RealisticScale/GameData/Extrasolar");
            customRootFolders.Add("KerbalConfigEditor", "Kerbal Config Editor");
            customRootFolders.Add("MasterTechAerospace", "Master Tech Aerospace V.3.6/Game Data");
            customRootFolders.Add("SETI-RemoteTech", "SETI-RemoteTech-1.0.0");
            customRootFolders.Add("ContractConfigurator-HistoricMissions", "HistoricMissions/RegularVersion/Full Pack/ContractPacks");
            customRootFolders.Add("SuperHeavyBoostersHistoricalNexus", "Nexus1.2.2/Nexus");
            customRootFolders.Add("TDIndustriesRCSandHypergolicengines", "RCS");
            customRootFolders.Add("WalkAbout", "v1.2");
            customRootFolders.Add("samv-client", "stand-alone-map-view/samv_client");
            customRootFolders.Add("samv-server", "stand-alone-map-view/samv_server");
            return customRootFolders;
        }

        private static Dictionary<string, HashSet<string>> GetCustomIgnoreFiles()
        {
            Dictionary<string, HashSet<string>> customIgnoreFiles = new Dictionary<string, HashSet<string>>();
            HashSet<string> AtmosphereAutoPilot = new HashSet<string>();
            AtmosphereAutoPilot.Add("LICENSE_AtmosphereAutopilot");
            AtmosphereAutoPilot.Add("LICENSE_ModuleManager");
            customIgnoreFiles.Add("AtmosphereAutopilot", AtmosphereAutoPilot);
            HashSet<string> ContractConfigurator = new HashSet<string>();
            ContractConfigurator.Add("ContractPacks");
            customIgnoreFiles.Add("ContractConfigurator", ContractConfigurator);
            HashSet<string> Firespitter = new HashSet<string>();
            Firespitter.Add("Old Example Craft.zip");
            customIgnoreFiles.Add("Firespitter", Firespitter);
            HashSet<string> FogOfTech = new HashSet<string>();
            FogOfTech.Add("src.zip");
            customIgnoreFiles.Add("FogOfTech", FogOfTech);
            HashSet<string> FuelSwitchtoeverytank = new HashSet<string>();
            FuelSwitchtoeverytank.Add("FSfuelSwitchForEveryTankThermalPaint.cfg");
            customIgnoreFiles.Add("FuelSwitchtoeverytank-default", FuelSwitchtoeverytank);
            HashSet<string> HermesInterplanetarySpacecraft = new HashSet<string>();
            HermesInterplanetarySpacecraft.Add("Hermes.craft");
            customIgnoreFiles.Add("HermesInterplanetarySpacecraft", HermesInterplanetarySpacecraft);
            HashSet<string> KduffersMultiwheels = new HashSet<string>();
            KduffersMultiwheels.Add("kdufferskorp");
            customIgnoreFiles.Add("KduffersMultiwheels", KduffersMultiwheels);
            HashSet<string> Pathfinder = new HashSet<string>();
            Pathfinder.Add("Buffalo");
            Pathfinder.Add("CommunityResourcePack");
            customIgnoreFiles.Add("Pathfinder", Pathfinder);
            HashSet<string> HistoricMissions = new HashSet<string>();
            HistoricMissions.Add("EXAMPLECRAFT");
            customIgnoreFiles.Add("ContractConfigurator-HistoricMissions", HistoricMissions);
            HashSet<string> SpiceLaunchSystem = new HashSet<string>();
            SpiceLaunchSystem.Add("crafts");
            customIgnoreFiles.Add("SpiceLaunchSystem", SpiceLaunchSystem);
            HashSet<string> StrykersAerospaceandIVAs = new HashSet<string>();
            StrykersAerospaceandIVAs.Add("StrykerArmory requires BDARMORY!!");
            customIgnoreFiles.Add("StrykersAerospaceandIVAs", StrykersAerospaceandIVAs);
            return customIgnoreFiles;
        }

        private static Dictionary<string, Dictionary<string,string>> GetCustomFolderRedirects()
        {
            Dictionary<string, Dictionary<string,string>> customRedirects = new Dictionary<string, Dictionary<string,string>>();
            Dictionary<string,string> SkunkWorks = new Dictionary<string, string>();
            SkunkWorks.Add("/", "/Skunk_Works/");
            customRedirects.Add("A7Corsair2", SkunkWorks);
            Dictionary<string,string> kdufferskorp = new Dictionary<string, string>();
            kdufferskorp.Add("/", "/kdufferskorp/");
            customRedirects.Add("BalastanksExtended", kdufferskorp);
            Dictionary<string,string> ChenowthMPPV = new Dictionary<string, string>();
            ChenowthMPPV.Add("/", "/Skunk_Works/ChenowthMPPV/");
            customRedirects.Add("ChenowthMPPV", ChenowthMPPV);
            Dictionary<string,string> CoherentContracts = new Dictionary<string, string>();
            CoherentContracts.Add("/", "/CoherentContracts/");
            customRedirects.Add("CoherentContracts", CoherentContracts);
            Dictionary<string,string> ContractConfiguratorAdvancedProgression = new Dictionary<string, string>();
            ContractConfiguratorAdvancedProgression.Add("/", "/ContractPacks/");
            customRedirects.Add("ContractConfigurator-AdvancedProgression", ContractConfiguratorAdvancedProgression);
            customRedirects.Add("EF2000EuroFighter", SkunkWorks);
            customRedirects.Add("F4E1PhantomInterceptor", SkunkWorks);
            Dictionary<string,string> InfernalRoboticsSequencer = new Dictionary<string, string>();
            InfernalRoboticsSequencer.Add("/", "/MagicSmokeIndustries/");
            customRedirects.Add("InfernalRobotics-Sequencer", InfernalRoboticsSequencer);    
            Dictionary<string,string> KduffersKorpHotrodWheels = new Dictionary<string, string>();
            KduffersKorpHotrodWheels.Add("/", "/Kduffers/");
            customRedirects.Add("KduffersKorpHotrodWheels", KduffersKorpHotrodWheels);
            Dictionary<string,string> KduffersMultiwheels = new Dictionary<string, string>();
            KduffersMultiwheels.Add("/", "/kdufferskorp/");
            customRedirects.Add("KduffersMultiwheels", KduffersMultiwheels);
            Dictionary<string,string> KerbalConfigEditor = new Dictionary<string, string>();
            KerbalConfigEditor.Add("/", "/../");
            customRedirects.Add("KerbalConfigEditor", KerbalConfigEditor);
            customRedirects.Add("MIG29Fulcrum", SkunkWorks);
            customRedirects.Add("covenantcapitalship", kdufferskorp);
            customRedirects.Add("Outboardmotor", kdufferskorp);
            Dictionary<string,string> Pathfinder = new Dictionary<string, string>();
            Pathfinder.Add("/WildBlueIndustries/CommunityResourcePack/", "/CommunityResourcePack/");
            customRedirects.Add("Pathfinder", Pathfinder);
            Dictionary<string,string> HistoricMissions = new Dictionary<string, string>();
            HistoricMissions.Add("/", "/ContractPacks/");
            customRedirects.Add("ContractConfigurator-HistoricMissions", HistoricMissions);
            Dictionary<string,string> StockRealismRebalances = new Dictionary<string, string>();
            StockRealismRebalances.Add("/", "/SRR/");
            customRedirects.Add("StockRealismRebalances", StockRealismRebalances);
            customRedirects.Add("SukhoiAircraftSU25FrogFoot", SkunkWorks);
            Dictionary<string,string> SuperHeavyBoostersHistoricalNexus = new Dictionary<string, string>();
            SuperHeavyBoostersHistoricalNexus.Add("/", "/Nexus/");
            customRedirects.Add("SuperHeavyBoostersHistoricalNexus", SuperHeavyBoostersHistoricalNexus);
            customRedirects.Add("UNSCPillarofAutumn", kdufferskorp);
            customRedirects.Add("heatshieldtile", kdufferskorp);
            customRedirects.Add("kspboatparts", kdufferskorp);
            Dictionary<string,string> samvclient = new Dictionary<string, string>();
            samvclient.Add("/", "/samv_client/");
            customRedirects.Add("samv-client", samvclient);
            Dictionary<string,string> samvserver = new Dictionary<string, string>();
            samvserver.Add("/", "/samv_server/");
            customRedirects.Add("samv-server", samvserver);
            return customRedirects;
        }

        private static Dictionary<string, Dictionary<string,string>> GetCustomFileRedirects()
        {
            Dictionary<string, Dictionary<string,string>> customRedirects = new Dictionary<string, Dictionary<string,string>>();
            Dictionary<string,string> EVAManager = new Dictionary<string, string>();
            EVAManager.Add("EVAManager.dll", "EVAManager/EVAManager.dll");
            customRedirects.Add("AntennaRange", EVAManager);
            customRedirects.Add("BDArmoryFPS", EVAManager);
            customRedirects.Add("TweakableEverything", EVAManager);
            return customRedirects;
        }

        private static HashSet<string> GetGlobalIgnoreFiles()
        {
            HashSet<string> ignoreFiles = new HashSet<string>();
            ignoreFiles.Add("LICENCE");
            ignoreFiles.Add("LICENSE");
            ignoreFiles.Add("License");
            ignoreFiles.Add("Thumbs.db");
            ignoreFiles.Add(".DS_Store");
            ignoreFiles.Add("Source");
            ignoreFiles.Add("source");
            ignoreFiles.Add("PluginData");
            ignoreFiles.Add("Ships");
            ignoreFiles.Add("MiniAVC.xml");
            ignoreFiles.Add("__MACOSX");
            ignoreFiles.Add("Thumbs.db:encryptable");
            ignoreFiles.Add("README.htm");
            ignoreFiles.Add("._.DS_Store");                
            ignoreFiles.Add("license");
            return ignoreFiles;
        }

        private static HashSet<string> GetGlobalIgnoreExtensions()
        {
            HashSet<string> ignoreExtensions = new HashSet<string>();
            ignoreExtensions.Add(".md");
            ignoreExtensions.Add(".txt");
            ignoreExtensions.Add(".cs");
            ignoreExtensions.Add(".pdf");
            ignoreExtensions.Add(".rtf");
            ignoreExtensions.Add(".mdb");
            return ignoreExtensions;
        }

        private static HashSet<string> GetSkipMods()
        {
            HashSet<string> skipMods = new HashSet<string>();
            /*
            skipMods.Add("");
            */
            /*
            skipMods.Add("AlternisRealSolarSystem");
            skipMods.Add("AntennaRangePatch4ORIGAMEAntenna");
            skipMods.Add("Asclepius-clouds");
            skipMods.Add("Asclepius");
            skipMods.Add("AutoRove");
            skipMods.Add("AdjustableLandingGear");
            skipMods.Add("B9-props");
            skipMods.Add("B9");
            skipMods.Add("BDArmoryFPS");
            skipMods.Add("CactEyeTelescopesContinued");
            skipMods.Add("ChinesePackContinued");
            skipMods.Add("ContractConfigurator-RoverMissionsRedux");
            skipMods.Add("ContractConfigurator-UnmannedContracts");
            skipMods.Add("CraftImport");
            skipMods.Add("CryoTanks");
            skipMods.Add("DeepSpaceExplorationVessels");
            skipMods.Add("DeployableEngines");
            skipMods.Add("DistantObject");
            skipMods.Add("EpicSuits");
            skipMods.Add("FASA");
            skipMods.Add("FilterExtensions");
            skipMods.Add("FilterExtensionsDefaultConfig");
            skipMods.Add("FirespitterCore");
            skipMods.Add("FirespitterResourcesConfig");
            skipMods.Add("FuelSwitchtoeverytank-ThermalPaint");
            skipMods.Add("HotSpot");
            skipMods.Add("InterstellarFuelSwitch-Core");
            skipMods.Add("KSPInterstellarExtended");
            skipMods.Add("KWRocketry-CommunityFixes");
            skipMods.Add("KWRocketry");
            skipMods.Add("KaribouExpeditionRover");
            skipMods.Add("KerbalAircraftExpansion");
            skipMods.Add("KerbalKonstructs");
            skipMods.Add("KerbalPlanetaryBaseSystems");
            skipMods.Add("KerbalStockLauncherOverhaul");
            skipMods.Add("KerbinSide");
            skipMods.Add("KerbodynePlus");
            skipMods.Add("KlockheedMartian-Gimbal");
            skipMods.Add("Kronkus");
            skipMods.Add("LittleGreenMenFromMars");
            skipMods.Add("Mk3HypersonicSystems");
            skipMods.Add("MK2KSPIIntegration");
            skipMods.Add("MarkIVSpaceplaneSystem");
            skipMods.Add("MasterTechAerospace");
            skipMods.Add("Mk2Expansion");
            skipMods.Add("MK2KSPIIntegration");
            skipMods.Add("Mk3KISCargoContainers");
            skipMods.Add("NearFutureElectrical-Core");
            skipMods.Add("NearFutureProps");
            skipMods.Add("PhoenixIndustriesFlags");
            skipMods.Add("RN-SalyutStations-SoyuzFerries");
            skipMods.Add("QuizTechAeroPack");
            skipMods.Add("RN-USProbesPack");
            skipMods.Add("RasterPropMonitor-Core");
            skipMods.Add("SHADO");
            skipMods.Add("SVE-HighResolution");
            skipMods.Add("SVE-LowResolution");
            skipMods.Add("SVE-MediumResolution");
            skipMods.Add("SVE-UltraResolution");
            skipMods.Add("ScienceRevisitedRevisited");
            skipMods.Add("SpaceLaunchSystemPartPack");
            skipMods.Add("SpacetuxSA");
            skipMods.Add("Super100ShootingStarSuper67LittleStarCommandPods");
            skipMods.Add("TACLS");
            skipMods.Add("TDIndustriesRCSandHypergolicengines");
            skipMods.Add("TundraExploration");
            skipMods.Add("VersatileToolboxSystem-core");
            skipMods.Add("UnchartedLands");
            skipMods.Add("WildBlueTools");
            */
            return skipMods;
        }
    }
}

