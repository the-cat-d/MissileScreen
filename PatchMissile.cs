extern alias Engine2;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MissileView
{
    internal class PatchMissile
    {

        public static List<Missile> missiles = new();
        public static int currentMissileIndex = 0;

        [HarmonyPatch(typeof(Missile), "StartMissile")]
        public static class OnMissileLaunch
        {

            static void Postfix(Missile __instance)
            {


                
                if (GameUtils.getAircraft() != __instance.owner || GameUtils.getAircraft() == null || PatchTacScreen.renderTexture == null)
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


                    Plugin.Logger.LogDebug(PluginConfig.cameraRenderDistance.Value);
                    missileCam.farClipPlane = PluginConfig.cameraRenderDistance.Value;
                    missileCam.fieldOfView = PluginConfig.cameraFOV.Value;
                    missileCam.transform.localPosition = Vector3.zero;
                    missileCam.transform.rotation = Quaternion.Euler(missileCam.transform.rotation.eulerAngles.x, missileCam.transform.rotation.eulerAngles.y, GameUtils.getAircraft().transform.rotation.eulerAngles.z);


                    missileCam.targetTexture = PatchTacScreen.renderTexture;

                    missileCam.gameObject.SetActive(true);

                    PatchTacScreen.missileName.SetText(__instance.unitName);

                    missiles.Add(__instance);

                    currentMissileIndex = missiles.Count - 1;


                }






                    Plugin.Logger.LogMessage("Missile Launched " + __instance.name);

            }
        }



        [HarmonyPatch(typeof(Missile), "UserCode_RpcDetonate_897349600")]
        public static class OnMissileDestroy
        {
            static void Postfix(Missile __instance, bool hitArmor)
            {
                if (GameUtils.getAircraft() != __instance.owner || GameUtils.getAircraft() == null) return; // missile isn't owned by player



                int removedIndex = missiles.IndexOf(__instance);

                if (removedIndex == -1) return;

                bool wasViewing = (removedIndex == currentMissileIndex);

                Camera missileCam = __instance.transform.GetComponentInChildren<Camera>(true);

                if (missileCam != null)
                {
                    GameObject.Destroy(missileCam.gameObject);
                }

                missiles.Remove(__instance);

                if (missiles.Count == 0)
                {
                    currentMissileIndex = 0;

                    PatchTacScreen.missileName.SetText("No Missile");
                    PatchTacScreen.missileIndex.SetActive(false);
                    PatchTacScreen.velocityVector.SetActive(false);
                    return;
                }

                if (wasViewing)
                {
                    if (removedIndex < missiles.Count)
                    {
                        currentMissileIndex = removedIndex;
                    } else
                    {
                        currentMissileIndex = missiles.Count - 1;
                    }

                    ActivateMissile(currentMissileIndex);
                } else if (removedIndex < currentMissileIndex)
                {
                    currentMissileIndex--;
                }

                

                Plugin.Logger.LogMessage("Missile Detonated " + __instance.name);


            }

        }

        public static void ActivateMissile(int index)
        {
            if (index < 0 || index >= missiles.Count)
            { return; }

            Camera missileCam = missiles[index].transform.GetComponentInChildren<Camera>(true);

            if (missileCam != null)
            {
                missileCam.targetTexture = PatchTacScreen.renderTexture;
                missileCam.gameObject.SetActive(true);

                PatchTacScreen.missileName.SetText(missiles[currentMissileIndex].unitName);
                PatchTacScreen.missileIndex.SetActive(true);
                PatchTacScreen.missileIndex.SetText($"{currentMissileIndex + 1}/{missiles.Count}");
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

                    if (missiles.Count == (currentMissileIndex + 1))
                    {

                        currentMissileIndex = 0;
                        PatchTacScreen.missileName.SetText("No Missile");
                        PatchTacScreen.missileIndex.SetActive(false);

                    }
                    else
                    {
                        currentMissileIndex++;
                    }

                    
                    Camera missileCam = missiles[currentMissileIndex].transform.GetComponentInChildren<Camera>(true);

                    if (missileCam != null)
                    {
                        missileCam.targetTexture = PatchTacScreen.renderTexture;
                        missileCam.gameObject.SetActive(true);
                        PatchTacScreen.missileName.SetText(missiles[currentMissileIndex].unitName);
                        PatchTacScreen.missileIndex.SetActive(true);
                        PatchTacScreen.missileIndex.SetText($"{currentMissileIndex + 1}/{missiles.Count}");
                    }

                }

            }

        }
    }
}
