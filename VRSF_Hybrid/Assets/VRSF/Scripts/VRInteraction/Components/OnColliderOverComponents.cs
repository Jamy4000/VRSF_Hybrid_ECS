using UnityEngine;
using VRSF.Controllers;
using VRSF.Gaze;

namespace VRSF.Interactions.Components
{
    /// <summary>
    /// Contains the variables for the OnColliderOverSystems
    /// </summary>
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class OnColliderOverComponents : MonoBehaviour
    {
        [Tooltip("Wheter we check if the Raycast is over the objects, set at runtime by checking if we use the controllers or the gaze")]
        [HideInInspector] public bool CheckRaycast = true;
    }
}
