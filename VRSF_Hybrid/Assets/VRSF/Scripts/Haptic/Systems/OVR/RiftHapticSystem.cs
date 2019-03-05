using Unity.Entities;
using VRSF.Inputs.Components;

namespace VRSF.Controllers.Haptic
{
    /// <summary>
    /// When raising the OnHapticRequestedEvent, if the user is using Oculus Rift, we trigger an haptic pulse in the requested controller.
    /// </summary>
    public class RiftHapticSystem : ComponentSystem
    {
        struct Filter
        {
            public RiftControllersInputCaptureComponent RiftInputCaptureComp;
            public OVRHapticComponent OVRHapticComp;
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
        /// Callback for when a Haptic Pulse is requested for Oculus
        /// </summary>
        /// <param name="onHapticRequested"></param>
        private void OnHapticEventCallback(OnHapticRequestedEvent onHapticRequested)
        {
            foreach (var e in GetEntities<Filter>())
            {
                var channel = onHapticRequested.Hand == EHand.RIGHT ? OVRHaptics.RightChannel : OVRHaptics.LeftChannel;

                switch (onHapticRequested.HapticForce)
                {
                    case EHapticForce.LIGHT:
                        channel.Preempt(e.OVRHapticComp.LightClip);
                        break;
                    case EHapticForce.MEDIUM:
                        channel.Preempt(e.OVRHapticComp.MediumClip);
                        break;
                    case EHapticForce.HARD:
                        channel.Preempt(e.OVRHapticComp.HardClip);
                        break;
                }
            }
        }
    }
}