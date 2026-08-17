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

        public static readonly List<Missile> missiles = [];
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
                        Plugin.logger.LogError($"Error in MissilePatches.OnClusterDestroy: {error.Message}\nMissile: {__instance.gameObject.GetComponent<Missile>()}");
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
                        Plugin.logger.LogError($"Missile Launch Error ({__instance.unitName}): {error.Message}{error.StackTrace}");
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
                        Plugin.logger.LogError($"Missile Detonation Error ({__instance.unitName}): {error.Message}{error.StackTrace}");
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
            
            Plugin.logger.LogDebug("Finding camera...");
            
            Camera missileCam = __instance.transform.GetComponentInChildren<Camera>(true);
            
            if (missileCam == null) {
                Plugin.logger.LogDebug("Camera not found creating...");
                GameObject missileCamObj = new GameObject("missileCam", typeof(Camera))
                {
                    transform =
                    {
                        parent = __instance.transform,
                    },
                };
                missileCam = missileCamObj.GetComponent<Camera>();
                missileCam.transform.localRotation = Quaternion.identity;
                Plugin.logger.LogDebug("Camera created");

            }
            
            if (missileCam != null)
            {
                
                if (missiles.Count > 0 && currentMissileIndex <= missiles.Count - 1)
                {
                    Plugin.logger.LogDebug("Disabling previous camera...");
                    if (missiles[currentMissileIndex] == null)
                    {
                        missiles.RemoveAt(currentMissileIndex);
                        CycleMissileViewUp();
                        return;
                    }
                    missiles[currentMissileIndex].transform.GetComponentInChildren<Camera>(true).gameObject.SetActive(false); // Disable the camera of any previous missile to avoid lag
                    Plugin.logger.LogDebug("Previous camera disabled");
                }
                

                _currentMissileCamera = missileCam;

                Plugin.logger.LogDebug("Setting camera initial values...");
                missileCam.farClipPlane = PluginConfig.cameraRenderDistance.Value;
                missileCam.fieldOfView = PluginConfig.cameraFOV.Value;
                missileCam.transform.localPosition = PluginConfig.missileCameraOffset.Value;
                missileCam.transform.rotation = Quaternion.Euler(missileCam.transform.rotation.eulerAngles.x, missileCam.transform.rotation.eulerAngles.y, GameUtils.GetAircraft().transform.rotation.eulerAngles.z);
                Plugin.logger.LogDebug("Camera values set up");
                
                Plugin.logger.LogDebug("setting camera volume layer...");
                
                UniversalAdditionalCameraData missileCamData = missileCam.GetComponent<UniversalAdditionalCameraData>();

                if (missileCamData == null)
                {
                    missileCamData = missileCam.gameObject.AddComponent<UniversalAdditionalCameraData>();
                } 

                missileCamData.renderPostProcessing = true;
                missileCamData.volumeLayerMask = 256;
                
                Plugin.logger.LogDebug("camera volume layer set");

                if (missileCam.GetComponent<AudioListener>())
                {
                    Plugin.logger.LogDebug("AudioListener found");
                    Component.Destroy(missileCam.GetComponent<AudioListener>());
                    Plugin.logger.LogDebug("AudioListener destroyed");
                }

                
                Plugin.logger.LogDebug("Setting camera target texture");
                missileCam.targetTexture = MissileScreenUIPatching.renderTexture;
                Plugin.logger.LogDebug("Camera target texture set");

                missileCam.gameObject.SetActive(true);
                ProfileManager.currentProfile.screen.SetActive(true);
           
                ProfileManager.currentProfile.SetMissileName(__instance.unitName);

                missiles.Add(__instance);

                currentMissileIndex = missiles.Count - 1;


            }


            Plugin.logger.LogMessage("Missile Launched " + __instance.name);
        }

        private static void OnDetonate(Missile __instance)
        {
            if (GameUtils.GetAircraft() != __instance.owner || GameUtils.GetAircraft() == null) return; // missile isn't owned by player



            int removedIndex = missiles.IndexOf(__instance);

            if (removedIndex == -1) return;

            bool wasViewing = removedIndex == currentMissileIndex;

            Camera missileCam = __instance.transform.GetComponentInChildren<Camera>(true);

            if (missileCam != null)
            {
                Object.Destroy(missileCam.gameObject);
            }

            missiles.Remove(__instance);

            if (missiles.Count == 0)
            {
                currentMissileIndex = 0;
                ProfileManager.currentProfile.NoMissileDisplay();

                return;
            }

            if (wasViewing)
            {
                if (removedIndex < missiles.Count)
                {
                    currentMissileIndex = removedIndex;
                }
                else
                {
                    currentMissileIndex = missiles.Count - 1;
                }

                ActivateMissile(currentMissileIndex);
            }
            else if (removedIndex < currentMissileIndex)
            {
                currentMissileIndex--;
            }



            Plugin.logger.LogMessage("Missile Detonated " + __instance.name);


        }


        public static void ActivateMissile(int index)
        {
            if (index < 0 || index >= missiles.Count)
            { return; }

            Camera missileCam = missiles[index].transform.GetComponentInChildren<Camera>(true);

            if (missileCam != null)
            {
                missileCam.targetTexture = MissileScreenUIPatching.renderTexture;
                missileCam.gameObject.SetActive(true);

                ProfileManager.currentProfile.SetMissileName(missiles[currentMissileIndex].unitName);

                ProfileManager.currentProfile.SetMissileIndex(currentMissileIndex, missiles.Count);

            }
        }

        public static void CycleMissileViewUp()
        {
           
            Plugin.logger.LogDebug($"count:{missiles.Count}, index: {currentMissileIndex}");
            if (currentMissileIndex >= 0 && currentMissileIndex < missiles.Count)
            {
               
                if (missiles[currentMissileIndex] != null)
                {

                    missiles[currentMissileIndex].transform.GetComponentInChildren<Camera>(true).gameObject.SetActive(false);

                    if (missiles.Count == currentMissileIndex + 1)
                    {
                        currentMissileIndex = 0;
                        ProfileManager.currentProfile.NoMissileDisplay();

                    }
                    else
                    {
                        currentMissileIndex++;
                    }

                    
                    Camera missileCam = missiles[currentMissileIndex].transform.GetComponentInChildren<Camera>(true);

                    if (missileCam != null)
                    {
                        missileCam.targetTexture = MissileScreenUIPatching.renderTexture;
                        missileCam.gameObject.SetActive(true);
                        ProfileManager.currentProfile.SetMissileName(missiles[currentMissileIndex].unitName);
                        ProfileManager.currentProfile.SetMissileIndex(currentMissileIndex, missiles.Count);
                        ProfileManager.currentProfile.screen.SetActive(true);
                    }

                }

            }

        }
        
        public static void CycleMissileViewDown()
        {
           
            Plugin.logger.LogDebug($"count:{missiles.Count}, index: {currentMissileIndex}");
            if (currentMissileIndex >= 0 && currentMissileIndex < missiles.Count)
            {
               
                if (missiles[currentMissileIndex] != null)
                {

                    missiles[currentMissileIndex].transform.GetComponentInChildren<Camera>(true).gameObject.SetActive(false);

                    if (0 > currentMissileIndex - 1)
                    {
                        currentMissileIndex = missiles.Count - 1;
                        ProfileManager.currentProfile.NoMissileDisplay();

                    }
                    else
                    {
                        currentMissileIndex--;
                        currentMissileIndex = Mathf.Clamp(currentMissileIndex, 0, missiles.Count - 1);
                    }

                    
                    Camera missileCam = missiles[currentMissileIndex].transform.GetComponentInChildren<Camera>(true);

                    if (missileCam != null)
                    {
                        missileCam.targetTexture = MissileScreenUIPatching.renderTexture;
                        missileCam.gameObject.SetActive(true);
                        ProfileManager.currentProfile.SetMissileName(missiles[currentMissileIndex].unitName);
                        ProfileManager.currentProfile.SetMissileIndex(currentMissileIndex, missiles.Count);
                        ProfileManager.currentProfile.screen.SetActive(true);
                    }

                }

            }
            else
            {
                currentMissileIndex = 0;
            }

        }


        public static void DisableAllMissileCams()
        {
            
            for (int i = 0; i < missiles.Count; i++)
            {
                if (missiles[i] == null)
                {
                    missiles.RemoveAt(i);
                    return;
                }
                var missileCam = missiles[i].transform.GetComponentInChildren<Camera>(true);
                if (missileCam != null)
                {
                    
                    missileCam.gameObject.SetActive(false);
                }
            }
        }

    }
}
