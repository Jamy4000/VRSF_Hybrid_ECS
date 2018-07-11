using Unity.Entities;
using UnityEngine;
using VRSF.Controllers;
using VRSF.Gaze;
using VRSF.Interactions.Components;

namespace VRSF.Interactions.Systems
{
    public class OnColliderOverSetupSystem : ComponentSystem
    {
        struct Filter
        {
            public OnColliderOverComponents OnOverComponents;
        }


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            
            // Set to true to avoid error on the first frame.
            InteractionVariableContainer.Instance.RightHit.isNull = true;
            InteractionVariableContainer.Instance.LeftHit.isNull = true;
            InteractionVariableContainer.Instance.GazeHit.isNull = true;

            foreach (var entity in GetEntities<Filter>())
            {
                entity.OnOverComponents.ControllersParameters = ControllersParametersVariable.Instance;
                entity.OnOverComponents.GazeParameters = GazeParametersVariable.Instance;
                entity.OnOverComponents.InteractionsContainer = InteractionVariableContainer.Instance;

                // if we don't use the controllers and the gaze
                entity.OnOverComponents.CheckRaycast = ControllersParametersVariable.Instance.UseControllers || GazeParametersVariable.Instance.UseGaze;

                entity.OnOverComponents.IsSetup = true;
            }

            this.Enabled = false;
        }

        protected override void OnUpdate() { }
        #endregion
    }
}