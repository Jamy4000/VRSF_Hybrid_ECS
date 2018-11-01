using Unity.Entities;
using UnityEngine;
using VRSF.Utils;
using VRSF.Utils.Components.ButtonActionChoser;

namespace VRSF.MoveAround.Teleport
{
    public class CurveTeleporterInitSystem : ComponentSystem
    {
        private struct Filter
        {
            public SceneObjectsComponent SceneObjects;
            public TeleportCalculationsComponent TeleportCalculations;
            public NavMeshAnimatorComponent NavMeshAnim;
            public BACGeneralComponent BACGeneral;
        }

        protected override void OnUpdate()
        {
            if (VRSF_Components.SetupVRIsReady)
            {
                var setupStillRunning = true;
                foreach (var e in GetEntities<Filter>())
                {
                    if (!e.TeleportCalculations._IsSetup)
                    {
                        InitVariables(e);
                        setupStillRunning = false;
                    }
                }
                this.Enabled = setupStillRunning;
            }
        }

        /// <summary>
        /// Initialize all variables necessary to use the teleport system.
        /// </summary>
        private void InitVariables(Filter e)
        {
            e.SceneObjects._TeleportNavMesh = GameObject.FindObjectOfType<TeleportNavMeshComponent>();
            if (e.SceneObjects._TeleportNavMesh == null)
            {
                Debug.LogError("VRSF : You need to add a TeleportNavMeshComponent in your scene to be able to use the Teleport Feature.");
            }

            // Ensure we mark the player as not teleporting
            e.TeleportCalculations.CurrentTeleportState = ETeleportState.None;
            
            try
            {
                e.SceneObjects.FadeComponent = VRSF_Components.VRCamera.GetComponentInChildren<TeleportFadeComponent>();
                e.SceneObjects.FadeComponent.TeleportState = ETeleportState.None;
            }
            catch (System.Exception exception)
            {
                Debug.Log("VRSF : No Fade Component found on the VRCamera. Teleporting user without fade effect.\n" + exception.ToString());
            }

            // Set some standard variables for the TeleportNavMeshComponent
            e.NavMeshAnim._NavmeshAnimator = e.NavMeshAnim.GetComponent<Animator>();
            e.NavMeshAnim._EnabledAnimatorID = Animator.StringToHash("Enabled");
            
            e.TeleportCalculations._IsSetup = true;
        }
    }
}