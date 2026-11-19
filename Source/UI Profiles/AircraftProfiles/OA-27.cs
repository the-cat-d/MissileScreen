using MissileScreen.UI;
using UnityEngine;


namespace MissileScreen.UI_Profiles.AircraftProfiles
{
    internal class OA27 // OA-27 Cavalier
    {
        public static void Load()
        {
            ProfileManager.AddProfile(new Profile
            {

                replacePanelName = "BasicFlightInstrument",
                hideGameObjectNames = ["BasicFlightInstrument"],
                clearOldPanel = false,
                
                fontSize = 23,
                velocityVectorIconScale = 0.35f,
                leadIconScale = 0.85f,
                lockboxCornerScale = 0.25f,
                lockboxMinSize = 45,

                
                missilePanelRectSize = new(230.49f, 300),
                missilePanelRectPosition =  new(118.45f,100),
            }, "Aryx_PropAttacker1");
        }
    }
}
