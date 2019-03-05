using UnityEngine;

namespace VRSF.Controllers.Haptic
{
    /// <summary>
    /// Contains the Haptics Clip necessary for the OVR Haptic Systems
    /// </summary>
    public class OVRHapticComponent : MonoBehaviour
    {
        public OVRHapticsClip LightClip;
        public OVRHapticsClip MediumClip;
        public OVRHapticsClip HardClip;
    }
}