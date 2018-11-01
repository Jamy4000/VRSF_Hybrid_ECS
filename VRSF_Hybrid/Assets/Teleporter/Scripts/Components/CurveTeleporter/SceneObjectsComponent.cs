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
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class SceneObjectsComponent : MonoBehaviour
    {
        /// <summary>
        /// Fade Component to place on the VRCamera. If not assigned, teleport the user without any fading effect.
        /// </summary>
        [System.NonSerialized] public TeleportFadeComponent FadeComponent;

        [System.NonSerialized] public TeleportNavMeshComponent _TeleportNavMesh;
    }
}