using UnityEngine;
using UnityEngine.UI;
using VRSF.Utils.ButtonActionChoser;
using VRSF.Core.Raycast;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Contains all variable necessary for the LongRangeTeleportSystems to work.
    /// It implements the ITeleportComponent to be able to display the boundaries limits with TeleportBoundaries.
    /// </summary>
    [RequireComponent(typeof(BACGeneralComponent), typeof(ScriptableRaycastComponent), typeof(TeleportGeneralComponent))]
    public class LongRangeTeleportComponent : MonoBehaviour
    {
        [Header("OPTIONAL : Loading Slider")]
        public bool UseLoadingTimer = true;
        public float LoadingTime = 1.5f;
        [HideInInspector] public float _LoadingTimer = 0.0f;

        [Tooltip("Those are used to display the loading slider on the controller when charging the teleport feature.\n" +
            "Can be useful when using multiple teleport feature, like StepByStep and Long Range teleports, on the same Button.")]
        [HideInInspector] public Image FillRect;
        [HideInInspector] public Text TeleportText;
    }
}