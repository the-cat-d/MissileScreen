using HarmonyLib;
using MissileScreen.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


namespace MissileScreen.Patches
{
    public static class MissilePatching
    {

        public static readonly List<Missile> Missiles = new List<Missile>();
        public static int currentMissileIndex;
        private static  Camera _currentMissileCamera;

        
        public static class MiscMissilePatches
        {
            [HarmonyPatch(typeof(SubmunitionDispenser), "JettisonCasings")]
            public static class OnClusterDestroy
            {
                static void Postfix(SubmunitionDispenser __instance)
                {
                    
                    try
                    {
                        OnDetonate(__instance.gameObject.GetComponent<Missile>());
                    }
                    catch (System.Exception error)
                    {
                        Plugin.Logger.LogError($"Error in MissilePatches.OnClusterDestroy: {error.Message}\nMissile: {__instance.gameObject.GetComponent<Missile>()}");
                    }
                }

            }
        }

        public static class MissilePatches 
        {

            [HarmonyPatch(typeof(Missile), "StartMissile")]
            public static class OnMissileLaunch
            {

                static void Postfix(Missile __instance)
                {

                   try
                   {
                        OnMissileStart(__instance);
                   } catch (System.Exception error)
                   {
                        Plugin.Logger.LogError($"Missile Launch Error ({__instance.unitName}): {error.Message}{error.StackTrace}");
                   }

                }
            }


            [HarmonyPatch(typeof(Missile), "UserCode_RpcDetonate_897349600")]
            public static class OnMissileDestroy
            {
                static void Postfix(Missile __instance)
                {
                   
                    try
                    {
                        OnDetonate(__instance);
                    }
                    catch (System.Exception error)
                    {
                        Plugin.Logger.LogError($"Missile Detonation Error ({__instance.unitName}): {error.Message}{error.StackTrace}");
                    }
                }

            }

        }

        public static class TargetCamPatches
        {

            [HarmonyPatch(typeof(TargetCam), "OnBeginCameraRendering")]
            public class TargetCamBeginUpdate
            {
                static bool Prefix(TargetCam __instance, ScriptableRenderContext context, Camera camera)
                {
                    if (camera == __instance.cam || camera == MissilePatching._currentMissileCamera)
                    {
                        RenderSettings.fog = !__instance.IRMode;
                    }
                    return false;
                }
            }

            [HarmonyPatch(typeof(TargetCam), "OnEndCameraRendering")]
            public static class TargetCamEndUpdate
            {
                static bool Prefix(TargetCam __instance, ScriptableRenderContext context, Camera camera)
                {
                    if (camera == __instance.cam || camera == MissilePatching._currentMissileCamera)
                    {
                        RenderSettings.fog = true;
                    }
                    return false;
                }
            }
        }



        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

       

        private static void OnMissileStart(Missile __instance)
        {
            if (GameUtils.GetAircraft() != __instance.owner || GameUtils.GetAircraft() == null || MissileScreenUIPatching.renderTexture == null)
            {
                return; // missile isnt owned by player
            }

            Camera missileCam = __instance.transform.GetComponentInChildren<Camera>(true);

            if (missileCam == null) {
                GameObject missileCamObj = new GameObject("missileCam", typeof(Camera));
                missileCamObj.transform.parent = __instance.transform;
                missileCam = missileCamObj.GetComponent<Camera>();
                missileCam.transform.localRotation = Quaternion.identity;

            }

            if (missileCam != null)
            {
                if (Missiles.Count > 0 && currentMissileIndex <= Missiles.Count - 1)
                {

                    Missiles[currentMissileIndex].transform.GetComponentInChildren<Camera>(true).gameObject.SetActive(false); // Disable the camera of any previous missile to avoid lag

                }


                _currentMissileCamera = missileCam;

                Plugin.Logger.LogDebug(PluginConfig.cameraRenderDistance.Value);
                missileCam.farClipPlane = PluginConfig.cameraRenderDistance.Value;
                missileCam.fieldOfView = PluginConfig.cameraFOV.Value;
                missileCam.transform.localPosition = PluginConfig.missileCameraOffset.Value;
                missileCam.transform.rotation = Quaternion.Euler(missileCam.transform.rotation.eulerAngles.x, missileCam.transform.rotation.eulerAngles.y, GameUtils.GetAircraft().transform.rotation.eulerAngles.z);

                UniversalAdditionalCameraData missileCamData;

                if (!missileCam.TryGetComponent(out missileCamData))
                {
                    missileCamData = missileCam.gameObject.AddComponent<UniversalAdditionalCameraData>();
                } 

                missileCamData.renderPostProcessing = true;
                missileCamData.volumeLayerMask = 256;

                Component.Destroy(missileCam.GetComponent<AudioListener>());

                

                missileCam.targetTexture = MissileScreenUIPatching.renderTexture;

                missileCam.gameObject.SetActive(true);
                ProfileManager.currentProfile.screen.SetActive(true);
           
                ProfileManager.currentProfile.missileName.SetText(__instance.unitName);

                Missiles.Add(__instance);

                currentMissileIndex = Missiles.Count - 1;


            }


            Plugin.Logger.LogMessage("Missile Launched " + __instance.name);
        }

        private static void OnDetonate(Missile __instance)
        {
            if (GameUtils.GetAircraft() != __instance.owner || GameUtils.GetAircraft() == null) return; // missile isn't owned by player



            int removedIndex = Missiles.IndexOf(__instance);

            if (removedIndex == -1) return;

            bool wasViewing = removedIndex == currentMissileIndex;

            Camera missileCam = __instance.transform.GetComponentInChildren<Camera>(true);

            if (missileCam != null)
            {
                Object.Destroy(missileCam.gameObject);
            }

            Missiles.Remove(__instance);

            if (Missiles.Count == 0)
            {
                currentMissileIndex = 0;
                ProfileManager.currentProfile.NoMissileDisplay();

                return;
            }

            if (wasViewing)
            {
                if (removedIndex < Missiles.Count)
                {
                    currentMissileIndex = removedIndex;
                }
                else
                {
                    currentMissileIndex = Missiles.Count - 1;
                }

                ActivateMissile(currentMissileIndex);
            }
            else if (removedIndex < currentMissileIndex)
            {
                currentMissileIndex--;
            }



            Plugin.Logger.LogMessage("Missile Detonated " + __instance.name);


        }


        public static void ActivateMissile(int index)
        {
            if (index < 0 || index >= Missiles.Count)
            { return; }

            Camera missileCam = Missiles[index].transform.GetComponentInChildren<Camera>(true);

            if (missileCam != null)
            {
                missileCam.targetTexture = MissileScreenUIPatching.renderTexture;
                missileCam.gameObject.SetActive(true);

                ProfileManager.currentProfile.missileName.SetText(Missiles[currentMissileIndex].unitName);
                ProfileManager.currentProfile.missileIndex.SetActive(true);
                ProfileManager.currentProfile.missileIndex.SetText($"{currentMissileIndex + 1}/{Missiles.Count}");
               
            }
        }

        public static void CycleMissileViewUp()
        {
           
            Plugin.Logger.LogDebug($"count:{Missiles.Count}, index: {currentMissileIndex}");
            if (currentMissileIndex >= 0 && currentMissileIndex < Missiles.Count)
            {
               
                if (Missiles[currentMissileIndex] != null)
                {

                    Missiles[currentMissileIndex].transform.GetComponentInChildren<Camera>(true).gameObject.SetActive(false);

                    if (Missiles.Count == currentMissileIndex + 1)
                    {
                        currentMissileIndex = 0;
                        ProfileManager.currentProfile.NoMissileDisplay();

                    }
                    else
                    {
                        currentMissileIndex++;
                    }

                    
                    Camera missileCam = Missiles[currentMissileIndex].transform.GetComponentInChildren<Camera>(true);

                    if (missileCam != null)
                    {
                        missileCam.targetTexture = MissileScreenUIPatching.renderTexture;
                        missileCam.gameObject.SetActive(true);
                        ProfileManager.currentProfile.missileName.SetText(Missiles[currentMissileIndex].unitName);
                        ProfileManager.currentProfile.missileIndex.SetActive(true);
                        ProfileManager.currentProfile.missileIndex.SetText($"{currentMissileIndex + 1}/{Missiles.Count}");
                        ProfileManager.currentProfile.screen.SetActive(true);
                    }

                }

            }

        }
        
        public static void CycleMissileViewDown()
        {
           
            Plugin.Logger.LogDebug($"count:{Missiles.Count}, index: {currentMissileIndex}");
            if (currentMissileIndex >= 0 && currentMissileIndex < Missiles.Count)
            {
               
                if (Missiles[currentMissileIndex] != null)
                {

                    Missiles[currentMissileIndex].transform.GetComponentInChildren<Camera>(true).gameObject.SetActive(false);

                    if (0 > currentMissileIndex - 1)
                    {
                        currentMissileIndex = Missiles.Count - 1;
                        ProfileManager.currentProfile.NoMissileDisplay();

                    }
                    else
                    {
                        currentMissileIndex--;
                        currentMissileIndex = Mathf.Clamp(currentMissileIndex, 0, Missiles.Count - 1);
                    }

                    
                    Camera missileCam = Missiles[currentMissileIndex].transform.GetComponentInChildren<Camera>(true);

                    if (missileCam != null)
                    {
                        missileCam.targetTexture = MissileScreenUIPatching.renderTexture;
                        missileCam.gameObject.SetActive(true);
                        ProfileManager.currentProfile.missileName.SetText(Missiles[currentMissileIndex].unitName);
                        ProfileManager.currentProfile.missileIndex.SetActive(true);
                        ProfileManager.currentProfile.missileIndex.SetText($"{currentMissileIndex + 1}/{Missiles.Count}");
                        ProfileManager.currentProfile.screen.SetActive(true);
                    }

                }

            }
            else
            {
                currentMissileIndex = 0;
            }

        }


       

    }
}
