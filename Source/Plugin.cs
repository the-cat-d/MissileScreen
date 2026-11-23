using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MissileScreen.UI;

namespace MissileScreen
{
    

    public static class PluginInfo
    {
        
        public const string PLUGIN_GUID = "MissileScreen";
        public const string PLUGIN_NAME = "Missile Screen";
        public const string PLUGIN_VERSION = "2.0.0";
    }


    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static new ManualLogSource logger;
        private Harmony _harmony;

        private void Awake()
        {

            // Plugin startup logic
            
            logger = base.Logger;
            logger.LogInfo($"{PluginInfo.PLUGIN_GUID} is loading...");

            logger.LogInfo($"Patching Harmony...");

            _harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            _harmony.PatchAll();


            logger.LogInfo($"Harmony Patched!");


            // Sprite Loading
            PluginSprites.LoadSprites();
            
           
            // Config Initialization

            PluginConfig.BindConfig(Config);
            

            // Profile Loading

            ProfileManager.LoadProfiles();


            logger.LogInfo($"{PluginInfo.PLUGIN_GUID} Loaded");
        }


        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
        }
    }
}
