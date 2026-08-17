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
        
        
        public static ConfigEntry<float> globalFontScale;

        public static ConfigEntry<bool> fixedLockBox;
        
        public static ConfigEntry<bool> hideNoMissile;
        
        public static ConfigEntry<bool> hideMissileName;
        public static ConfigEntry<bool> hideMissileIndex;
        public static ConfigEntry<bool> hideTargetName;
        public static ConfigEntry<bool> hideMissileSpeed;
        public static ConfigEntry<bool> hideMissileAltitude;
        public static ConfigEntry<bool> hideMissileRange;
        
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

        

        public static void BindConfig(ConfigFile config)
        {
            Plugin.logger.LogInfo("Binding Configs...");

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
            
            missileCameraOffset = config.Bind(
                "Camera Config",
                "Missile Camera Offset",
                new Vector3(0, 0, 4)
                );
            
            // UI Config \\
            
            
            // HMD 
            
            hmdMissileScreen = config.Bind(
                "UI Config - HMD",
                "HMD Missile Screen",
                false,
                "Changes the missile screen to be displayed on the HUD instead of the cockpit tac screen."
                );
            
            hmdMissileScreenScale = config.Bind(
                "UI Config - HMD",
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
                "UI Config - HMD",
                "HMD Missile Screen Position",
                new Vector2(-740, -30),
                "The position of the missile screen panel. This only affects the HMD missile screen" 
                );

            hmdMissileScreenPosition.SettingChanged += (sender, args) =>
            {
                if (ProfileManager.currentProfile == null || hmdMissileScreen.Value == false ) return;
  
                
                ProfileManager.currentProfile.missilePanel.GetComponent<RectTransform>().anchoredPosition = hmdMissileScreenPosition.Value;
                
            };
            
            
            // Colors
            
            missileNameColor = config.Bind(
                "UI Config - Colors",
                "Missile Name Color",
                new Color(1f, 0f, 1f),
                "The color of the missile name text the missile screen. Changes are applied when changing aircraft."
                );
            
            velocityVectorColor = config.Bind(
                "UI Config - Colors",
                "Velocity Vector Color",
                Color.green,
                "The color of the velocity vector in the missile screen. Changes are applied when changing aircraft."
                );
           
            leadIconColor = config.Bind(
                "UI Config - Colors", 
                "Lead Icon Color", 
                Color.green, 
                "The color of the target lead icon in the missile screen. Changes are applied when changing aircraft."
                );
          
            orientationIndicatorColor = config.Bind(
                "UI Config - Colors",
                "Orientation Indicator Color",
                Color.green,
                "The color of the missiles orientation indicator. Changes are applied when changing aircraft."
                );
          
            lockBoxColor = config.Bind(
                "UI Config - Colors",
                "Target Lockbox Color",
                new Color(0f, 1f, 1f),
                "The color of the target lock box corners in the missile screen. Changes are applied when changing aircraft."
                );
            
            
            // Outlines
            
            lockBoxOutlineColor = config.Bind(
                "UI Config - Outlines",
                "Target Lockbox Outline Color", Color.black,
                "The outline color of the target lockbox corners in the missile screen. Changes are applied when changing aircraft."
                );
            lockBoxOutlineThickness = config.Bind(
                "UI Config - Outlines",
                "Target Lockbox Outline Thickness", 
                1f,
                "The thickness of the outline of the target lockbox corners in the missile screen. Changes are applied when changing aircraft."
                );
            
            orientationIndicatorOutlineColor = config.Bind(
                "UI Config - Outlines", 
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
            
            leadIconOutlineColor = config.Bind(
                "UI Config - Outlines", 
                "Lead Icon Outline Color", 
                Color.black,
                "The outline color of the target lead icon in the missile screen. Changes are applied when changing aircraft."
                );
            leadIconOutlineThickness = config.Bind(
                "UI Config - Outlines",
                "Lead Icon Outline Thickness",
                1f,
                "The thickness of the outline of the target lead icon in the missile screen. Changes are applied when changing aircraft."
                );
            
            velocityVectorOutlineColor = config.Bind(
                "UI Config - Outlines",
                "Velocity Vector Outline Color", 
                Color.black,
                "The outline color of the velocity vector in the missile screen. Changes are applied when changing aircraft."
                );
            velocityVectorOutlineThickness = config.Bind(
                "UI Config - Outlines",
                "Velocity Vector Outline Thickness",
                1f,
                "The thickness of the outline of the velocity vector in the missile screen. Changes are applied when changing aircraft."
                );
            
            // Elements
            
            hideMissileName  = config.Bind(
                "UI Config - Elements",
                "Hide Missile Name",
                false,
                "Hides the text that shows the name of the currently shown missile. Requires aircraft change to apply changes."
                );
            
            hideMissileIndex  = config.Bind(
                "UI Config - Elements",
                "Hide Missile Index",
                false,
                "Hides the text that shows the index of the current missile in the list of active missiles. Requires aircraft change to apply changes."
                );
            
            hideMissileSpeed  = config.Bind(
                "UI Config - Elements",
                "Hide Missile Speed",
                false,
                "Hides the text that shows the speed of the currently shown missile. Requires aircraft change to apply changes."
                );
            
            hideMissileAltitude  = config.Bind(
                "UI Config - Elements",
                "Hide Missile Altitude",
                false,
                "Hides the text that shows the altitude of the currently shown missile. Requires aircraft change to apply changes." 
                );
            
            hideMissileRange  = config.Bind(
                "UI Config - Elements",
                "Hide Missile Range",
                false,
                "Hides the text that shows the range of the currently shown missile to its target. Requires aircraft change to apply changes."
                );
            
            hideTargetName  = config.Bind(
                "UI Config - Elements",
                "Hide Target Name",
                false,
                "Hides the text that shows the locked target of the currently shown missile. Requires aircraft change to apply changes."
                );
            
            // Misc Config
            
            hideNoMissile =config.Bind(
                "Misc",
                "Hide No Missile Display",
                false,
                "Hide the missile name whenever there is no missile currently active."
                );
            
            fixedLockBox = config.Bind(
                "Misc",
                "Fixed Target Lockbox Size",
                false,
                "Changes the target lockbox to always use a fixed size (should use the minimum size). This setting mildly affects performance."
                );
            
           globalFontScale = config.Bind(
               "Misc",
               "Font Scale",
               1f
               );
            
           


            Plugin.logger.LogInfo("Configs successfully binded!");
        }
    }

    
 }
