using UnityEngine;
using VRSF.Core.Controllers;

namespace VRSF.Gaze
{
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class ReticleVisibilityComponent : MonoBehaviour
    {
        [Tooltip("The current state of the Reticle.")]
        public EPointerState ReticleState = EPointerState.ON;

        [Tooltip("How fast the reticle is disappearing when not hitting something")]
        public float DisappearanceSpeed = 1.0f;
    }
}