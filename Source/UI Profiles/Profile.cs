using MissileScreen.Source;
using System.Collections.Generic;
using UnityEngine;


namespace MissileScreen.UI
{
    
    internal class Profile
    {
        // Public Static

        public static readonly Quaternion rotationDefault = Quaternion.Euler(1,1,1);
        public static readonly Vector2 positionDefault = new Vector2(1.0001f, 1.001f); // yes really specific ik...



        // UI

        public GameObject screen;
        
        public UIDraw.UILabel missileName;
        public UIDraw.UILabel missileTargetName;
        public UIDraw.UILabel missileIndex;
        public UIDraw.UILabel missileSpeed;
        public UIDraw.UILabel missileRange;
        public UIDraw.UILabel missileAltitude;


        public UIDraw.UIImage velocityVector;
        public UIDraw.UIImage orientationIndicator;
        public UIDraw.UIImage leadIcon;

        public GameObject lockBox;


        // Misc


        public Transform weaponPanel;
        public Transform missilePanel;

        public Vector2 missilePanelSize;

        // Hides a specific game object from the TacScreen when the missile is active.
        public readonly List<GameObject>  hideGameObjects = new List<GameObject>();


        // Unique Aircraft Config

        public Vector2 missilePanelRectPosition = positionDefault;
        public Vector2 missilePanelRectSize;
        public Quaternion missilePanelRectRotation = rotationDefault;

        public float rightPanelPivotYOffset = 2.2f;
        public float rightPanelPivotYOffsetIncrement = 1;

        public float leftPanelPivotYOffset = 2.5f;

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
           
            velocityVector.SetActive(active);
           
            lockBox.SetActive(active);
         
            leadIcon.SetActive(active);

           
            missileIndex.SetActive(active);
            missileSpeed.SetActive(active);
            missileRange.SetActive(active);
            missileAltitude.SetActive(active);
            missileTargetName.SetActive(active);
           

            if (hideGameObjects.Count > 0)
            {
                foreach (var go in hideGameObjects)
                {
                    if (go)
                    {
                        go.SetActive(!active);
                    }
                }
            }


        }

        public  void NoMissileDisplay()
        {
            
            missileName.SetText("No Missile");
            missileIndex.SetActive(false);
            velocityVector.SetActive(false);
            screen.SetActive(false);

            if (hideGameObjects.Count > 0)
            {
                foreach (var go in hideGameObjects)
                {
                   if (go)
                   {
                        go.SetActive(true);
                   }
                }
            }
        }

       
      

    }
}