using UnityEngine;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Contains all variable necessary for the BezierTeleportSystems to work.
    /// It implements the ITeleportComponent to be able to display the boundaries limits with TeleportBoundaries.
    /// </summary>
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class TeleportGeneralComponent : MonoBehaviour
    {
        [HideInInspector] public bool CanTeleport = false;

        /// <summary>
        /// Indicates the current use of teleportation.
        /// None: The player is not using teleportation right now
        /// Selecting: The player is currently selecting a teleport destination (holding down on touchpad)
        /// Teleporting: The player has selected a teleport destination and is currently teleporting now (fading in/out)
        /// </summary>
        [HideInInspector] public ETeleportState CurrentTeleportState { get; set; }

        [HideInInspector] public Vector3 PointToGoTo = Vector3.zero;
    }
} 