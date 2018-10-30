using UnityEngine;

namespace VRSF.Utils.Components
{
    public class RuntimeDestroy : MonoBehaviour
    {
        void Awake()
        {
            Destroy(gameObject);
        }
    }
}