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


                missilePanelRectSize = new(227, 120),
                missilePanelRectPosition = new(140, 192),

                fontSize = 21,

                lockboxCornerScale = 0.5f,
                lockboxMinSize = 45,
                velocityVectorIconScale = 0.45f,
                leadIconScale = 0.3f,

                LeftPanelPivotYOffset = 2.61f,
                RightPanelPivotYOffset = 2.35f


            }, nameof(Multirole1));
        }
    }
}
