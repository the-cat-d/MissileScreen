using HarmonyLib;
using MissileView.UI;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;


namespace MissileView.Patches
{
    internal class PatchTacScreen
    {


        public static RenderTexture renderTexture;

        public static Vector2 missilePanelSize;


        public static bool isPlaneCompatible = false;


        // Runs pretty much when the player enters the aircraft
        // All the UI related work from the mod is done in this patch
        [HarmonyPatch(typeof(TacScreen), "Initialize")]
        public class TacScreenInit
        {

            static void Postfix(TacScreen __instance, Aircraft aircraft, Cockpit cockpit)
            {
                if (aircraft == GameUtils.getAircraft())
                {
                    isPlaneCompatible = false;


                    ProfileManager.InjectProfileUI(aircraft.definition.name,__instance,aircraft);


                    // Clear missile data on start up

                    PatchMissile.currentMissileIndex = 0;
                    PatchMissile.missiles.Clear();

                    Plugin.Logger.LogDebug("Loaded TacScreen");

                }

            }
        }



        static FieldInfo targetField = typeof(Missile).GetField("target", BindingFlags.NonPublic | BindingFlags.Instance);

        // The main update function of the mod
        // i did this because i only want update to run whenever the player is in an aircraft and since the TacScreen 
        // component only exists for the player whenever in a cockpit this is exactly what i want.
        [HarmonyPatch(typeof(TacScreen), "Update")]
        public class TacScreenUpdate
        {
            static int LockBoxSizeConstraint = GetLockboxSizeConstraint(GameUtils.getAircraft());
            static void Postfix()
            {
                if (isPlaneCompatible == true)
                {
                    if (Input.GetKeyDown(PluginConfig.cycleKey.Value))
                    {

                        PatchMissile.CycleMissileView();

                    }


                    if (PatchMissile.missiles.Count > 0 && PatchMissile.currentMissileIndex < PatchMissile.missiles.Count)
                    {
                        if (PatchMissile.missiles[PatchMissile.currentMissileIndex] != null)
                        {

                            Missile currentMissile = PatchMissile.missiles[PatchMissile.currentMissileIndex];

                            Camera missileCam = currentMissile.transform.GetComponentInChildren<Camera>(true);


                            if (missileCam != null)
                            {
                                Vector3 screenCenter = new Vector3(missilePanelSize.x / 2, missilePanelSize.y / 2, 0);
                                ProfileManager.currentProfile.ToggleElements(true);

                                // Velocity Vector
                                Vector3 position = missileCam.transform.position + currentMissile.rb.velocity * 6;
                                Vector3 vector = Vector3.Scale(missileCam.WorldToScreenPoint(position), new Vector3(1f, 1f, 0f)) - screenCenter;
                                vector = ClampToScreen(vector);
                                ProfileManager.currentProfile.velocityVector.transform.localPosition = vector;

                                // Orientation Indicator
                                ProfileManager.currentProfile.orientationIndicator.transform.localRotation = Quaternion.Euler(0, 0, -missileCam.transform.rotation.eulerAngles.z);


                                //Target Box (Position)

                                Renderer renderer;

                                Unit target = (Unit)targetField.GetValue(currentMissile);
                                Vector3 targetPosition;

                                if (target != null)
                                {
                                    targetPosition = target.transform.position;
                                    renderer = target.gameObject.GetComponentInChildren<Renderer>();
                                }
                                else
                                {
                                    var aimPoint = Traverse.Create(currentMissile).Field("aimPoint").GetValue<GlobalPosition>();
                                    targetPosition = aimPoint.ToLocalPosition();
                                    renderer = null;
                                    ProfileManager.currentProfile.lockBox.SetActive(false);
                                }


                                Vector3 viewportTargetPosition = missileCam.WorldToScreenPoint(targetPosition);

                                Vector3 uiPos = Vector3.Scale(viewportTargetPosition, new Vector3(1, 1, 0)) - screenCenter;

                                uiPos = ClampToScreen(uiPos); 

                                Plugin.Logger.LogDebug($"{uiPos}, {target}");

                                ProfileManager.currentProfile.lockBox.transform.localPosition = uiPos;



                                if (renderer != null)
                                {
                                    Bounds bounds = renderer.bounds;

                                    Vector3[] corners = new Vector3[8];

                                    Vector3 min = bounds.min;
                                    Vector3 max = bounds.max;

                                    corners[0] = new Vector3(min.x, min.y, min.z);
                                    corners[1] = new Vector3(max.x, min.y, min.z);
                                    corners[2] = new Vector3(min.x, max.y, min.z);
                                    corners[3] = new Vector3(max.x, max.y, min.z);

                                    corners[4] = new Vector3(min.x, min.y, max.z);
                                    corners[5] = new Vector3(max.x, min.y, max.z);
                                    corners[6] = new Vector3(min.x, max.y, max.z);
                                    corners[7] = new Vector3(max.x, max.y, max.z);

                                    Vector3 minScreen = new Vector3(float.MaxValue, float.MaxValue, 0);
                                    Vector3 maxScreen = new Vector3(float.MinValue, float.MinValue, 0);

                                    foreach (var corner in corners)
                                    {
                                        Vector3 screenPoint = missileCam.WorldToScreenPoint(corner);

                                        //Ignore points behind camera
                                        if (screenPoint.z < 0)
                                            continue;

                                        minScreen = Vector3.Min(minScreen, screenPoint);
                                        maxScreen = Vector3.Max(maxScreen, screenPoint);
                                    }

                                    Vector3 center = (minScreen + maxScreen) / 2f;
                                    Vector3 size = maxScreen - minScreen;





                                    RectTransform rect = ProfileManager.currentProfile.lockBox.GetComponent<RectTransform>();

                                    

                                    rect.sizeDelta = new Vector2(
                                       Mathf.Max(size.x, LockBoxSizeConstraint),
                                       Mathf.Max(size.y, LockBoxSizeConstraint)
                                    );

                                }
                                else
                                {
                                    RectTransform rect = ProfileManager.currentProfile.lockBox.GetComponent<RectTransform>();

                                    rect.sizeDelta = new Vector2(LockBoxSizeConstraint, LockBoxSizeConstraint);
                                }

                                if (target != null)
                                {
                                    if (target.NetworkHQ == null)
                                    {
                                        ProfileManager.currentProfile.missileTargetName.SetColor(Color.white);
                                    } else
                                    {
                                        ProfileManager.currentProfile.missileTargetName.SetColor(target.NetworkHQ == GameUtils.getHQ() ? GameAssets.i.HUDFriendly : GameAssets.i.HUDHostile);
                                    }

                                    ProfileManager.currentProfile.missileTargetName.SetText(target is Aircraft ? target.definition.unitName : target.unitName);

                                    float targetDistance = FastMath.Distance(targetPosition.ToGlobalPosition(), currentMissile.transform.GlobalPosition());

                                    ProfileManager.currentProfile.missileRange.SetText($"RNG {UnitConverter.DistanceReading(targetDistance)}");

                                    if (target.GetComponent<Rigidbody>() != null)
                                    {
                                        Vector3 targetVelocity = target.rb.velocity;

                                        Vector3 predictedPosition = targetPosition + targetVelocity * (targetDistance / currentMissile.rb.velocity.magnitude);

                                        Vector3 leadUiPos = Vector3.Scale(missileCam.WorldToScreenPoint(predictedPosition) - screenCenter, new(1, 1, 0));

                                        leadUiPos = ClampToScreen(leadUiPos);

                                        ProfileManager.currentProfile.leadIcon.transform.localPosition = leadUiPos;
                                    }
                                }
                                else
                                {
                                    ProfileManager.currentProfile.missileTargetName.SetColor(Color.white);
                                    ProfileManager.currentProfile.missileTargetName.SetText("No Target");
                                    ProfileManager.currentProfile.missileRange.SetText("RNG --");
                                    ProfileManager.currentProfile.leadIcon.SetActive(false);

                                }

                                ProfileManager.currentProfile.missileIndex.SetText($"{PatchMissile.currentMissileIndex + 1}/{PatchMissile.missiles.Count}");
                                ProfileManager.currentProfile.missileSpeed.SetText($"SPD {UnitConverter.SpeedReading(Mathf.Round(currentMissile.speed))}");
                                ProfileManager.currentProfile.missileAltitude.SetText($"ALT {UnitConverter.AltitudeReading(currentMissile.radarAlt)}");

                                return;
                            }


                        }
                        else
                        {
                            ProfileManager.currentProfile.ToggleElements(false);
                            
                        }
                        
                    }
                    else
                    {
                        ProfileManager.currentProfile.ToggleElements(false);
                    }
                }
            }
        }


        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

       

       

        private static Vector3 ClampToScreen(Vector3 vector)
        {
            return new(Mathf.Clamp(vector.x, -missilePanelSize.x / 2, missilePanelSize.x / 2), Mathf.Clamp(vector.y, -missilePanelSize.y / 2, missilePanelSize.y / 2), 0);
        }

        private static string GetPanelName(Aircraft aircraft)
        {
            if (aircraft.definition.name == "CAS1")
            {
                return "SystemStatus";
            }
            else if (aircraft.definition.name == "Trainer" || aircraft.definition.name == "CAS1")
            {
                return "frontProfile";
            }

            return "WeaponPanel";
        }

        
        private static int GetFontSize(Aircraft aircraft)
        {
            if (aircraft.definition.name == "SFB" || aircraft.definition.name == "QuatVTOL1")
            {
                return 28;
            }
            else if (aircraft.definition.name == "Multirole1")
            {
                return 21;
            }

                return 37;
        }

        
        private static float GetCornerScale(Aircraft aircraft)
        {
            if (aircraft.definition.name == "SFB")
            {
                return 0.2f;
            }
            else if (aircraft.definition.name == "Multirole1")
            {
                return 0.1f;
            }

            return 0.3f;
        }

        private static int GetLockboxSizeConstraint(Aircraft aircraft)
        {
            if (aircraft.definition.name == "SFB")
            {
                return 20;
            }
            else if (aircraft.definition.name == "Multirole1" || aircraft.definition.name == "QuatVTOL1")
            {
                return 35;
            }

            return 50;
        }

        private static float GetVelocityVectorScale(Aircraft aircraft)
        {
            if (aircraft.definition.name == "SFB")
            {
                return 0.45f;
            }
            else if (aircraft.definition.name == "Multirole1")
            {
                return 0.45f;
            }

            return 0.5f;
        }
        private static float GetLeadScale(Aircraft aircraft)
        {
          
            if (aircraft.definition.name == "Multirole1")
            {
                return 0.3f;
            }

            return 1;
        }

       
    }
}