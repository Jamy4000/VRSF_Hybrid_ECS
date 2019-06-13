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

        [Tooltip("Whether the reticle should be placed parallelly to a surface.")]
        public bool UseNormal = true;
    }
}