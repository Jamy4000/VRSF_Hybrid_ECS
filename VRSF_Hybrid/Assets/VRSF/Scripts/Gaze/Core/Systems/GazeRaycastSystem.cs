using UnityEngine;
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
            public ScriptableRaycastComponent RaycastComponents;
            public GazeRaycastComponent GazeRaycast;
        }

        #region ComponentSystem_Methods
        protected override void OnCreateManager()
        {
            OnSetupVRReady.Listeners += Init;
            base.OnCreateManager();
        }

        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.RaycastComponents.IsSetup)
                {
                    Ray rayToUse = VRSF_Components.DeviceLoaded == EDevice.SIMULATOR ? e.GazeRaycast._VRCamera.ScreenPointToRay(Input.mousePosition) : new Ray(e.GazeRaycast.transform.position, e.GazeRaycast.transform.TransformDirection(Vector3.forward));
                    e.RaycastComponents.RayVar.SetValue(rayToUse);
                    RaycastHitHandler(e.RaycastComponents.RayVar.Value, e.RaycastComponents.MaxRaycastDistance, ~e.RaycastComponents.ExcludedLayer, ref e.RaycastComponents.RaycastHitVar);
                }
            }
        }

        protected override void OnDestroyManager()
        {
            OnSetupVRReady.Listeners -= Init;
            base.OnDestroyManager();
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
        private void RaycastHitHandler(Ray ray, float defaultDistance, int layerToIgnore, ref RaycastHitVariable hitVariable)
        {
            var hits = Physics.RaycastAll(ray, defaultDistance, layerToIgnore);

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

        
        private void Init(OnSetupVRReady info)
        {
            foreach (var e in GetEntities<Filter>())
            {
                e.GazeRaycast._VRCamera = VRSF_Components.VRCamera.GetComponent<Camera>();
            }
        }
        #endregion PRIVATE_METHODS
    }
}