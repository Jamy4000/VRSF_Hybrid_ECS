using UnityEngine;
using UnityEngine.UI;
using VRSF.Utils.Components;
using VRSF.Utils.Components.ButtonActionChoser;

namespace VRSF.MoveAround.Teleport.Components
{
    /// <summary>
    /// Contains all variable necessary for the LongRangeTeleportSystems to work.
    /// It implements the ITeleportComponent to be able to display the boundaries limits with TeleportBoundaries.
    /// </summary>
    [RequireComponent(typeof(BACGeneralComponent), typeof(ScriptableRaycastComponent), typeof(TeleportGeneralComponent))]
    public class LongRangeTeleportComponent : MonoBehaviour
    {
        [Header("Teleport Far Away Parameters")]
        [Tooltip("The loading time for the TeleportFarAway feature.")]
        public float TimerBeforeTeleport = 1.0f;
        

        [Header("OPTIONAL : Loading Slider")]
        [Tooltip("Those are used to display the loading slider on the controller when charging the teleport feature.\n" +
            "Can be useful when using multiple teleport feature, like StepByStep and Long Range teleports, on the same Button.")]
        [HideInInspector] public bool UseLoadingSlider;
        [HideInInspector] public Image FillRect;
        [HideInInspector] public Text TeleportText;
    }
}