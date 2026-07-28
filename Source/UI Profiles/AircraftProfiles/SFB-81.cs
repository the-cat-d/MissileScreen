using MissileView.UI;
using UnityEngine;

namespace MissileView.UI_Profiles.AircraftProfiles
{
    internal class SFB // SFB-81 Darkreach
    {
        public static void Load()
        {
            ProfileManager.AddProfile(new Profile
            {

                velocityVectorIconScale = 0.3f,
                lockboxMinSize = 20,
                lockboxCornerScale = 0.2f,

                missilePanelRectSize = new(300, 200),
                missilePanelRectRotation = Quaternion.Euler(0, 0, -90),

                
                velocityVectorThing = 1,
                leadIconThing = 1,
            }, nameof(SFB));
        }
    }
}
