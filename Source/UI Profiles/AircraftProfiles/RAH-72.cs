using MissileScreen.UI;
using UnityEngine;


namespace MissileScreen.UI_Profiles.AircraftProfiles
{
    internal class RAH72 // RAH-72 Knockout
    {
        public static void Load()
        {
            ProfileManager.AddProfile(new Profile
            {

                replacePanelName = "WeaponPanel",
                hideGameObjectNames = ["WeaponPanel"],
                
                fontSize = 40,
                // velocityVectorIconScale = 0.6f,
                // leadIconScale = 1.6f,
                // lockboxCornerScale = 0.4f,
                // lockboxMinSize = 70,
                //
                // leftPanelPivotYOffset = 2.45f,
                // rightPanelPivotYOffset = 2.15f,
                //

            }, "Aryx_LightHelicopter1_Definition");
        }
    }
}
