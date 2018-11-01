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
    [RequireComponent(typeof(BorderRendererComponent), typeof(Unity.Entities.GameObjectEntity))]
    public class SceneObjectsComponent : MonoBehaviour
    {
        // Parabolic Pointer object to pull destination points from, and to assign to each controller
        [System.NonSerialized] public PointerCalculationsComponent Pointer;

        /// Origin of the tracking space (basically the cameraRig)
        [System.NonSerialized] public Transform _originTransform;

        /// Origin of the player's head
        [System.NonSerialized] public Transform _headTransform;

        /// BorderRenderer to render the chaperone bounds (when choosing a location to teleport to)
        [System.NonSerialized] public BorderRendererComponent _roomBorder;

        /// SteamVR controllers that should be polled.
        [System.NonSerialized] public GameObject _activeController;

        /// <summary>
        /// Fade Component to place on the VRCamera. If not assigned, teleport the user without any fading effect.
        /// </summary>
        [System.NonSerialized] public TeleportFadeComponent FadeComponent;
    }
}