using MissileScreen.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace MissileScreen.UI_Profiles.AircraftProfiles
{
    internal class AttackHelo1 // SAH-41 Chicane
    {

      

        public static void Load()
        {
            ProfileManager.AddProfile(new Profile
            {
                replacePanelName = "BasicFlightInstrument",
                clearOldPanel = false,

                lockboxMinSize = 42,
                fontSize = 24,

                leftPanelPivotYOffset = 2.6f,
                rightPanelPivotYOffset = 2.3f,

           
            }, nameof(AttackHelo1));
        }

    }
}
