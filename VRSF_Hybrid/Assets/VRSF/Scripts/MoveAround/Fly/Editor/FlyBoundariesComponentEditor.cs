using UnityEditor;
using UnityEngine;

namespace VRSF.MoveAround.Fly
{
    /// <summary>
    /// Just add a message to the user to let him know where the parameters are set.
    /// </summary>
    [CustomEditor(typeof(FlyBoundariesComponent))]
    public class FlyBoundariesComponentEditor : UnityEditor.Editor
    {
        #region PRIVATE_VARIABLES
        FlyBoundariesComponent _flyComp;
        #endregion


        #region MONOBEHAVIOUR_METHOD
        /// <summary>
        /// Used to get the Parameters Variable Instance
        /// </summary>
        private void OnEnable()
        {
            _flyComp = (FlyBoundariesComponent)target;
        }
        #endregion MONOBEHAVIOUR_METHOD


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ShowFlyingParameters();
        }



        /// <summary>
        /// Display the Flying Parameters
        /// </summary>
        private void ShowFlyingParameters()
        {
            Undo.RecordObject(_flyComp, "Option Clicked");
            Undo.RecordObject(this, "Option Clicked");

            // Begin the Vertical for the Flying mode Boundaries
            GUILayout.BeginVertical();
            
            _flyComp.UseHorizontalBoundaries = EditorGUILayout.ToggleLeft("Use Flying Boundaries for X and Z axis", _flyComp.UseHorizontalBoundaries);

            if (_flyComp.UseHorizontalBoundaries)
            {
                _flyComp.MinAvatarPosition = EditorGUILayout.Vector3Field("Minimum Position in Scene", _flyComp.MinAvatarPosition);
                _flyComp.MaxAvatarPosition = EditorGUILayout.Vector3Field("Maximum Position in Scene", _flyComp.MaxAvatarPosition);
            }
            else
            {
                _flyComp.MinAvatarYPosition = EditorGUILayout.FloatField("Minimum Position in Y", _flyComp.MinAvatarYPosition);
                _flyComp.MaxAvatarYPosition = EditorGUILayout.FloatField("Maximum Position in Y", _flyComp.MaxAvatarYPosition);
            }

            GUILayout.EndVertical();
            Undo.FlushUndoRecordObjects();
        }
    }
}