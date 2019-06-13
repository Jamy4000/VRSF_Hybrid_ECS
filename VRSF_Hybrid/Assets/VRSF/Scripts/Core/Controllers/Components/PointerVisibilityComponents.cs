using UnityEngine;
using VRSF.Core.Raycast;

namespace VRSF.Core.Controllers
{
    /// <summary>
    /// Contains all the variable for the ControllerPointer Systems
    /// </summary>
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity), typeof(ScriptableRaycastComponent))]
    public class PointerVisibilityComponents : MonoBehaviour
    {
        [Tooltip("The current state of the Pointer.")]
        public EPointerState PointerState = EPointerState.ON;

        [Tooltip("How fast the pointer is disappearing when not hitting something")]
        public float DisappearanceSpeed = 1.0f;

        [HideInInspector] public float _BaseStartWidth;

        [HideInInspector] public float _BaseEndWidth;
    }
} 