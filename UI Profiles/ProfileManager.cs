using MissileView.Patches;
using MissileView.UI_Profiles.AircraftProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MissileView.UI
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
                Plugin.Logger.LogInfo($"Profile For \"{aircraftName}\" Added ");
            }
        }

        public static void LoadProfiles()
        {

            List<MethodInfo> profiles = typeof(ProfileManager).Assembly.GetTypes().Where(val => val.Namespace == "MissileView.UI_Profiles.AircraftProfiles").Select(method => method.GetMethod("Load")).ToList();


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
                return LoadedProfiles[name];
            } else
            {
                Plugin.Logger.LogError($"Couldn't find Profile \"{name}\". Refering to Default Profile");
                return defaultProfile;
                
            }
        }

        public static void InjectProfileUI(string AircraftName, TacScreen tacScreenInstance, Aircraft aircraft )
        {
            Profile profile = GetProfileFromName(AircraftName);

            if (profile == null )
            {
                Plugin.Logger.LogError($"Couldn't find Profile \"{AircraftName}\" ");
               
                return; // If the default profile is somehow not returned, then exit
            }

            

            profile.weaponPanel = GameUtils.FindChildRecursive(tacScreenInstance.transform,profile.ReplacePanelName);

            if (profile.weaponPanel == null)
            {

                return; // If the plane doesn't have the panel, then exit

            }

            PatchTacScreen.isPlaneCompatible = true;

           

            if (profile.weaponPanel.GetComponent<HorizontalOrVerticalLayoutGroup>() != null)
            {
                UnityEngine.Object.Destroy(profile.weaponPanel.GetComponent<HorizontalOrVerticalLayoutGroup>());
            }


            profile.missilePanel = UnityEngine.Object.Instantiate(profile.weaponPanel, profile.weaponPanel.transform.parent);
            profile.missilePanel.name = "missilePanel";
            PatchTacScreen.missilePanelSize = profile.missilePanel.GetComponent<RectTransform>().sizeDelta;




            foreach (Transform child in profile.missilePanel)
            {
                UnityEngine.Object.Destroy(child.gameObject); // Removing all the children in the new missile panel that was cloned
            }

            //Component[] panelComponents = profile.missilePanel.GetComponents<Component>();

            //foreach (Component component in panelComponents)
            //{
            //    if (!)
            //}

            if (!profile.clearOldPanel)
            {
                profile.weaponPanel.gameObject.SetActive(false); // Clearing of the old panel that was instantiated
            }


            if (profile.missilePanel.GetComponent<GridLayoutGroup>() != null)
            {
                UnityEngine.Object.Destroy(profile.missilePanel.GetComponent<GridLayoutGroup>());
                UnityEngine.Object.Destroy(profile.missilePanel.GetComponent<SystemStatusDisplay>());

            }
            // Aircraft specific panel configurations
            // Any other aircraft that isn't in these if statement uses the normal configuration


            profile.missilePanel.GetComponent<RectTransform>().sizeDelta = profile.missilePanelRectSize != Vector2.zero ? profile.missilePanelRectSize : profile.missilePanel.GetComponent<RectTransform>().sizeDelta;
            profile.missilePanel.GetComponent<RectTransform>().anchoredPosition = profile.missilePanelRectPosition != Vector2.zero ? profile.missilePanelRectPosition : profile.missilePanel.GetComponent<RectTransform>().anchoredPosition;
            profile.missilePanel.localRotation = profile.missilePanelRectRotation != Profile.rotationDefault ? profile.missilePanelRectRotation : profile.missilePanel.localRotation;

            
            PatchTacScreen.renderTexture = new((int)PatchTacScreen.missilePanelSize.x, (int)PatchTacScreen.missilePanelSize.y, 16, RenderTextureFormat.ARGB32);
            
            GameObject screen = new("missileScreen");
            screen.transform.parent = profile.missilePanel;
            screen.transform.localPosition = Vector3.zero;
            screen.transform.localRotation = Quaternion.Euler(0, 0, 0);
            screen.transform.localScale = new Vector3(1, 1, 0);


            RectTransform screenRect = screen.AddComponent<RectTransform>();
            screenRect.anchorMin = Vector2.zero;
            screenRect.anchorMax = Vector2.one;
            screenRect.sizeDelta = Vector2.zero;


            RawImage screenImage = screen.AddComponent<RawImage>();
            screenImage.texture = PatchTacScreen.renderTexture;
            screenImage.color = Color.white;


            // Flight Path

            profile.velocityVector = new("FlightPath", typeof(RectTransform));
            profile.velocityVector.transform.parent = profile.missilePanel;
            profile.velocityVector.transform.localPosition = Vector3.zero;
            profile.velocityVector.transform.localRotation = Quaternion.Euler(0, 0, 0);
            profile.velocityVector.transform.localScale = new Vector3(profile.velocityVectorIconScale, profile.velocityVectorIconScale, 0);

            GameObject velImage = UnityEngine.Object.Instantiate(profile.velocityVector, Vector3.zero, Quaternion.identity, profile.velocityVector.transform);
            velImage.name = "Attack Vector";
            velImage.transform.localPosition = Vector3.zero;
            velImage.transform.localRotation = Quaternion.identity;

            Image velImageComp = velImage.AddComponent<Image>();
            velImageComp.sprite = PluginSprites.AttackSprite;
            velImageComp.color = Color.green;
            velImage.AddComponent<Outline>();

            // Orienation Indicator

            profile.orientationIndicator = new("orientationIndicator", typeof(Image));
            profile.orientationIndicator.transform.parent = profile.velocityVector.transform;
            profile.orientationIndicator.transform.localPosition = Vector3.zero;
            profile.orientationIndicator.transform.localRotation = Quaternion.Euler(0, 0, 0);
            profile.orientationIndicator.transform.localScale = new Vector3(2f, 0.7f, 0);

            Image orientationImageComp = profile.orientationIndicator.GetComponent<Image>();
            orientationImageComp.sprite = PluginSprites.OrientationSprite;
            orientationImageComp.color = Color.green;


            profile.orientationIndicator.AddComponent<Outline>();

            // Lockbox

            profile.lockBox = new("lockBox", typeof(RectTransform), typeof(Outline));
            profile.lockBox.transform.parent = profile.missilePanel.transform;
            profile.lockBox.transform.localPosition = Vector3.zero;
            profile.lockBox.transform.localRotation = Quaternion.Euler(0, 0, 0);
            profile.lockBox.transform.localScale = new Vector3(profile.leadIconScale, profile.leadIconScale, 0);


            CreateLockBoxCorner(profile.lockBox.transform, new(profile.lockboxCornerScale, profile.lockboxCornerScale, 0), new(0, 1), new(0, 1));
            CreateLockBoxCorner(profile.lockBox.transform, new(-profile.lockboxCornerScale, profile.lockboxCornerScale, 0), new(1, 1), new(0, 1));

            CreateLockBoxCorner(profile.lockBox.transform, new(-profile.lockboxCornerScale, -profile.lockboxCornerScale, 0), new(1, 0), new(0, 1));
            CreateLockBoxCorner(profile.lockBox.transform, new(profile.lockboxCornerScale, -profile.lockboxCornerScale, 0), new(0, 0), new(0, 1));


            // Lead Icon
            profile.leadIcon = new("leadIcon", typeof(Image), typeof(Outline));
            profile.leadIcon.transform.parent = profile.missilePanel.transform;
            profile.leadIcon.transform.localPosition = Vector3.zero;
            profile.leadIcon.transform.localRotation = Quaternion.identity;
            profile.leadIcon.transform.localScale = new(profile.leadIconScale, profile.leadIconScale,0);
            Image leadImage = profile.leadIcon.GetComponent<Image>();
            leadImage.sprite = PluginSprites.leadSprite;
            leadImage.color = new(0, 1, 0);


            // Missile Name 

            profile.missileName = new("missileName", new(0, 0), profile.missilePanel);
            profile.missileName.SetText("No Missile");
            profile.missileName.SetColor(new(1f, 0f, 1f));
            profile.missileName.SetFontSize(profile.fontSize);
            profile.missileName.SetTextAlignment(TextAnchor.MiddleLeft);

            RectTransform nameRect = profile.missileName.GetRectTransform();
            nameRect.anchorMin = new(0, 1);
            nameRect.anchorMax = new(0, 1);
            nameRect.pivot = new(0, 1);
            nameRect.anchoredPosition = new(5, -5);


            // Missile Index

            profile.missileIndex = new("missileIndex", new(0, 0), profile.missilePanel);
            profile.missileIndex.SetText("0/0");
            profile.missileIndex.SetFontSize(Mathf.RoundToInt(profile.fontSize / 1.3f));
            profile.missileIndex.SetColor(Color.white);
            profile.missileIndex.SetTextAlignment(TextAnchor.MiddleLeft);


            RectTransform indexRect = profile.missileIndex.GetRectTransform();
            indexRect.anchorMin = new(0, 1);
            indexRect.anchorMax = new(0, 1);
            indexRect.pivot = new(0, profile.LeftPanelPivotYOffset);
            indexRect.anchoredPosition = new(5, 0);


            // Missile Target 

            profile.missileTargetName = new("missileTargetName", new(0, 0), profile.missilePanel);
            profile.missileTargetName.SetText("No Target");
            profile.missileTargetName.SetColor(new(1, 1, 1));
            profile.missileTargetName.SetFontSize(Mathf.RoundToInt(profile.fontSize / 1.5f));
            profile.missileTargetName.SetTextAlignment(TextAnchor.MiddleRight);

            RectTransform targetRect = profile.missileTargetName.GetRectTransform();
            targetRect.anchorMin = new(1, 1);
            targetRect.anchorMax = new(1, 1);
            targetRect.pivot = new(1, 1);
            targetRect.anchoredPosition = new(-5, -5);


            // Missile Speed

            profile.missileSpeed = new("missileSpeed", new(0, 0), profile.missilePanel);
            profile.missileSpeed.SetText("SPD --");
            profile.missileSpeed.SetFontSize(Mathf.RoundToInt(profile.fontSize / 1.5f));
            profile.missileSpeed.SetColor(Color.white);

            profile.missileSpeed.SetTextAlignment(TextAnchor.MiddleRight);

            RectTransform speedRect = profile.missileSpeed.GetRectTransform();
            speedRect.anchorMin = new(1, 1);
            speedRect.anchorMax = new(1, 1);
            speedRect.pivot = new(1, profile.RightPanelPivotYOffset);
            speedRect.anchoredPosition = new(-5, 0);

            // Missile Altitude

            profile.missileAltitude = new("missileAltitude", new(0, 0), profile.missilePanel);
            profile.missileAltitude.SetText("ALT --");
            profile.missileAltitude.SetFontSize(Mathf.RoundToInt(profile.fontSize / 1.5f));
            profile.missileAltitude.SetColor(Color.white);
            profile.missileAltitude.SetTextAlignment(TextAnchor.MiddleRight);

            RectTransform altitudeRect = profile.missileAltitude.GetRectTransform();
            altitudeRect.anchorMin = new(1, 1);
            altitudeRect.anchorMax = new(1, 1);
            altitudeRect.pivot = new(1, profile.RightPanelPivotYOffset + profile.RightPanelPivotYOffsetIncrement);
            altitudeRect.anchoredPosition = new(-5, 0);

            // Missile Range

            profile.missileRange = new("missileRange", new(0, 0), profile.missilePanel);
            profile.missileRange.SetText("RNG --");
            profile.missileRange.SetFontSize(Mathf.RoundToInt(profile.fontSize / 1.5f));
            profile.missileRange.SetColor(Color.white);
            profile.missileRange.SetTextAlignment(TextAnchor.MiddleRight);

            RectTransform rangeRect = profile.missileRange.GetRectTransform();
            rangeRect.anchorMin = new(1, 1);
            rangeRect.anchorMax = new(1, 1);
            rangeRect.pivot = new(1, profile.RightPanelPivotYOffset + profile.RightPanelPivotYOffsetIncrement * 2);
            rangeRect.anchoredPosition = new(-5,0);


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
            lockCorner.GetComponent<Image>().color = new(0f, 1f, 1f);
            RectTransform cornerTLRect = lockCorner.GetComponent<RectTransform>();
            cornerTLRect.anchorMin = anchor;
            cornerTLRect.anchorMax = anchor;
            cornerTLRect.pivot = pivot;
            lockCorner.AddComponent<Outline>();
        }

    }
}
