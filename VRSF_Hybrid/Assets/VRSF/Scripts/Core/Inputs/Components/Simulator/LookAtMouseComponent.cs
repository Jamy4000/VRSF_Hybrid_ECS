using UnityEngine;

namespace VRSF.Core.Inputs
{
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class LookAtMouseComponent : MonoBehaviour
    {
        [Header("Speed of the Camera")]
        public float Speed;
    }
}
