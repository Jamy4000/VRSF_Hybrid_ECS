using ScriptableFramework.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace VRSF.Core.Controllers
{
    /// <summary>
    /// Contain all the parameters for the Controllers
    /// </summary>
    public class ControllersParametersVariable : ScriptableSingleton<ControllersParametersVariable>
    {
        #region PUBLIC_VARIABLE
        [Multiline(10)]
        public string DeveloperDescription = "";

        [Header("General Parameters")]
        [Tooltip("Wheter you wanna use the controllers or not.")]
        public bool UseControllers = true;


        [Header("Left Controllers Parameters")]
        [Tooltip("Wheter you wanna use the Ray Pointers on the Left Controller or not.")]
        public bool UsePointerLeft = true;
        
        [Tooltip("OPTIONAL : Layer(s) to exclude from the raycast with the Left controller Pointer. Use the Layer number given in the Layer list.")]
        public LayerMask LeftExclusionLayer = 0;
        
        [Tooltip("Themaximum distance at which the left pointer is going.")]
        public float MaxDistancePointerLeft = 1000f;


        [Header("Right Controller Parameters")]
        [Tooltip("Wheter you wanna use the Ray Pointers on the Right Controller or not.")]
        public bool UsePointerRight = true;

        [Tooltip("OPTIONAL : Layer(s) to exclude from the raycast with the Right controller Pointer. Use the Layer number given in the Layer list.")]
        public LayerMask RightExclusionLayer = 0;
        
        [Tooltip("Themaximum distance at which the right pointer is going.")]
        public float MaxDistancePointerRight = 1000f;
        #endregion PUBLIC_VARIABLE


        #region PUBLIC_METHODS
        /// <summary>
        /// Reset all parameters to their default values
        /// </summary>
        public void ResetParameters()
        {
            UseControllers = true;

            UsePointerLeft = true;
            LeftExclusionLayer = 0;
            MaxDistancePointerLeft = 20f;

            UsePointerRight = true;
            RightExclusionLayer = 0;
            MaxDistancePointerRight = 20f;
        }

        /// <summary>
        /// Give the Exclusion Layer(s) depending on the IgnoreOnlyExclusionLayer bools and the specified exclusion layers
        /// </summary>
        /// <param name="hand">The hand to which the exclusion layer is set (LEFT OR RIGHT)</param>
        /// <returns>the exclusion layers as an int</returns>
        public LayerMask GetExclusionsLayer(EHand hand)
        {
            switch (hand)
            {
                case (EHand.LEFT):
                    return ~LeftExclusionLayer;

                case (EHand.RIGHT):
                    return ~RightExclusionLayer;

                default:
                    Debug.LogError("No Hand provided, returning -1");
                    return -1;

            }
        }


        /// <summary>
        /// Allow us to get the names of all the layers
        /// </summary>
        /// <returns>The list of layers names</returns>
        public static string[] GetLayerMaskList()
        {
            List<string> layerNames = new List<string>();

            for (int i = 0; i <= 31; i++) // Unity supports 31 layers
            {
                layerNames.Add(LayerMask.LayerToName(i));
            }

            return layerNames.ToArray();
        }
        #endregion PUBLIC_METHODS
    }
}