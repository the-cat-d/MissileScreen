using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;


namespace MissileView
{
    internal class PatchTacScreen
    {


        public static RenderTexture renderTexture;

        public static Vector2 missilePanelSize;

        public static GameUtils.Draw.UILabel missileName;
        public static GameUtils.Draw.UILabel missileTargetName;
        public static GameUtils.Draw.UILabel missileIndex;
        public static GameUtils.Draw.UILabel missileSpeed;
        public static GameUtils.Draw.UILabel missileRange;
        public static GameUtils.Draw.UILabel missileAltitude;

        public static GameObject velocityVector;
        public static GameObject orientationIndicator;
        public static GameObject lockBox;
        public static GameObject leadIcon;

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

                    Transform weaponPanel;
                    weaponPanel = GameUtils.FindChildRecursive(__instance.transform, GetPanelName(aircraft));

                    if (weaponPanel == null)
                    {
                      
                        return;

                    }

                    isPlaneCompatible = true;

                    if (weaponPanel.GetComponent<HorizontalOrVerticalLayoutGroup>() != null)
                    {
                        Component.Destroy(weaponPanel.GetComponent<HorizontalOrVerticalLayoutGroup>());
                    }
                    
                        
                    Transform missilePanel = GameObject.Instantiate(weaponPanel, weaponPanel.transform.parent);
                    missilePanel.name = "missilePanel";
                    missilePanelSize = missilePanel.GetComponent<RectTransform>().sizeDelta;




                    foreach (Transform child in missilePanel)
                    {
                        GameObject.Destroy(child.gameObject);
                    }

                    // Clearing of the old panel that was instantiated, besides the panels of the brawler and compass

                    if (aircraft.definition.name == "Trainer" || aircraft.definition.name == "CAS1")
                    {
                        if (missilePanel.GetComponent<GridLayoutGroup>() != null)
                        {
                            Component.Destroy(missilePanel.GetComponent<GridLayoutGroup>());
                            Component.Destroy(missilePanel.GetComponent<SystemStatusDisplay>());
                            
                        }
                    }
                    else
                    {
                        //GameObject.Destroy(weaponPanel.gameObject);
                        weaponPanel.gameObject.SetActive(false);
                    }

                    // Aircraft specific panel configurations
                    // Any other aircraft that isn't in these if statement uses the normal configuration

                    if (aircraft.definition.name == "SFB")
                    {
    
                        missilePanel.GetComponent<RectTransform>().sizeDelta = new(300, 200);
                        missilePanel.localRotation = Quaternion.Euler(0, 0, -90);
                    }
                    else if (aircraft.definition.name == "Multirole1")
                    {
                        missilePanel.GetComponent<RectTransform>().anchoredPosition = new(140, 192);
                        missilePanel.GetComponent<RectTransform>().sizeDelta = new(227, 120);
                    }
                    else if (aircraft.definition.name == "EW1")
                    {
                        missilePanel.GetComponent<RectTransform>().anchoredPosition = new(284, 160);
                        missilePanel.GetComponent<RectTransform>().sizeDelta = new(445, 184);
                    }
                    else if (aircraft.definition.name == "Trainer")
                    {
                        missilePanel.GetComponent<RectTransform>().anchoredPosition = new(313, 23);
                        missilePanel.GetComponent<RectTransform>().sizeDelta = new(500, 325);
                    }
                    else if (aircraft.definition.name == "CAS1")
                    {
                        missilePanel.GetComponent<RectTransform>().anchoredPosition = new(312, 24);
                        missilePanel.GetComponent<RectTransform>().sizeDelta = new(384, 250);

                        missilePanel.transform.localRotation = Quaternion.identity;
                    }


                    //renderTexture = new(PluginConfig.cameraResolution.Value,PluginConfig.cameraResolution.Value, 16, RenderTextureFormat.ARGB32);
                    renderTexture = new((int)missilePanelSize.x, (int)missilePanelSize.y, 16, RenderTextureFormat.ARGB32);

                    GameObject screen = new("missileScreen");
                    screen.transform.parent = missilePanel;
                    screen.transform.localPosition = Vector3.zero;
                    screen.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    screen.transform.localScale = new Vector3(1, 1, 0);
                    

                    RectTransform screenRect = screen.AddComponent<RectTransform>();
                    screenRect.anchorMin = Vector2.zero;
                    screenRect.anchorMax = Vector2.one;
                    screenRect.sizeDelta = Vector2.zero;


                    RawImage screenImage = screen.AddComponent<RawImage>();
                    screenImage.texture = renderTexture;
                    screenImage.color = Color.white;

                  
                    // Flight Path

                    float velocityVectorScale = GetVelocityVectorScale(aircraft);

                    velocityVector = new("FlightPath",typeof(RectTransform));
                    velocityVector.transform.parent = missilePanel;
                    velocityVector.transform.localPosition = Vector3.zero;
                    velocityVector.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    velocityVector.transform.localScale = new Vector3(velocityVectorScale, velocityVectorScale, 0);
   
                    GameObject velImage = GameObject.Instantiate(velocityVector,Vector3.zero,Quaternion.identity, velocityVector.transform);
                    velImage.name = "Attack Vector";
                    velImage.transform.localPosition = Vector3.zero;
                    velImage.transform.localRotation = Quaternion.identity;
    
                    Image velImageComp = velImage.AddComponent<Image>();
                    velImageComp.sprite = PluginSprites.AttackSprite;
                    velImageComp.color = Color.green;
                    velImage.AddComponent<Outline>();

                    // Orienation Indicator

                    orientationIndicator = new("orientationIndicator",typeof(Image));
                    orientationIndicator.transform.parent = velocityVector.transform;
                    orientationIndicator.transform.localPosition = Vector3.zero;
                    orientationIndicator.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    orientationIndicator.transform.localScale = new Vector3(2f, 0.7f, 0);

                    Image orientationImageComp = orientationIndicator.GetComponent<Image>();
                    orientationImageComp.sprite = PluginSprites.OrientationSprite;
                    orientationImageComp.color = Color.green;


                    orientationIndicator.AddComponent<Outline>();

                    // Lockbox

                    lockBox = new("lockBox",typeof(RectTransform),typeof(Outline));
                    lockBox.transform.parent = missilePanel.transform;
                    lockBox.transform.localPosition = Vector3.zero;
                    lockBox.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    lockBox.transform.localScale = new Vector3(GetLeadScale(aircraft), GetLeadScale(aircraft), 0);


                    float lockCornerScale = GetCornerScale(aircraft);

                    CreateLockBoxCorner(lockBox.transform, new(lockCornerScale, lockCornerScale, 0), new(0, 1), new(0, 1));
                    CreateLockBoxCorner(lockBox.transform, new(-lockCornerScale, lockCornerScale, 0), new(1, 1), new(0, 1));

                    CreateLockBoxCorner(lockBox.transform, new(-lockCornerScale, -lockCornerScale, 0), new(1, 0), new(0, 1));
                    CreateLockBoxCorner(lockBox.transform, new(lockCornerScale, -lockCornerScale, 0), new(0, 0), new(0, 1));


                    // Lead Icon
                    leadIcon = new("leadIcon", typeof(Image), typeof(Outline));
                    leadIcon.transform.parent = missilePanel.transform;
                    leadIcon.transform.localPosition = Vector3.zero;
                    leadIcon.transform.localRotation = Quaternion.identity;
                    leadIcon.transform.localScale = Vector3.one;
                    Image leadImage = leadIcon.GetComponent<Image>();
                    leadImage.sprite = PluginSprites.leadSprite;
                    leadImage.color = new(0,1,0);
                    

                    // Missile Name 

                    missileName = new("missileName", new(0, 0), missilePanel);
                    missileName.SetText("No Missile");
                    missileName.SetColor(new(1f, 0f, 1f));
                    missileName.SetFontSize(GetFontSize(aircraft));
                    missileName.SetTextAlignment(TextAnchor.MiddleLeft);

                    RectTransform nameRect = missileName.GetRectTransform();
                    nameRect.anchorMin = new(0, 1);
                    nameRect.anchorMax = new(0, 1);
                    nameRect.pivot = new(0, 1);
                    nameRect.anchoredPosition = new(5, -5);


                    // Missile Index

                    missileIndex = new("missileIndex", new(0, 0), missilePanel);
                    missileIndex.SetText("0/0");
                    missileIndex.SetFontSize(Mathf.RoundToInt(GetFontSize(aircraft) / 1.3f));
                    missileIndex.SetColor(Color.white);
                    missileIndex.SetTextAlignment(TextAnchor.MiddleLeft);


                    RectTransform indexRect = missileIndex.GetRectTransform();
                    indexRect.anchorMin = new(0, 1);
                    indexRect.anchorMax = new(0, 1);
                    indexRect.pivot = new(0, 1);
                    indexRect.anchoredPosition = new(5, -42);


                    // Missile Target 

                    missileTargetName = new("missileTargetName", new(0, 0), missilePanel);
                    missileTargetName.SetText("No Target");
                    missileTargetName.SetColor(new(1, 1, 1));
                    missileTargetName.SetFontSize(Mathf.RoundToInt(GetFontSize(aircraft) / 1.5f));
                    missileTargetName.SetTextAlignment(TextAnchor.MiddleRight);

                    RectTransform targetRect = missileTargetName.GetRectTransform();
                    targetRect.anchorMin = new(1, 1);
                    targetRect.anchorMax = new(1, 1);
                    targetRect.pivot = new(1, 1);
                    targetRect.anchoredPosition = new(-5, -5);


                    // Missile Speed

                    missileSpeed = new("missileSpeed", new(0, 0), missilePanel);
                    missileSpeed.SetText("SPD --");
                    missileSpeed.SetFontSize(Mathf.RoundToInt(GetFontSize(aircraft) / 1.5f));
                    missileSpeed.SetColor(Color.white);

                    missileSpeed.SetTextAlignment(TextAnchor.MiddleRight);

                    RectTransform speedRect = missileSpeed.GetRectTransform();
                    speedRect.anchorMin = new(1, 1);
                    speedRect.anchorMax = new(1, 1);
                    speedRect.pivot = new(1, 1);
                    speedRect.anchoredPosition = new(-5, -missileSpeed.GetTextSize().y - missileTargetName.GetTextSize().y);

                    // Missile Altitude

                    missileAltitude = new("missileAltitude", new(0, 0), missilePanel);
                    missileAltitude.SetText("ALT --");
                    missileAltitude.SetFontSize(Mathf.RoundToInt(GetFontSize(aircraft) / 1.5f));
                    missileAltitude.SetColor(Color.white);
                    missileAltitude.SetTextAlignment(TextAnchor.MiddleRight);

                    RectTransform altitudeRect = missileAltitude.GetRectTransform();
                    altitudeRect.anchorMin = new(1, 1);
                    altitudeRect.anchorMax = new(1, 1);
                    altitudeRect.pivot = new(1, 1);
                    altitudeRect.anchoredPosition = new(-5, -missileAltitude.GetTextSize().y - 5 - missileSpeed.GetTextSize().y - missileTargetName.GetTextSize().y);

                    // Missile Range

                    missileRange = new("missileRange", new(0, 0), missilePanel);
                    missileRange.SetText("RNG --");
                    missileRange.SetFontSize(Mathf.RoundToInt(GetFontSize(aircraft) / 1.5f));
                    missileRange.SetColor(Color.white);
                    missileRange.SetTextAlignment(TextAnchor.MiddleRight);

                    RectTransform rangeRect = missileRange.GetRectTransform();
                    rangeRect.anchorMin = new(1, 1);
                    rangeRect.anchorMax = new(1, 1);
                    rangeRect.pivot = new(1, 1);
                    rangeRect.anchoredPosition = new(-5, -missileRange.GetTextSize().y - 3 - missileSpeed.GetTextSize().y - missileTargetName.GetTextSize().y - missileAltitude.GetTextSize().y);


                   


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
                                ToggleElements(true);

                                // Velocity Vector
                                Vector3 position = missileCam.transform.position + currentMissile.rb.velocity * 6;
                                Vector3 vector = Vector3.Scale(missileCam.WorldToScreenPoint(position), new Vector3(1f, 1f, 0f)) - screenCenter;
                                vector = ClampToScreen(vector);
                                velocityVector.transform.localPosition = vector;

                                // Orientation Indicator
                                orientationIndicator.transform.localRotation = Quaternion.Euler(0, 0, -missileCam.transform.rotation.eulerAngles.z);


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
                                    lockBox.SetActive(false);
                                }


                                Vector3 viewportTargetPosition = missileCam.WorldToScreenPoint((Vector3)targetPosition);

                                Vector3 uiPos = Vector3.Scale(viewportTargetPosition, new Vector3(1, 1, 0)) - screenCenter;

                                uiPos = ClampToScreen(uiPos); 

                                Plugin.Logger.LogDebug($"{uiPos}, {target}");

                                lockBox.transform.localPosition = uiPos;



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





                                    RectTransform rect = lockBox.GetComponent<RectTransform>();

                                    

                                    rect.sizeDelta = new Vector2(
                                       Mathf.Max(size.x, LockBoxSizeConstraint),
                                       Mathf.Max(size.y, LockBoxSizeConstraint)
                                    );

                                }
                                else
                                {
                                    RectTransform rect = lockBox.GetComponent<RectTransform>();

                                    rect.sizeDelta = new Vector2(LockBoxSizeConstraint, LockBoxSizeConstraint);
                                }

                                if (target != null)
                                {
                                    if (target.NetworkHQ == null)
                                    {
                                        missileTargetName.SetColor(Color.white);
                                    } else
                                    {
                                        missileTargetName.SetColor(((target.NetworkHQ == GameUtils.getHQ()) ? GameAssets.i.HUDFriendly : GameAssets.i.HUDHostile));
                                    }

                                    missileTargetName.SetText(((target is Aircraft) ? target.definition.unitName : target.unitName));

                                    float targetDistance = FastMath.Distance(targetPosition.ToGlobalPosition(), currentMissile.transform.GlobalPosition());

                                    missileRange.SetText($"RNG {UnitConverter.DistanceReading(targetDistance)}");

                                    if (target.GetComponent<Rigidbody>() != null)
                                    {
                                        Vector3 targetVelocity = target.rb.velocity;

                                        Vector3 predictedPosition = targetPosition + targetVelocity * (targetDistance / currentMissile.rb.velocity.magnitude);

                                        Vector3 leadUiPos = Vector3.Scale(missileCam.WorldToScreenPoint(predictedPosition) - screenCenter, new(1, 1, 0));

                                        leadUiPos = ClampToScreen(leadUiPos);

                                        leadIcon.transform.localPosition = leadUiPos;
                                    }
                                }
                                else
                                {
                                    missileTargetName.SetColor(Color.white);
                                    missileTargetName.SetText("No Target");
                                    missileRange.SetText("RNG --");
                                    leadIcon.SetActive(false);

                                }

                                PatchTacScreen.missileIndex.SetText($"{PatchMissile.currentMissileIndex + 1}/{PatchMissile.missiles.Count}");
                                missileSpeed.SetText($"SPD {UnitConverter.SpeedReading(Mathf.Round(currentMissile.speed))}");
                                missileAltitude.SetText($"ALT {UnitConverter.AltitudeReading(currentMissile.radarAlt)}");

                                return;
                            }


                        }
                        else
                        {
                            ToggleElements(false);
                        }

                    }
                    else
                    {
                        ToggleElements(false);
                    }
                }
            }
        }


        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

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

        private static void ToggleElements(bool active)
        {
            velocityVector.SetActive(active);
            lockBox.SetActive(active);
            leadIcon.SetActive(active);

            missileIndex.SetActive(active);
            missileSpeed.SetActive(active);
            missileRange.SetActive(active);
            missileAltitude.SetActive(active);
            missileTargetName.SetActive(active);


        }

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