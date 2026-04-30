using MissileView.Patches;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MissileView.UI
{
    
    internal class Profile
    {

        public static Quaternion rotationDefault = Quaternion.Euler(1,1,1);

        // UI

        public GameUtils.Draw.UILabel missileName;
        public GameUtils.Draw.UILabel missileTargetName;
        public GameUtils.Draw.UILabel missileIndex;
        public GameUtils.Draw.UILabel missileSpeed;
        public GameUtils.Draw.UILabel missileRange;
        public GameUtils.Draw.UILabel missileAltitude;

        public GameObject velocityVector;
        public GameObject orientationIndicator;
        public GameObject lockBox;
        public GameObject leadIcon;


        // Misc


        public Transform weaponPanel;
        public Transform missilePanel;


        // Unique Aircraft Config

        public Vector2 missilePanelRectPosition;
        public Vector2 missilePanelRectSize;
        public Quaternion missilePanelRectRotation = rotationDefault;

        public float RightPanelPivotYOffset = 2.2f;
        public float RightPanelPivotYOffsetIncrement = 1;

        public float LeftPanelPivotYOffset = 2.5f;

        public float velocityVectorIconScale = 0.5f;
        public string ReplacePanelName = "weaponPanel";
        public int fontSize = 37;
        public float leadIconScale = 1;
        public float lockboxCornerScale = 0.3f;
        public int lockboxMinSize = 50;
        public bool clearOldPanel = false;
        public int hierachyOrder = -1;

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


    }
}