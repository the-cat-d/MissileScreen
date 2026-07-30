using MissileScreen.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace MissileScreen.UI_Profiles.AircraftProfiles
{
    internal class Trainer // T/A-30 Compass
    {
        public static void Load()
        {
            ProfileManager.AddProfile(new Profile
            {

                ReplacePanelName = "frontProfile",
                clearOldPanel = false,

                missilePanelRectSize = new(500, 325),
                missilePanelRectPosition = new(313, 23)
            }, nameof(Trainer));
        }

    }
}
