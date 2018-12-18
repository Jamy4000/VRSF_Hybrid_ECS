using ScriptableFramework.Variables;
using UnityEngine;
using VRSF.Controllers;

namespace VRSF.Utils.Components
{
    public class ScriptableRaycastComponent : MonoBehaviour
    {
        [Header("The Raycast Origin for this script")]
        public EHand RayOrigin = EHand.NONE;

        // The RaycastHitVariable and Ray to check for this feature
        [HideInInspector] public RaycastHitVariable RaycastHitVar;
        [HideInInspector] public RayVariable RayVar;
    }
}