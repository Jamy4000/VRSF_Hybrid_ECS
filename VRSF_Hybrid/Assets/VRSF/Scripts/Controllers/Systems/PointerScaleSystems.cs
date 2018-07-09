using Unity.Entities;
using UnityEngine;
using VRSF.Controllers.Components;

namespace VRSF.Controllers.Systems
{
    public class PointerScaleSystems : ComponentSystem
    {
        struct Filter
        {
            public ControllerPointerComponents ControllerPointerComp;
        }

        private ControllersParametersVariable _controllersParameters;


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            _controllersParameters = ControllersParametersVariable.Instance;
        }


        // Update is called once per frame
        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                // As the vive send errors if the controller are not seen on the first frame, we need to put that in the update method
                if (e.ControllerPointerComp._IsSetup && _controllersParameters.UseControllers)
                {
                    CheckPointersScale(e.ControllerPointerComp);
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
            comp._RightHandPointer.startWidth = comp._CameraRigTransform.lossyScale.x / 100;
            comp._RightHandPointer.endWidth = comp._CameraRigTransform.lossyScale.x / 100;

            comp._LeftHandPointer.startWidth = comp._CameraRigTransform.lossyScale.x / 100;
            comp._LeftHandPointer.endWidth = comp._CameraRigTransform.lossyScale.x / 100;
        }
    }
}