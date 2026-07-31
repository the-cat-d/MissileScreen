using MissileScreen.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace MissileScreen.UI_Profiles.AircraftProfiles
{
    internal class COIN // cricket
    {
        public static void Load()
        {
            ProfileManager.AddProfile(new Profile
            {

                replacePanelName = "EngPanel",
                clearOldPanel = false,

                lockboxMinSize = 36,
                fontSize = 24,

                missilePanelRectSize = new(160, 220),
                missilePanelRectPosition = new(407, 25.5f),
                velocityVectorIconScale = 0.3f,

                leftPanelPivotYOffset = 2.6f,
                rightPanelPivotYOffset = 2.3f,

            }, "CI-22");
        }

    }
}
