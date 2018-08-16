using Unity.Entities;
using UnityEngine;
using VRSF.Controllers.Components;

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
            var cameraRigScale = Utils.VRSF_Components.CameraRig.transform.lossyScale;

            comp._RightHandPointer.startWidth = cameraRigScale.x / 100;
            comp._RightHandPointer.endWidth = cameraRigScale.x / 100;

            comp._LeftHandPointer.startWidth = cameraRigScale.x / 100;
            comp._LeftHandPointer.endWidth = cameraRigScale.x / 100;
        }
    }
}