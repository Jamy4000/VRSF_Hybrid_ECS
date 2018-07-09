#if UNITY_EDITOR
using UnityEngine;
using VRSF.MoveAround.Teleport.Interfaces;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Display lines for the Teleport Boundaries in Edit Mode.
    /// It require a script that implements the ITeleportComponent as it try to access it with GetComponent.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(ITeleportComponent))]
    public class TeleportBoundariesDisplayer : MonoBehaviour
    {
        #region PUBLIC_VARIABLES
        // Choose the Unlit/Color shader in the Material Settings
        // You can change that color, to change the color of the connecting lines
        public Material LineMat;

        [Tooltip("The color of the Bounding box displayed in the Scene view for this FlyComponent.")]
        public Color BoundariesLinesColor = Color.blue;
        #endregion


        #region PRIVATE_VARIABLES
        private ITeleportComponent _teleportScript;
        #endregion


        #region MONOBEHAVIOUR_METHODS
        /// <summary>
        /// To show the lines in the editor
        /// </summary>
        void OnDrawGizmos()
        {
            _teleportScript = GetComponent<ITeleportComponent>();

            if (_teleportScript.UseBoundaries())
            {
                DrawConnectingLines();
            }
        }
        #endregion


        // EMPTY
        #region PUBLIC_METHODS
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Connect all the points together by getting there coordinates from the FlyingParamertersVariable
        /// </summary>
        void DrawConnectingLines()
        {
            Vector3 minPos = _teleportScript.MinPosBoundaries();
            Vector3 maxPos = _teleportScript.MaxPosBoundaries();

            // List of points/vertices
            Vector3[] vertices = new Vector3[8]
            {
                new Vector3(minPos.x, minPos.y, minPos.z),
                new Vector3(minPos.x, maxPos.y, minPos.z),
                new Vector3(minPos.x, maxPos.y, maxPos.z),
                new Vector3(minPos.x, minPos.y, maxPos.z),
                new Vector3(maxPos.x, minPos.y, minPos.z),
                new Vector3(maxPos.x, maxPos.y, minPos.z),
                new Vector3(maxPos.x, maxPos.y, maxPos.z),
                new Vector3(maxPos.x, minPos.y, maxPos.z),
            };

            // List of indices/Vector2 between which a line must be made
            Vector2[] indices = new Vector2[12]
            {
                new Vector2(0, 1),
                new Vector2(1, 2),
                new Vector2(2, 3),
                new Vector2(3, 0),

                new Vector2(4, 5),
                new Vector2(5, 6),
                new Vector2(6, 7),
                new Vector2(7, 4),

                new Vector2(0, 4),
                new Vector2(1, 5),
                new Vector2(2, 6),
                new Vector2(3, 7),
            };

            // Loop through each indices to connect the points together
            foreach (Vector2 i in indices)
            {
                GL.Begin(GL.LINES);
                LineMat.color = BoundariesLinesColor;
                LineMat.SetPass(0);
                GL.Color(new Color(LineMat.color.r, LineMat.color.g, LineMat.color.b, LineMat.color.a));
                GL.Vertex3(vertices[(int)i.x].x, vertices[(int)i.x].y, vertices[(int)i.x].z);
                GL.Vertex3(vertices[(int)i.y].x, vertices[(int)i.y].y, vertices[(int)i.y].z);
                GL.End();
            }

            Vector3 labelPos = new Vector3(minPos.x, maxPos.y, maxPos.z);
            GUIStyle style = new GUIStyle();

            style.normal.textColor = BoundariesLinesColor;

            UnityEditor.Handles.Label(labelPos, _teleportScript.GetType().Name + " Boundaries", style);
        }
        #endregion


        // EMPTY
        #region GETTERS_SETTERS

        #endregion
    }
}
#endif