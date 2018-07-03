using VRSF;
using UnityEngine;

namespace Absolute.Utils
{
    /// <summary>
    /// Maintain the size of an object depending on the distance with the CameraRig.
    /// This script is mainly use for UI, to avoid that the User modify the size of them.
    /// The better use case is when the UI are following the User wherever he goes.
    /// </summary> 
    public class FixedAngularSize : MonoBehaviour
    {
        #region PUBLIC_VARIABLE
        [Header("Off sets the scale ratio so that text does not scale down too much")]
        [Tooltip("Set to zero for linear scaling")]
        public float SizeRatio = 0;
        #endregion PUBLIC_VARIABLE


        #region PRIVATE_VARIABLE
        // The ratio between the transform's local scale and its starting distance from the camera.
        private float startingDistance;
        private Vector3 startingScale;
        private GameObject CameraRig;
        #endregion PRIVATE_VARIABLE


        #region MONOBEHAVIOUR_METHODS
        private void Update()
        {
            if (CameraRig != null)
            {
                float distanceToHologram = Vector3.Distance(CameraRig.transform.position, transform.position);
                // create an offset ratio based on the starting position. This value creates a new angle that pivots
                // on the starting position that is more or less drastic than the normal scale ratio.
                float curvedRatio = 1 - startingDistance * SizeRatio;
                transform.localScale = startingScale * (distanceToHologram * SizeRatio + curvedRatio);
            }
            else
            {
                Initialize();
            }
        }
        #endregion MONOBEHAVIOUR_METHODS


        //EMPTY
        #region PUBLIC_METHODS
        #endregion PUBLIC_METHODS
            

        #region PRIVATE_METHODS
        /// <summary>
        /// Initialization of the references
        /// </summary>
        void Initialize() 
        {
            try
            {
                CameraRig = VRSF.Utils.VRSF_Components.CameraRig;
                // Calculate the XYZ ratios for the transform's localScale over its initial distance from the camera.

                startingDistance = Vector3.Distance(CameraRig.transform.position, transform.position);

                startingScale = transform.localScale;

                SetSizeRatio(SizeRatio);
            } catch
            {
                Debug.Log("CameraRig is not on Scene yet.");
            }
        }

        /// <summary>
        /// Manually update the OverrideSizeRatio during runtime or through UnityEvents in the editor
        /// </summary>
        /// <param name="ratio"> 0 - 1 : Use 0 for linear scaling</param>
        void SetSizeRatio(float ratio)
        {
            if (ratio == 0)
            {
                if (startingDistance > 0.0f)
                {
                    // Set to a linear scale ratio
                    SizeRatio = 1 / startingDistance;
                }
                else
                {
                    // If the transform and the camera are both in the same position (that is, the distance between them is zero),
                    // disable this Behaviour so we don't get a DivideByZero error later on.
                    enabled = false;
#if UNITY_EDITOR
                    Debug.LogWarning("The object and the camera are in the same position at Start(). The attached FixedAngularSize Behaviour is now disabled.");
#endif //UNITY_EDITOR
                }
            }
            else
            {
                SizeRatio = ratio;
            }
        }
        #endregion PRIVATE_METHODS
    }
}