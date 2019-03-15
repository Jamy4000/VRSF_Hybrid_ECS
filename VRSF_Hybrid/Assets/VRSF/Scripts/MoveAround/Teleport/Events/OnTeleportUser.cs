namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Event raised when the user has to be teleport somewhere
    /// </summary>
    public class OnTeleportUser : EventCallbacks.Event<OnTeleportUser>
    {
        public readonly TeleportGeneralComponent TeleportGeneral;

        public readonly SceneObjectsComponent SceneObjects;

        public OnTeleportUser(TeleportGeneralComponent teleportGeneral, SceneObjectsComponent sceneObjects) : base("Event raised when the user has to be teleport somewhere")
        {
            TeleportGeneral = teleportGeneral;
            SceneObjects = sceneObjects;
            FireEvent(this);
        }
    }
}