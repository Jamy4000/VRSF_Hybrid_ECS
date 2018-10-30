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
    [RequireComponent(typeof(Animator))]
    public class TeleportNavMeshComponent : MonoBehaviour
    {
        /// Animator used to fade in/out the teleport area.  This should have a boolean parameter "Enabled" where if true
        /// the selectable area is displayed on the ground.
        /// Should have a boolean \"Enabled\" parameter that is set to true when the player is choosing a place to teleport
        [System.NonSerialized] public Animator _navmeshAnimator;

        [System.NonSerialized] public int _enabledAnimatorID;
    }
}