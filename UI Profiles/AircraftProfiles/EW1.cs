using MissileView.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace MissileView.UI_Profiles.AircraftProfiles
{
    internal class EW1 // EW-25 Medusa
    {
        public static void Load()
        {
            ProfileManager.AddProfile(new Profile
            {

                missilePanelRectSize = new(445, 184),
                missilePanelRectPosition = new(284, 160)

            }, nameof(EW1));
        }
    }
}
