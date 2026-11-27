using MissileScreen.Source;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace MissileScreen.UI
{
    
    internal class Profile
    {
        // Public Static

        public static readonly Quaternion _rotationDefault = Quaternion.Euler(1,1,1);
        public static readonly Vector2 _positionDefault = new Vector2(1.0001f, 1.001f); // yes really specific ik...



        // UI

        public GameObject screen;
        
        public UIDraw.UILabel missileName;
        public UIDraw.UILabel missileTargetName;
        public UIDraw.UILabel missileIndex;
        public UIDraw.UILabel missileSpeed;
        public UIDraw.UILabel missileToF;
        public UIDraw.UILabel missileRange;
        public UIDraw.UILabel missileAltitude;
        
        public UIDraw.UIImage velocityVector;
        public UIDraw.UIImage orientationIndicator;
        public UIDraw.UIImage leadIcon;
        public UIDraw.UIText pitchLabel;

        public RectTransform pitchIndicator;
        public RectTransform yawIndicator;
        
        public GameObject lockBox;
        public readonly List<Image> lockboxCorners = [];

        // Misc


        public Transform weaponPanel;
        public Transform missilePanel;
        public RawImage screenImage;
        
        public Vector2 missilePanelSize;

        public bool useSimplePitchIndicator = true;
        
        public int degreeIncrement = 10;
        public List<UIDraw.UILadderRung> rungs = [];
        
        // Hides a specific game object from the TacScreen when the missile is active.
        public readonly List<GameObject>  hideGameObjects = new List<GameObject>();


        // Unique Aircraft Config

        public Vector2 missilePanelRectPosition = _positionDefault;
        public Vector2 missilePanelRectSize;
        public Quaternion missilePanelRectRotation = _rotationDefault;

     

        public float velocityVectorIconScale = 0.4f;
        public string replacePanelName = "weaponPanel";
        public int fontSize = 37;
        public float leadIconScale = 1;
        public float lockboxCornerScale = 0.3f;
        public int lockboxMinSize = 50;
        public bool clearOldPanel = true;
        public int hierarchyOrder = -1;

        public float velocityVectorThing = 4;
        public float leadIconThing = 2;

       
        // Hides specific game objects from the TacScreen when a missile is active.
        public List<string> hideGameObjectNames = new List<string>();

        public void ToggleElements(bool active)
        {
            if (PluginConfig.hideNoMissile.Value)
            {
                missileName?.SetActive(active);
            }
            
            velocityVector?.SetActive(active);
           
            lockBox?.SetActive(active) ;
         
            leadIcon?.SetActive(active);
            
            if (pitchIndicator && pitchIndicator.gameObject) pitchIndicator.gameObject.SetActive(active);
            if (yawIndicator && yawIndicator.gameObject) yawIndicator.gameObject.SetActive(active);
            
          
            missileIndex?.SetActive(active);
            
            missileSpeed?.SetActive(active);
            missileToF?.SetActive(active);
            missileRange?.SetActive(active);
            missileAltitude?.SetActive(active);
            missileTargetName?.SetActive(active);


            if (hideGameObjects.Count <= 0) return;

            foreach (GameObject go in hideGameObjects.Where(go => go))
            {
                go.SetActive(!active);
            }


        }

        public  void NoMissileDisplay()
        {

            if (PluginConfig.hideNoMissile.Value)
            {
                missileName?.SetActive(false);
            }
            else
            {
                missileName?.SetText("No Missile");
            }

            
            missileIndex?.SetActive(false);
            velocityVector?.SetActive(false);
            if (screen) screen.SetActive(false);

            if (hideGameObjects.Count > 0)
            {
                foreach (GameObject go in hideGameObjects)
                {
                 
                    if(go) go.SetActive(true);
                   
                }
            }
        }

        public void SetMissileName(string name)
        {
                missileName?.SetText(name);
        }

        public void SetMissileIndex(int index,int listCount,bool setActive=true)
        {
           
                if (setActive) missileIndex?.SetActive(true);
            
                missileIndex?.SetText($"{index + 1}/{listCount}");
            
        }


        private static string FormatTime(float time)
        {
            if (float.IsInfinity(time) || float.IsNegative(time))
            {
                return "--:--";
            }
            
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            
            return $"{minutes:00}:{seconds:00}";
        }

        private static float CalculateETA(Vector3 targetPos, Vector3 missilePos,Vector3 missileVel, Vector3 targetVel)
        {
            float targetDistance = Vector3.Distance(targetPos,  missilePos);
            Vector3 targetNormalDir = (targetPos - missilePos) / targetDistance;

            Vector3 relativeVelocity =  missileVel -targetVel;
            float closingSpeed = Vector3.Dot(relativeVelocity, targetNormalDir);
            
            return targetDistance / closingSpeed;
            
        }
        
        public void SetMissileToF(Unit target, Missile missile)
        {
            WeaponInfo missileInfo = missile.GetWeaponInfo();
            OpticalSeekerBomb seeker = missile.seeker as OpticalSeekerBomb;
            
            if (missileInfo.nuclear && seeker)
            {
                missileToF?.SetText(missile.timeSinceSpawn <= seeker.armDelay ? $"ToD {Mathf.Floor(missile.timeSinceSpawn)}/{seeker.armDelay}" : "ToD --/--");
            }
            else
            {
                float eta;
            
                if (target)
                {
                    eta = CalculateETA(target.transform.position, missile.transform.position, missile.rb.velocity, missile.targetVel);

                } else if (missileInfo.bomb || missileInfo.laserGuided || missileInfo.glideBomb)
                {
                    

                    if (Physics.Raycast(missile.transform.position, missile.rb.velocity.normalized, out RaycastHit pip, Mathf.Infinity,~PhysicsLayers.IgnoreCollisions))
                    {
                        SetMissileRange(Vector3.Distance(missile.transform.position,pip.point));
                        eta = CalculateETA(pip.point, missile.transform.position, missile.rb.velocity,missile.targetVel );
                    }
                    else
                    {
                        eta = -1;
                    }
                    
                    
                }
                else
                {
                    eta = -1;
                }
            
            
                missileToF?.SetText(eta > 0 ? $"ToF {FormatTime(eta)}" : "ToF --:--");
            }
            
           
        }
       
        
        public void SetMissileSpeed(float speed)
        {
            missileSpeed?.SetText($"SPD {UnitConverter.SpeedReading(Mathf.Round(speed))}");
            
        }
        
        public void SetMissileAltitude(float altitude)
        {
            missileAltitude?.SetText($"ALT {UnitConverter.AltitudeReading(altitude)}");
        }
        
        public void SetMissileRange(float? targetDistance = null)
        {
            
            
            missileRange?.SetText(targetDistance != null ? $"RNG {UnitConverter.DistanceReading((float)targetDistance)}" : "RNG --");
            
        }
        
        public void SetMissileTarget(string name,Color? color = null)
        {
            if (missileTargetName != null)
            {
                missileTargetName.SetColor(color ?? Color.white);
                missileTargetName.SetText(name);
            }
        }

    
        public void UpdatePitchIndicator(Transform missileAngle)
        {
          
            
            
            float pitch = Mathf.Asin(Mathf.Clamp(missileAngle.forward.y,-1,1)) * Mathf.Rad2Deg;


            if (useSimplePitchIndicator)
            {
                pitchLabel?.SetText($"{Mathf.Floor(pitch)}°");
            }
            else
            {
                foreach (UIDraw.UILadderRung rung in rungs)
                {
                    if (!rung.GetGameObject())
                    {
                        rungs.Remove(rung);
                        return;
                    }
                
                    float rel = rung.degree - pitch;
                    float pitchOffset = (Mathf.Tan(rel * Mathf.Deg2Rad) / Mathf.Tan((float)PluginConfig.cameraFOV.Value * Mathf.Deg2Rad * 0.5f)) * (missilePanelSize.y * 0.5f);
                
                    rung.SetPosition(new Vector2(-missilePanelSize.x * PluginConfig.pitchIndicatorPanelPosition.Value,pitchOffset));
                }
            

                Vector3 upwardVectorRef = Vector3.ProjectOnPlane(Vector3.up,missileAngle.forward).normalized;
                float roll = Vector3.SignedAngle(upwardVectorRef, missileAngle.up, missileAngle.forward);

                pitchIndicator.localEulerAngles = new(0,0,-roll);
            }
        }
        
        public void UpdateElementsColor()
        {
            Plugin.logger.LogDebug(missileRange == null);
            missileName?.SetColor();
            missileTargetName?.SetColor();
            missileIndex?.SetColor();
            missileSpeed?.SetColor();
            missileToF?.SetColor();
            missileRange?.SetColor();
            missileAltitude?.SetColor();
            velocityVector?.SetColor();
            orientationIndicator?.SetColor();
            leadIcon?.SetColor();
            velocityVector?.SetColor();
            

            foreach (Image lockboxImage in lockboxCorners.Where(lockboxImage => lockboxImage))
            {
                lockboxImage.color = PluginConfig.lockBoxColor.Value.WithAlpha(PluginConfig.hmdMissileScreen.Value ? PluginConfig.hmdMissileScreenTransparency.Value : 1);
            }

            foreach (UIDraw.UILadderRung rung in rungs)
            {
                rung.SetColor(PluginConfig.pitchIndicatorColor.Value);
            }
            
            if (screenImage) screenImage.color = new(1, 1, 1, PluginConfig.hmdMissileScreen.Value ? PluginConfig.hmdMissileScreenTransparency.Value : 1);
            
        }

        public void Destroy()
        {
            rungs.Clear();
            Object.Destroy(missilePanel.gameObject);
            
        }

    }
}