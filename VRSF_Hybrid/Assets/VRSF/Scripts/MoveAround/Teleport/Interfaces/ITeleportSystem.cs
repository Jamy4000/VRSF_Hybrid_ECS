using UnityEngine;

namespace VRSF.MoveAround.Teleport.Interfaces
{
    /// <summary>
    /// Contains all the basic methods for the Teleport Systems.
    /// </summary>
    public interface ITeleportSystem
    {
        void TeleportUser(ITeleportFilter teleportFilter);

        void CheckNewPosWithBoundaries(ITeleportFilter teleportFilter, ref Vector3 PosToCheck);
    }
}