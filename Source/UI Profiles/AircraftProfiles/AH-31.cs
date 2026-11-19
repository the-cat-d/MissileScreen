using MissileScreen.UI;
using UnityEngine;


namespace MissileScreen.UI_Profiles.AircraftProfiles
{
    internal class AH31 // AH-31 Kukri
    {
        public static void Load()
        {
            ProfileManager.AddProfile(new Profile
            {

                replacePanelName = "TelemetryPanel",
                clearOldPanel = false,
                //
                // missilePanelRectSize = new(384, 250),
                // missilePanelRectPosition = new(312, 24),
                // missilePanelRectRotation = Quaternion.identity
            }, "AH31_Kukri");
        }
    }
}
