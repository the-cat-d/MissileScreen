using MissileView.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace MissileView.UI_Profiles.AircraftProfiles
{
    internal class VTOLTrainer1 // VT-7 Vagrant
    {
        public static void Load()
        {
            ProfileManager.AddProfile(new Profile
            {

                ReplacePanelName = "frontProfile",


                missilePanelRectSize = new(509, 333),
                missilePanelRectPosition = new(313, 25),

                hideGameObjectNames = ["nozzleGauge"],

                
            }, nameof(VTOLTrainer1));
        }

    }
}
