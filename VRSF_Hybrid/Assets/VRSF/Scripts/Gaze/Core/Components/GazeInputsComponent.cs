using ScriptableFramework.Variables;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;

namespace VRSF.Gaze
{
    /// <summary>
    /// Contains the bool variable references for the gaze inputs
    /// </summary>
    public class GazeInputsComponent : MonoBehaviour
    {
        [Header("Gaze Click Buttons")]
        [Tooltip("The Button you wanna use to click with the gaze")]
        public EControllersButton GazeButton = EControllersButton.NONE;

        [Tooltip("The Hand on which the button is placed")]
        public EHand GazeButtonHand = EHand.NONE;

        // TODO Setup those variable
        [HideInInspector] public BoolVariable GazeIsClicking;
        [HideInInspector] public BoolVariable GazeIsTouching;
    }
}