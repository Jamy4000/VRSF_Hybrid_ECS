using ScriptableFramework.Events;
using UnityEngine;

namespace VRSF.Inputs.Components.Vive
{
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class ViveControllersInputCaptureComponent : MonoBehaviour
    {
        [Header("SteamVR Tracked Object from the two Controllers")]
        public SteamVR_TrackedObject LeftTrackedObject;
        public SteamVR_TrackedObject RightTrackedObject;

        [Header("To handle the setup with the Vive")]
        [HideInInspector] public bool ControllersParametersSetup = false;

        [Header("The two parameters struct for the controllers")]
        [HideInInspector] public ViveInputParameters RightParameters;
        [HideInInspector] public ViveInputParameters LeftParameters;
        
        // A Temp GameEvent to raise in the InputCaptureSystems
        [HideInInspector] public GameEvent TempEvent;
        
        public SteamVR_Controller.Device LeftController
        {
            get { return SteamVR_Controller.Input((int)LeftTrackedObject.index); }
        }

        public SteamVR_Controller.Device RightController
        {
            get { return SteamVR_Controller.Input((int)RightTrackedObject.index); }
        }
    }
}