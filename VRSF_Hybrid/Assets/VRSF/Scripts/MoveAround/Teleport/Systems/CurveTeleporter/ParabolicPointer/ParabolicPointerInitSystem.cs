using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using VRSF.Controllers;
using VRSF.Utils;
using VRSF.Utils.Components.ButtonActionChoser;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Init the values for the Parabolic Pointers Feature
    /// 
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// </summary>
    public class ParabolicPointerInitSystem : ComponentSystem
    {
        private struct Filter
        {
            public ParabolObjectsComponent PointerObjects;
            public ParabolCalculationsComponent PointerCalculations;
            public BACGeneralComponent BACGeneral;
            public TeleportGeneralComponent TeleportGeneral;
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            foreach (var e in GetEntities<Filter>())
            {
                e.PointerObjects.StartCoroutine(CheckHand(e));
                InitValues(e);
            }
        }

        protected override void OnUpdate() { }

        /// <summary>
        /// Initialize the values for the Parabolic Pointer feature.
        /// </summary>
        /// <param name="e">The entity to check in the scene</param>
        private void InitValues(Filter e)
        {
            e.PointerObjects.ParabolaPoints = new List<Vector3>(e.PointerCalculations.PointCount);

            e.PointerObjects._parabolaMesh = new Mesh();
            e.PointerObjects._parabolaMesh.MarkDynamic();
            e.PointerObjects._parabolaMesh.name = "Parabolic Pointer";
            e.PointerObjects._parabolaMesh.vertices = new Vector3[0];
            e.PointerObjects._parabolaMesh.triangles = new int[0];

            if (e.PointerObjects._selectionPadPrefab != null)
            {
                e.PointerObjects._selectionPadObject = GameObject.Instantiate(e.PointerObjects._selectionPadPrefab);
                e.PointerObjects._selectionPadObject.transform.SetParent(e.PointerObjects.transform);
                e.PointerObjects._selectionPadObject.SetActive(false);
            }

            if (e.PointerObjects._invalidPadPrefab != null)
            {
                e.PointerObjects._invalidPadObject = GameObject.Instantiate(e.PointerObjects._invalidPadPrefab);
                e.PointerObjects._invalidPadObject.transform.SetParent(e.PointerObjects.transform);
                e.PointerObjects._invalidPadObject.SetActive(false);
            }
        }


        /// <summary>
        /// Set the ExclusionLayer and the ControllerPointer (LineRederer) reference depending on the Hand holding the script.
        /// </summary>
        /// <param name="e">The entity to check in the scene</param>
        private IEnumerator<WaitForEndOfFrame> CheckHand(Filter entity)
        {
            // We wait until the controllers are set
            while (VRSF_Components.RightController == null)
            {
                yield return new WaitForEndOfFrame();
            }

            switch (entity.BACGeneral.ButtonHand)
            {
                case (EHand.LEFT):
                    entity.TeleportGeneral.ExclusionLayer = ControllersParametersVariable.Instance.GetExclusionsLayer(EHand.LEFT);

                    if (ControllersParametersVariable.Instance.UsePointerLeft)
                        entity.PointerObjects._ControllerPointer = VRSF_Components.LeftController.GetComponent<LineRenderer>();
                    break;

                case (EHand.RIGHT):
                    entity.TeleportGeneral.ExclusionLayer = ControllersParametersVariable.Instance.GetExclusionsLayer(EHand.RIGHT);

                    if (ControllersParametersVariable.Instance.UsePointerRight)
                        entity.PointerObjects._ControllerPointer = VRSF_Components.RightController.GetComponent<LineRenderer>();
                    break;

                case (EHand.GAZE):
                    entity.TeleportGeneral.ExclusionLayer = Gaze.GazeParametersVariable.Instance.GetGazeExclusionsLayer();
                    break;

                default:
                    Debug.LogError("Please specify a valid hand in the BezierTeleport script. The Gaze cannot be used.");
                    break;
            }
        }
    }
}