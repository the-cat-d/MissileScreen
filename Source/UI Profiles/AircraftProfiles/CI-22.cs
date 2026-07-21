using MissileView.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace MissileView.UI_Profiles.AircraftProfiles
{
    internal class COIN // cricket
    {
        public static void Load()
        {
            ProfileManager.AddProfile(new Profile
            {

                ReplacePanelName = "EngPanel",
                clearOldPanel = false,

                lockboxMinSize = 36,
                fontSize = 24,

                missilePanelRectSize = new(160, 220),
                missilePanelRectPosition = new(407, 25.5f)
            }, "CI-22");
        }

    }
}
