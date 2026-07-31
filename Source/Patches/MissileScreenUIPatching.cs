using HarmonyLib;
using MissileScreen.UI;
using UnityEngine;



namespace MissileScreen.Patches
{
    internal class MissileScreenUIPatching
    {


        public static RenderTexture renderTexture;

        //public static Vector2 missilePanelSize;


        public static bool isPlaneCompatible = false;


        //// Patches \\\\

        
        public class MissileScreenUIPatches
        {
            private const float _errorFrequency = 3;
            private static int _errCount;

            // Runs when the player enters the aircraft
            // All the UI related work from the mod is done in this patch
            [HarmonyPatch(typeof(TacScreen), "Initialize")]
            public class TacScreenInit
            {

                static void Postfix(TacScreen __instance, Aircraft aircraft, Cockpit cockpit)
                {
                    var playerAircraft = GameUtils.GetAircraft();
                    if (aircraft == null || playerAircraft == null || aircraft != playerAircraft) return;

                    try
                    {
                        isPlaneCompatible = false;


                        // Create Missile Screen UI

                        ProfileManager.InjectProfileUI(aircraft.definition.name, __instance);


                        // Clear missile data on start up

                        MissilePatching.currentMissileIndex = 0;
                        MissilePatching.Missiles.Clear();

                        Plugin.Logger.LogDebug("Loaded TacScreen");

                    }
                    catch (System.Exception error)
                    {
                        Plugin.Logger.LogError($"Failed to inject UI ({aircraft.definition.name}): {error.Message}\n{error.StackTrace}");
                    }



                }


            }



            // The main update function of the mod
            // i only want to run whenever the player is in an aircraft and because the TacScreen 
            // component only exists for the player whenever in a cockpit.
            [HarmonyPatch(typeof(TacScreen), "Update")]
            public class TacScreenUpdate
            {

                private static float _timeSinceErr = 0;
                

                static void Postfix(TacScreen __instance)
                {

                    try
                    {

                        Update();
                        
                        _timeSinceErr = 0;
                        _errCount = 0;

                    } catch (System.Exception error)
                    {
                        _errCount += 1;
                        _timeSinceErr -= Time.deltaTime;

                        if (_timeSinceErr <= 0)
                        {

                            string finalErrCount = _errCount > 1 ? "x" + _errCount : "";

                            Plugin.Logger.LogError($"Missile Screen Update Failure {finalErrCount} ({__instance.aircraft.definition.name}): {error.Message}{error.StackTrace}");
                            

                            _timeSinceErr = _errorFrequency;
                        }

                    }

                }
            }
        }


        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        //// Functions \\\\


        private static Vector3 lastTargetPos;

        private static void Update()
        {
            if (isPlaneCompatible == false) return;
           

            if (Input.GetKeyDown(PluginConfig.cycleUpKey.Value))
            {

                MissilePatching.CycleMissileViewUp();

            }

            if (Input.GetKeyDown(PluginConfig.cycleDownKey.Value))
            {
                MissilePatching.CycleMissileViewDown();
            }


            if (MissilePatching.Missiles.Count > 0 && MissilePatching.currentMissileIndex < MissilePatching.Missiles.Count)
            {
                if (MissilePatching.Missiles[MissilePatching.currentMissileIndex] != null)
                {

                    Missile currentMissile = MissilePatching.Missiles[MissilePatching.currentMissileIndex];

                    Camera missileCam = currentMissile.transform.GetComponentInChildren<Camera>(true);


                    if (missileCam != null)
                    {
                        int lockboxMinSize = ProfileManager.currentProfile.lockboxMinSize;



                        Vector3 screenCenter = new Vector3(ProfileManager.currentProfile.missilePanelSize.x / 2, ProfileManager.currentProfile.missilePanelSize.y / 2, 0);
                        ProfileManager.currentProfile.ToggleElements(true);


                        // Target Box (Size)

                        Vector2 minLockbox = new Vector2(lockboxMinSize, lockboxMinSize);

                        Renderer targetRenderer;

                        Unit target = currentMissile.target;
                        Vector3 targetPosition;



                        if (target != null)
                        {
                            targetPosition = target.transform.position;
                            targetRenderer = target.gameObject.GetComponentInChildren<Renderer>();
                        }
                        else
                        {
                            var aimPoint = currentMissile.aimPoint;
                            targetPosition = aimPoint.ToLocalPosition();
                            targetRenderer = null;
                            ProfileManager.currentProfile.lockBox.SetActive(false);
                        }

                        Vector3 viewportTargetPosition = missileCam.WorldToScreenPoint(targetPosition);


                        Vector2 lockboxSize = minLockbox;




                        RectTransform lockBoxRect = ProfileManager.currentProfile.lockBox.GetComponent<RectTransform>();
                        if (targetRenderer != null && PluginConfig.fixedLockBox.Value == false)
                        {
                            Bounds targetBounds = targetRenderer.bounds;

                            Vector3[] corners = new Vector3[8];

                            Vector3 min = targetBounds.min;
                            Vector3 max = targetBounds.max;

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

                                
                                if (screenPoint.z < 0)
                                    continue;

                                minScreen = Vector3.Min(minScreen, screenPoint);
                                maxScreen = Vector3.Max(maxScreen, screenPoint);
                            }

                            Vector3 center = (minScreen + maxScreen) / 2f;
                            Vector3 size = maxScreen - minScreen;



                            lockBoxRect.sizeDelta = new Vector2(
                               Mathf.Max(size.x, lockboxMinSize),
                               Mathf.Max(size.y, lockboxMinSize)
                            );
                            lockboxSize = lockBoxRect.sizeDelta;

                        }
                        else if (lockBoxRect.sizeDelta != minLockbox)
                        {


                            lockBoxRect.sizeDelta = new Vector2(lockboxMinSize, lockboxMinSize);
                            lockboxSize = lockBoxRect.sizeDelta;
                        }

                        // Position

                        if (viewportTargetPosition.z > 0 && viewportTargetPosition != Vector3.zero && target != null)
                        {
                            ProfileManager.currentProfile.lockBox.SetActive(true);
                            Vector3 targetUiPos = Vector3.Scale(viewportTargetPosition, new Vector3(1, 1, 0)) - screenCenter;

                            targetUiPos = GameUtils.ClampToScreen(targetUiPos, ProfileManager.currentProfile.missilePanelSize, lockboxSize);


                            ProfileManager.currentProfile.lockBox.transform.localPosition = targetUiPos;

                            lastTargetPos = targetUiPos;
                        }
                        else
                        {
                            ProfileManager.currentProfile.lockBox.SetActive(false);
                        }





                        if (target != null)
                        {
                            if (target.NetworkHQ == null)
                            {
                                ProfileManager.currentProfile.missileTargetName.SetColor(Color.white);
                            }
                            else
                            {
                                ProfileManager.currentProfile.missileTargetName.SetColor(target.NetworkHQ == GameUtils.GetPlayerTeam() ? GameAssets.i.HUDFriendly : GameAssets.i.HUDHostile);
                            }

                            ProfileManager.currentProfile.missileTargetName.SetText(target is Aircraft ? target.definition.unitName : target.unitName);

                            float targetDistance = FastMath.Distance(targetPosition.ToGlobalPosition(), currentMissile.transform.GlobalPosition());

                            ProfileManager.currentProfile.missileRange.SetText($"RNG {UnitConverter.DistanceReading(targetDistance)}");

                            if (target.GetComponent<Rigidbody>() != null)
                            {
                                ProfileManager.currentProfile.leadIcon.SetActive(true);

                                Vector3 targetVelocity = target.rb.velocity;

                                Vector3 predictedPosition = targetPosition + targetVelocity * (targetDistance / currentMissile.rb.velocity.magnitude);

                                Vector3 leadUiPos = Vector3.Scale(missileCam.WorldToScreenPoint(predictedPosition) - screenCenter, new(1, 1, 0));



                                leadUiPos = GameUtils.ClampToScreen(leadUiPos, ProfileManager.currentProfile.missilePanelSize, ProfileManager.currentProfile.leadIcon.GetGameObject().GetComponent<RectTransform>().sizeDelta / ProfileManager.currentProfile.leadIconThing);

                                ProfileManager.currentProfile.leadIcon.GetGameObject().transform.localPosition = leadUiPos;
                            }
                            else
                            {

                                
                                ProfileManager.currentProfile.leadIcon.SetActive(false);
                                
                            }
                        }
                        else
                        {
                            ProfileManager.currentProfile.missileTargetName.SetColor(Color.white);
                            ProfileManager.currentProfile.missileTargetName.SetText("No Target");
                            ProfileManager.currentProfile.missileRange.SetText("RNG --");
                            ProfileManager.currentProfile.leadIcon.SetActive(false);

                        }

                        currentMissile.UpdateRadarAlt();

                        ProfileManager.currentProfile.missileIndex.SetText($"{MissilePatching.currentMissileIndex + 1}/{MissilePatching.Missiles.Count}");
                        ProfileManager.currentProfile.missileSpeed.SetText($"SPD {UnitConverter.SpeedReading(Mathf.Round(currentMissile.speed))}");
                        ProfileManager.currentProfile.missileAltitude.SetText($"ALT {UnitConverter.AltitudeReading(currentMissile.radarAlt)}");


                        // Velocity Vector
                        Vector3 position = missileCam.transform.position + currentMissile.rb.velocity * 6;


                        Vector3 vector = Vector3.Scale(missileCam.WorldToScreenPoint(position), new Vector3(1f, 1f, 0f)) - screenCenter;
                        vector = GameUtils.ClampToScreen(vector, ProfileManager.currentProfile.missilePanelSize, ProfileManager.currentProfile.velocityVector.GetGameObject().GetComponent<RectTransform>().sizeDelta / ProfileManager.currentProfile.velocityVectorThing);
                        ProfileManager.currentProfile.velocityVector.GetGameObject().transform.localPosition = vector;

                        // Orientation Indicator

                        ProfileManager.currentProfile.orientationIndicator.GetGameObject().transform.localRotation = Quaternion.Euler(0, 0, -missileCam.transform.rotation.eulerAngles.z);




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