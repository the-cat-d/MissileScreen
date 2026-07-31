using MissileScreen.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace MissileScreen.UI_Profiles.AircraftProfiles
{
    internal class FastBomber1 // AB-4 Alkyon
    {

        public static void Load()
        {
            ProfileManager.AddProfile(new Profile
            {


                fontSize = 28,


                rightPanelPivotYOffset = 2.25f,

            }, nameof(FastBomber1));
        }

    }
}
