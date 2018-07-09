using UnityEngine;

namespace VRSF.Inputs.Components.Vive
{
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class ViveInputCaptureComponent : VRInputCaptureComponent
    {
        [Header("SteamVR Tracked Object from the two Controllers")]
        public SteamVR_TrackedObject LeftTrackedObject;
        public SteamVR_TrackedObject RightTrackedObject;

        [Header("The References to the SteamVR Devices")]
        [HideInInspector] public SteamVR_Controller.Device GazeController;

        [Header("To handle the setup with the Vive")]
        [HideInInspector] public bool ViveReferencesSetup = false;
        [HideInInspector] public bool ControllersParametersSetup = false;

        [Header("The two parameters struct for the controllers")]
        [HideInInspector] public ViveInputParameters RightParameters;
        [HideInInspector] public ViveInputParameters LeftParameters;


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