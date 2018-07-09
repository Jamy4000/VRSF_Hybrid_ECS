using UnityEngine;
using UnityEngine.UI;

namespace VRSF.Gaze.Components
{
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class GazeComponent : MonoBehaviour
    {
        #region INSPECTOR_VARIABLES
        [Header("References linked to SDK")]
        [Tooltip("We need to affect the reticle's transform.")]
        public Transform ReticleTransform;

        [Tooltip("Reference to the images components that represents the reticle.")]
        public Image ReticleTarget;
        public Image ReticleBackground;
        #endregion INSPECTOR_VARIABLES


        #region SYSTEMS_VARIABLES
        [HideInInspector] public Vector3 _OriginalScale;        // Since the scale of the reticle changes, the original scale needs to be stored.
        [HideInInspector] public Quaternion _OriginalRotation;  // Used to store the original rotation of the reticle.
        [HideInInspector] public Transform _VRCamera;

        [HideInInspector] public bool _IsSetup;
        #endregion SYSTEMS_VARIABLES
    }
}