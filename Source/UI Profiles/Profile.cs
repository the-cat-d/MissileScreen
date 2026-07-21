using UnityEngine;


namespace MissileView.UI
{
    
    internal class Profile
    {
        // Public Static

        public static Quaternion rotationDefault = Quaternion.Euler(1,1,1);
        public static Vector2 positionDefault = new Vector2(1.0001f, 1.001f); // yes really specific ik...



        // UI

        public GameObject screen;

        public GameUtils.Draw.UILabel missileName;
        public GameUtils.Draw.UILabel missileTargetName;
        public GameUtils.Draw.UILabel missileIndex;
        public GameUtils.Draw.UILabel missileSpeed;
        public GameUtils.Draw.UILabel missileRange;
        public GameUtils.Draw.UILabel missileAltitude;


        public GameUtils.Draw.UIImage velocityVector;
        public GameUtils.Draw.UIImage orientationIndicator;
        public GameUtils.Draw.UIImage leadIcon;

        public GameObject lockBox;


        // Misc


        public Transform weaponPanel;
        public Transform missilePanel;




        // Unique Aircraft Config

        public Vector2 missilePanelRectPosition = positionDefault;
        public Vector2 missilePanelRectSize;
        public Quaternion missilePanelRectRotation = rotationDefault;

        public float RightPanelPivotYOffset = 2.2f;
        public float RightPanelPivotYOffsetIncrement = 1;

        public float LeftPanelPivotYOffset = 2.5f;

        public float velocityVectorIconScale = 0.4f;
        public string ReplacePanelName = "weaponPanel";
        public int fontSize = 37;
        public float leadIconScale = 1;
        public float lockboxCornerScale = 0.3f;
        public int lockboxMinSize = 50;
        public bool clearOldPanel = true;
        public int hierachyOrder = -1;

        public float velocityVectorThing = 4;
        public float leadIconThing = 2;

       

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


        }

        public  void NoMissileDisplay()
        {
            
            missileName.SetText("No Missile");
            missileIndex.SetActive(false);
            velocityVector.SetActive(false);
            screen.SetActive(false);
        }

       
      

    }
}