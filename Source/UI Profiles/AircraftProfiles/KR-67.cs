using MissileScreen.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace MissileScreen.UI_Profiles.AircraftProfiles
{
    internal class Multirole1 // KR-67 Ifrit
    {
        public static void Load()
        {
            ProfileManager.AddProfile(new Profile
            {
                ReplacePanelName = "StatusGauges",

                fontSize = 47,

                lockboxCornerScale = 0.5f,
                lockboxMinSize = 70,
                velocityVectorIconScale = 0.4f,
                leadIconScale = 1.3f,

                LeftPanelPivotYOffset = 2.45f,
                RightPanelPivotYOffset = 2.15f,


            }, nameof(Multirole1));
        }
    }
}
