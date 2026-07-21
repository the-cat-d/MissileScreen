using HarmonyLib;
using MissileView.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


namespace MissileView.Patches
{
    internal class MissilePatching
    {

        public static List<Missile> missiles = new();
        public static int currentMissileIndex = 0;

        // TODO: gbmlr breaks for some reason
        internal class MiscMissilePatches
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

        internal class MissilePatches 
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
                        Plugin.Logger.LogError($"Error in MissilePatches.OnMissileLaunch: {error.Message}\nMissile: {__instance}");
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
                        Plugin.Logger.LogError($"Error in MissilePatches.OnMissileDestroy: {error.Message}\nnMissile: {__instance}");
                    }
                }

            }

        }

        internal class TargetCamPatches
        {

            [HarmonyPatch(typeof(TargetCam), "OnBeginCameraRendering")]
            public class TargetCamBeginUpdate
            {
                static bool Prefix(TargetCam __instance, ScriptableRenderContext context, Camera camera)
                {
                    if (camera == __instance.cam || camera == MissilePatching.currentMissileCamera)
                    {
                        RenderSettings.fog = !__instance.IRMode;
                    }
                    return false;
                }
            }

            [HarmonyPatch(typeof(TargetCam), "OnEndCameraRendering")]
            public class TargetCamEndUpdate
            {
                static bool Prefix(TargetCam __instance, ScriptableRenderContext context, Camera camera)
                {
                    if (camera == __instance.cam || camera == MissilePatching.currentMissileCamera)
                    {
                        RenderSettings.fog = true;
                    }
                    return false;
                }
            }
        }



        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        static public Camera currentMissileCamera = null;

        private static void OnMissileStart(Missile __instance)
        {
            if (GameUtils.getAircraft() != __instance.owner || GameUtils.getAircraft() == null || MissileScreenUIPatching.renderTexture == null)
            {
                return; // missile isnt owned by player
            }

            Camera missileCam = __instance.transform.GetComponentInChildren<Camera>(true);

            if (missileCam == null)
            {
                GameObject missileCamObj = new("missileCam", typeof(Camera));
                missileCamObj.transform.parent = __instance.transform;
                missileCam = missileCamObj.GetComponent<Camera>();
                missileCam.transform.localRotation = Quaternion.identity;

            }

            if (missileCam != null)
            {
                if (missiles.Count > 0 && currentMissileIndex <= missiles.Count - 1)
                {

                    missiles[currentMissileIndex].transform.GetComponentInChildren<Camera>(true).gameObject.SetActive(false); // Disable the camera of any previous missile to avoid lag

                }


                currentMissileCamera = missileCam;

                Plugin.Logger.LogDebug(PluginConfig.cameraRenderDistance.Value);
                missileCam.farClipPlane = PluginConfig.cameraRenderDistance.Value;
                missileCam.fieldOfView = PluginConfig.cameraFOV.Value;
                missileCam.transform.localPosition = PluginConfig.missileCameraOffset.Value;
                missileCam.transform.rotation = Quaternion.Euler(missileCam.transform.rotation.eulerAngles.x, missileCam.transform.rotation.eulerAngles.y, GameUtils.getAircraft().transform.rotation.eulerAngles.z);

                missileCam.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
                missileCam.GetComponent<UniversalAdditionalCameraData>().volumeLayerMask = 256;

                Component.Destroy(missileCam.GetComponent<AudioListener>());

                

                missileCam.targetTexture = MissileScreenUIPatching.renderTexture;

                missileCam.gameObject.SetActive(true);
                ProfileManager.currentProfile.screen.SetActive(true);
           
                ProfileManager.currentProfile.missileName.SetText(__instance.unitName);

                missiles.Add(__instance);

                currentMissileIndex = missiles.Count - 1;


            }


            Plugin.Logger.LogMessage("Missile Launched " + __instance.name);
        }

        private static void OnDetonate(Missile __instance)
        {
            if (GameUtils.getAircraft() != __instance.owner || GameUtils.getAircraft() == null) return; // missile isn't owned by player



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



            Plugin.Logger.LogMessage("Missile Detonated " + __instance.name);


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

                ProfileManager.currentProfile.missileName.SetText(missiles[currentMissileIndex].unitName);
                ProfileManager.currentProfile.missileIndex.SetActive(true);
                ProfileManager.currentProfile.missileIndex.SetText($"{currentMissileIndex + 1}/{missiles.Count}");
               
            }
        }

        public static void CycleMissileView()
        {
            Plugin.Logger.LogDebug($"count:{missiles.Count}, index: {currentMissileIndex}");
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
                        ProfileManager.currentProfile.missileName.SetText(missiles[currentMissileIndex].unitName);
                        ProfileManager.currentProfile.missileIndex.SetActive(true);
                        ProfileManager.currentProfile.missileIndex.SetText($"{currentMissileIndex + 1}/{missiles.Count}");
                        ProfileManager.currentProfile.screen.SetActive(true);
                    }

                }

            }

        }
    
       

    }
}
