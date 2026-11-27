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
        public static ConfigEntry<bool> changeToNewMissile;
        
        // UI Config

        public static ConfigEntry<bool> hmdMissileScreen;
        public static ConfigEntry<Vector2> hmdMissileScreenPosition;
        public static ConfigEntry<float> hmdMissileScreenScale;
        public static ConfigEntry<float> hmdMissileScreenTransparency;
        
        
        public static ConfigEntry<float> globalFontScale;

        public static ConfigEntry<bool> fixedLockBox;
        
        public static ConfigEntry<bool> hideNoMissile;
        
        public static ConfigEntry<bool> hideMissileName;
        public static ConfigEntry<bool> hideMissileIndex;
        public static ConfigEntry<bool> hideTargetName;
        public static ConfigEntry<bool> hideMissileToF;
        public static ConfigEntry<bool> hideMissileSpeed;
        public static ConfigEntry<bool> hideMissileAltitude;
        public static ConfigEntry<bool> hideMissileRange;
        
        public static ConfigEntry<bool> hideVelocityVector;
        public static ConfigEntry<bool> hideOrientationIndicator;
        public static ConfigEntry<bool> hideLockbox;
        public static ConfigEntry<bool> hideLeadIndicator;
        public static ConfigEntry<bool> hidePitchIndicator;
        public static ConfigEntry<bool> hideHeadingIndicator;
        
        public static ConfigEntry<bool> simplePitchIndicator;
        public static ConfigEntry<float> pitchIndicatorPanelPosition;
        public static ConfigEntry<bool> onlyShowHorizon;
        public static ConfigEntry<float> horizonLineWidthScale;
        
        
        public static ConfigEntry<float> pitchIndicatorOutlineThickness;
        public static ConfigEntry<Color> pitchIndicatorOutlineColor;
        public static ConfigEntry<Color> pitchIndicatorColor;

        public static ConfigEntry<bool> simpleHeadingIndicator;
        public static ConfigEntry<Color> headingIndicatorColor;
        
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
                "Missile Camera Config",
                "Missile Camera FOV",
                40,
                "The field of view of the missile camera, or how zoomed in the missile camera is."
                );
            
            cameraRenderDistance = config.Bind(
                "Missile Camera Config",
                "Missile Camera Render Distance",
                80000,
                "How far the camera can see and render. This setting can affect performance, reduce if necessary."
                );
            
            missileCameraOffset = config.Bind(
                "Missile Camera Config",
                "Missile Camera Offset",
                new Vector3(0, 0, 4),
                "Offset the missile camera placement inside the missile"
                );
            
            changeToNewMissile = config.Bind(
                "Missile Camera Config",
                "Always Change To Newest Missile",
                true,
                "Automatically switch to the newest active missile if enabled, if not it will stay on the missile that's being currently viewed"
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
            
            hmdMissileScreenTransparency = config.Bind(
                "UI Config - HMD",
                "HMD Missile Screen Transparency",
                1f,
                new ConfigDescription("Transparency of the missile screen panel. This only affects the HMD missile screen" , 
                    new AcceptableValueRange<float>(0,1)
                    )
                );

            hmdMissileScreenTransparency.SettingChanged += (_, _) =>
            {
                
                if (ProfileManager.currentProfile != null) ProfileManager.currentProfile?.UpdateElementsColor();
                
            };
            
            // Colors
            
            missileNameColor = config.Bind(
                "UI Config - Colors",
                "Missile Name Color",
                new Color(1f, 0f, 1f),
                "The color of the missile name text the missile screen. Requires aircraft reload to apply changes."
                );
            
            velocityVectorColor = config.Bind(
                "UI Config - Colors",
                "Velocity Vector Color",
                Color.green,
                "The color of the velocity vector in the missile screen. Requires aircraft reload to apply changes."
                );
           
            leadIconColor = config.Bind(
                "UI Config - Colors", 
                "Lead Icon Color", 
                Color.green, 
                "The color of the target lead icon in the missile screen. Requires aircraft reload to apply changes."
                );
          
            orientationIndicatorColor = config.Bind(
                "UI Config - Colors",
                "Orientation Indicator Color",
                Color.green,
                "The color of the missiles orientation indicator. Requires aircraft reload to apply changes."
                );
          
            lockBoxColor = config.Bind(
                "UI Config - Colors",
                "Target Lockbox Color",
                new Color(0f, 1f, 1f),
                "The color of the target lock box corners in the missile screen. Requires aircraft reload to apply changes."
                );
            
            pitchIndicatorColor = config.Bind(
                "UI Config - Colors",
                "Pitch Indicator Color",
                Color.green,
                "The color of the pitch ladder in the missile screen. Requires aircraft reload to apply changes."
                );

            
            // Outlines
            
            lockBoxOutlineColor = config.Bind(
                "UI Config - Outlines",
                "Target Lockbox Outline Color", Color.black,
                "The outline color of the target lockbox corners in the missile screen. Requires aircraft reload to apply changes."
                );
            lockBoxOutlineThickness = config.Bind(
                "UI Config - Outlines",
                "Target Lockbox Outline Thickness", 
                1f,
                "The thickness of the outline of the target lockbox corners in the missile screen. Requires aircraft reload to apply changes."
                );
            
            orientationIndicatorOutlineColor = config.Bind(
                "UI Config - Outlines", 
                "Orientation Indicator Outline Color",
                Color.black,
                "The outline color of the missile orientation indicator in the missile screen. Requires aircraft reload to apply changes."
                );
            orientationIndicatorOutlineThickness = config.Bind(
                "UI Config - Outlines", 
                "Orientation Indicator Outline Thickness", 
                1f,
                "The thickness of the outline of the missile orientation indicator in the missile screen. Requires aircraft reload to apply changes."
                );
            
            leadIconOutlineColor = config.Bind(
                "UI Config - Outlines", 
                "Lead Icon Outline Color", 
                Color.black,
                "The outline color of the target lead icon in the missile screen. Requires aircraft reload to apply changes."
                );
            leadIconOutlineThickness = config.Bind(
                "UI Config - Outlines",
                "Lead Icon Outline Thickness",
                1f,
                "The thickness of the outline of the target lead icon in the missile screen. Requires aircraft reload to apply changes."
                );
            
            velocityVectorOutlineColor = config.Bind(
                "UI Config - Outlines",
                "Velocity Vector Outline Color", 
                Color.black,
                "The outline color of the velocity vector in the missile screen. Requires aircraft reload to apply changes."
                );
            velocityVectorOutlineThickness = config.Bind(
                "UI Config - Outlines",
                "Velocity Vector Outline Thickness",
                1f,
                "The thickness of the outline of the velocity vector in the missile screen. Requires aircraft reload to apply changes."
                );
            
            
            pitchIndicatorOutlineColor = config.Bind(
                "UI Config - Outlines",
                "Pitch Indicator Outline Color", 
                Color.black,
                "The outline color of the pitch ladder in the missile screen. Requires aircraft reload to apply changes."
                );
            pitchIndicatorOutlineThickness = config.Bind(
                "UI Config - Outlines",
                "Pitch Indicator Outline Thickness",
                1f,
                "The thickness of the outline of the pitch ladder in the missile screen. Requires aircraft reload to apply changes."
                );
            
            // Elements
            
            globalFontScale = config.Bind(
                "UI Config - Text",
                "Font Scale",
                1f
                );
        
            
            // Symbology
            
            fixedLockBox = config.Bind(
                "UI Config - Symbology",
                "Fixed Target Lockbox Size",
                false,
                "Changes the target lockbox to always use a fixed size (should use the minimum size). This setting mildly affects performance."
                );
            
            pitchIndicatorPanelPosition = config.Bind(
                "UI Config - Elements - Pitch Indicator",
                "Pitch Indicator Horizontal Panel Position",
                0.25f,
                "The location of the pitch indicator relative to the panel size. 0 is left, 0.5 is center and 1 is right."
                );
            
            simplePitchIndicator = config.Bind("UI Config - Elements - Pitch Indicator",
                "Use Simple Pitch Indicator",
                false,
                "Indicates the pitch of the currently shown missile in a label displayed above the velocity vector instead of a pitch ladder. Requires aircraft reload to apply changes."
                );

            onlyShowHorizon = config.Bind("UI Config - Elements - Pitch Indicator", 
                "Only Show Horizon",
                false,
                "Will only show the horizon rung (Requires pitch ladder, which needs simple pitch indicator to be turned off). Requires aircraft reload to apply changes."
                );
            
            horizonLineWidthScale = config.Bind("UI Config - Elements - Pitch Indicator", 
                "Horizon Line Width Scale",
                1f,
                "Scales the width of the horizon line in the pitch ladder (Requires pitch ladder, which needs simple pitch indicator to be turned off). Requires aircraft reload to apply changes."
                );

            // Labels
            
            hideMissileName  = config.Bind(
                "UI Config - Declutter - Labels",
                "Hide Missile Name",
                false,
                "Hides the text that shows the name of the currently shown missile. Requires aircraft reload to apply changes."
                );
            
            hideMissileIndex  = config.Bind(
                "UI Config - Declutter - Labels",
                "Hide Missile Index",
                false,
                "Hides the text that shows the index of the current missile in the list of active missiles. Requires aircraft reload to apply changes."
                );
            

            hideMissileSpeed  = config.Bind(
                "UI Config - Declutter - Labels",
                "Hide Missile Speed",
                false,
                "Hides the text that shows the speed of the currently shown missile. Requires aircraft reload to apply changes."
                );
            
            hideMissileToF  = config.Bind(
                "UI Config - Declutter - Labels",
                "Hide Missile ToF (Time of Flight)",
                false,
                "Hides the text that shows the ToF of the currently shown missile to its target. Requires aircraft reload to apply changes."
                );

            
            hideMissileAltitude  = config.Bind(
                "UI Config - Declutter - Labels",
                "Hide Missile Altitude",
                false,
                "Hides the text that shows the altitude of the currently shown missile. Requires aircraft reload to apply changes." 
                );
            
            hideMissileRange  = config.Bind(
                "UI Config - Declutter - Labels",
                "Hide Missile Range",
                false,
                "Hides the text that shows the range of the currently shown missile to its target. Requires aircraft reload to apply changes."
                );
            
            hideTargetName  = config.Bind(
                "UI Config - Declutter - Labels",
                "Hide Target Name",
                false,
                "Hides the text that shows the locked target of the currently shown missile. Requires aircraft reload to apply changes."
                );
            
            hideNoMissile =config.Bind(
                "UI Config - Declutter - Labels",
                "Hide No Missile Display",
                false,
                "Hide the missile name whenever there is no missile currently active."
                );
            
            // Symbology
            
            hideOrientationIndicator = config.Bind("UI Config - Declutter - Symbology",
                "Hide Orientation Indicator",
                true,
                "Hides the 2 lines which are displayed on the velocity vector that indicate orientation relative to world of the currently shown missile. Requires aircraft reload to apply changes."
                );
            
            hidePitchIndicator = config.Bind("UI Config - Declutter - Symbology",
                "Hide Pitch Indicator",
                false,
                "Hides the pitch ladder of the currently shown missile. Requires aircraft reload to apply changes."
                );
           
            // Misc Config
            
           
           
            
         
           


            Plugin.logger.LogInfo("Configs successfully bound!");
        }
    }

    
 }
