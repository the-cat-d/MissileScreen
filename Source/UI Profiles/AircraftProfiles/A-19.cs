using MissileScreen.UI;
using UnityEngine;


namespace MissileScreen.UI_Profiles.AircraftProfiles
{
    internal class CAS1 // A-19 Brawler
    {
        public static void Load()
        {
            ProfileManager.AddProfile(new Profile
            {

                ReplacePanelName = "SystemStatus",
                clearOldPanel = false,

                missilePanelRectSize = new(384, 250),
                missilePanelRectPosition = new(312, 24),
                missilePanelRectRotation = Quaternion.identity
            }, "CAS1");
        }
    }
}
