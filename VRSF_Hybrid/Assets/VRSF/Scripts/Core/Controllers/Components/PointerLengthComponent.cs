using UnityEngine;

namespace VRSF.Core.Controllers
{
    /// <summary>
    /// Only used on the controllers, allow us to know how long the pointer should be
    /// </summary>
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class PointerLengthComponent : MonoBehaviour
    {
        [Header("The length of this pointer")]
        [Tooltip("Basically how far we should check the raycast for this controller, and how far is the LineRenderer going.")]
        public float PointerMaxLength = 200.0f;
    }
}