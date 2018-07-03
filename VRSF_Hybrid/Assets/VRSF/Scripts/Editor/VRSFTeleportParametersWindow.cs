using UnityEngine;
using UnityEditor;

namespace VRSF.MoveAround.Teleport.Editor
{
    /// <summary>
    /// Create a new window to set the parameters for the Teleport feature
    /// </summary>
    public class VRSFTeleportParametersWindow : EditorWindow
    {
        // EMPTY
        #region PUBLIC_VARIABLES
        #endregion

        
        #region PRIVATE_VARIABLES
        TeleportParametersVariable _TeleportParameters; 

        Vector2 _ScrollPosition = Vector2.zero;
        #endregion


        #region MONOBEHAVIOUR_METHOD
        /// <summary>
        /// Used to get the Parameters Variable Instance
        /// </summary>
        private void OnEnable()
        {
            _TeleportParameters = TeleportParametersVariable.Instance;
        }
        #endregion MONOBEHAVIOUR_METHOD


        #region PUBLIC_METHODS
        [MenuItem("Window/VRSF/Teleport Parameters")]
        public static void ShowWindow()
        {
            GetWindow<VRSFTeleportParametersWindow>("VRSF Teleport Parameters");
        }
        #endregion


        #region PRIVATE_METHODS
        private void OnDestroy()
        {
            EditorUtility.SetDirty(_TeleportParameters);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void OnDisable()
        {
            EditorUtility.SetDirty(_TeleportParameters);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void OnGUI()
        {
            // Add a Vertical Scrollview
            _ScrollPosition = GUILayout.BeginScrollView(_ScrollPosition);

            // Add a Title
            GUILayout.Label("VRSF Teleport Parameters", EditorStyles.boldLabel);

            // Add UseTeleportFeature toggle and record the event for Undo
            Undo.RecordObject(_TeleportParameters, "Use Teleport Feature");
            Undo.RecordObject(this, "Use Teleport Feature");
            
            // if we use the controllers, we show the controllers parameters
            ShowTeleportParameters();
            EditorGUILayout.HelpBox("These values will modify the parameters of the Teleport scripts. Don't forget to add it to at least one GameObject.\n" +
                "For more info, you can check the wiki section on the GitHub Repository and the tooltips in the TeleportParameters Scriptable Object in the Resources Folder.", MessageType.Warning);


            // Add a Reset all parameters button
            if (GUILayout.Button("Reset All Parameters to Default"))
            {
                Undo.RecordObject(_TeleportParameters, "Reset Teleport Parameters");
                Undo.RecordObject(this, "Reset Teleport Parameters");
                _TeleportParameters.ResetParameters();
            }

            GUILayout.EndScrollView();

            Undo.FlushUndoRecordObjects();
        }

        /// <summary>
        /// Display the Teleport Parameters
        /// </summary>
        private void ShowTeleportParameters()
        {
            EditorGUILayout.Space();

            // Begin the Vertical for the Teleport Far Away Parameters
            GUILayout.BeginVertical();

            GUILayout.Label("Teleport Far Away Parameters", EditorStyles.boldLabel);
            _TeleportParameters.TimerBeforeTeleport = EditorGUILayout.FloatField("Time Before Teleport", _TeleportParameters.TimerBeforeTeleport);
            _TeleportParameters.AdjustHeight = EditorGUILayout.Toggle("Adjust User Height", _TeleportParameters.AdjustHeight);
            _TeleportParameters.HeightAboveGround = EditorGUILayout.FloatField("Height to Adjust", _TeleportParameters.HeightAboveGround);

            GUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // Begin the Vertical for the Step by step Teleport
            GUILayout.BeginVertical();

            GUILayout.Label("Bezier Teleportation", EditorStyles.boldLabel);

            Undo.RecordObject(_TeleportParameters, "bezier");
            Undo.RecordObject(this, "bezier");
            
            _TeleportParameters.BezierAngle = EditorGUILayout.FloatField("Bezier Angle", _TeleportParameters.BezierAngle);
            _TeleportParameters.BezierStrength = EditorGUILayout.FloatField("Bezier Strength", _TeleportParameters.BezierStrength);
            _TeleportParameters.ColorBezierHitting = EditorGUILayout.ColorField("Color Hitting", _TeleportParameters.ColorBezierHitting);
            _TeleportParameters.ColorBezierNotHitting = EditorGUILayout.ColorField("Color Not Hitting", _TeleportParameters.ColorBezierNotHitting);

            GUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // Begin the Vertical for the Step by step Teleport
            GUILayout.BeginVertical();

            GUILayout.Label("Step by step Teleport", EditorStyles.boldLabel);

            Undo.RecordObject(_TeleportParameters, "step by step");
            Undo.RecordObject(this, "step by step");
            
            _TeleportParameters.DistanceStepByStep = EditorGUILayout.FloatField("Distance Step by Step Teleport", _TeleportParameters.DistanceStepByStep);
            _TeleportParameters.MoveOnVerticalAxis = EditorGUILayout.Toggle("Move on the vertical axis", _TeleportParameters.MoveOnVerticalAxis);
            
            GUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // Begin the Vertical for the Flying mode Boundaries
            GUILayout.BeginVertical(); 

            GUILayout.Label("Teleport Mode Boundaries", EditorStyles.boldLabel);

            _TeleportParameters.UseBoundaries = EditorGUILayout.Toggle("Use Teleport Boundaries", _TeleportParameters.UseBoundaries);

            if (_TeleportParameters.UseBoundaries)
            {
                _TeleportParameters.MinAvatarPosition = EditorGUILayout.Vector3Field("Minimum Position in Scene", _TeleportParameters.MinAvatarPosition);
                _TeleportParameters.MaxAvatarPosition = EditorGUILayout.Vector3Field("Maximum Position in Scene", _TeleportParameters.MaxAvatarPosition);
                _TeleportParameters.TeleportingBoundariesColor = EditorGUILayout.ColorField("Boundaries color", _TeleportParameters.TeleportingBoundariesColor);
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