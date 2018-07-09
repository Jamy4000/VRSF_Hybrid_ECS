using UnityEngine;
using VRSF.MoveAround.Teleport.Interfaces;
using VRSF.Utils.Components;
using VRSF.Utils.Components.ButtonActionChoser;

namespace VRSF.MoveAround.Teleport.Components
{
    /// <summary>
    /// Contains all variable necessary for the StepByStepSystems to work.
    /// It implements the ITeleportComponent to be able to display the boundaries limits with TeleportBoundaries.
    /// </summary>
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity), typeof(ButtonActionChoserComponents), typeof(ScriptableRaycastComponent))]
    public class StepByStepComponent : MonoBehaviour, ITeleportComponent
    {
        [Header("Teleport Step by Step Parameters")]
        [Tooltip("The distance to move the Camera (in Meters) for the step by step feature.")]
        public float DistanceStepByStep = 1.0f;
        [Tooltip("If you want to move on the vertical axis with the step by step feature.")]
        public bool MoveOnVerticalAxis = false;


        #region Boundaries
        [Tooltip("Wheter you wanna use boundaries for the flying mode or not.")]
        [HideInInspector] public bool _UseBoundaries = false;

        [Tooltip("The minimun position at which the user can go if UseHorizontalBoundaries is at true.")]
        [HideInInspector] public Vector3 _MinUserPosition = new Vector3(-100.0f, -1.0f, -100.0f);

        [Tooltip("The maximum position at which the user can go if UseHorizontalBoundaries is at true.")]
        [HideInInspector] public Vector3 _MaxUserPosition = new Vector3(100.0f, 100.0f, 100.0f);
        #endregion Boundaries


        #region Getters_ITeleportComponent
        public bool UseBoundaries()
        {
            return _UseBoundaries;
        }

        public Vector3 MaxPosBoundaries()
        {
            return _MaxUserPosition;
        }

        public Vector3 MinPosBoundaries()
        {
            return _MinUserPosition;
        }
        #endregion Getters_ITeleportComponent
    }
}