using UnityEngine;
using UnityEngine.UI;

// This code is a refactored copy of the one provided by the Unity's VR Sample. 
// For more info, please check the project here : https://assetstore.unity.com/packages/essentials/tutorial-projects/vr-samples-51519
namespace VRSF.Gaze
{
    /// <summary>
    /// The reticle is a small point at the centre of the screen.
    /// It is used as a visual aid for aiming.The position of the
    /// reticle is either at a default position in space or on the
    /// surface of a VRInteractiveItem as determined by the VREyeRaycaster.
    /// </summary>
    public class Gaze : MonoBehaviour
    {
        #region PUBLIC_VARIABLES
        [Header("References linked to SDK")]
        [Tooltip("We need to affect the reticle's transform.")]
        public Transform ReticleTransform;

        [Tooltip("The reticle is always placed relative to the camera.")]
        public Transform m_Camera;

        [Tooltip("Reference to the images components that represents the reticle.")]
        public Image ReticleTarget;
        public Image ReticleBackground;
        #endregion PUBLIC_VARIABLES


        #region PRIVATE_VARIABLES
        // The Scriptable Object containing the reticle parameters
        private GazeParametersVariable _gazeParameters;

        private Vector3 m_OriginalScale;        // Since the scale of the reticle changes, the original scale needs to be stored.
        private Quaternion m_OriginalRotation;  // Used to store the original rotation of the reticle.
        #endregion PRIVATE_VARIABLES


        #region MONOBEHAVIOUR_METHODS
        private void Start()
        {
            _gazeParameters = GazeParametersVariable.Instance;

            if (!_gazeParameters.UseGaze)
                this.enabled = false;

            GazeSetup();
        }
        #endregion MONOBEHAVIOUR_METHODS


        #region PUBLIC_METHODS
        /// <summary>
        /// Hide the Reticle
        /// </summary>
        public void Hide()
        {
            ReticleTarget.enabled = false;
        }
        
        /// <summary>
        /// Show the Reticle
        /// </summary>
        public void Show()
        {
            ReticleTarget.enabled = true;
        }
        
        /// <summary>
        /// This method is called when the reticle didn't hit anything.
        /// It set it back to the "normal" position.
        /// </summary>
        public void SetPositionToNormal()
        {
            // Set the position of the reticle to the default distance in front of the camera.
            ReticleTransform.position = 
                m_Camera.position + m_Camera.forward * _gazeParameters.DefaultDistance;

            // Set the scale based on the original and the distance from the camera.
            ReticleTransform.localScale = m_OriginalScale * _gazeParameters.DefaultDistance;

            // The rotation should just be the default.
            ReticleTransform.rotation = m_OriginalRotation;
        }


        /// <summary>
        /// This overload of SetPosition is used when the Gaze Raycast has hit something.
        /// </summary>
        public void SetPosition(RaycastHit hit)
        {
            ReticleTransform.position = hit.point;// - Vector3.one;
            ReticleTransform.localScale = m_OriginalScale * hit.distance;

            // If the reticle should use the normal of what has been hit...
            if (_gazeParameters.UseNormal)
                // ... set it's rotation based on it's forward vector facing along the normal.
                ReticleTransform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
            else
                // However if it isn't using the normal then it's local rotation should be as it was originally.
                ReticleTransform.rotation = m_OriginalRotation;
        }

        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        /// <summary>
        /// Setup the Gaze Parameters based on the ScriptableSingleton, set in the VRSF Interaction Parameters Window
        /// </summary>
        private void GazeSetup()
        {
            if (_gazeParameters.ReticleSprite != null)
            {
                ReticleBackground.sprite = _gazeParameters.ReticleSprite;
            }

            if (_gazeParameters.ReticleTargetSprite != null)
            {
                ReticleTarget.sprite = _gazeParameters.ReticleTargetSprite;
            }

            transform.localScale = _gazeParameters.ReticleSize;

            // Store the original scale and rotation.
            m_OriginalScale = ReticleTransform.localScale;
            m_OriginalRotation = ReticleTransform.localRotation;
        }
        #endregion PRIVATE_METHODS


        // EMPTY
        #region GETTERS_SETTERS
        #endregion GETTERS_SETTERS
    }
}