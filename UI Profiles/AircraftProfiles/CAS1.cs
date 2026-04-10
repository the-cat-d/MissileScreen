using MissileView.UI;
using UnityEngine;


namespace MissileView.UI_Profiles.AircraftProfiles
{
    internal class CAS1
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
            }, nameof(CAS1));
        }
    }
}
