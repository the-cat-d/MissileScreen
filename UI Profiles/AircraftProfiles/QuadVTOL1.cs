using MissileView.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace MissileView.UI_Profiles.AircraftProfiles
{
    internal class QuadVTOL1
    {
        public static void Load()
        {
            ProfileManager.AddProfile(new Profile
            {


                fontSize = 30,
                lockboxMinSize = 20,

                LeftPanelPivotYOffset = 2.52f,
                RightPanelPivotYOffset = 2.25f


            }, nameof(QuadVTOL1));
        }
    }
}
