using Unity.Entities;

namespace VRSF.Utils.Systems.ButtonActionChoser
{
    public abstract class BACListenersSetupSystem : ComponentSystem
    {
        public abstract void SetupListenersResponses(object entity);
        public abstract void RemoveListenersOnEndApp(object entity);

        protected override void OnUpdate() { }
    }
}