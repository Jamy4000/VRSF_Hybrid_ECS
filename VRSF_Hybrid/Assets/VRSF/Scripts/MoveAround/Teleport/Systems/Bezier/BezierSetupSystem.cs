using UnityEngine;
using VRSF.Controllers;
using VRSF.Gaze;
using VRSF.Inputs;
using VRSF.MoveAround.Teleport.Components;
using VRSF.MoveAround.Teleport.Interfaces;
using VRSF.Utils;
using VRSF.Utils.Components;
using VRSF.Utils.Components.ButtonActionChoser;
using VRSF.Utils.Systems.ButtonActionChoser;

namespace VRSF.MoveAround.Teleport.Systems
{
    /// <summary>
    /// Handle the Jobs to setup the BezierCurveComponents
    /// </summary>
    public class BezierSetupSystem : BACUpdateSystem, ITeleportSystem
    {
        struct Filter : ITeleportFilter
        {
            public ButtonActionChoserComponents BAC_Comp;
            public ScriptableRaycastComponent RayComp;
            public BezierTeleportComponent BezierComp;
        }


        #region PRIVATE_VARIABLES
        private Filter _currentSetupEntity;
        private ControllersParametersVariable _controllersParameters;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            foreach (var e in GetEntities<Filter>())
            {
                _currentSetupEntity = e;
                SetupListenersResponses();
                InitializeValues(e);
            }
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            // Check if the entities are all setup. 
            bool entitiesNotSetup = false;

            foreach (var e in GetEntities<Filter>())
            {
                if (!e.BezierComp._IsSetup)
                {
                    entitiesNotSetup = true;
                    InitializeValues(e);
                }
            }
            // If all the entities were setup, the bool stay at false, and the current system don't need to run anymore
            this.Enabled = entitiesNotSetup;
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();

            foreach (var e in GetEntities<Filter>())
            {
                _currentSetupEntity = e;
                RemoveListenersOnEndApp();
            }
        }
        #endregion ComponentSystem_Methods


        #region PUBLIC_METHODS

        #region Listeners_Setup
        public override void SetupListenersResponses()
        {
            if ((_currentSetupEntity.BAC_Comp.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                _currentSetupEntity.BAC_Comp.OnButtonStartClicking.AddListener(delegate { ToggleDisplay(_currentSetupEntity, true); });
                _currentSetupEntity.BAC_Comp.OnButtonStopClicking.AddListener(delegate { TeleportUser(_currentSetupEntity); });
            }

            if ((_currentSetupEntity.BAC_Comp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                _currentSetupEntity.BAC_Comp.OnButtonStartTouching.AddListener(delegate { ToggleDisplay(_currentSetupEntity, true); });
                _currentSetupEntity.BAC_Comp.OnButtonStopTouching.AddListener(delegate { TeleportUser(_currentSetupEntity); });
            }
        }

        public override void RemoveListenersOnEndApp()
        {
            if ((_currentSetupEntity.BAC_Comp.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                _currentSetupEntity.BAC_Comp.OnButtonStartClicking.RemoveAllListeners();
            }

            if ((_currentSetupEntity.BAC_Comp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                _currentSetupEntity.BAC_Comp.OnButtonStartTouching.RemoveAllListeners();
            }
        }
        #endregion Listeners_Setup


        #region Teleport_Interface
        /// <summary>
        /// Method to call from StartClicking or StartTouching, teleport the user one meter away.
        /// </summary>
        public void TeleportUser(ITeleportFilter teleportFilter)
        {
            Filter entity = (Filter)teleportFilter;

            if (entity.BezierComp._GroundDetected || entity.BezierComp._LimitDetected)
            {
                VRSF_Components.CameraRig.transform.position = entity.BezierComp._GroundPos + new Vector3(0, entity.BezierComp.HeightAboveGround + VRSF_Components.CameraRig.transform.localScale.x, 0) + entity.BezierComp._LastNormal * 0.1f;
            }
            ToggleDisplay(entity, false);
        }


        /// <summary>
        /// Check the newPos for theStep by Step feature depending on the Teleport Boundaries
        /// </summary>
        public Vector3 CheckNewPosWithBoundaries(ITeleportFilter teleportFilter, Vector3 posToCheck)
        {
            // HANDLE IN BezierTeleportSystem
            return Vector3.zero;
        }
        #endregion

        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Active Teleporter Arc Path
        /// </summary>
        /// <param name="active"></param>
        private void ToggleDisplay(Filter entity, bool active)
        {
            entity.BezierComp._ArcRenderer.enabled = active;
            entity.BezierComp.TargetMarker.SetActive(active);
            entity.BezierComp._DisplayActive = active;

            // Change pointer activation if the user is using it
            if ((entity.RayComp.RayOrigin == EHand.LEFT && _controllersParameters.UsePointerLeft) || (entity.RayComp.RayOrigin == EHand.RIGHT && _controllersParameters.UsePointerRight))
                entity.BezierComp._ControllerPointer.enabled = !active;
        }


        /// <summary>
        /// Initialize the variable for this script
        /// </summary>
        private void InitializeValues(Filter entity)
        {
            try
            {
                _controllersParameters = ControllersParametersVariable.Instance;

                CheckHand(entity);

                entity.BezierComp._TeleportLayer = LayerMask.NameToLayer("Teleport");

                if (entity.BezierComp._TeleportLayer == -1)
                {
                    Debug.Log("VRSF : You won't be able to teleport on the floor, as you didn't set the Ground Layer");
                }

                entity.BezierComp._ArcRenderer = entity.BezierComp.GetComponentInChildren<LineRenderer>();
                entity.BezierComp._ArcRenderer.enabled = false;
                entity.BezierComp.TargetMarker.SetActive(false);

                entity.BezierComp._IsSetup = true;
            }
            catch (System.Exception e)
            {
                Debug.Log("VRSF : Couldn't setup correctly the Bezier Teleport, waiting for next frame.\n" + e);
            }
        }


        /// <summary>
        /// Set the RayOrigin Transform reference depending on the Hand holding the script.
        /// </summary>
        private void CheckHand(Filter entity)
        {
            switch (entity.RayComp.RayOrigin)
            {
                case (EHand.LEFT):
                    entity.BezierComp._CurveOrigin = VRSF_Components.LeftController.transform;
                    entity.BezierComp._ExclusionLayer = _controllersParameters.GetExclusionsLayer(EHand.LEFT);

                    if (_controllersParameters.UsePointerLeft)
                        entity.BezierComp._ControllerPointer = VRSF_Components.LeftController.GetComponent<LineRenderer>();
                    break;

                case (EHand.RIGHT):
                    entity.BezierComp._CurveOrigin = VRSF_Components.RightController.transform;
                    entity.BezierComp._ExclusionLayer = _controllersParameters.GetExclusionsLayer(EHand.RIGHT);

                    if (_controllersParameters.UsePointerRight)
                        entity.BezierComp._ControllerPointer = VRSF_Components.RightController.GetComponent<LineRenderer>();
                    break;

                case (EHand.GAZE):
                    entity.BezierComp._CurveOrigin = VRSF_Components.VRCamera.transform;
                    entity.BezierComp._ExclusionLayer = GazeParametersVariable.Instance.GetGazeExclusionsLayer();
                    break;

                default:
                    Debug.LogError("Please specify a valid hand in the BezierTeleport script. The Gaze cannot be used.");
                    break;
            }
        }
        #endregion PRIVATE_METHODS
    }
}