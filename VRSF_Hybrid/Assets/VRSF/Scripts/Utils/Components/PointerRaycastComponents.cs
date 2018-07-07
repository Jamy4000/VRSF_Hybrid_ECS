using UnityEngine;

namespace VRSF.Utils.Components
{
    /// <summary>
    /// Contains the variables for the PointerRaycastSystems
    /// </summary>
    public class PointerRaycastComponents : MonoBehaviour
    {
        // Wheter we check the raycast, set at runtime by checking if we use the controllers or the gaze
        [HideInInspector] public bool CheckRaycast = true;
    }
}