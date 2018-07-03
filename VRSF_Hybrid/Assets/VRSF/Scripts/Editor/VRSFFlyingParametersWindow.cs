using UnityEngine;
using UnityEditor;
using VRSF.MoveAround;

namespace VRSF.Editor
{
    /// <summary>
    /// Create a new window to set the parameters for the Flying mode
    /// </summary>
    public class VRSFFlyingParametersWindow : EditorWindow
    {
        // EMPTY
        #region PUBLIC_VARIABLES
        #endregion


        #region PRIVATE_VARIABLES
        FlyingParametersVariable _FlyingParameters;

        Vector2 _ScrollPosition = Vector2.zero;
        #endregion


        #region MONOBEHAVIOUR_METHOD
        /// <summary>
        /// Used to get the Parameters Variable Instance
        /// </summary>
        private void OnEnable()
        {
            _FlyingParameters = FlyingParametersVariable.Instance;
        }
        #endregion MONOBEHAVIOUR_METHOD


        #region PUBLIC_METHODS
        [MenuItem("Window/VRSF/Flying Parameters")]
        public static void ShowWindow()
        {
            GetWindow<VRSFFlyingParametersWindow>("VRSF Flying Parameters");
        }
        #endregion


        #region PRIVATE_METHODS
        private void OnDestroy()
        {
            EditorUtility.SetDirty(_FlyingParameters);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void OnDisable()
        {
            EditorUtility.SetDirty(_FlyingParameters);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void OnGUI()
        {
            // Add a Vertical Scrollview
            _ScrollPosition = GUILayout.BeginScrollView(_ScrollPosition);

            // Add a Title
            GUILayout.Label("VRSF Flying Parameters", EditorStyles.boldLabel);


            // Record the event for Undo
            Undo.RecordObject(_FlyingParameters, "Flying Parameters");

            EditorGUILayout.HelpBox("These values will modify the parameters of the Fly scripts. Don't forget to add it to at least one GameObject.\n" +
                "For more info, you can check the wiki section on the GitHub Repository and the tooltips in the FlyingParameters Scriptable Object in the Resources Folder.", MessageType.Warning);

            ShowFlyingParameters();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // Add a Reset all parameters button
            if (GUILayout.Button("Reset All Parameters to Default"))
            {
                Undo.RecordObject(_FlyingParameters, "Reset Flying Parameters");
                _FlyingParameters.ResetParameters();
            }

            GUILayout.EndScrollView();

            Undo.FlushUndoRecordObjects();
        }

        /// <summary>
        /// Display the Flying Parameters
        /// </summary>
        private void ShowFlyingParameters()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("General Flying Parameters", EditorStyles.boldLabel);

            Undo.RecordObject(_FlyingParameters, "Option Clicked");
            Undo.RecordObject(this, "Option Clicked");

            // Begin the Vertical for the General Parameters
            GUILayout.BeginVertical();

            _FlyingParameters.FlyingSpeed = EditorGUILayout.FloatField("Velocity Factor", _FlyingParameters.FlyingSpeed);
            _FlyingParameters.ChangeSpeedDependingOnHeight = EditorGUILayout.ToggleLeft("Change Speed Depending on User's Height", _FlyingParameters.ChangeSpeedDependingOnHeight);
            _FlyingParameters.ChangeSpeedDependingOnScale = EditorGUILayout.ToggleLeft("Change Speed Depending on User's Scale", _FlyingParameters.ChangeSpeedDependingOnScale);

            GUILayout.EndVertical();

            EditorGUILayout.Space();

            // Begin the Vertical for the Sliding Effect Parameters
            GUILayout.BeginVertical();

            _FlyingParameters.AccelerationDecelerationEffect = EditorGUILayout.ToggleLeft("Use Acceleration and Deceleration", _FlyingParameters.AccelerationDecelerationEffect);
            if (_FlyingParameters.AccelerationDecelerationEffect)
            {
                EditorGUILayout.LabelField("The bigger the values are, the longer the effect will take place.", EditorStyles.miniBoldLabel);
                EditorGUILayout.Space();

                _FlyingParameters.AccelerationEffectFactor = EditorGUILayout.FloatField("Acceleration Factor", _FlyingParameters.AccelerationEffectFactor);
                _FlyingParameters.DecelerationEffectFactor = EditorGUILayout.FloatField("Deceleration Factor", _FlyingParameters.DecelerationEffectFactor);
            }

            GUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // Begin the Vertical for the Flying mode Boundaries
            GUILayout.BeginVertical();

            GUILayout.Label("Flying Mode Boundaries", EditorStyles.boldLabel);

            _FlyingParameters.UseBoundaries = EditorGUILayout.ToggleLeft("Use Flying Boundaries for X and Z axis", _FlyingParameters.UseBoundaries);

            if (_FlyingParameters.UseBoundaries)
            {
                _FlyingParameters.MinAvatarPosition = EditorGUILayout.Vector3Field("Minimum Position in Scene", _FlyingParameters.MinAvatarPosition);
                _FlyingParameters.MaxAvatarPosition = EditorGUILayout.Vector3Field("Maximum Position in Scene", _FlyingParameters.MaxAvatarPosition);
                _FlyingParameters.FlyingBoundariesColor = EditorGUILayout.ColorField("Boundaries color", _FlyingParameters.FlyingBoundariesColor);
            }
            else
            {
                _FlyingParameters.MinAvatarYPosition = EditorGUILayout.FloatField("Minimum Position in Y", _FlyingParameters.MinAvatarYPosition);
                _FlyingParameters.MaxAvatarYPosition = EditorGUILayout.FloatField("Maximum Position in Y", _FlyingParameters.MaxAvatarYPosition);
            }

            GUILayout.EndVertical();
            Undo.FlushUndoRecordObjects();
        }
        #endregion


        // EMPTY
        #region GETTERS_SETTERS

        #endregion
    }
}