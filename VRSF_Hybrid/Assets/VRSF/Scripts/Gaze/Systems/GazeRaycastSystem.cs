using UnityEngine;
using VRSF.Core.Gaze;
using Unity.Entities;
using VRSF.Core.Raycast;
using VRSF.Core.SetupVR;
using ScriptableFramework.Variables;
using System.Linq;

namespace VRSF.Gaze.Raycast
{
    public class GazeRaycastSystem : ComponentSystem
    {
        struct Filter
        {
            public ControllersScriptableRaycastComponent RaycastComponents;
        }

        private GazeParametersVariable _gazeParameters;

        #region ComponentSystem_Methods
        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            _gazeParameters = GazeParametersVariable.Instance;
        }

        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.RaycastComponents.RayOrigin == ERayOrigin.CAMERA && e.RaycastComponents.IsSetup && e.RaycastComponents.CheckRaycast)
                {
                    if (VRSF_Components.DeviceLoaded == EDevice.SIMULATOR)
                        e.RaycastComponents.RayVar.SetValue(VRSF_Components.VRCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition));
                    else if (VRSF_Components.DeviceLoaded != EDevice.NULL)
                        e.RaycastComponents.RayVar.SetValue(new Ray(e.RaycastComponents.RayOriginTransform.position, e.RaycastComponents.RayOriginTransform.TransformDirection(Vector3.forward)));

                    RaycastHitHandler(e.RaycastComponents.RayVar.Value, e.RaycastComponents.IgnoredLayers, ref e.RaycastComponents.RaycastHitVar);
                }
            }
        }
        #endregion


        #region PRIVATE_METHODS  
        /// <summary>
        /// Check if the Ray from a controller is hitting something
        /// </summary>
        /// <param name="ray">The ray to check</param>
        /// <param name="distance">The maximum distance to which we raycast</param>
        /// <param name="layerToIgnore">The layer(s) to ignore from raycasting</param>
        /// <param name="hitVariable">The RaycastHitVariable in which we store the hit value</param>
        private void RaycastHitHandler(Ray ray, int layerToIgnore, ref RaycastHitVariable hitVariable)
        {
            var distance = _gazeParameters.DefaultDistance;
            var hits = Physics.RaycastAll(ray, distance, layerToIgnore);

            if (hits.Length > 0)
            {
                var first3DHit = hits.OrderBy(x => x.distance).First();
                hitVariable.SetValue(first3DHit);
                hitVariable.SetIsNull(false);
            }
            else
            {
                hitVariable.SetIsNull(true);
            }
        }
        #endregion PRIVATE_METHODS
    }
}