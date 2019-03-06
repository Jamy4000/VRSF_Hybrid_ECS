using UnityEngine;

namespace VRSF.Core.Inputs
{
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class ViveControllersInputCaptureComponent : MonoBehaviour
    {
        [Header("To handle the setup with the Vive")]
        [HideInInspector] public bool ControllersParametersSetup = false;

        [Header("The two parameters struct for the controllers")]
        [HideInInspector] public ViveInputParameters RightParameters;
        [HideInInspector] public ViveInputParameters LeftParameters;
    }
}