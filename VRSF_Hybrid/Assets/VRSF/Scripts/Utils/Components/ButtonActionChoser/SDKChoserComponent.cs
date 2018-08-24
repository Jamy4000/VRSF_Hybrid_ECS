using UnityEngine;

namespace VRSF.Utils.Components
{
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity), typeof(ButtonActionChoser.ButtonActionChoserComponents))]
    public class SDKChoserComponent : MonoBehaviour
    {
        [Header("SDKs using this ButtonActionChoserComponent")]
        [HideInInspector] public bool UseRift = true;
        [HideInInspector] public bool UseOpenVR = true;
        [HideInInspector] public bool UseSimulator = true;

        [HideInInspector] public bool IsSetup = false;
    }
}