using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
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
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            foreach (var e in GetEntities<Filter>())
            {
                e.PointerCalculations.StartCoroutine(InitValues(e));
            }
        }

        protected override void OnUpdate() { }

        /// <summary>
        /// Initialize the values for the Parabolic Pointer feature.
        /// </summary>
        /// <param name="e">The entity to check in the scene</param>
        private IEnumerator<WaitForEndOfFrame> InitValues(Filter e)
        {
            while (!Utils.VRSF_Components.SetupVRIsReady)
            {
                yield return new WaitForEndOfFrame();
            }

            e.PointerObjects._ControllerPointer = e.BACGeneral.ButtonHand == Controllers.EHand.LEFT ?
                Utils.VRSF_Components.LeftController.GetComponentInChildren<LineRenderer>() :
                Utils.VRSF_Components.RightController.GetComponentInChildren<LineRenderer>();

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
    }
}