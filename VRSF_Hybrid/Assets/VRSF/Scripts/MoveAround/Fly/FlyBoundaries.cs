#if UNITY_EDITOR
using UnityEngine;

namespace VRSF.MoveAround
{
    /// <summary>
    /// Display lines for the Fly Boundaries in Edit Mode.
    /// 
    /// As it's using OnPostRender and OnDrawGizmos methods, derived from monoBehaviour, 
    /// and it's only a UnityEditor Script, this wasn't refactored to fit the Hybrid System.
    /// </summary>
    [ExecuteInEditMode]
	public class FlyBoundaries : MonoBehaviour 
	{
        #region PUBLIC_VARIABLES
        [Header("Boundaries Color Material")]
        [Tooltip("Choose the Unlit/Color shader in the Material Settings. You can change the color of the connecting lines through this mat.")]
        public Material LineMat;
        #endregion

        
        #region PRIVATE_VARIABLES
        private FlyingParametersVariable _flyingParams;
        #endregion


        #region MONOBEHAVIOUR_METHODS
        /// <summary>
        /// To show the lines in the game window whne it is running
        /// </summary>
        void OnPostRender()
        {
            _flyingParams = FlyingParametersVariable.Instance;

            if (_flyingParams.UseBoundaries)
            {
                DrawConnectingLines();
            }
        }

        /// <summary>
        /// To show the lines in the editor
        /// </summary>
        void OnDrawGizmos()
        {
            _flyingParams = FlyingParametersVariable.Instance;

            if (_flyingParams.UseBoundaries)
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
                new Vector3(_flyingParams.MinAvatarPosition.x, _flyingParams.MinAvatarPosition.y, _flyingParams.MinAvatarPosition.z),
                new Vector3(_flyingParams.MinAvatarPosition.x, _flyingParams.MaxAvatarPosition.y, _flyingParams.MinAvatarPosition.z),
                new Vector3(_flyingParams.MinAvatarPosition.x, _flyingParams.MaxAvatarPosition.y, _flyingParams.MaxAvatarPosition.z),
                new Vector3(_flyingParams.MinAvatarPosition.x, _flyingParams.MinAvatarPosition.y, _flyingParams.MaxAvatarPosition.z),
                new Vector3(_flyingParams.MaxAvatarPosition.x, _flyingParams.MinAvatarPosition.y, _flyingParams.MinAvatarPosition.z),
                new Vector3(_flyingParams.MaxAvatarPosition.x, _flyingParams.MaxAvatarPosition.y, _flyingParams.MinAvatarPosition.z),
                new Vector3(_flyingParams.MaxAvatarPosition.x, _flyingParams.MaxAvatarPosition.y, _flyingParams.MaxAvatarPosition.z),
                new Vector3(_flyingParams.MaxAvatarPosition.x, _flyingParams.MinAvatarPosition.y, _flyingParams.MaxAvatarPosition.z),
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
                LineMat.color = _flyingParams.FlyingBoundariesColor;
                LineMat.SetPass(0);
                GL.Color(new Color(LineMat.color.r, LineMat.color.g, LineMat.color.b, LineMat.color.a));
                GL.Vertex3(vertices[(int)i.x].x, vertices[(int)i.x].y, vertices[(int)i.x].z);
                GL.Vertex3(vertices[(int)i.y].x, vertices[(int)i.y].y, vertices[(int)i.y].z);
                GL.End();
            }

            Vector3 labelPos = new Vector3(_flyingParams.MinAvatarPosition.x, _flyingParams.MaxAvatarPosition.y, _flyingParams.MinAvatarPosition.z);
            GUIStyle style = new GUIStyle();

            style.normal.textColor = _flyingParams.FlyingBoundariesColor;

            UnityEditor.Handles.Label(labelPos, "Flying Boundaries", style);
        }
        #endregion


        // EMPTY
        #region GETTERS_SETTERS

        #endregion
    }
}
#endif