using Unity.Entities;
using VRSF.Controllers.Components;
using VRSF.Core.SetupVR;

namespace VRSF.Controllers.Systems
{
    /// <summary>
    /// Handle the Scale of the pointer depending on the user's scale.
    /// </summary>
    public class PointerScaleSystem : ComponentSystem
    {
        struct Filter
        {
            public ControllerPointerComponents ControllerPointerComp;
            public UnityEngine.LineRenderer PointerRenderer;
        }
        

        #region ComponentSystem_Methods
        // Update is called once per frame
        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                // As the vive send errors if the controller are not seen on the first frame, we need to put that in the update method
                if (e.ControllerPointerComp.IsSetup && e.ControllerPointerComp._PointerState != EPointerState.OFF)
                    CheckPointersScale(e);
            }
        }
        #endregion ComponentSystem_Methods


        /// <summary>
        /// Check the scale of the pointer, if the user is going bigger for some reason
        /// transform here is the CameraRig object
        /// </summary>
        private void CheckPointersScale(Filter e)
        {
            var cameraRigScale = VRSF_Components.CameraRig.transform.lossyScale;

            e.PointerRenderer.startWidth = cameraRigScale.x / 500;
            e.PointerRenderer.endWidth = cameraRigScale.x / 500;
        }
    }
}