using UnityEngine;

namespace VRSF.Inputs.Simulator
{
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class LookAtMouseComponent : MonoBehaviour
    {
        [Header("Speed of the Camera")]
        public float Speed;
    }
}
