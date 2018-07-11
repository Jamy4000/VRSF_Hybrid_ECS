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
        [Tooltip("Reference to the Left and Right Click Variables, meaning the Trigger")]
        [HideInInspector] public BoolVariable LeftClickBool;
        [HideInInspector] public BoolVariable RightClickBool; 
    }

}