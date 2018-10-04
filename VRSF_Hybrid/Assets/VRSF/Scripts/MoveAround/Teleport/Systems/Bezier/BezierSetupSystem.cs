using UnityEngine;
using UnityEngine.SceneManagement;
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
    public class BezierSetupSystem : BACUpdateSystem<BezierTeleportParametersComponent>, ITeleportSystem
    {
        new struct Filter : ITeleportFilter
        {
            public BACGeneralVariablesComponents BAC_Comp;
            public ScriptableRaycastComponent RayComp;
            public BezierTeleportCalculationComponent BezierComp;
            public TeleportGeneralComponent GeneralTeleport;
            public BezierTeleportParametersComponent BezierParameters;
        }


        #region PRIVATE_VARIABLES
        private ControllersParametersVariable _controllersParameters;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            SceneManager.sceneUnloaded += OnSceneUnloaded;

            foreach (var e in GetEntities<Filter>())
            {
                SetupListenersResponses(e);
                InitializeValues(e);
            }
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            
            foreach (var e in GetEntities<Filter>())
            {
                if (!e.BezierComp._IsSetup)
                {
                    InitializeValues(e);
                }
            }
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();

            foreach (var e in GetEntities<Filter>())
            {
                RemoveListenersOnEndApp(e);
            }
            
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
        #endregion ComponentSystem_Methods


        #region PUBLIC_METHODS

        #region Listeners_Setup
        public override void SetupListenersResponses(object entity)
        {
            var e = (Filter)entity;
            if ((e.BAC_Comp.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                e.BAC_Comp.OnButtonIsClicking.AddListener(delegate { ToggleDisplay(e, true); });
                e.BAC_Comp.OnButtonStopClicking.AddListener(delegate { TeleportUser(e); });
            }

            if ((e.BAC_Comp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.BAC_Comp.OnButtonIsTouching.AddListener(delegate { ToggleDisplay(e, true); });
                e.BAC_Comp.OnButtonStopTouching.AddListener(delegate { TeleportUser(e); });
            }
        }

        public override void RemoveListenersOnEndApp(object entity)
        {
            var e = (Filter)entity;
            if ((e.BAC_Comp.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                e.BAC_Comp.OnButtonStartClicking.RemoveAllListeners();
            }

            if ((e.BAC_Comp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.BAC_Comp.OnButtonStartTouching.RemoveAllListeners();
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
                var newPos = entity.BezierComp._GroundPos;
                switch (VRSF_Components.DeviceLoaded)
                {
                    case EDevice.OPENVR:
                        VRSF_Components.CameraRig.transform.position = entity.BezierComp._GroundPos;
                        break;
                    default:
                        newPos.y += 1.8f;
                        VRSF_Components.CameraRig.transform.position = newPos;
                        break;
                }
            }

            ToggleDisplay(entity, false);
        }


        /// <summary>
        /// Check the newPos for theStep by Step feature depending on the Teleport Boundaries
        /// HANDLE IN THE BEZIER Update System
        /// </summary>
        public void CheckNewPosWithBoundaries(ITeleportFilter teleportFilter, ref Vector3 posToCheck) { }
        #endregion

        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Active Teleporter Arc Path
        /// </summary>
        /// <param name="active"></param>
        private void ToggleDisplay(Filter entity, bool active)
        {
            // If the user is aiming to the UI, we don't activate the system
            if (!entity.RayComp.RaycastHitVar.isNull && entity.RayComp.RaycastHitVar.Value.collider.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                return;
            }
            else
            {
                HandleExclusionLayer(entity.GeneralTeleport, active);

                entity.BezierComp._ArcRenderer.enabled = active;
                entity.BezierParameters.TargetMarker.SetActive(active);
                entity.BezierComp._DisplayActive = active;

                // Change pointer activation if the user is using it
                if ((entity.RayComp.RayOrigin == EHand.LEFT && _controllersParameters.UsePointerLeft) || (entity.RayComp.RayOrigin == EHand.RIGHT && _controllersParameters.UsePointerRight))
                    entity.BezierComp._ControllerPointer.enabled = !active;
            }
        }


        private void HandleExclusionLayer(TeleportGeneralComponent generalTeleport, bool active)
        {
            if (active)
            {
                _controllersParameters.RightExclusionLayer = _controllersParameters.RightExclusionLayer.RemoveFromMask(generalTeleport.TeleportLayer);
                _controllersParameters.LeftExclusionLayer = _controllersParameters.LeftExclusionLayer.RemoveFromMask(generalTeleport.TeleportLayer);
            }
            else
            {
                _controllersParameters.RightExclusionLayer = _controllersParameters.RightExclusionLayer.AddToMask(generalTeleport.TeleportLayer);
                _controllersParameters.LeftExclusionLayer = _controllersParameters.LeftExclusionLayer.AddToMask(generalTeleport.TeleportLayer);
            }
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

                entity.GeneralTeleport.TeleportLayer = LayerMask.NameToLayer("Teleport");

                if (entity.GeneralTeleport.TeleportLayer == -1)
                {
                    Debug.Log("VRSF : You won't be able to teleport on the floor, as you didn't set the Ground Layer");
                }

                entity.BezierComp._ArcRenderer = entity.BezierComp.GetComponentInChildren<LineRenderer>();
                entity.BezierComp._ArcRenderer.enabled = false;
                entity.BezierParameters.TargetMarker.SetActive(false);

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
                    entity.GeneralTeleport.ExclusionLayer = _controllersParameters.GetExclusionsLayer(EHand.LEFT);

                    if (_controllersParameters.UsePointerLeft)
                        entity.BezierComp._ControllerPointer = VRSF_Components.LeftController.GetComponent<LineRenderer>();
                    break;

                case (EHand.RIGHT):
                    entity.BezierComp._CurveOrigin = VRSF_Components.RightController.transform;
                    entity.GeneralTeleport.ExclusionLayer = _controllersParameters.GetExclusionsLayer(EHand.RIGHT);

                    if (_controllersParameters.UsePointerRight)
                        entity.BezierComp._ControllerPointer = VRSF_Components.RightController.GetComponent<LineRenderer>();
                    break;

                case (EHand.GAZE):
                    entity.BezierComp._CurveOrigin = VRSF_Components.VRCamera.transform;
                    entity.GeneralTeleport.ExclusionLayer = GazeParametersVariable.Instance.GetGazeExclusionsLayer();
                    break;

                default:
                    Debug.LogError("Please specify a valid hand in the BezierTeleport script. The Gaze cannot be used.");
                    break;
            }
        }


        /// <summary>
        /// Reactivate the System when switching to another Scene.
        /// </summary>
        /// <param name="oldScene">The previous scene before switching</param>
        private void OnSceneUnloaded(Scene oldScene)
        {
            this.Enabled = true;
        }
        #endregion PRIVATE_METHODS
    }
}