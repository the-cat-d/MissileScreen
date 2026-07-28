using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MissileView.UI;
using System;
using System.Reflection;
using UnityEngine;

namespace MissileView
{

    // TODO: use Bepinex pubilicizer instead of reflection
    // TODO: maybe add more logging?
    // TODO: someway to show missile pitch cleanly
    // TODO: estimated flight time
    // TODO: gmblr info (i.e distance till submunition jettison)
    // TODO: more profile fine tuning
    // TODO: add an option for the missile camera to be displayed in the HUD
    // TODO: hide lockbox and lead indicator when position is off screen (when its behind the camera it will still be shown on the screen)



    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "MissileView";
        public const string PLUGIN_NAME = "Missile View";
        public const string PLUGIN_VERSION = "1.0.0";
    }


    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;
        internal Harmony harmony;

        private void Awake()
        {

            // Plugin startup logic
            
            Logger = base.Logger;
            Logger.LogInfo($"{PluginInfo.PLUGIN_GUID} is loading...");

            Logger.LogInfo($"Patching Harmony...");

            harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();


            Logger.LogInfo($"Harmony Patched!");


            // Sprite Loading
            PluginSprites.LoadSprites();
            
           
            // Config Initialization

            PluginConfig.BindConfig(Config);
            


            // Profile Loading

            ProfileManager.LoadProfiles();


            Logger.LogInfo($"{PluginInfo.PLUGIN_GUID} Loaded");
        }


        private void OnDestroy()
        {
            harmony?.UnpatchSelf();
        }
    }
}
