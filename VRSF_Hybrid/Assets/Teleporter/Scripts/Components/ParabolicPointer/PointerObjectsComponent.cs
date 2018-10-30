using System.Collections.Generic;
using UnityEngine;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Class calculating and displaying the Parabolic/Bezier Line
    /// 
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// </summary>
    public class PointerObjectsComponent : MonoBehaviour
    {
        [Header("Selection Pad Properties")]
        [Tooltip("Prefab to use as the selection pad when the player is pointing at a valid teleportable surface.")]
        public GameObject _selectionPadPrefab;
        [Tooltip("Prefab to use as the selection pad when the player is pointing at an invalid teleportable surface.")]
        public GameObject _invalidPadPrefab;

        [System.NonSerialized] public GameObject _selectionPadObject;
        [System.NonSerialized] public GameObject _invalidPadObject;

        [System.NonSerialized] public Mesh _parabolaMesh;

        [System.NonSerialized] public List<Vector3> ParabolaPoints;

#if UNITY_EDITOR
        // Only used for the OnDrawGizmos method
        [System.NonSerialized] public List<Vector3> ParabolaPoints_Gizmo;
#endif
    }
}