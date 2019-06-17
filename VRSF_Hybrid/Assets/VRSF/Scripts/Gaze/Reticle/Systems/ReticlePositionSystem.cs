using Unity.Entities;
using UnityEngine;
using VRSF.Core.Raycast;

namespace VRSF.Gaze
{
    public class ReticlePositionSystem : ComponentSystem
    {
        struct Filter
        {
            public ReticleCalculationsComponent ReticleCalculations;
            public ScriptableRaycastComponent ScriptableRaycast;
        }


        #region ComponentSystem_Methods
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
            if (!entity.ScriptableRaycast.RaycastHitVar.IsNull)
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
            Transform camTransform = e.ScriptableRaycast._VRCamera.transform;

            // Set the position of the reticle to the default distance in front of the camera.
            e.ReticleCalculations._ReticleTransform.position = camTransform.position + camTransform.forward * e.ScriptableRaycast.MaxRaycastDistance;

            // Set the scale based on the original and the distance from the camera.
            if (!e.ReticleCalculations.KeepSameScale)
                e.ReticleCalculations._ReticleTransform.localScale = e.ReticleCalculations._OriginalScale * e.ScriptableRaycast.MaxRaycastDistance;

            // The rotation should just be the default.
            e.ReticleCalculations._ReticleTransform.rotation = e.ReticleCalculations._OriginalRotation;
        }


        /// <summary>
        /// This overload of SetPosition is used when the Gaze Raycast has hit something.
        /// </summary>
        private void SetPositionToHit(Filter e)
        {
            var hitVar = e.ScriptableRaycast.RaycastHitVar;

            e.ReticleCalculations._ReticleTransform.position = hitVar.Value.point;

            if (!e.ReticleCalculations.KeepSameScale)
                e.ReticleCalculations._ReticleTransform.localScale = e.ReticleCalculations._OriginalScale * hitVar.Value.distance;

            // If the reticle should use the normal of what has been hit...
            // ... set it's rotation based on it's forward vector facing along the normal OR it's local rotation should be as it was originally.
            e.ReticleCalculations._ReticleTransform.rotation = e.ReticleCalculations.UseNormal ? Quaternion.FromToRotation(Vector3.forward, hitVar.Value.normal) : e.ReticleCalculations._OriginalRotation;
        }
        #endregion PUBLIC_METHODS
    }
}