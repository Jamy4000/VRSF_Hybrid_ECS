#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using VRSF.Controllers;
using UnityEditorInternal;

namespace VRSF.Editor
{
    /// <summary>
    /// Create a new window to set the VR Controllers parameters
    /// </summary>
    public class VRSFControllersWindow : EditorWindow
    {
        // EMPTY
        #region PUBLIC_VARIABLES
        #endregion


        #region PRIVATE_VARIABLES
        private ControllersParametersVariable _controllersParameters;
        private Vector2 _scrollPosition = Vector2.zero;
        #endregion


        #region MONOBEHAVIOUR_METHOD
        /// <summary>
        /// Used to get the Parameters Variable Instance
        /// </summary>
        private void OnEnable()
        {
            _controllersParameters = ControllersParametersVariable.Instance;
        }
        #endregion MONOBEHAVIOUR_METHOD


        #region PUBLIC_METHODS
        [MenuItem("Window/VRSF/Controllers Parameters")]
        public static void ShowWindow()
        {
            GetWindow<VRSFControllersWindow>("VRSF Controllers Parameters");
        }
        #endregion


        #region PRIVATE_METHODS
        private void OnDestroy()
        {
            EditorUtility.SetDirty(_controllersParameters);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void OnDisable()
        {
            EditorUtility.SetDirty(_controllersParameters);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void OnGUI()
        {
            // Add a Vertical Scrollview
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            // Add a Title
            GUILayout.Label("VRSF Controllers Parameters", EditorStyles.boldLabel);


            // Add UseController toggle and record the event for Undo
            Undo.RecordObject(_controllersParameters, "Use Controller");
            _controllersParameters.UseControllers = EditorGUILayout.Toggle("Use Controllers", _controllersParameters.UseControllers);

            EditorGUILayout.Space();

            // if we use the controllers, we show the controllers parameters
            if (_controllersParameters.UseControllers)
            {
                ShowControllersParameters();

                // Add a Reset parameters button for controller parameters
                if (GUILayout.Button("Reset Controllers Parameters to default"))
                {
                    Undo.RecordObject(_controllersParameters, "Reset Controllers Parameters");
                    _controllersParameters.ResetParameters();
                }
            }

            GUILayout.EndScrollView();

            Undo.FlushUndoRecordObjects();
        }

        /// <summary>
        /// Display the Controllers Parameters
        /// </summary>
        private void ShowControllersParameters()
        {
            EditorGUILayout.Space();

            // Begin the Vertical for the Left Controller Parameters
            GUILayout.BeginVertical();

            GUILayout.Label("Left Controller Parameters", EditorStyles.boldLabel);

            Undo.RecordObject(_controllersParameters, "Use pointer Left");
            _controllersParameters.UsePointerLeft = EditorGUILayout.Toggle("Use Left Pointer", _controllersParameters.UsePointerLeft);

            if (_controllersParameters.UsePointerLeft)
            {
                ShowPointerParameters(EHand.LEFT);
            }

            GUILayout.EndVertical();

            EditorGUILayout.Space();

            // Begin the Vertical for the Right Controller Parameters
            GUILayout.BeginVertical();

            GUILayout.Label("Right Controller Parameters", EditorStyles.boldLabel);

            Undo.RecordObject(_controllersParameters, "Use pointer Right");
            _controllersParameters.UsePointerRight = EditorGUILayout.Toggle("Use Right Pointer", _controllersParameters.UsePointerRight);

            if (_controllersParameters.UsePointerRight)
            {
                ShowPointerParameters(EHand.RIGHT);
            }

            GUILayout.EndVertical();

            Undo.FlushUndoRecordObjects();
        }

        /// <summary>
        /// Display the Controllers Pointer Parameters
        /// </summary>
        private void ShowPointerParameters(EHand hand)
        {
            Undo.RecordObject(_controllersParameters, "Modifiying pointer");

            switch (hand)
            {
                case (EHand.RIGHT):
                    _controllersParameters.RightPointerState = (EPointerState)EditorGUILayout.EnumPopup("Right Pointer State", _controllersParameters.RightPointerState);

                    LayerMask rightTempMask = EditorGUILayout.MaskField("Excluded layer Right", InternalEditorUtility.LayerMaskToConcatenatedLayersMask(_controllersParameters.RightExclusionLayer), InternalEditorUtility.layers);
                    _controllersParameters.RightExclusionLayer = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(rightTempMask);

                    _controllersParameters.ColorMatOnRight = EditorGUILayout.ColorField("Color On Right", _controllersParameters.ColorMatOnRight);
                    _controllersParameters.ColorMatOffRight = EditorGUILayout.ColorField("Color Off Right", _controllersParameters.ColorMatOffRight);
                    _controllersParameters.ColorMatSelectableRight = EditorGUILayout.ColorField("Color Selectable Right", _controllersParameters.ColorMatSelectableRight);

                    _controllersParameters.MaxDistancePointerRight = EditorGUILayout.FloatField("Max Distance Right Pointer", _controllersParameters.MaxDistancePointerRight);
                    break;

                case (EHand.LEFT):
                    _controllersParameters.LeftPointerState = (EPointerState)EditorGUILayout.EnumPopup("Left Pointer State", _controllersParameters.LeftPointerState);

                    LayerMask leftTempMask = EditorGUILayout.MaskField("Excluded layer Left", InternalEditorUtility.LayerMaskToConcatenatedLayersMask(_controllersParameters.LeftExclusionLayer), InternalEditorUtility.layers);
                    _controllersParameters.LeftExclusionLayer = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(leftTempMask);

                    _controllersParameters.ColorMatOnLeft = EditorGUILayout.ColorField("Color On Left", _controllersParameters.ColorMatOnLeft);
                    _controllersParameters.ColorMatOffLeft = EditorGUILayout.ColorField("Color Off Left", _controllersParameters.ColorMatOffLeft);
                    _controllersParameters.ColorMatSelectableLeft = EditorGUILayout.ColorField("Color Selectable Left", _controllersParameters.ColorMatSelectableLeft);

                    _controllersParameters.MaxDistancePointerLeft = EditorGUILayout.FloatField("Max Distance Left Pointer", _controllersParameters.MaxDistancePointerLeft);
                    break;

                default:
                    Debug.LogError("Error in ShowPointerParameters, the hand wasn't recognized.");
                    break;
            }

            Undo.FlushUndoRecordObjects();
        }
        #endregion


        // EMPTY
        #region GETTERS_SETTERS

        #endregion
    }
}
#endif