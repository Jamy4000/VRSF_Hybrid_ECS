using Unity.Entities;
using UnityEngine;
using VRSF.Core.Raycast;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Controllers
{
    /// <summary>
    /// Make the Pointer appear only when it's not on Exluded Layer
    /// TODO : Refactor, not really working
    /// </summary>
    public class PointerStatusSystem : ComponentSystem
    {
        struct Filter
        {
            public ScriptableRaycastComponent RaycastComp;
            public ControllerPointerComponents ControllerPointerComp;
            public LineRenderer PointerRenderer;
        }


        #region ComponentSystem_Methods

        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            // TODO : Remove this line
            this.Enabled = false;
        }

        // Update is called once per frame
        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.ControllerPointerComp.IsSetup && e.ControllerPointerComp._PointerState != EPointerState.OFF)
                    CheckPointerState(ref e.ControllerPointerComp._PointerState, e);
            }
        }
        #endregion ComponentSystem_Methods


        #region PRIVATE_METHODS
        /// <summary>
        /// Check if the pointer is on a non Excluded Layer
        /// </summary>
        /// <param name="isOver">If the Raycast is over something</param>
        /// <param name="pointerState">The current state of the pointer</param>
        /// <param name="pointer">The linerenderer to which the material is attached</param>
        /// <returns>The new state of the pointer</returns>
        private void CheckPointerState(ref EPointerState pointerState, Filter e)
        {
            // If the pointer is over something and it's state is not at Selectable
            if (pointerState != EPointerState.SELECTABLE)
            {
                e.PointerRenderer.enabled = true;
                if (e.ControllerPointerComp.OptionalLasersObjects.PointersParticles != null)
                {
                    foreach (var ps in e.ControllerPointerComp.OptionalLasersObjects.PointersParticles)
                    {
                        ps.Play();
                    }
                }
                pointerState = EPointerState.SELECTABLE;
            }
            else if (pointerState != EPointerState.ON)
            {
                e.PointerRenderer.enabled = false;
                if (e.ControllerPointerComp.OptionalLasersObjects.PointersParticles != null)
                {
                    foreach (var ps in e.ControllerPointerComp.OptionalLasersObjects.PointersParticles)
                    {
                        ps.Stop();
                        ps.Clear();
                    }
                }
                pointerState = EPointerState.ON;
            }
        }
        #endregion PRIVATE_METHODS
    }
}