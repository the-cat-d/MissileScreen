using MissileView.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace MissileView.UI_Profiles.AircraftProfiles
{
    internal class Trainer
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
