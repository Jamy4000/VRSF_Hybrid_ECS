#if UNITY_EDITOR
using UnityEngine;

namespace VRSF.MoveAround
{
    /// <summary>
    /// Display lines for the Fly Boundaries in Edit Mode
    /// </summary>
    [ExecuteInEditMode]
	public class FlyBoundaries : MonoBehaviour 
	{
        #region PUBLIC_VARIABLES
        // Choose the Unlit/Color shader in the Material Settings
        // You can change that color, to change the color of the connecting lines
        public Material LineMat;
        #endregion

        
        #region PRIVATE_VARIABLES
        FlyingParametersVariable _FlyingParametersVariable;
        #endregion


        #region MONOBEHAVIOUR_METHODS
        /// <summary>
        /// To show the lines in the game window whne it is running
        /// </summary>
        void OnPostRender()
        {
            _FlyingParametersVariable = FlyingParametersVariable.Instance;

            if (_FlyingParametersVariable.UseBoundaries)
            {
                DrawConnectingLines();
            }
        }

        /// <summary>
        /// To show the lines in the editor
        /// </summary>
        void OnDrawGizmos()
        {
            _FlyingParametersVariable = FlyingParametersVariable.Instance;

            if (_FlyingParametersVariable.UseBoundaries)
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
            // List of points/vertices
            Vector3[] vertices = new Vector3[8]
            {
                new Vector3(_FlyingParametersVariable.MinAvatarPosition.x, _FlyingParametersVariable.MinAvatarPosition.y, _FlyingParametersVariable.MinAvatarPosition.z),
                new Vector3(_FlyingParametersVariable.MinAvatarPosition.x, _FlyingParametersVariable.MaxAvatarPosition.y, _FlyingParametersVariable.MinAvatarPosition.z),
                new Vector3(_FlyingParametersVariable.MinAvatarPosition.x, _FlyingParametersVariable.MaxAvatarPosition.y, _FlyingParametersVariable.MaxAvatarPosition.z),
                new Vector3(_FlyingParametersVariable.MinAvatarPosition.x, _FlyingParametersVariable.MinAvatarPosition.y, _FlyingParametersVariable.MaxAvatarPosition.z),
                new Vector3(_FlyingParametersVariable.MaxAvatarPosition.x, _FlyingParametersVariable.MinAvatarPosition.y, _FlyingParametersVariable.MinAvatarPosition.z),
                new Vector3(_FlyingParametersVariable.MaxAvatarPosition.x, _FlyingParametersVariable.MaxAvatarPosition.y, _FlyingParametersVariable.MinAvatarPosition.z),
                new Vector3(_FlyingParametersVariable.MaxAvatarPosition.x, _FlyingParametersVariable.MaxAvatarPosition.y, _FlyingParametersVariable.MaxAvatarPosition.z),
                new Vector3(_FlyingParametersVariable.MaxAvatarPosition.x, _FlyingParametersVariable.MinAvatarPosition.y, _FlyingParametersVariable.MaxAvatarPosition.z),
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
                LineMat.color = _FlyingParametersVariable.FlyingBoundariesColor;
                LineMat.SetPass(0);
                GL.Color(new Color(LineMat.color.r, LineMat.color.g, LineMat.color.b, LineMat.color.a));
                GL.Vertex3(vertices[(int)i.x].x, vertices[(int)i.x].y, vertices[(int)i.x].z);
                GL.Vertex3(vertices[(int)i.y].x, vertices[(int)i.y].y, vertices[(int)i.y].z);
                GL.End();
            }

            Vector3 labelPos = new Vector3(_FlyingParametersVariable.MinAvatarPosition.x, _FlyingParametersVariable.MaxAvatarPosition.y, _FlyingParametersVariable.MinAvatarPosition.z);
            GUIStyle style = new GUIStyle();

            style.normal.textColor = _FlyingParametersVariable.FlyingBoundariesColor;

            UnityEditor.Handles.Label(labelPos, "Flying Boundaries", style);
        }
        #endregion


        // EMPTY
        #region GETTERS_SETTERS

        #endregion
    }
}
#endif