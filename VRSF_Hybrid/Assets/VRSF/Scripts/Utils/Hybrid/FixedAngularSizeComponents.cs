using UnityEngine;

namespace VRSF.Utils.Hybrid
{
    public class FixedAngularSizeComponents : MonoBehaviour
    {
        [Header("Off sets the scale ratio so that text does not scale down too much")]
        [Tooltip("Set to zero for linear scaling")]
        public float SizeRatio = 0;
    }
}

