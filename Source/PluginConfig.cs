using BepInEx.Configuration;
using MissileScreen.UI;
using UnityEngine;

namespace MissileScreen
{
    public static class PluginConfig
    {
        // Keybinds

        public static ConfigEntry<KeyCode> cycleUpKey;
        public static ConfigEntry<KeyCode> cycleDownKey;

        // Camera Config

        public static ConfigEntry<int> cameraFOV;
        public static ConfigEntry<int> cameraRenderDistance;

        // UI Config

        public static ConfigEntry<bool> hmdMissileScreen;
        public static ConfigEntry<Vector2> hmdMissileScreenPosition;
        public static ConfigEntry<float> hmdMissileScreenScale;
        

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

        public static void BindConfig(ConfigFile config)
        {
            Plugin.Logger.LogInfo("Binding Configs...");

            // Keybinds
            cycleUpKey = config.Bind(
                "Keybinds",
                "Cycle missile view up keybind",
                KeyCode.K,
                "Change between the missile cameras by +1."
                );
            
            cycleDownKey = config.Bind(
                "Keybinds",
                "Cycle missile view down keybind",
                KeyCode.J,
                "Change between the missile cameras by -1."
                );


            // Camera Config
            cameraFOV = config.Bind(
                "Camera Config",
                "Camera FOV",
                40,
                "The field of view of the missile camera, or how zoomed in the missile camera is."
                );
            
            cameraRenderDistance = config.Bind(
                "Camera Config",
                "Camera Render Distance",
                80000,
                "How far the camera can see and render. This setting can heavily affect performance, reduce if necessary."
                );

            // UI Config
            
            hmdMissileScreen = config.Bind(
                "UI Config",
                "HMD Missile Screen",
                false,
                "Changes the missile screen to be displayed on the HUD instead of the cockpit tac screen."
                );
            
            hmdMissileScreenScale = config.Bind(
                "UI Config",
                "HMD Missile Screen Scale",
                1f,
                "The scale of the missile screen panel. This only affects the HMD missile screen"
                );
            
            hmdMissileScreenScale.SettingChanged += (sender, args) =>
            {
                if (ProfileManager.currentProfile == null || hmdMissileScreen.Value == false ) return;
  
                
                ProfileManager.currentProfile.missilePanel.localScale = Vector3.one * hmdMissileScreenScale.Value;
                
            };
            
            hmdMissileScreenPosition = config.Bind(
                "UI Config",
                "HMD Missile Screen Position",
                new Vector2(-740, -30),
                "The position of the missile screen panel. This only affects the HMD missile screen" 
                );

            hmdMissileScreenPosition.SettingChanged += (sender, args) =>
            {
                if (ProfileManager.currentProfile == null || hmdMissileScreen.Value == false ) return;
  
                
                ProfileManager.currentProfile.missilePanel.GetComponent<RectTransform>().anchoredPosition = hmdMissileScreenPosition.Value;
                
            };
            
            fixedLockBox = config.Bind(
                "UI Config",
                "Fixed Target Lockbox Size",
                false,
                "Changes the target lockbox to always use a fixed size (should use the minimum size). This setting mildly affects performance."
                );


            missileNameColor = config.Bind(
                "UI Config",
                "Missile Name Color",
                new Color(1f, 0f, 1f),
                "The color of the missile name text the missile screen. Changes are applied when changing aircraft."
                );


            velocityVectorColor = config.Bind(
                "UI Config",
                "Velocity Vector Color",
                Color.green,
                "The color of the velocity vector in the missile screen. Changes are applied when changing aircraft."
                );
            velocityVectorOutlineColor = config.Bind(
                "UI Config",
                "Velocity Vector Outline Color", 
                Color.black,
                "The outline color of the velocity vector in the missile screen. Changes are applied when changing aircraft."
                );
            velocityVectorOutlineThickness = config.Bind(
                "UI Config",
                "Velocity Vector Outline Thickness",
                1f,
                "The thickness of the outline of the velocity vector in the missile screen. Changes are applied when changing aircraft."
                );

            leadIconColor = config.Bind(
                "UI Config", 
                "Lead Icon Color", 
                Color.green, 
                "The color of the target lead icon in the missile screen. Changes are applied when changing aircraft."
                );
            leadIconOutlineColor = config.Bind(
                "UI Config", 
                "Lead Icon Outline Color", 
                Color.black,
                "The outline color of the target lead icon in the missile screen. Changes are applied when changing aircraft."
                );
            leadIconOutlineThickness = config.Bind(
                "UI Config",
                "Lead Icon Outline Thickness",
                1f,
                "The thickness of the outline of the target lead icon in the missile screen. Changes are applied when changing aircraft."
                );
            orientationIndicatorColor = config.Bind(
                "UI Config",
                "Orientation Indicator Color",
                Color.green,
                "The color of the missiles orientation indicator. Changes are applied when changing aircraft."
                );
            orientationIndicatorOutlineColor = config.Bind(
                "UI Config", 
                "Orientation Indicator Outline Color",
                Color.black,
                "The outline color of the missile orientation indicator in the missile screen. Changes are applied when changing aircraft."
                );
            orientationIndicatorOutlineThickness = config.Bind(
                "UI Config", 
                "Orientation Indicator Outline Thickness", 
                1f,
                "The thickness of the outline of the missile orientation indicator in the missile screen. Changes are applied when changing aircraft."
                );

            lockBoxColor = config.Bind(
                "UI Config",
                "Target Lockbox Color",
                new Color(0f, 1f, 1f),
                "The color of the target lock box corners in the missile screen. Changes are applied when changing aircraft."
                );
            lockBoxOutlineColor = config.Bind(
                "UI Config",
                "Target Lockbox Outline Color", Color.black,
                "The outline color of the target lockbox corners in the missile screen. Changes are applied when changing aircraft."
                );
            lockBoxOutlineThickness = config.Bind(
                "UI Config",
                "Target Lockbox Outline Thickness", 
                1f,
                "The thickness of the outline of the target lockbox corners in the missile screen. Changes are applied when changing aircraft."
                );

            // Misc Config
            missileCameraOffset = config.Bind(
                "Misc",
                "Missile Camera Offset",
                new Vector3(0, 0, 4)
                );

            Plugin.Logger.LogInfo("Configs successfully binded!");
        }
    }

    
 }
