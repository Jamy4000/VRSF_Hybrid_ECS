using ScriptableFramework.Variables;
using UnityEngine;
using VRSF.Core.Controllers;

namespace VRSF.Core.Raycast
{
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class ControllersScriptableRaycastComponent : MonoBehaviour
    {
        [Header("The Raycast Origin for this script")]
        public ERayOrigin RayOrigin = ERayOrigin.NONE;

        [Header("Wheter we want to check the raycast for this component")]
        public bool CheckRaycast = false;

        /// <summary>
        /// The Transform of the origin from the Ray
        /// </summary>
        [HideInInspector] public Transform RayOriginTransform;

        /// <summary>
        /// The Layers ignored by the raycast
        /// </summary>
        [HideInInspector] private LayerMask _ignoredLayers;

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
        [HideInInspector] public bool IsSetup = false;

        /// <summary>
        /// The Maximum distance to which the Raycast should go
        /// </summary>
        private float _raycastMaxDistance;
        
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
                        case ERayOrigin.LEFT_HAND:
                            return _controllersParameters.MaxDistancePointerLeft;
                        case ERayOrigin.RIGHT_HAND:
                            return _controllersParameters.MaxDistancePointerRight;
                        default:
                            return Mathf.Max(_controllersParameters.MaxDistancePointerLeft, _controllersParameters.MaxDistancePointerRight);
                    }
                }
                catch
                {
                    _controllersParameters = ControllersParametersVariable.Instance;
                    return 0;
                }
            }

            set => _raycastMaxDistance = value;
        }

        public LayerMask IgnoredLayers
        {
            get
            {
                try
                {
                    switch (RayOrigin)
                    {
                        case ERayOrigin.LEFT_HAND:
                            return _controllersParameters.GetExclusionsLayer(EHand.LEFT);
                        case ERayOrigin.RIGHT_HAND:
                            return _controllersParameters.GetExclusionsLayer(EHand.RIGHT);
                        default:
                            Debug.LogError("<b>[VRSF] :</b> Please specify a valid RayOrigin in the ScriptableRaycastComponent.", gameObject);
                            return -1;
                    }
                }
                catch
                {
                    _controllersParameters = ControllersParametersVariable.Instance;
                    return 0;
                }
            }
        }
    }
}