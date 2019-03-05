using ScriptableFramework.Variables;
using UnityEngine;

namespace VRSF.Interactions.Components
{
    /// <summary>
    /// Contains the variables for the OnColliderClickSystems
    /// </summary>
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class OnColliderClickComponent : MonoBehaviour
    {
        /// <summary>
        /// Whether the user is able to click on stuffs with the left trigger
        /// </summary>
        public static bool LeftTriggerCanClick = true;

        /// <summary>
        /// Whether the user is able to click on stuffs with the right trigger
        /// </summary>
        public static bool RightTriggerCanClick = true;

        /// <summary>
        /// Reference to the Left Click Variables, meaning the Trigger
        /// </summary>
        [HideInInspector] public BoolVariable LeftClickBool;

        /// <summary>
        /// Reference to the Right Click Variables, meaning the Trigger
        /// </summary>
        [HideInInspector] public BoolVariable RightClickBool; 
    }
}