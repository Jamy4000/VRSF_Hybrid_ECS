using ScriptableFramework.Variables;
using UnityEngine;
using VRSF.Controllers;
using VRSF.Gaze;
using VRSF.Inputs;

namespace VRSF.Interactions.Components
{
    /// <summary>
    /// Contains the variables for the OnColliderClickSystems
    /// </summary>
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class OnColliderClickComponents : MonoBehaviour
    {
        [Tooltip("Reference to the Left and Right Click Variables, meaning the Trigger")]
        [HideInInspector] public BoolVariable LeftClickBool;
        [HideInInspector] public BoolVariable RightClickBool;

        [Tooltip("Wheter we check if the Raycast is over the objects, set at runtime by checking if we use the controllers or the gaze")]
        [HideInInspector] public bool CheckRaycast = true;
    }

}