using Unity.Entities;
using UnityEngine;
using VRSF.Utils.Components;

namespace VRSF.Utils.Systems.ButtonActionChoser
{
    /// <summary>
    /// Check if we use the button action choser script for this SDK thanks to the toggle in the Inspector.
    /// </summary>
    public class BACSDKCheckingSystem : ComponentSystem
    {
        struct Filter
        {
            public ButtonActionChoserComponents ButtonComponents;
        }
        

        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            foreach (var entity in GetEntities<Filter>())
            {
                if (!CheckUseSDKToggles(entity.ButtonComponents))
                {
                    entity.ButtonComponents.CanBeUsed = false;
                }
            }
        }
        
        protected override void OnUpdate() { }
        #endregion


        #region PRIVATES_METHODS
        /// <summary>
        /// Check if at least one of the three toggles for the SDK to Use is set at true, and if the current loaded Device is listed in those bool
        /// </summary>
        /// <returns>true if the current loaded SDK is selected in the inspector</returns>
        private bool CheckUseSDKToggles(ButtonActionChoserComponents comp)
        {
            if (!comp.UseOpenVR && !comp.UseOVR && !comp.UseSimulator)
            {
                Debug.LogError("VRSF : You need to chose at least one SDK to use the " + GetType().Name + " script. Setting CanBeUsed of ButtonActionChoserComponents to false.");
                comp.CanBeUsed = false;
                return false;
            }

            switch (VRSF_Components.DeviceLoaded)
            {
                case EDevice.OPENVR:
                    return comp.UseOpenVR;

                case EDevice.OVR:
                    return comp.UseOVR;

                case EDevice.SIMULATOR:
                    return comp.UseSimulator;
            }

            return true;
        }
        #endregion PRIVATES_METHODS
    }
}
