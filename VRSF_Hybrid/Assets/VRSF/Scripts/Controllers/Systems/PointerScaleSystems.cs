using Unity.Entities;
using VRSF.Controllers.Components;

namespace VRSF.Controllers.Systems
{
    public class PointerScaleSystems : ComponentSystem
    {
        struct Filter
        {
            public ControllerPointerComponents colorPointerComp;
        }

        #region ComponentSystem_Methods
        // Update is called once per frame
        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                // As the vive send errors if the controller are not seen on the first frame, we need to put that in the update method
                if (e.colorPointerComp.IsSetup && e.colorPointerComp.ControllersParameters.UseControllers)
                {
                    CheckPointersScale(e.colorPointerComp);
                }
            }
        }
        #endregion ComponentSystem_Methods

        /// <summary>
        /// Check the scale of the pointer, if the user is going bigger for some reason
        /// transform here is the CameraRig object
        /// </summary>
        private void CheckPointersScale(ControllerPointerComponents comp)
        {
            comp.RightHandPointer.startWidth = comp.CameraRigTransform.lossyScale.x / 100;
            comp.RightHandPointer.endWidth = comp.CameraRigTransform.lossyScale.x / 100;

            comp.LeftHandPointer.startWidth = comp.CameraRigTransform.lossyScale.x / 100;
            comp.LeftHandPointer.endWidth = comp.CameraRigTransform.lossyScale.x / 100;
        }
    }
}