using UnityEngine;
using UnityEngine.UI;

namespace MissileScreen
{
    internal class GameUtils
    {
        internal static Aircraft getAircraft()
        {
            return SceneSingleton<CombatHUD>.i.aircraft;
        }

        internal static FactionHQ getHQ()
        {
            return getAircraft().NetworkHQ;
        }

        internal static Transform FindChildRecursive(Transform parent, string name)
        {

            foreach (Transform child in parent)
            {

                if (child.name.ToLower() == name.ToLower())
                {
                    return child;
                }

                Transform found = FindChildRecursive(child, name);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        
       


        internal static Vector3 ClampToScreen(Vector3 vector, Vector2 screenSize,Vector2 elementSize)
        {
            return new(Mathf.Clamp(vector.x, (-screenSize.x / 2) + (elementSize.x / 2), (screenSize.x / 2) - (elementSize.x / 2)), Mathf.Clamp(vector.y, (-screenSize.y / 2) + (elementSize.y / 2), (screenSize.y / 2) - (elementSize.y / 2)), 0);
        }


      
        
    }

  
}
