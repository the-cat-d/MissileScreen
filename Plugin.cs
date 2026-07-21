using BepInEx;
using BepInEx.Configuration;
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
    // TODO: IR mode

    public class PluginConfig
    {
        // Keybinds

        public static ConfigEntry<KeyCode> cycleKey;

        // Camera Config

        public static ConfigEntry<int> cameraFOV;
        public static ConfigEntry<int> cameraRenderDistance;

        // UI Config

        public static ConfigEntry<int> cameraResolution; // unused

        public static ConfigEntry<bool> fixedLockBox;

        public static ConfigEntry<Color> missileNameColor;

        public static ConfigEntry<Color> velocityVectorColor;
        public static ConfigEntry<Color> velocityVectorOutlineColor;
        public static ConfigEntry<float> velocityVectorOutlineThickness;

        public static ConfigEntry<Color> leadIconColor;
        public static ConfigEntry<Color> leadIconOutlineColor;
        public static ConfigEntry<float> leadIconOutlineThickness;

        public static ConfigEntry<Color> orientationIndicatorColor;
        public static ConfigEntry<Color> orientationIndicatorOutlineColor;
        public static ConfigEntry<float> orientationIndicatorOutlineThickness;

        public static ConfigEntry<Color> lockBoxColor;
        public static ConfigEntry<Color> lockBoxOutlineColor;
        public static ConfigEntry<float> lockBoxOutlineThickness;


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

           
            // Config Initialization

            Logger.LogInfo("Binding Configs...");

            // Keybinds
            PluginConfig.cycleKey = Config.Bind("Keybinds", "Cycle missile view keybind", KeyCode.K, "The keybind which cycles the camera view between active missiles.");


            // Camera Config
            PluginConfig.cameraFOV = Config.Bind("Camera Config", "Camera FOV", 40, "The field of view of the missile camera, or how zoomed in the missile camera is.");
            PluginConfig.cameraRenderDistance = Config.Bind("Camera Config", "Camera Render Distance", 80000, "How far the camera can see and render. This setting can heavily affect performance, reduce if nessecary.");

            // UI Config
            //PluginConfig.cameraResolution = Config.Bind("UI Config", "Camera Resolution", 1024, "The resolution of the missile screen camera. Changes are applied when changing aircraft. This setting can heavily affect performance, reduce if nessecary.");

            PluginConfig.fixedLockBox = Config.Bind("UI Config", "Fixed Target Lockbox Size", false, "Changes the target lockbox to always use a fixed size (should use the minimum size). This setting mildly affects performance.");


            PluginConfig.missileNameColor = Config.Bind("UI Config", "Missile Name Color", new Color(1f, 0f, 1f), "The color of the missile name text the missile screen. Changes are applied when changing aircraft.");


            PluginConfig.velocityVectorColor = Config.Bind("UI Config", "Velocity Vector Color", Color.green, "The color of the velocity vector in the missile screen. Changes are applied when changing aircraft.");
            PluginConfig.velocityVectorOutlineColor = Config.Bind("UI Config", "Velocity Vector Outline Color", Color.black, "The outline color of the velocity vector in the missile screen. Changes are applied when changing aircraft.");
            PluginConfig.velocityVectorOutlineThickness = Config.Bind("UI Config", "Velocity Vector Outline Thickness", 1f, "The thickness of the outline of the velocity vector in the missile screen. Changes are applied when changing aircraft.");

            PluginConfig.leadIconColor = Config.Bind("UI Config", "Lead Icon Color", Color.green, "The color of the target lead icon in the missile screen. Changes are applied when changing aircraft.");
            PluginConfig.leadIconOutlineColor = Config.Bind("UI Config", "Lead Icon Outline Color", Color.black, "The outline color of the target lead icon in the missile screen. Changes are applied when changing aircraft.");
            PluginConfig.leadIconOutlineThickness = Config.Bind("UI Config", "Lead Icon Outline Thickness", 1f, "The thickness of the outline of the target lead icon in the missile screen. Changes are applied when changing aircraft.");

            PluginConfig.orientationIndicatorColor = Config.Bind("UI Config", "Orientation Indicator Color", Color.green, "The color of the missiles orientation indicator. Changes are applied when changing aircraft.");
            PluginConfig.orientationIndicatorOutlineColor = Config.Bind("UI Config", "Orientation Indicator Outline Color", Color.black, "The outline color of the missile orientation indicator in the missile screen. Changes are applied when changing aircraft.");
            PluginConfig.orientationIndicatorOutlineThickness = Config.Bind("UI Config", "Orientation Indicator Outline Thickness", 1f, "The thickness of the outline of the missile orientation indicator in the missile screen. Changes are applied when changing aircraft.");

            PluginConfig.lockBoxColor = Config.Bind("UI Config", "Target Lockbox Color", new Color(0f, 1f, 1f), "The color of the target lock box corners in the missile screen. Changes are applied when changing aircraft.");
            PluginConfig.lockBoxOutlineColor = Config.Bind("UI Config", "Target Lockbox Outline Color", Color.black, "The outline color of the target lockbox corners in the missile screen. Changes are applied when changing aircraft.");
            PluginConfig.lockBoxOutlineThickness = Config.Bind("UI Config", "Target Lockbox Outline Thickness", 1f, "The thickness of the outline of the target lockbox corners in the missile screen. Changes are applied when changing aircraft.");


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
