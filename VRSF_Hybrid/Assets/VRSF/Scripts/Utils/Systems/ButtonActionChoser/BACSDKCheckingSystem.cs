using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRSF.Utils.Components;
using VRSF.Utils.Components.ButtonActionChoser;

namespace VRSF.Utils.Systems.ButtonActionChoser
{
    /// <summary>
    /// Check if we use the button action choser script for this SDK thanks to the toggle in the Inspector.
    /// </summary>
    public class BACSDKCheckingSystem : ComponentSystem
    {
        struct Filter
        {
            public SDKChoserComponent SDKComp;
            public ButtonActionChoserComponents BAC_Comp;
        }
        

        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            SceneManager.activeSceneChanged += OnSceneChanged;

            foreach (var entity in GetEntities<Filter>())
            {
                // Is put in an if method as the CanBeUsed is set in other script and we don't want to set it at true (true being is default value)
                entity.BAC_Comp.CorrectSDK = CheckUseSDKToggles(entity);
                entity.SDKComp.IsSetup = true;
            }

            this.Enabled = false;
        }
        
        protected override void OnUpdate() { }
        #endregion


        #region PRIVATES_METHODS
        /// <summary>
        /// Check if at least one of the three toggles for the SDK to Use is set at true, and if the current loaded Device is listed in those bool
        /// </summary>
        /// <returns>true if the current loaded SDK is selected in the inspector</returns>
        private bool CheckUseSDKToggles(Filter entity)
        {
            if (!entity.SDKComp.UseOpenVR && !entity.SDKComp.UseOVR && !entity.SDKComp.UseSimulator)
            {
                Debug.LogError("VRSF : You need to chose at least one SDK to use the " + GetType().Name + " script. Setting CanBeUsed of ButtonActionChoserComponents to false.");
                return false;
            }

            switch (VRSF_Components.DeviceLoaded)
            {
                case EDevice.OPENVR:
                    return entity.SDKComp.UseOpenVR;

                case EDevice.OVR:
                    return entity.SDKComp.UseOVR;

                case EDevice.SIMULATOR:
                    return entity.SDKComp.UseSimulator;

                default:
                    return false;
            }
        }


        /// <summary>
        /// Reactivate the System when switching to another Scene.
        /// </summary>
        /// <param name="oldScene">The previous scene before switching</param>
        /// <param name="newScene">The new scene after switching</param>
        private void OnSceneChanged(Scene oldScene, Scene newScene)
        {
            this.Enabled = true;
        }
        #endregion PRIVATES_METHODS
    }
}
