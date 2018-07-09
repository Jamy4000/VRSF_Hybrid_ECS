using Unity.Entities;
using UnityEngine;
using VRSF.Gaze.Components;
using VRSF.Interactions;

namespace VRSF.Gaze.Systems
{
    public class GazeReticlePositionSystem : ComponentSystem
    {
        struct Filter
        {
            public GazeComponent GazeComp;
        }


        #region PRIVATE_VARIABLES
        private GazeParametersVariable _gazeParameters;
        private InteractionVariableContainer _interactionContainer;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            _gazeParameters = GazeParametersVariable.Instance;
            _interactionContainer = InteractionVariableContainer.Instance;
        }

        protected override void OnUpdate()
        {
            if (_gazeParameters.UseGaze)
            {
                foreach (var e in GetEntities<Filter>())
                {
                    if (e.GazeComp._IsSetup)
                    {
                        CheckGazePosition(e.GazeComp);
                    }
                }
            }
        }
        #endregion ComponentSystem_Methods


        #region PUBLIC_METHODS
        /// <summary>
        /// Check if the Gaze ray has hit something on the way
        /// </summary>
        private void CheckGazePosition(GazeComponent comp)
        {
            if (!_interactionContainer.GazeHit.isNull)
            {
                //Reduce the reticle positon to the object that was hit
                SetPositionToHit(comp);
            }
            else
            {
                //put back the reticle positon to its normal distance if nothing was hit
                SetPositionToNormal(comp);
            }
        }

        /// <summary>
        /// This method is called when the reticle didn't hit anything.
        /// It set it back to the "normal" position.
        /// </summary>
        private void SetPositionToNormal(GazeComponent comp)
        {
            // Set the position of the reticle to the default distance in front of the camera.
            comp.ReticleTransform.position =
                comp._VRCamera.position + comp._VRCamera.forward * _gazeParameters.DefaultDistance;

            // Set the scale based on the original and the distance from the camera.
            comp.ReticleTransform.localScale = comp._OriginalScale * _gazeParameters.DefaultDistance;

            // The rotation should just be the default.
            comp.ReticleTransform.rotation = comp._OriginalRotation;
        }


        /// <summary>
        /// This overload of SetPosition is used when the Gaze Raycast has hit something.
        /// </summary>
        private void SetPositionToHit(GazeComponent comp)
        {
            comp.ReticleTransform.position = _interactionContainer.GazeHit.Value.point;

            comp.ReticleTransform.localScale = comp._OriginalScale * _interactionContainer.GazeHit.Value.distance;

            // If the reticle should use the normal of what has been hit...
            if (_gazeParameters.UseNormal)
                // ... set it's rotation based on it's forward vector facing along the normal.
                comp.ReticleTransform.rotation = Quaternion.FromToRotation(Vector3.forward, _interactionContainer.GazeHit.Value.normal);
            else
                // However if it isn't using the normal then it's local rotation should be as it was originally.
                comp.ReticleTransform.rotation = comp._OriginalRotation;
        }
        #endregion PUBLIC_METHODS
    }
}