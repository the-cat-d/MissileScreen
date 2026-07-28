using MissileView.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace MissileView.UI_Profiles.AircraftProfiles
{
    internal class QuadVTOL1 // VL-49 Taruntula
    {
        public static void Load()
        {
            ProfileManager.AddProfile(new Profile
            {
                missilePanelRectSize = new(490, 364),
                missilePanelRectPosition = new(0, 0),

                //fontSize = 30,
                //lockboxMinSize = 20,



                //LeftPanelPivotYOffset = 2.42f,
                //RightPanelPivotYOffset = 2.4f,

                hideGameObjectNames = ["BasicFlightInstrument", "wingAngleGauge"],


            }, nameof(QuadVTOL1));
        }
    }
}
