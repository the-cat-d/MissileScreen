using MissileScreen.UI;
using UnityEngine;


namespace MissileScreen.UI_Profiles.AircraftProfiles
{
    internal class FS3 // FS-3 Ternion
    {
        public static void Load()
        {
            ProfileManager.AddProfile(new Profile
            {

                replacePanelName = "WeaponPanel",
                hideGameObjectNames = ["WeaponPanel"],
                
                fontSize = 30,
                
                missilePanelRectSize = new(315, 180),
                missilePanelRectPosition = new(292.6f, 128),
                
                velocityVectorIconScale = 0.3f,
                
 
             
            }, "P_Trisurface1_definition");
        }
    }
}
