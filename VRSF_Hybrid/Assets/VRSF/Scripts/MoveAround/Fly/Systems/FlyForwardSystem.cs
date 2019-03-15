using Unity.Entities;
using VRSF.Utils.ButtonActionChoser;

namespace VRSF.MoveAround.Fly
{
    /// <summary>
    /// If the user is interacting, calculate if we fly forward or backward
    /// </summary>
    public class FlyForwardSystem : ComponentSystem
    {
        struct Filter
        {
            public FlyParametersComponent FlyComponents;
            public BACGeneralComponent BACGeneral;
            public BACCalculationsComponent BACCalculations;
        }


        #region ComponentSystem_Methods
        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.FlyComponents._IsInteracting)
                {
                    CalculateFlyForward(e);
                }
            }
        }
        #endregion ComponentSystem_Methods


        #region PRIVATE_METHODS
        /// <summary>
        /// Calculate if the user is flying forward or backward and init some values.
        /// </summary>
        private void CalculateFlyForward(Filter entity)
        {
            entity.FlyComponents._FlyForward = entity.BACCalculations.ThumbPos.Value.y >= 0.0f;

            // If user just started to press/touch the thumbstick
            if (!entity.FlyComponents._WantToFly)
            {
                entity.FlyComponents._TimeSinceStartFlying = 0.0f;
                entity.FlyComponents._WantToFly = true;
            }
        }
        #endregion PRIVATE_METHODS
    }
}