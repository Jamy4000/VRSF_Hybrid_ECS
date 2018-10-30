using UnityEngine;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Class calculating and displaying the Parabolic/Bezier Line
    /// 
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// </summary>
    public class PointerCalculationsComponent : MonoBehaviour
    {
        [Header("Parabola Trajectory")]
        [Tooltip("Initial velocity of the parabola, in local space.")]
        public Vector3 InitialVelocity = Vector3.forward * 10f;
        [Tooltip("World-space \"acceleration\" of the parabola.  This effects the falloff of the curve.")]
        public Vector3 Acceleration = Vector3.up * -9.8f;

        [Header("Parabola Mesh Properties")]
        [Tooltip("Number of points on the parabola mesh.  Greater point counts lead to a higher poly/smoother mesh.")]
        public int PointCount = 10;
        [Tooltip("Approximate spacing between each of the points on the parabola mesh.")]
        public float PointSpacing = 0.5f;
        [Tooltip("Thickness of the parabola mesh")]
        public float GraphicThickness = 0.2f;
        [Tooltip("Material to use to render the parabola mesh")]
        public Material GraphicMaterial;

        public Vector3 SelectedPoint { get; set; }
        public bool PointOnNavMesh { get; set; }
        public float CurrentParabolaAngleY { get; set; }
        public Vector3 CurrentPointVector { get; set; }
    }
}