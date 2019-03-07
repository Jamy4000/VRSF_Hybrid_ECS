using UnityEditor;
using UnityEngine;

namespace VRSF.MoveAround.Fly
{
    /// <summary>
    /// Just add a message to the user to let him know where the parameters are set.
    /// </summary>
    [CustomEditor(typeof(FlyParametersComponent))]
    public class FlyComponentEditor : UnityEditor.Editor
    {
        #region PRIVATE_VARIABLES
        FlyParametersComponent _flyComp;
        #endregion


        #region MONOBEHAVIOUR_METHOD
        /// <summary>
        /// Used to get the Parameters Variable Instance
        /// </summary>
        private void OnEnable()
        {
            _flyComp = (FlyParametersComponent)target;
        }
        #endregion MONOBEHAVIOUR_METHOD


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ShowFlyingParameters();

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("To use the Fly System, you only need to set the parameters of the Fly Components, the ButtonActionChoser Component and the ScriptableRaycast Component.\n" +
                "The response to the buttons you chose are already handled in script.", MessageType.Info);
        }



        /// <summary>
        /// Display the Flying Parameters
        /// </summary>
        private void ShowFlyingParameters()
        {
            Undo.RecordObject(_flyComp, "Option Clicked");
            Undo.RecordObject(this, "Option Clicked");

            // Begin the Vertical for the General Parameters
            GUILayout.BeginVertical();

            EditorGUILayout.LabelField("General Flying Parameters", EditorStyles.boldLabel);

            _flyComp.FlyingSpeed = EditorGUILayout.FloatField("Velocity Factor", _flyComp.FlyingSpeed);
            _flyComp.ChangeSpeedDependingOnHeight = EditorGUILayout.ToggleLeft("Change Speed Depending on User's Height", _flyComp.ChangeSpeedDependingOnHeight);
            _flyComp.ChangeSpeedDependingOnScale = EditorGUILayout.ToggleLeft("Change Speed Depending on User's Scale", _flyComp.ChangeSpeedDependingOnScale);

            GUILayout.EndVertical();

            Undo.FlushUndoRecordObjects();
        }
    }
}