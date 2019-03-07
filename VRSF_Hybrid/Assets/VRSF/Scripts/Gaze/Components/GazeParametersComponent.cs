using UnityEngine;
using UnityEngine.UI;

namespace VRSF.Gaze
{
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity), typeof(GazeCalculationsComponent))]
    public class GazeParametersComponent : MonoBehaviour
    {
        [Header("References linked to SDK")]
        [Tooltip("We need to affect the reticle's transform.")]
        public Transform ReticleTransform;

        [Tooltip("Reference to the images components that represents the reticle.")]
        public Image ReticleTarget;
        public Image ReticleBackground;
    }
}