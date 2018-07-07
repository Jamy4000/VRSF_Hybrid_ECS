using ScriptableFramework.Util;
using VRSF.Controllers;
using UnityEngine;
using VRSF.Inputs;

namespace VRSF.Gaze
{
    /// <summary>
    /// Contain all the parameters for the Gaze Reticle 
    /// </summary>
    public class GazeParametersVariable : ScriptableSingleton<GazeParametersVariable>
    {
        #region PUBLIC_VARIABLES

        [Multiline(10)]
        public string DeveloperDescription = "General Parameters for the Gaze. Those can either be edited in the Scriptable Object, or in the dedicated Window (Window/VRSF/VR Interaction Parameters)";

        [Tooltip("Wheter you wanna use the gaze or not.")]
        public bool UseGaze = false;

        [Tooltip("Layer(s) to exclude from the raycast with the Right controller Pointer. Use the Layer number given in the Layer list.")]
        public LayerMask GazeExclusionLayer = 0;
        
        [Tooltip("Whether the reticle should use different type of states.")]
        public bool UseDifferentStates = false;

        #region UseDifferentStatesAtTrue

        [Tooltip("The current state of the Gaze.")]
        public EPointerState GazePointerState = EPointerState.ON;

        [Tooltip("The color of the Reticle Background when the state is on.")]
        public Color ColorOnReticleBackgroud = Color.blue;

        [Tooltip("The color of the Reticle Target when the state is on.")]
        public Color ColorOnReticleTarget = Color.blue;

        [Tooltip("The color of the Reticle Background when the state is off.")]
        public Color ColorOffReticleBackgroud = Color.red;

        [Tooltip("The color of the Reticle Target when the state is off.")]
        public Color ColorOffReticleTarget = Color.red;

        [Tooltip("The color of the Reticle Background when the state is Selectable.")]
        public Color ColorSelectableReticleBackgroud = Color.green;

        [Tooltip("The color of the Reticle Target when the state is Selectable.")]
        public Color ColorSelectableReticleTarget = Color.green;

        #endregion

        #region UseDifferentStatesAtFalse

        [Tooltip("The basic color of the Reticle.")]
        public Color ReticleColor = Color.red;

        [Tooltip("The basic color of the Reticle Target.")]
        public Color ReticleTargetColor = Color.red;

        #endregion


        [Tooltip("Whether the reticle should be placed parallel to a surface.")]
        public bool UseNormal = true;

        [Tooltip("The default distance away from the camera the reticle is placed.")]
        public float DefaultDistance = 200.0f;

        [Tooltip("The default size of the reticle.")]
        public Vector3 ReticleSize = Vector3.one;

        [Tooltip("The Reticle image.")]
        public Sprite ReticleSprite;

        [Tooltip("The Reticle Target Image.")]
        public Sprite ReticleTargetSprite;

        [Header("Gaze Click Buttons")]
        [Tooltip("The Button you wanna use to click with the OVR SDK")]
        public EControllersInput GazeButtonOVR = EControllersInput.NONE;
        [Tooltip("The Button you wanna use to click with the OpenVR SDK")]
        public EControllersInput GazeButtonOpenVR = EControllersInput.NONE;
        [Tooltip("The Button you wanna use to click with the Simulator SDK")]
        public EControllersInput GazeButtonSimulator = EControllersInput.NONE;
        #endregion PUBLIC_VARIABLES


        #region PUBLIC_METHODS
        /// <summary>
        /// Reset all parameters to their default values
        /// </summary>
        public void ResetParameters()
        {
            UseGaze = false;
            DefaultDistance = 200.0f;
            UseNormal = true;
            ReticleSize = Vector3.one;
            GazeExclusionLayer = 0;
            
            ReticleColor = Color.red;
            ReticleTargetColor = Color.red;

            GazePointerState = EPointerState.ON;

            ColorOnReticleBackgroud = Color.blue;
            ColorOnReticleTarget = Color.blue;

            ColorOffReticleBackgroud = Color.red;
            ColorOffReticleTarget = Color.red;

            ColorSelectableReticleBackgroud = Color.green;
            ColorSelectableReticleTarget = Color.green;

            UseDifferentStates = false;

            GazeButtonOVR = EControllersInput.NONE;
            GazeButtonOpenVR = EControllersInput.NONE;
            GazeButtonSimulator = EControllersInput.NONE;
        }


        /// <summary>
        /// Give the Exclusion Layer(s) depending on the IgnoreOnlyExclusionLayerGaze and the specified exclusion layers
        /// </summary>
        /// <returns>the exclusion layers as an int</returns>
        public int GetGazeExclusionsLayer()
        {
            return ~GazeExclusionLayer;
        }
        #endregion PUBLIC_METHODS


        // EMPTY
        #region GETTERS_SETTERS
        #endregion GETTERS_SETTERS
    }
}