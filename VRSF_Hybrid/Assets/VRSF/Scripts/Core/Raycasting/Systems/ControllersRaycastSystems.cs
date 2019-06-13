using ScriptableFramework.Variables;
using System.Linq;
using Unity.Entities;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Raycast
{
    /// <summary>
    /// Check the Raycast of the two controllers and the Gaze and reference them in RaycastHit and Ray ScriptableVariable
    /// </summary>
    public class ControllersRaycastSystems : ComponentSystem
    {
        struct Filter
        {
            public ScriptableRaycastComponent RaycastComponents;
        }
        
        private Controllers.ControllersParametersVariable _controllersParameters;

        #region ComponentSystem_Methods
        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            _controllersParameters = Controllers.ControllersParametersVariable.Instance;
        }

        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (IsControllersOrigin(e.RaycastComponents.RayOrigin) && e.RaycastComponents.IsSetup)
                {
                    if (VRSF_Components.DeviceLoaded == EDevice.SIMULATOR)
                        e.RaycastComponents.RayVar.SetValue(VRSF_Components.VRCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition));
                    else if (VRSF_Components.DeviceLoaded != EDevice.NULL)
                        e.RaycastComponents.RayVar.SetValue(new Ray(e.RaycastComponents.RayOriginTransform.position, e.RaycastComponents.RayOriginTransform.TransformDirection(Vector3.forward)));

                    RaycastHitHandler(e.RaycastComponents.RayVar.Value, e.RaycastComponents.RayOrigin, e.RaycastComponents.ExcludedLayer, ref e.RaycastComponents.RaycastHitVar);
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
        private void RaycastHitHandler(Ray ray, ERayOrigin rayOrigin, int layerToIgnore, ref RaycastHitVariable hitVariable)
        {
            var distance = GetDistanceFromOrigin();
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


            float GetDistanceFromOrigin()
            {
                switch (rayOrigin)
                {
                    case ERayOrigin.LEFT_HAND:
                        return _controllersParameters.MaxDistancePointerLeft;
                    default:
                        return _controllersParameters.MaxDistancePointerRight;
                }
            }
        }


        private bool IsControllersOrigin(ERayOrigin rayOrigin)
        {
            return rayOrigin == ERayOrigin.LEFT_HAND || rayOrigin == ERayOrigin.RIGHT_HAND;
        }
        #endregion PRIVATE_METHODS
    }
}