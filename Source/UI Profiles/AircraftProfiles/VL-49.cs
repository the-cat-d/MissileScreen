using MissileScreen.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace MissileScreen.UI_Profiles.AircraftProfiles
{
    internal class QuadVTOL1 // VL-49 Tarantula
    {
        public static void Load()
        {
            ProfileManager.AddProfile(new Profile
            {
                missilePanelRectSize = new(490, 364),
                missilePanelRectPosition = new(0, 0),

      
                hideGameObjectNames = ["BasicFlightInstrument", "wingAngleGauge"],


            }, nameof(QuadVTOL1));
        }
    }
}
