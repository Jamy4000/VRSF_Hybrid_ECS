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
            public GazeParametersComponent GazeParameters;
            public GazeCalculationsComponent GazeCalculations;
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
                    GeneralGazeSetup(e);
                }
            }
        }

        protected override void OnUpdate()
        {
            if (GetEntities<Filter>().Length == 0)
            {
                this.Enabled = false;
            }

            if (_gazeParameters.UseGaze)
            {
                foreach (var e in GetEntities<Filter>())
                {
                    if (!e.GazeCalculations._IsSetup)
                    {
                        GeneralGazeSetup(e);
                    }
                    else
                    {
                        // AS there is only one gaze, we disable this system when it's setup.
                        this.Enabled = false;
                    }
                }
            }
        }
        #endregion ComponentSystem_Methods


        #region PRIVATE_METHODS
        /// <summary>
        /// Setup the Gaze Parameters based on the ScriptableSingleton, set in the VRSF Interaction Parameters Window
        /// </summary>
        private void GeneralGazeSetup(Filter entity)
        {
            try
            {
                if (_gazeParameters.ReticleSprite != null)
                {
                    entity.GazeParameters.ReticleBackground.sprite = _gazeParameters.ReticleSprite;
                }

                if (_gazeParameters.ReticleTargetSprite != null)
                {
                    entity.GazeParameters.ReticleTarget.sprite = _gazeParameters.ReticleTargetSprite;
                }

                entity.GazeParameters.transform.localScale = _gazeParameters.ReticleSize;

                // Store the original scale and rotation.
                entity.GazeCalculations._OriginalScale = entity.GazeParameters.ReticleTransform.localScale;
                entity.GazeCalculations._OriginalRotation = entity.GazeParameters.ReticleTransform.localRotation;
                entity.GazeCalculations._VRCamera = VRSF_Components.VRCamera.transform;
                entity.GazeCalculations._IsSetup = true;
            }
            catch (System.Exception e)
            {
                Debug.Log("VRSF : The VR Components are not set in the scene yet, waiting for next frame.\n" + e);
            }
        }
        #endregion PRIVATE_METHODS
    }
}