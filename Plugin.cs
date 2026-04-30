using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using MissileView.UI;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace MissileView
{

    // TODO: use Bepinex pubilicizer instead of reflection
    // TODO: maybe add more logging?

    public class PluginConfig
    {
        // Keybinds

        public static ConfigEntry<KeyCode> cycleKey;

        // Camera Config

        public static ConfigEntry<int> cameraFOV;
        public static ConfigEntry<int> cameraRenderDistance;

        // UI Config

        public static ConfigEntry<int> cameraResolution;


        // Misc

        public static ConfigEntry<Vector3> missileCameraOffset; 

        // TODO: add configs for color adjustment
    }

    public class PluginSprites
    {
        public static Sprite AttackSprite;
        public static Sprite OrientationSprite;
        public static Sprite lockBoxSprite;
        public static Sprite lockCursorSprite;
        public static Sprite lockCornerSprite;
        public static Sprite leadSprite;
    }

    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;
        internal Harmony harmony;

        private void Awake()
        {

            // Plugin startup logic
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

            Logger.LogInfo($"Patching Harmony...");
            harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll();


            Logger.LogInfo($"Harmony Patched!");

            Logger.LogInfo($"Loading Sprites...");

            // Sprite Loading

            var assembly = Assembly.GetExecutingAssembly();
            string[] resourceNames = assembly.GetManifestResourceNames();
            foreach (string name in resourceNames)
            {
                Console.WriteLine(name);
            }


            PluginSprites.AttackSprite = GameUtils.LoadImageInStream("attackIcon.png");
            PluginSprites.OrientationSprite = GameUtils.LoadImageInStream("orientationIndicator.png");
            PluginSprites.lockBoxSprite = GameUtils.LoadImageInStream("lockBox.png");
            PluginSprites.lockCursorSprite = GameUtils.LoadImageInStream("lockCursor.png");
            PluginSprites.lockCornerSprite = GameUtils.LoadImageInStream("lockBoxCorner.png");
            PluginSprites.leadSprite = GameUtils.LoadImageInStream("leadIcon.png");

            //PluginSprites.AttackSprite = GameUtils.LoadingImage(Path.Combine(Paths.PluginPath, MyPluginInfo.PLUGIN_GUID, "Assets", "attackIcon.png"));
            //PluginSprites.OrientationSprite = GameUtils.LoadingImage(Path.Combine(Paths.PluginPath, MyPluginInfo.PLUGIN_GUID, "Assets", "orientationIndicator.png"));
            //PluginSprites.lockBoxSprite = GameUtils.LoadingImage(Path.Combine(Paths.PluginPath, MyPluginInfo.PLUGIN_GUID, "Assets", "lockBox.png"));
            //PluginSprites.lockCursorSprite = GameUtils.LoadingImage(Path.Combine(Paths.PluginPath, MyPluginInfo.PLUGIN_GUID, "Assets", "lockCursor.png"));
            //PluginSprites.lockCornerSprite = GameUtils.LoadingImage(Path.Combine(Paths.PluginPath, MyPluginInfo.PLUGIN_GUID, "Assets", "lockBoxCorner.png"));
            //PluginSprites.leadSprite = GameUtils.LoadingImage(Path.Combine(Paths.PluginPath, MyPluginInfo.PLUGIN_GUID, "Assets", "leadIcon.png"));

            // Config Initialization

            Logger.LogInfo("Binding Configs...");

            // Keybinds
            PluginConfig.cycleKey = Config.Bind("Keybinds", "Cycle missile view keybind", KeyCode.K, "The keybind which cycles the camera view between active missiles.");


            // Camera Config
            PluginConfig.cameraFOV = Config.Bind("Camera Config", "Camera FOV", 40, "The field of view of the camera, or how zoomed in the camera is.");
            PluginConfig.cameraRenderDistance = Config.Bind("Camera Config", "Camera Render Distance", 80000, "How far the camera can see and render. This setting can heavily affect performance, reduce if nessecary.");

            // UI Config
            PluginConfig.cameraResolution = Config.Bind("UI Config", "Camera Resolution", 1024, "The resolution of the missile screen camera. Changes are applied when changing aircraft. This setting can heavily affect performance, reduce if nessecary.");
            
            // TODO: fixed size lockbox option for performance

            // Misc Config
            PluginConfig.missileCameraOffset = Config.Bind("Misc", "Missile Camera Offset", new Vector3(0,0,4), "Missile");

            // ProfileLoading
            ProfileManager.LoadProfiles();
        }


        private void OnDestroy()
        {
            harmony?.UnpatchSelf();
        }
    }
}
