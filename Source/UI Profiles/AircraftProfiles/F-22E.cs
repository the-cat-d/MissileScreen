using MissileScreen.UI;
using UnityEngine;


namespace MissileScreen.UI_Profiles.AircraftProfiles
{
    internal class KingRaptor // F-22E King Raptor
    {
        public static void Load()
        {
            ProfileManager.AddProfile(new Profile
            {

                replacePanelName = "RightScreen",
                clearOldPanel = false,
                    
                fontSize = 26,
                velocityVectorIconScale = 0.25f,
                leadIconScale = 0.8f,
                lockboxCornerScale = 0.25f,
                lockboxMinSize = 40,

                
            }, "Aryx_KingRaptor_Definition");
        }
    }
}
