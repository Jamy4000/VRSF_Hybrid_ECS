using UnityEngine;
using VRSF.Utils.Components;
using VRSF.Utils.Components.ButtonActionChoser;

namespace VRSF.MoveAround.Teleport.Components
{
    /// <summary>
    /// Contains all variable necessary for the StepByStepSystems to work.
    /// It implements the ITeleportComponent to be able to display the boundaries limits with TeleportBoundaries.
    /// </summary>
    [RequireComponent(typeof(BACGeneralComponent), typeof(ScriptableRaycastComponent))]
    public class StepByStepComponent : MonoBehaviour
    {
        [Header("Teleport Step by Step Parameters")]
        [Tooltip("The distance to move the Camera (in Meters) for the step by step feature.")]
        public float DistanceStepByStep = 1.0f;

        [Tooltip("The NavMesh Build index for the Teleport feature. If you only use the NavMesh for the Teleport feature, let it to 0.")]
        public int TeleportNavMeshBuildIndex = 0;

        /// <summary>
        /// Step height, specified in the NavMesh
        /// </summary>
        [HideInInspector] public float StepHeight = 0.5f;
    }
}