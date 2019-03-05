using UnityEngine;
using VRSF.Utils.Components;

namespace VRSF.Controllers.Components
{
    /// <summary>
    /// Contains all the variable for the ControllerPointer Systems
    /// </summary>
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity), typeof(ScriptableRaycastComponent), typeof(LineRenderer))]
    public class ControllerPointerComponents : MonoBehaviour
    {
        public EPointerState _PointerState = EPointerState.ON;
        
        // LineRenderer attached to the right and left controllers
        public OptionalLaserObjects OptionalLasersObjects;

        [HideInInspector] public bool IsSetup = false;
    }
} 