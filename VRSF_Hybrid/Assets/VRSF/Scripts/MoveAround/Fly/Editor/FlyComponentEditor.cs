using UnityEditor;
using UnityEngine;
using VRSF.MoveAround.Components;
using VRSF.Utils.Editor;

namespace VRSF.MoveAround.Editor
{
    /// <summary>
    /// Just add a message to the user to let him know where the parameters are set.
    /// </summary>
    [CustomEditor(typeof(FlyComponent))]
    public class FlyComponentEditor : UnityEditor.Editor
    {
        #region PRIVATE_VARIABLES
        FlyComponent _flyComp;
        #endregion


        #region MONOBEHAVIOUR_METHOD
        /// <summary>
        /// Used to get the Parameters Variable Instance
        /// </summary>
        private void OnEnable()
        {
            _flyComp = (FlyComponent)target;
        }
        #endregion MONOBEHAVIOUR_METHOD


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ShowFlyingParameters();

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("To use the Fly System, you only need to set the parameters of the FlyComponent and of the ButtonActionChoserComponents.\n" +
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

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // Begin the Vertical for the Sliding Effect Parameters
            GUILayout.BeginVertical();

            EditorGUILayout.LabelField("Acceleration/Deceleration Parameters", EditorStyles.boldLabel);

            _flyComp.AccelerationDecelerationEffect = EditorGUILayout.ToggleLeft("Use Acceleration and Deceleration", _flyComp.AccelerationDecelerationEffect);
            if (_flyComp.AccelerationDecelerationEffect)
            {
                EditorGUILayout.LabelField("The bigger the values are, the longer the effect will take place.", EditorStyles.miniBoldLabel);
                EditorGUILayout.Space();

                _flyComp.AccelerationEffectFactor = EditorGUILayout.FloatField("Acceleration Factor", _flyComp.AccelerationEffectFactor);
                _flyComp.DecelerationEffectFactor = EditorGUILayout.FloatField("Deceleration Factor", _flyComp.DecelerationEffectFactor);
            }

            GUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // Begin the Vertical for the Flying mode Boundaries
            GUILayout.BeginVertical();

            GUILayout.Label("Flying Mode Boundaries", EditorStyles.boldLabel);

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