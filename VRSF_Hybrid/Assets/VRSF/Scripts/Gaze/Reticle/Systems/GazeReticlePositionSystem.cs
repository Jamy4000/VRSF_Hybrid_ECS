using Unity.Entities;
using UnityEngine;
using VRSF.Core.Raycast;
using VRSF.Interactions;

namespace VRSF.Gaze
{
    public class GazeReticlePositionSystem : ComponentSystem
    {
        struct Filter
        {
            public GazeRaycastComponent GazeRaycast;
            public ScriptableRaycastComponent ScriptableRaycast;
            public ReticleCalculationsComponent ReticleCalculations;
            public Transform ReticleTransform;
        }


        #region PRIVATE_VARIABLES
        private InteractionVariableContainer _interactionContainer;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            _interactionContainer = InteractionVariableContainer.Instance;
        }

        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.ScriptableRaycast.IsSetup)
                    CheckGazePosition(e);
            }
        }
        #endregion ComponentSystem_Methods


        #region PUBLIC_METHODS
        /// <summary>
        /// Check if the Gaze ray has hit something on the way
        /// </summary>
        private void CheckGazePosition(Filter entity)
        {
            if (!_interactionContainer.GazeHit.IsNull)
                //Reduce the reticle positon to the object that was hit
                SetPositionToHit(entity);
            else
                //put back the reticle positon to its normal distance if nothing was hit
                SetPositionToNormal(entity);
        }

        /// <summary>
        /// This method is called when the reticle didn't hit anything.
        /// It set it back to the "normal" position.
        /// </summary>
        private void SetPositionToNormal(Filter e)
        {
            Transform camTransform = e.GazeRaycast._VRCamera.transform;

            // Set the position of the reticle to the default distance in front of the camera.
            e.ReticleTransform.position = camTransform.position + camTransform.forward * e.ScriptableRaycast.MaxRaycastDistance;

            // Set the scale based on the original and the distance from the camera.
            e.ReticleTransform.localScale = e.ReticleCalculations._OriginalScale * e.ScriptableRaycast.MaxRaycastDistance;

            // The rotation should just be the default.
            e.ReticleTransform.rotation = e.ReticleCalculations._OriginalRotation;
        }


        /// <summary>
        /// This overload of SetPosition is used when the Gaze Raycast has hit something.
        /// </summary>
        private void SetPositionToHit(Filter e)
        {
            e.ReticleTransform.position = _interactionContainer.GazeHit.Value.point;

            e.ReticleTransform.localScale = e.ReticleCalculations._OriginalScale * _interactionContainer.GazeHit.Value.distance;

            // If the reticle should use the normal of what has been hit...
            // ... set it's rotation based on it's forward vector facing along the normal OR it's local rotation should be as it was originally.
            e.ReticleTransform.rotation = e.ReticleCalculations.UseNormal ? Quaternion.FromToRotation(Vector3.forward, _interactionContainer.GazeHit.Value.normal) : e.ReticleCalculations._OriginalRotation;
        }
        #endregion PUBLIC_METHODS
    }
}