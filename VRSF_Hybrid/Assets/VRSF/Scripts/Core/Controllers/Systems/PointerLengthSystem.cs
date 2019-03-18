using Unity.Entities;
using UnityEngine;

namespace VRSF.Core.Controllers
{
    /// <summary>
    /// Handle the Length of the Pointer depending on if the raycast is hitting something
    /// </summary>
    [UpdateAfter(typeof(Raycast.PointerRaycastSystems))]
    public class PointerLengthSystem : ComponentSystem
    {
        struct Filter
        {
            public ControllerPointerComponents PointerComp;
            public Raycast.ScriptableRaycastComponent RaycastComp;
            public LineRenderer PointerRenderer;
        }


        #region ComponentSystem_Methods
        // Update is called once per frame
        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.PointerComp.IsSetup && e.PointerComp._PointerState != EPointerState.OFF)
                    SetControllerRayLength(e);
            }
        }
        #endregion ComponentSystem_Methods


        #region PRIVATE_METHODS
        /// <summary>
        /// Set the size of the line renderer depending on the hit from the RayCast.
        /// </summary>
        /// <param name="hit">The RaycastHitVariable containing the RaycastHit for the controller</param>
        /// <param name="controller">The controller GameObject from which the ray started</param>
        /// <param name="hand">The hand rom which we are checking the raycastHit</param>
        private void SetControllerRayLength(Filter e)
        {
            try
            {
                if (!e.RaycastComp.RaycastHitVar.IsNull)
                {
                    //Reduce lineRenderer from the controllers position to the object that was hit
                    e.PointerRenderer.SetPositions(new Vector3[]
                    {
                        Vector3.zero,
                        e.RaycastComp.RayOriginTransform.InverseTransformPoint(e.RaycastComp.RaycastHitVar.Value.point),
                    });

                    if (e.PointerComp.OptionalLasersObjects.PointersEndPoint != null)
                        CheckEndPoint();
                }
                else
                {
                    //put back lineRenderer to its normal length if nothing was hit
                    e.PointerRenderer.SetPositions(new Vector3[]
                    {
                        Vector3.zero,
                        new Vector3(0, 0, e.RaycastComp.RaycastMaxDistance),
                    });

                    e.PointerComp.OptionalLasersObjects.PointersEndPoint?.gameObject.SetActive(false);
                }
            }
            catch (System.Exception exception)
            {
                Debug.Log("<b>[VRSF] :</b> VR Components not setup yet, waiting for next frame.\n" + exception.ToString());
            }


            void CheckEndPoint()
            {
                if (e.RaycastComp.RaycastHitVar.RaycastHitIsOnUI())
                {
                    e.PointerComp.OptionalLasersObjects.PointersEndPoint.gameObject.SetActive(true);
                    e.PointerComp.OptionalLasersObjects.PointersEndPoint.position = e.RaycastComp.RaycastHitVar.Value.point;
                }
                else
                {
                    e.PointerComp.OptionalLasersObjects.PointersEndPoint?.gameObject.SetActive(false);
                }
            }
        }
        #endregion PRIVATE_METHODS
    }
}