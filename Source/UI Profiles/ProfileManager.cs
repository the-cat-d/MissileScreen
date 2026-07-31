using HarmonyLib;
using MissileScreen.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;


namespace MissileScreen.UI
{
    internal class ProfileManager
    {
        public static Profile currentProfile = null;
        public static Profile defaultProfile = new Profile();
       

        private static Dictionary<string,Profile> LoadedProfiles = new Dictionary<string,Profile>(StringComparer.Ordinal);

        public static void AddProfile(Profile profile,string aircraftName)
        {
  
            if (profile == null)
            {
                Plugin.Logger.LogError($"Aircraft Profile is null!");
                return;
            }
            else
            {
                LoadedProfiles[aircraftName] = profile;
                Plugin.Logger.LogInfo($"Profile For \"{aircraftName}\" Added");
            }
        }

        public static void LoadProfiles()
        {

            List<MethodInfo> profiles = typeof(ProfileManager).Assembly.GetTypes().Where(val => val.Namespace == "MissileScreen.UI_Profiles.AircraftProfiles").Select(method => method.GetMethod("Load")).ToList();


            for (int i = 0; i < profiles.Count; i++)
            {
               try 
               { 
                    profiles[i].Invoke(null, null);
               } catch { }
            }
        }

        private static Profile GetProfileFromName(string name)
        {
            if (LoadedProfiles.ContainsKey(name))
            {
                Plugin.Logger.LogDebug($"Found \"{name}\" Profile, Loading...");
                return LoadedProfiles[name];
            } else
            {
                Plugin.Logger.LogError($"Couldn't find Profile \"{name}\". Refering to Default Profile");
                return defaultProfile;
                
            }
        }

        public static void InjectProfileUI(string AircraftName, TacScreen tacScreenInstance)
        {

            Profile profile = GetProfileFromName(AircraftName);

            //// Validation Checks \\\\

            if (profile == null )
            {
                Plugin.Logger.LogError($"Couldn't find Profile \"{AircraftName}\" ");
               
                return; // If the default profile is somehow not returned, then exit
            }

            

            profile.weaponPanel = GameUtils.FindChildRecursive(tacScreenInstance.transform,profile.replacePanelName);

            if (profile.weaponPanel == null)
            {

                return; // If the plane doesn't have the panel, then exit

            }

            MissileScreenUIPatching.isPlaneCompatible = true;

            



            //// Missile Panel Creation \\\\

            // Missile Panel GameObject

            profile.missilePanel = UnityEngine.Object.Instantiate(profile.weaponPanel, profile.weaponPanel.transform.parent);
            profile.missilePanel.name = "missilePanel";
            


            // Clear Missile Panel Children

            foreach (Transform child in profile.missilePanel)
            {
                UnityEngine.Object.Destroy(child.gameObject); 
            }


            // Clear Missile Panel Components (besides RectTransform and CanvasRenderer)

            Object[] panelComponents = profile.missilePanel.GetComponents<Component>();

            foreach (Component component in panelComponents)
            {
                if (!(component is RectTransform) && !(component is CanvasRenderer)) {
                    Component.Destroy(component);
                }
            }

            // Clearing of the old panel that was instantiated from (if the profile is set to do so)

            if (profile.clearOldPanel)
            {
                profile.weaponPanel.gameObject.SetActive(true);
            }

            // Getting hideGameObject (if the profile is set to do so)
            
            if (profile.hideGameObjectNames.Count > 0)
            {
                for (int i = 0; i < profile.hideGameObjectNames.Count; i++)
                {
                    Transform hideGameObject = GameUtils.FindChildRecursive(tacScreenInstance.transform, profile.hideGameObjectNames[i]);
                    if (hideGameObject != null)
                    {
                       profile.hideGameObjects.Add(hideGameObject.gameObject);
                        
                    }
                }
                
            }
         


            //// Panel Configuration \\\\
            // These will just use the values it had before if the profile values are unassigned

            // Missile Panel Size
            profile.missilePanel.GetComponent<RectTransform>().sizeDelta = profile.missilePanelRectSize != Vector2.zero ? profile.missilePanelRectSize : profile.missilePanel.GetComponent<RectTransform>().sizeDelta;

            // Missile Panel Position
            profile.missilePanel.GetComponent<RectTransform>().anchoredPosition = profile.missilePanelRectPosition != Profile.positionDefault ? profile.missilePanelRectPosition : profile.missilePanel.GetComponent<RectTransform>().anchoredPosition;

            // Missile Panel Rotation
            profile.missilePanel.localRotation = profile.missilePanelRectRotation != Profile.rotationDefault ? profile.missilePanelRectRotation : profile.missilePanel.localRotation;

            //Hierarchy - currently unused, keeping it just incase if i need it

            profile.missilePanel.SetSiblingIndex(profile.hierarchyOrder != -1 ? profile.hierarchyOrder : profile.missilePanel.GetSiblingIndex());

            profile.missilePanelSize = profile.missilePanel.GetComponent<RectTransform>().sizeDelta;

            //// Panel Creation \\\\


            // Screen

            MissileScreenUIPatching.renderTexture = new((int)profile.missilePanelSize.x, (int)profile.missilePanelSize.y, 16, RenderTextureFormat.ARGB32);

            profile.screen = new("missileScreen");
            profile.screen.transform.parent = profile.missilePanel;
            profile.screen.transform.localPosition = Vector3.zero;
            profile.screen.transform.localRotation = Quaternion.Euler(0, 0, 0);
            profile.screen.transform.localScale = new Vector3(1, 1, 0);
            profile.screen.SetActive(false);

            RectTransform screenRect = profile.screen.AddComponent<RectTransform>();
            screenRect.anchorMin = Vector2.zero;
            screenRect.anchorMax = Vector2.one;
            screenRect.sizeDelta = Vector2.zero;


            RawImage screenImage = profile.screen.AddComponent<RawImage>();
            screenImage.texture = MissileScreenUIPatching.renderTexture;
            screenImage.color = Color.white;

      

            // Flight Path

            profile.velocityVector = new(
                name: "FlightPath",
                UIParent: profile.missilePanel.transform,
                sprite: PluginSprites.attackSprite,
                imageColor: PluginConfig.velocityVectorColor.Value,
                imageScale: new(profile.velocityVectorIconScale, profile.velocityVectorIconScale),
                createOutline: true,
                outlineColor: PluginConfig.velocityVectorOutlineColor.Value,
                outlineThickness: new(PluginConfig.velocityVectorOutlineThickness.Value, PluginConfig.velocityVectorOutlineThickness.Value)
            );
            




            // Orienation Indicator

            profile.orientationIndicator = new(
                name: "orientationIndicator",
                UIParent: profile.velocityVector.GetGameObject().transform,
                sprite: PluginSprites.orientationSprite,
                imageColor: PluginConfig.orientationIndicatorColor.Value,
                imageScale: new Vector3(3, 1, 0),
                createOutline: true,
                outlineColor: PluginConfig.orientationIndicatorOutlineColor.Value,
                outlineThickness: new(PluginConfig.orientationIndicatorOutlineThickness.Value, PluginConfig.orientationIndicatorOutlineThickness.Value)
            );
          


            // Lead Icon
           
            profile.leadIcon = new(
                name:"leadIcon",
                UIParent:profile.missilePanel.transform,
                sprite:PluginSprites.leadSprite,
                imageColor:PluginConfig.leadIconColor.Value,
                imageScale:new(profile.leadIconScale, profile.leadIconScale),
                createOutline:true,
                outlineColor:PluginConfig.leadIconOutlineColor.Value,
                outlineThickness:new(PluginConfig.leadIconOutlineThickness.Value, PluginConfig.leadIconOutlineThickness.Value)
            );



            // Lockbox

            profile.lockBox = new("lockBox", typeof(RectTransform), typeof(Outline));
            profile.lockBox.transform.parent = profile.missilePanel.transform;
            profile.lockBox.transform.localPosition = Vector3.zero;
            profile.lockBox.transform.localRotation = Quaternion.Euler(0, 0, 0);
            profile.lockBox.transform.localScale = new Vector3(profile.leadIconScale, profile.leadIconScale, 0);
            profile.lockBox.GetComponent<RectTransform>().sizeDelta = new(profile.lockboxMinSize, profile.lockboxMinSize);


            CreateLockBoxCorner(profile.lockBox.transform, new(profile.lockboxCornerScale, profile.lockboxCornerScale, 0), new(0, 1), new(0, 1));
            CreateLockBoxCorner(profile.lockBox.transform, new(-profile.lockboxCornerScale, profile.lockboxCornerScale, 0), new(1, 1), new(0, 1));

            CreateLockBoxCorner(profile.lockBox.transform, new(-profile.lockboxCornerScale, -profile.lockboxCornerScale, 0), new(1, 0), new(0, 1));
            CreateLockBoxCorner(profile.lockBox.transform, new(profile.lockboxCornerScale, -profile.lockboxCornerScale, 0), new(0, 0), new(0, 1));



            // Missile Name 


            profile.missileName = new(

                name: "missileName",
                position: new(5, -5),
                UIParent: profile.missilePanel,
                text: "No Missile",
                fontSize: profile.fontSize,
                textColor: PluginConfig.missileNameColor.Value

             );


            profile.missileName.SetAnchorPivot(
                new(0, 1),
                new(0, 1),
                new(0, 1)
            );




           // Missile Index

           profile.missileIndex = new(

                name:"missileIndex", 
                position:new(5, 0),
                UIParent:profile.missilePanel,
                text:"0/0",
                fontSize:Mathf.RoundToInt(profile.fontSize / 1.3f), 
                textColor:Color.white

           );
  

           
            profile.missileIndex.SetAnchorPivot(
                new(0, 1),
                new(0, 1),
                new(0, profile.leftPanelPivotYOffset)
            );

            

            // Missile Target 

            profile.missileTargetName = new(

                name: "missileTargetName",
                position: new(-5, -5),
                UIParent: profile.missilePanel,
                text: "No Target",
                fontSize: Mathf.RoundToInt(profile.fontSize / 1.5f),
                textColor: Color.white

            );

            profile.missileTargetName.SetAnchorPivot(
                new(1, 1),
                new(1, 1),
                new(1, 1)
            );




            // Missile Speed

            profile.missileSpeed = new(

                name: "missileSpeed",
                position: new(-5, 0),
                UIParent: profile.missilePanel,
                text: "SPD ---",
                fontSize: Mathf.RoundToInt(profile.fontSize / 1.5f)
               
                );

            profile.missileSpeed.SetAnchorPivot(
                new(1, 1),
                new(1, 1),
                new(1, profile.rightPanelPivotYOffset)
                );

    



            // Missile Altitude

            profile.missileAltitude = new(

                name: "missileAltitude",
                position: new(-5, 0),
                UIParent: profile.missilePanel,
                text: "ALT ---",
                fontSize: Mathf.RoundToInt(profile.fontSize / 1.5f)
               
                );

            profile.missileAltitude.SetAnchorPivot(
                new(1, 1),
                new(1, 1),
                new(1, profile.rightPanelPivotYOffset + profile.rightPanelPivotYOffsetIncrement)
            ); 

       



            // Missile Range

            profile.missileRange = new(

                name: "missileRange",
                position: new(-5, 0),
                UIParent: profile.missilePanel,
                text: "RNG ---",
                fontSize: Mathf.RoundToInt(profile.fontSize / 1.5f)

            );

            profile.missileRange.SetAnchorPivot(
              new(1, 1),
              new(1, 1),
              new(1, profile.rightPanelPivotYOffset + profile.rightPanelPivotYOffsetIncrement * 2)
             );

           

            currentProfile = profile;

        }



        private static void CreateLockBoxCorner(Transform parent, Vector3 Scale, Vector2 anchor, Vector2 pivot)
        {
            GameObject lockCorner = new();
            lockCorner.name = "lockBoxCorner";
            lockCorner.transform.parent = parent;
            lockCorner.transform.localPosition = Vector3.zero;
            lockCorner.transform.localRotation = Quaternion.Euler(0, 0, 0);
            lockCorner.transform.localScale = Scale;

            lockCorner.AddComponent<Image>().sprite = PluginSprites.lockCornerSprite;
            lockCorner.GetComponent<Image>().color = PluginConfig.lockBoxColor.Value;
            RectTransform cornerTLRect = lockCorner.GetComponent<RectTransform>();
            cornerTLRect.anchorMin = anchor;
            cornerTLRect.anchorMax = anchor;
            cornerTLRect.pivot = pivot;
            Outline lockCornerOutline = lockCorner.AddComponent<Outline>();
            lockCornerOutline.effectColor = PluginConfig.lockBoxOutlineColor.Value;
            lockCornerOutline.effectDistance = new(PluginConfig.lockBoxOutlineThickness.Value, PluginConfig.lockBoxOutlineThickness.Value);
        }

    }
}
