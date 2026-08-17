using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MissileScreen.UI;

namespace MissileScreen
{
    // TODO: should definitely turn off any missile camera that is active in missile launch error handling
    // TODO: option to turn off individual icons (velocity vector and such)
    // TODO: add more logging
    // TODO: add a sound or an indicator that a missile successfully hit its target
    // TODO: indicate when a missile is about to be intercepted (i.e show a indicator when a missile is being targeted)
    // TODO: nuke info (i.e arming timer)
    // TODO: someway to show missile pitch cleanly
    // TODO: estimated flight time
    // TODO: gmblr info (i.e distance till submunition jettison)
    // TODO: use accessibility option custom colors for team colors
    // TODO: more profile fine tuning
    



    public static class PluginInfo
    {
        
        public const string PLUGIN_GUID = "MissileScreen";
        public const string PLUGIN_NAME = "Missile Screen";
        public const string PLUGIN_VERSION = "1.2.0";
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
