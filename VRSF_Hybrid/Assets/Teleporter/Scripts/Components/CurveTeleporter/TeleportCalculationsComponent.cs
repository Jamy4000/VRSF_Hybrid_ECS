using UnityEngine;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// A generic component that renders a border using the given polylines.  
    /// The borders are double sided and are oriented upwards (ie normals are parallel to the XZ plane)
    /// 
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// </summary>
    [RequireComponent(typeof(Utils.Components.ScriptableRaycastComponent))]
    public class TeleportCalculationsComponent : MonoBehaviour
    {
        /// Measure in degrees of how often the controller should respond with a haptic click.  Smaller value=faster clicks
        [Tooltip("The player feels a haptic pulse in the controller when they raise / lower the controller by this many degrees.  Lower value = faster pulses.")]
        public float HapticClickAngleStep = 10;

        [System.NonSerialized] public Vector3 _lastClickAngle = Vector3.zero;

        [System.NonSerialized] public bool IsClicking = false;

        [System.NonSerialized] public float _teleportTimeMarker = -1;

        [System.NonSerialized] public bool _IsSetup;

        /// <summary>
        /// Indicates the current use of teleportation.
        /// None: The player is not using teleportation right now
        /// Selecting: The player is currently selecting a teleport destination (holding down on touchpad)
        /// Teleporting: The player has selected a teleport destination and is currently teleporting now (fading in/out)
        /// </summary>
        public ETeleportState CurrentTeleportState { get; set; }
    }
}