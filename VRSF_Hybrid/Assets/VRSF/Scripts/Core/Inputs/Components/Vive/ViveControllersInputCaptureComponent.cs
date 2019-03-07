using UnityEngine;
using Valve.VR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Required to have the references to the controllers parameters and capture the inputs
    /// </summary>
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class ViveControllersInputCaptureComponent : MonoBehaviour
    {
        /// <summary>
        /// To handle the setup with the Vive
        /// </summary>
        [HideInInspector] public bool ControllersParametersSetup = false;

        /// <summary>
        /// The two parameters struct for the controllers
        /// </summary>
        [HideInInspector] public ViveInputParameters RightParameters;
        [HideInInspector] public ViveInputParameters LeftParameters;

        public SteamVR_Action_Vibration LeftControllerHaptic;
        public SteamVR_Action_Vibration RightControllerHaptic;
    }
}