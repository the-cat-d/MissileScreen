using MissileView.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace MissileView.UI_Profiles.AircraftProfiles
{
    internal class Multirole1 // KR-67 Ifrit
    {
        public static void Load()
        {
            ProfileManager.AddProfile(new Profile
            {
                ReplacePanelName = "StatusGauges",

                fontSize = 47,

                lockboxCornerScale = 0.6f,
                lockboxMinSize = 63,
                velocityVectorIconScale = 0.6f,
                leadIconScale = 1.2f,

        


            }, nameof(Multirole1));
        }
    }
}
