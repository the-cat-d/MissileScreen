using MissileScreen.UI;
using UnityEngine;


namespace MissileScreen.UI_Profiles.AircraftProfiles
{
    internal class F16M // F-16M King Viper
    {
        public static void Load()
        {
            ProfileManager.AddProfile(new Profile
            {

                replacePanelName = "Engine",
                hideGameObjectNames = ["Engine"],
                
                fontSize = 50,
                velocityVectorIconScale = 0.6f,
                leadIconScale = 1.6f,
                lockboxCornerScale = 0.4f,
                lockboxMinSize = 70,
                
                
  
             

            }, "Aryx_F16M_KingViper_AircraftDefinition");
        }
    }
}
