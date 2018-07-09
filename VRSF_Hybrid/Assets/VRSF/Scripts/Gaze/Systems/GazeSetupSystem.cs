using Unity.Entities;
using UnityEngine;
using VRSF.Gaze.Components;
using VRSF.Utils;

namespace VRSF.Gaze.Systems
{
    public class GazeSetupSystem : ComponentSystem
    {
        struct Filter
        {
            public GazeComponent GazeComp;
        }


        #region PRIVATE_VARIABLES
        private GazeParametersVariable _gazeParameters;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            _gazeParameters = GazeParametersVariable.Instance;

            if (_gazeParameters.UseGaze)
            {
                foreach (var e in GetEntities<Filter>())
                {
                    GeneralGazeSetup(e.GazeComp);
                }
            }
        }

        protected override void OnUpdate()
        {
            if (_gazeParameters.UseGaze)
            {
                // We will only use one gaze.
                var e = GetEntities<Filter>()[0];

                if (!e.GazeComp._IsSetup)
                {
                    GeneralGazeSetup(e.GazeComp);
                }
                else
                {
                    this.Enabled = false;
                }
            }
        }
        #endregion ComponentSystem_Methods


        #region PRIVATE_METHODS
        /// <summary>
        /// Setup the Gaze Parameters based on the ScriptableSingleton, set in the VRSF Interaction Parameters Window
        /// </summary>
        private void GeneralGazeSetup(GazeComponent comp)
        {
            try
            {
                if (_gazeParameters.ReticleSprite != null)
                {
                    comp.ReticleBackground.sprite = _gazeParameters.ReticleSprite;
                }

                if (_gazeParameters.ReticleTargetSprite != null)
                {
                    comp.ReticleTarget.sprite = _gazeParameters.ReticleTargetSprite;
                }

                comp.transform.localScale = _gazeParameters.ReticleSize;

                // Store the original scale and rotation.
                comp._OriginalScale = comp.ReticleTransform.localScale;
                comp._OriginalRotation = comp.ReticleTransform.localRotation;
                comp._VRCamera = VRSF_Components.VRCamera.transform;
                comp._IsSetup = true;
            }
            catch (System.Exception e)
            {
                Debug.Log("VRSF : The VR Components are not set in the scene yet, waiting for next frame.\n" + e);
            }
        }
        #endregion PRIVATE_METHODS
    }
}