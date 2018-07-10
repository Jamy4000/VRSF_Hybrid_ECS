using UnityEngine;

namespace VRSF.Controllers.Components
{
    /// <summary>
    /// Contains all the variable for the ControllerPointer Systems
    /// </summary>
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class PointerColorComponent : MonoBehaviour
    {
        // LineRenderer attached to the right and left controllers
        [HideInInspector] public LineRenderer _RightHandPointer;
        [HideInInspector] public LineRenderer _LeftHandPointer;

        [HideInInspector] public Transform _CameraRigTransform;

        [HideInInspector] public bool _IsSetup = false;
    }
}