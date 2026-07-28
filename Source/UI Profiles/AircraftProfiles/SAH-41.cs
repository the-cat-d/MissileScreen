using MissileView.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace MissileView.UI_Profiles.AircraftProfiles
{
    internal class AttackHelo1 // SAH-41 Chicane
    {

      

        public static void Load()
        {
            ProfileManager.AddProfile(new Profile
            {
                ReplacePanelName = "BasicFlightInstrument",
                clearOldPanel = false,

                lockboxMinSize = 42,
                fontSize = 24,

                LeftPanelPivotYOffset = 2.6f,
                RightPanelPivotYOffset = 2.3f,

           
            }, nameof(AttackHelo1));
        }

    }
}
