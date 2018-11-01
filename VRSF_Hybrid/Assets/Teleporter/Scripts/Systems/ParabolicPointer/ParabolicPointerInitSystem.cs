using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Init the values for the Parabolic Pointers Classes
    /// 
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// </summary>
    public class ParabolicPointerInitSystem : ComponentSystem
    {
        private struct Filter
        {
            public PointerObjectsComponent PointerObjects;
            public PointerCalculationsComponent PointerCalculations;
            public NavMeshAnimatorComponent NavMeshAnimator;
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            foreach (var e in GetEntities<Filter>())
            {
                InitValues(e);
            }
        }

        protected override void OnUpdate() { }

        private void InitValues(Filter e)
        {
            e.NavMeshAnimator._TeleportNavMesh = GameObject.FindObjectOfType<TeleportNavMeshComponent>();

            e.PointerObjects.ParabolaPoints = new List<Vector3>(e.PointerCalculations.PointCount);

            e.PointerObjects._parabolaMesh = new Mesh();
            e.PointerObjects._parabolaMesh.MarkDynamic();
            e.PointerObjects._parabolaMesh.name = "Parabolic Pointer";
            e.PointerObjects._parabolaMesh.vertices = new Vector3[0];
            e.PointerObjects._parabolaMesh.triangles = new int[0];

            if (e.PointerObjects._selectionPadPrefab != null)
            {
                e.PointerObjects._selectionPadObject = GameObject.Instantiate(e.PointerObjects._selectionPadPrefab);
                e.PointerObjects._selectionPadObject.SetActive(false);
            }

            if (e.PointerObjects._invalidPadPrefab != null)
            {
                e.PointerObjects._invalidPadObject = GameObject.Instantiate(e.PointerObjects._invalidPadPrefab);
                e.PointerObjects._invalidPadObject.SetActive(false);
            }
        }
    }
}