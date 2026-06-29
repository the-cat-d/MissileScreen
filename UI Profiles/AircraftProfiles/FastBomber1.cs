using MissileView.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace MissileView.UI_Profiles.AircraftProfiles
{
    internal class FastBomber1
    {

        public static void Load()
        {
            ProfileManager.AddProfile(new Profile
            {


                fontSize = 28,



            }, nameof(FastBomber1));
        }

    }
}
