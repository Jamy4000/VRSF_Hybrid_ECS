using ScriptableFramework.Variables;
using UnityEngine;
using VRSF.Controllers;

namespace VRSF.Core.Raycast
{
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class ScriptableRaycastComponent : MonoBehaviour
    {
        [Header("The Raycast Origin for this script")]
        public EHand RayOrigin = EHand.NONE;

        /// <summary>
        /// The Transform of the origin from the Ray
        /// </summary>
        [HideInInspector] public Transform RayOriginTransform;

        /// <summary>
        /// The Layers ignored by the raycast
        /// </summary>
        [HideInInspector] public LayerMask IgnoredLayers;

        /// <summary>
        /// Reference to the RaycastHitVariable link to this hand
        /// </summary>
        [HideInInspector] public RaycastHitVariable RaycastHitVar;

        /// <summary>
        /// Reference to the RayVariable link to this hand
        /// </summary>
        [HideInInspector] public RayVariable RayVar;

        /// <summary>
        /// Wheter we check the raycast, set at runtime by checking if we use the controllers or the gaze
        /// </summary>
        [HideInInspector] public bool CheckRaycast = false;

        /// <summary>
        /// Wheter we check the raycast, set at runtime by checking if we use the controllers or the gaze
        /// </summary>
        [HideInInspector] public bool IsSetup = false;

        /// <summary>
        /// The Maximum distance to which the Raycast should go
        /// </summary>
        private float _raycastMaxDistance;

        private Gaze.GazeParametersVariable _gazeParameters;
        private ControllersParametersVariable _controllersParameters;

        /// <summary>
        /// The Maximum distance to which the Raycast should go
        /// </summary>
        public float RaycastMaxDistance
        {
            get
            {
                try
                {
                    switch (RayOrigin)
                    {
                        case EHand.GAZE:
                            return _gazeParameters.DefaultDistance;
                        case EHand.LEFT:
                            return _controllersParameters.MaxDistancePointerLeft;
                        case EHand.RIGHT:
                            return _controllersParameters.MaxDistancePointerRight;
                        default:
                            return Mathf.Max(_controllersParameters.MaxDistancePointerLeft, _controllersParameters.MaxDistancePointerRight);
                    }
                }
                catch
                {
                    if (RayOrigin == EHand.GAZE)
                        _gazeParameters = Gaze.GazeParametersVariable.Instance;
                    else
                        _controllersParameters = ControllersParametersVariable.Instance;

                    return 0;
                }
            }

            set => _raycastMaxDistance = value;
        }
    }
}