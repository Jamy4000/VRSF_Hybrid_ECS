using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRSF.MoveAround.Teleport.Interfaces;

namespace VRSF.MoveAround.Teleport.Components
{
    /// <summary>
    /// Contains all variable necessary for the LongRangeTeleportSystems to work.
    /// It implements the ITeleportComponent to be able to display the boundaries limits with TeleportBoundaries.
    /// </summary>
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity), typeof(Utils.Components.ButtonActionChoserComponents))]
    public class LongRangeTeleportComponent : MonoBehaviour, ITeleportComponent
    {
        [HideInInspector] public LayerMask _teleportLayer = -1;
        [HideInInspector] public bool CanTeleport;
        

        [Header("Teleport Far Away Parameters")]
        [Tooltip("The loading time for the TeleportFarAway feature.")]
        public float TimerBeforeTeleport = 1.0f;
        [Tooltip("If you want to adjust the height to the point that was hit.")]
        public bool AdjustHeight = true;
        [Tooltip("The height at which the user is teleported above the ground.")]
        public float HeightAboveGround = 1.8f;
        

        [Header("OPTIONAL : Loading Slider")]
        [Tooltip("Those are used to display the loading slider on the controller when charging the teleport feature.\n" +
            "Can be useful when using multiple teleport feature, like StepByStep and Long Range teleports, on the same Button.")]
        [HideInInspector] public bool UseLoadingSlider;
        [HideInInspector] public Image FillRect;
        [HideInInspector] public Text TeleportText;


        #region Boundaries
        [Header("Teleport Boundaries Parameters")]
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