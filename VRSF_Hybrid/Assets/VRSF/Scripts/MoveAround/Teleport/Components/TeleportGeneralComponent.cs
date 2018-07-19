using UnityEngine;

namespace VRSF.MoveAround.Teleport.Components
{
    /// <summary>
    /// Contains all variable necessary for the BezierTeleportSystems to work.
    /// It implements the ITeleportComponent to be able to display the boundaries limits with TeleportBoundaries.
    /// </summary>
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity), typeof(TeleportBoundariesComponent))]
    public class TeleportGeneralComponent : MonoBehaviour
    {
        [HideInInspector] public LayerMask TeleportLayer = -1;             // The Layer for the Ground
        [HideInInspector] public int ExclusionLayer = -1;
        [HideInInspector] public bool CanTeleport = false;
    }
}