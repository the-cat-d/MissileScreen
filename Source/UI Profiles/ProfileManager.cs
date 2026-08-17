using MissileScreen.Patches;
using MissileScreen.Source;
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
        public static Profile currentProfile;
        private static readonly Profile _defaultProfile = new Profile();
       

        private static readonly Dictionary<string,Profile> _loadedProfiles = new Dictionary<string,Profile>(StringComparer.Ordinal);

        public static void AddProfile(Profile profile,string aircraftName)
        {
  
            if (profile == null)
            {
                Plugin.logger.LogError($"Aircraft Profile is null!");
            }
            else
            {
                _loadedProfiles[aircraftName] = profile;
                Plugin.logger.LogDebug($"Profile For \"{aircraftName}\" Added");
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
               }
               catch
               {
                   // ignored
               }
            }
        }

        private static Profile GetProfileFromName(string name)
        {

            if (_loadedProfiles.TryGetValue(name,out Profile profile))
            {
                Plugin.logger.LogDebug($"Found \"{name}\" Profile, Loading...");
                return profile;
            } else
            {
                Plugin.logger.LogWarning($"Couldn't find Profile \"{name}\". Referring to Default Profile");
                return _defaultProfile;
                
            }
        }

        public static void InjectProfileUI(string aircraftName, GameObject mainInterface)
        {

            Profile profile = GetProfileFromName(aircraftName);

            //// Validation Checks \\\\

            if (profile == null )
            {
                Plugin.logger.LogError($"Couldn't find Profile \"{aircraftName}\" ");
               
                return; // If the default profile is somehow not returned, then exit
            }


            Transform foundPanelHUD = SceneSingleton<FlightHud>.i.HMDCenter.transform.Find("missilePanel"); 
            
            if (foundPanelHUD)
            {
                GameObject.Destroy(foundPanelHUD.gameObject);
            }

            profile.weaponPanel = GameUtils.FindChildRecursive(mainInterface.transform,profile.replacePanelName);

            if (profile.weaponPanel == null && PluginConfig.hmdMissileScreen.Value == false)
            {

                return; // If the plane doesn't have the panel, then exit

            }

            MissileScreenUIPatching.isPlaneCompatible = true;


            //// Missile Panel Creation \\\\

            // Missile Panel GameObject

            if (PluginConfig.hmdMissileScreen.Value == false)
            {
                // TacScreen Panel Creation
                
                profile.missilePanel = UnityEngine.Object.Instantiate(profile.weaponPanel, profile.weaponPanel.transform.parent);
                profile.missilePanel.name = "missilePanel";
                
                // Clear Missile Panel Children

                foreach (Transform child in profile.missilePanel)
                {
                    Object.Destroy(child.gameObject); 
                }
                
                // Clear Missile Panel Components (besides RectTransform and CanvasRenderer)


                foreach (Component component in profile.missilePanel.GetComponents<Component>())
                {
                    if (component is not RectTransform && component is not CanvasRenderer) {
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
                        Transform hideGameObject = GameUtils.FindChildRecursive(mainInterface.transform, profile.hideGameObjectNames[i]);
                        if (hideGameObject != null)
                        {
                            profile.hideGameObjects.Add(hideGameObject.gameObject);
                        
                        }
                    }
                
                }
                
            }
            else
            {
                // HMD Panel Creation
                
                GameObject newPanel = new GameObject("missilePanel",typeof(RectTransform));
                newPanel.transform.SetParent(mainInterface.transform);  
                newPanel.transform.localPosition = Vector3.zero;
                newPanel.transform.localRotation = Quaternion.identity;
                newPanel.transform.localScale = Vector3.one * PluginConfig.hmdMissileScreenScale.Value;
                
                newPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(430,250);
                newPanel.GetComponent<RectTransform>().anchoredPosition = PluginConfig.hmdMissileScreenPosition.Value;
                
                
                profile.missilePanel = newPanel.transform;
            }
           


            //// Panel Configuration \\\\
            // These will just use the values it had before if the profile values are unassigned

            // Missile Panel Size
            profile.missilePanel.GetComponent<RectTransform>().sizeDelta = profile.missilePanelRectSize != Vector2.zero ? profile.missilePanelRectSize : profile.missilePanel.GetComponent<RectTransform>().sizeDelta;

            // Missile Panel Position
            profile.missilePanel.GetComponent<RectTransform>().anchoredPosition = profile.missilePanelRectPosition != Profile.positionDefault ? profile.missilePanelRectPosition : profile.missilePanel.GetComponent<RectTransform>().anchoredPosition;

            // Missile Panel Rotation
            profile.missilePanel.localRotation = profile.missilePanelRectRotation != Profile.rotationDefault ? profile.missilePanelRectRotation : profile.missilePanel.localRotation;

            //Hierarchy - currently unused, keeping it just in case if I need it

            profile.missilePanel.SetSiblingIndex(profile.hierarchyOrder != -1 ? profile.hierarchyOrder : profile.missilePanel.GetSiblingIndex());

            profile.missilePanelSize = profile.missilePanel.GetComponent<RectTransform>().sizeDelta;

            //// Panel Creation \\\\

            
            // Screen

            MissileScreenUIPatching.renderTexture = new((int)profile.missilePanelSize.x, (int)profile.missilePanelSize.y, 16, RenderTextureFormat.ARGB32);

            profile.screen = new("missileScreen")
            {
                transform =
                {
                    parent = profile.missilePanel,
                    localPosition = Vector3.zero,
                    localRotation = Quaternion.Euler(0, 0, 0),
                    localScale = new Vector3(1, 1, 0),
                },
            };
            profile.screen.SetActive(false);

            RectTransform screenRect = profile.screen.AddComponent<RectTransform>();
            screenRect.anchorMin = Vector2.zero;
            screenRect.anchorMax = Vector2.one;
            screenRect.sizeDelta = Vector2.zero;


            RawImage screenImage = profile.screen.AddComponent<RawImage>();
            screenImage.texture = MissileScreenUIPatching.renderTexture;
            screenImage.color = Color.white;

            // Icons \\

            // Flight Path

            profile.velocityVector = new(
                newName: "FlightPath",
                uiParent: profile.missilePanel.transform,
                sprite: PluginSprites.attackSprite,
                imageColor: PluginConfig.velocityVectorColor.Value,
                imageScale: new(profile.velocityVectorIconScale, profile.velocityVectorIconScale),
                createOutline: true,
                outlineColor: PluginConfig.velocityVectorOutlineColor.Value,
                outlineThickness: new(PluginConfig.velocityVectorOutlineThickness.Value, PluginConfig.velocityVectorOutlineThickness.Value)
            );
            




            // Orientation Indicator

            profile.orientationIndicator = new(
                newName: "orientationIndicator",
                uiParent: profile.velocityVector.GetGameObject().transform,
                sprite: PluginSprites.orientationSprite,
                imageColor: PluginConfig.orientationIndicatorColor.Value,
                imageScale: new Vector3(3, 1, 0),
                createOutline: true,
                outlineColor: PluginConfig.orientationIndicatorOutlineColor.Value,
                outlineThickness: new(PluginConfig.orientationIndicatorOutlineThickness.Value, PluginConfig.orientationIndicatorOutlineThickness.Value)
            );
          


            // Lead Icon
           
            profile.leadIcon = new(
                newName:"leadIcon",
                uiParent:profile.missilePanel.transform,
                sprite:PluginSprites.leadSprite,
                imageColor:PluginConfig.leadIconColor.Value,
                imageScale:new(profile.leadIconScale, profile.leadIconScale),
                createOutline:true,
                outlineColor:PluginConfig.leadIconOutlineColor.Value,
                outlineThickness:new(PluginConfig.leadIconOutlineThickness.Value, PluginConfig.leadIconOutlineThickness.Value)
            );



            // Lockbox

            profile.lockBox = new GameObject("lockBox", typeof(RectTransform), typeof(Outline))
            {
                transform =
                {
                    parent = profile.missilePanel.transform,
                    localPosition = Vector3.zero,
                    localRotation = Quaternion.Euler(0, 0, 0),
                    localScale = new Vector3(profile.leadIconScale, profile.leadIconScale, 0),
                },
            };
            profile.lockBox.GetComponent<RectTransform>().sizeDelta = new(profile.lockboxMinSize, profile.lockboxMinSize);


            CreateLockBoxCorner(profile.lockBox.transform, new(profile.lockboxCornerScale, profile.lockboxCornerScale, 0), new(0, 1), new(0, 1));
            CreateLockBoxCorner(profile.lockBox.transform, new(-profile.lockboxCornerScale, profile.lockboxCornerScale, 0), new(1, 1), new(0, 1));

            CreateLockBoxCorner(profile.lockBox.transform, new(-profile.lockboxCornerScale, -profile.lockboxCornerScale, 0), new(1, 0), new(0, 1));
            CreateLockBoxCorner(profile.lockBox.transform, new(profile.lockboxCornerScale, -profile.lockboxCornerScale, 0), new(0, 0), new(0, 1));


            
            // Labels \\
            
            // Missile Name 

            if (!PluginConfig.hideMissileName.Value)
            {
                profile.missileName = new(

                    newName: "missileName",
                    position: new(5, -5),
                    uiParent: profile.missilePanel,
                    text: "No Missile",
                    fontSize: Mathf.RoundToInt(profile.fontSize * PluginConfig.globalFontScale.Value),
                    textColor: PluginConfig.missileNameColor.Value,
                    alignElement: false

                    );


                profile.missileName.SetAnchorPivot(
                    new(0, 1),
                    new(0, 1),
                    new(0, 1)
                    );

            } else
            {
                UIDraw.lastElement = null;
            }




            // Missile Index

           if (!PluginConfig.hideMissileIndex.Value)
           {
               profile.missileIndex = new(

                   newName:"missileIndex", 
                   position:new(5,-5),
                   uiParent:profile.missilePanel,
                   text:"0/0",
                   fontSize:Mathf.RoundToInt((profile.fontSize / 1.3f) * PluginConfig.globalFontScale.Value), 
                   textColor:Color.white

                   );
            
               profile.missileIndex.SetAnchorPivot(
                   new(0, 1),
                   new(0, 1),
                   new(0, 1)
                   );

           } else
           {
               UIDraw.lastElement = null;
           }


           

           // Missile Target 
            
            if (!PluginConfig.hideTargetName.Value)
            {
                profile.missileTargetName = new(

                    newName: "missileTargetName",
                    position: new(-5, -5),
                    uiParent: profile.missilePanel,
                    text: "No Target",
                    fontSize: Mathf.RoundToInt((profile.fontSize / 1.5f) * PluginConfig.globalFontScale.Value),
                    textColor: Color.white,
                
                    alignElement: false

                    );

                profile.missileTargetName.SetAnchorPivot(
                    new(1, 1),
                    new(1, 1),
                    new(1, 1)
                    );
            }
            else
            {
                UIDraw.lastElement = null;
            }
            
            

            // Missile Speed
            
            if (!PluginConfig.hideMissileSpeed.Value)
            {
                profile.missileSpeed = new(

                    newName: "missileSpeed",
                    position: new(-5, -5),
                    uiParent: profile.missilePanel,
                    text: "SPD ---",
                    fontSize: Mathf.RoundToInt((profile.fontSize / 1.5f) * PluginConfig.globalFontScale.Value)
               
                    );
                
                profile.missileSpeed.SetAnchorPivot(
                    new(1, 1),
                    new(1, 1),
                    new(1, 1)
                    );
            } 
            


            // Missile Altitude

            if (!PluginConfig.hideMissileAltitude.Value)
            {
                profile.missileAltitude = new(

                    newName: "missileAltitude",
                    position: new(-5, -5),
                    uiParent: profile.missilePanel,
                    text: "ALT ---",
                    fontSize: Mathf.RoundToInt((profile.fontSize / 1.5f) * PluginConfig.globalFontScale.Value)

                    );

                profile.missileAltitude.SetAnchorPivot(
                    new(1, 1),
                    new(1, 1),
                    new(1, 1)
                    );
            } 



            // Missile Range

            if (!PluginConfig.hideMissileRange.Value)
            {

                profile.missileRange = new(

                    newName: "missileRange",
                    position: new(-5, -5),
                    uiParent: profile.missilePanel,
                    text: "RNG ---",
                    fontSize: Mathf.RoundToInt((profile.fontSize / 1.5f) * PluginConfig.globalFontScale.Value)

                    );

                profile.missileRange.SetAnchorPivot(
                    new(1, 1),
                    new(1, 1),
                    new(1, 1)
                    );
            }

            currentProfile = profile;

        }
       




        private static void CreateLockBoxCorner(Transform parent, Vector3 Scale, Vector2 anchor, Vector2 pivot)
        {
            GameObject lockCorner = new()
            {
                name = "lockBoxCorner",
                transform =
                {
                    parent = parent,
                    localPosition = Vector3.zero,
                    localRotation = Quaternion.Euler(0, 0, 0),
                    localScale = Scale,
                },
            };

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
