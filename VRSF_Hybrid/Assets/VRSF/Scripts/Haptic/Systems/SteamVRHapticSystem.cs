using Unity.Entities;
using VRSF.Core.Inputs;

namespace VRSF.Core.Controllers.Haptic
{
    /// <summary>
    /// When raising the OnHapticRequestedEvent, if the user is using SteamVR, we trigger an haptic pulse in the requested controller.
    /// </summary>
    public class SteamVRHapticSystem : ComponentSystem
    {
        struct Filter
        {
            public ViveControllersInputCaptureComponent ViveInputCaptureComp;
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            OnHapticRequestedEvent.RegisterListener(OnHapticEventCallback);
        }

        protected override void OnUpdate() { }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            OnHapticRequestedEvent.UnregisterListener(OnHapticEventCallback);
        }

        /// <summary>
        /// Callback for when a Haptic Pulse is requested for SteamVR
        /// </summary>
        /// <param name="onHapticRequested"></param>
        private void OnHapticEventCallback(OnHapticRequestedEvent onHapticRequested)
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (onHapticRequested.Hand == EHand.LEFT)
                    e.ViveInputCaptureComp.LeftControllerHaptic.Execute(0, onHapticRequested.HapticDuration, onHapticRequested.HapticFrequency, onHapticRequested.HapticAmplitude, Valve.VR.SteamVR_Input_Sources.LeftHand);
                else
                    e.ViveInputCaptureComp.RightControllerHaptic.Execute(0, onHapticRequested.HapticDuration, onHapticRequested.HapticFrequency, onHapticRequested.HapticAmplitude, Valve.VR.SteamVR_Input_Sources.RightHand);
            }
        }
    }
}