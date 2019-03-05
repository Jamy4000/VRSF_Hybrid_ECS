using Unity.Entities;
using VRSF.Inputs.Components.Vive;

namespace VRSF.Controllers.Haptic
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

            this.Enabled = false;
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
            //foreach (var e in GetEntities<Filter>())
            //{
            //    if (onHapticRequested.Hand == EHand.LEFT)
            //        e.ViveInputCaptureComp.LeftController.TriggerHapticPulse((ushort)onHapticRequested.HapticForce);
            //    else
            //        e.ViveInputCaptureComp.RightController.TriggerHapticPulse((ushort)onHapticRequested.HapticForce);
            //}
            UnityEngine.Debug.LogError("TODO : Redo HAPTIC SteamVR");
        }
    }
}