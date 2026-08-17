using MissileScreen.UI;
using UnityEngine;


namespace MissileScreen.UI_Profiles.AircraftProfiles
{
    internal class FS99 // FS-99 Shrike
    {
        public static void Load()
        {
            ProfileManager.AddProfile(new Profile
            {

                replacePanelName = "LeftPanel",
                hideGameObjectNames = ["LeftPanel"],
                
                // fontSize = 50,
                // velocityVectorIconScale = 0.6f,
                // leadIconScale = 1.6f,
                // lockboxCornerScale = 0.4f,
                // lockboxMinSize = 70,
                //
                // leftPanelPivotYOffset = 2.45f,
                // rightPanelPivotYOffset = 2.15f,
                //

            }, "Aryx_LightFighter1_Definition");
        }
    }
}
