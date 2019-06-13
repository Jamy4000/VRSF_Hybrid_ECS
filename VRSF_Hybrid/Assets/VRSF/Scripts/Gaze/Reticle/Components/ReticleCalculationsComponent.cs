using UnityEngine;

namespace VRSF.Gaze
{
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class ReticleCalculationsComponent : MonoBehaviour
    {
        /// <summary>
        /// Since the scale of the reticle changes, the original scale needs to be stored.
        /// </summary>
        [HideInInspector] public Vector3 _OriginalScale;

        /// <summary>
        /// Used to store the original rotation of the reticle.
        /// </summary>
        [HideInInspector] public Quaternion _OriginalRotation;

        /// <summary>
        /// The image of the reticle
        /// </summary>
        [HideInInspector] public UnityEngine.UI.Image _ReticleImage;

        /// <summary>
        /// The transform of the reticle.
        /// </summary>
        [HideInInspector] public Transform _ReticleTransform;

        [Tooltip("The Scriptable raycast of the Gaze Object, find in the parent.")]
        public Core.Raycast.ScriptableRaycastComponent GazeScriptableRaycast;

        [Tooltip("Whether the reticle should be placed parallelly to a surface.")]
        public bool UseNormal = true;
    }
}