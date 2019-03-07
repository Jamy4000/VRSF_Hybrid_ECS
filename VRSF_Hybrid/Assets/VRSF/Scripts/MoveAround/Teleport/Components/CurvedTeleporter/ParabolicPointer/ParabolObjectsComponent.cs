using System.Collections.Generic;
using UnityEngine;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Class containing the references to the objects that the parabole need
    /// 
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// </summary>
    public class ParabolObjectsComponent : MonoBehaviour
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

        /// <summary>
        /// Reference to the pointer placed on the end of the user
        /// </summary>
        [System.NonSerialized] public LineRenderer _ControllerPointer;

        /// <summary>
        /// Reference to the pointer's distance
        /// </summary>
        [System.NonSerialized] public float _ControllerPointerDistance;

#if UNITY_EDITOR
        // Only used for the OnDrawGizmos method
        [System.NonSerialized] public List<Vector3> ParabolaPoints_Gizmo;
#endif
    }
}