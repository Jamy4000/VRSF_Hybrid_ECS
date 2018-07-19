using UnityEngine;
using VRSF.Utils.Components;
using VRSF.Utils.Components.ButtonActionChoser;

namespace VRSF.MoveAround.Teleport.Components
{
    /// <summary>
    /// Contains all variable necessary for the StepByStepSystems to work.
    /// It implements the ITeleportComponent to be able to display the boundaries limits with TeleportBoundaries.
    /// </summary>
    [RequireComponent(typeof(TeleportBoundariesComponent), typeof(ButtonActionChoserComponents), typeof(ScriptableRaycastComponent))]
    public class StepByStepComponent : MonoBehaviour
    {
        [Header("Teleport Step by Step Parameters")]
        [Tooltip("The distance to move the Camera (in Meters) for the step by step feature.")]
        public float DistanceStepByStep = 1.0f;
        [Tooltip("If you want to move on the vertical axis with the step by step feature.")]
        public bool MoveOnVerticalAxis = false;
    }
}