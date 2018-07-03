using UnityEngine;
using UnityEditor;
using VRSF.Gaze;
using VRSF.Controllers;
using VRSF.Inputs;
using UnityEditorInternal;

namespace VRSF.Editor
{
    /// <summary>
    /// Create a new window to set the VR Gaze parameters
    /// </summary>
    public class VRSFGazeWindow : EditorWindow
    {
        // EMPTY
        #region PUBLIC_VARIABLES
        #endregion

        
        #region PRIVATE_VARIABLES
        GazeParametersVariable _gazeParameters;
        ControllersParametersVariable _controllersParameters;
        Vector2 _scrollPosition = Vector2.zero;
        #endregion


        #region MONOBEHAVIOUR_METHOD
        /// <summary>
        /// Used to get the Parameters Variable Instance
        /// </summary>
        private void OnEnable()
        {
            _gazeParameters = GazeParametersVariable.Instance;
            _controllersParameters = ControllersParametersVariable.Instance;
        }
        #endregion MONOBEHAVIOUR_METHOD


        #region PUBLIC_METHODS
        [MenuItem("Window/VRSF/Gaze Parameters")]
        public static void ShowWindow()
        {
            GetWindow<VRSFGazeWindow>("VRSF Gaze Parameters");
        }
        #endregion


        #region PRIVATE_METHODS
        private void OnDestroy()
        {
            EditorUtility.SetDirty(_gazeParameters);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void OnDisable()
        {
            EditorUtility.SetDirty(_gazeParameters);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void OnGUI()
        {
            // Add a Vertical Scrollview
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            // Add a Title
            GUILayout.Label("VRSF Gaze Parameters", EditorStyles.boldLabel);

            // Add UseGaze toggle and record the event for Undo
            Undo.RecordObject(_gazeParameters, "Use Gaze");
            _gazeParameters.UseGaze = EditorGUILayout.Toggle("Use Gaze", _gazeParameters.UseGaze);

            EditorGUILayout.Space();

            // if we use the Gaze, we show the Gaze parameters
            if (_gazeParameters.UseGaze)
            {
                ShowGazeParameters();

                // Add a Reset parameters button for Gaze parameters
                if (GUILayout.Button("Reset Gaze Parameters to default"))
                {
                    Undo.RecordObject(_gazeParameters, "Reset Gaze Parameters");
                    _gazeParameters.ResetParameters();
                }
            }

            GUILayout.EndScrollView();

            Undo.FlushUndoRecordObjects();
        }
        
        /// <summary>
        /// Display the Gaze Parameters
        /// </summary>
        private void ShowGazeParameters()
        {
            Undo.RecordObject(_gazeParameters, "Modify Gaze Parameters");

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (_controllersParameters.UseControllers)
            {
                GUILayout.Label("Gaze Interaction Button for each SDK", EditorStyles.boldLabel);
                GUILayout.Label("If you want to Click and Touch on object with the Gaze, you can here define which button you're gonna use for that.", EditorStyles.miniBoldLabel);
                GUILayout.Label("WARNING : Some buttons are SDK Specific (Example : the A, B, X and Y buttons are only available for the Oculus).", EditorStyles.miniBoldLabel);
                _gazeParameters.GazeButtonOpenVR = (EControllersInput)EditorGUILayout.EnumPopup("Button for OpenVR SDK", _gazeParameters.GazeButtonOpenVR);
                _gazeParameters.GazeButtonOVR = (EControllersInput)EditorGUILayout.EnumPopup("Button for OVR SDK", _gazeParameters.GazeButtonOVR);
                _gazeParameters.GazeButtonSimulator = (EControllersInput)EditorGUILayout.EnumPopup("Button for Simulator SDK", _gazeParameters.GazeButtonSimulator);

                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }

            GUILayout.Label("Gaze Hit Parameters", EditorStyles.boldLabel);

            LayerMask leftTempMask = EditorGUILayout.MaskField("Excluded layer Left", InternalEditorUtility.LayerMaskToConcatenatedLayersMask(_gazeParameters.GazeExclusionLayer), InternalEditorUtility.layers);
            _gazeParameters.GazeExclusionLayer = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(leftTempMask);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
            GUILayout.Label("Gaze Sprites Color", EditorStyles.boldLabel);

            _gazeParameters.UseDifferentStates = EditorGUILayout.Toggle("Use Different States", _gazeParameters.UseDifferentStates);

            if (_gazeParameters.UseDifferentStates)
            {
                EditorGUILayout.Space();

                _gazeParameters.GazePointerState = (EPointerState)EditorGUILayout.EnumPopup("Gaze State", _gazeParameters.GazePointerState);

                EditorGUILayout.Space();

                ShowStatesColor();
            }
            else
            {
                EditorGUILayout.Space();
                _gazeParameters.ReticleColor = EditorGUILayout.ColorField("Color Background Reticle", _gazeParameters.ReticleColor);
                _gazeParameters.ReticleTargetColor = EditorGUILayout.ColorField("Color Target Reticle", _gazeParameters.ReticleTargetColor);
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUILayout.Label("Gaze Transform Parameters", EditorStyles.boldLabel);

            _gazeParameters.UseNormal = EditorGUILayout.Toggle("Use Normals", _gazeParameters.UseNormal);
            _gazeParameters.DefaultDistance = EditorGUILayout.FloatField("Default Reticle Distance from user", _gazeParameters.DefaultDistance);
            _gazeParameters.ReticleSize = EditorGUILayout.Vector3Field("Default Reticle Size", _gazeParameters.ReticleSize);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUILayout.Label("Gaze Image Parameters", EditorStyles.boldLabel);

            _gazeParameters.ReticleSprite = (Sprite)EditorGUILayout.ObjectField("Reticle Sprite", _gazeParameters.ReticleSprite, typeof(Sprite), allowSceneObjects: false);
            _gazeParameters.ReticleTargetSprite = (Sprite)EditorGUILayout.ObjectField("Reticle Target Sprite", _gazeParameters.ReticleTargetSprite, typeof(Sprite), allowSceneObjects: false);

            Undo.FlushUndoRecordObjects();
        }

        /// <summary>
        /// Shoz the different color for the Reticle State. 
        /// </summary>
        private void ShowStatesColor()
        {
            _gazeParameters.ColorOnReticleBackgroud = EditorGUILayout.ColorField("Color On Background Reticle", _gazeParameters.ColorOnReticleBackgroud);
            _gazeParameters.ColorOnReticleTarget = EditorGUILayout.ColorField("Color On Target Reticle", _gazeParameters.ColorOnReticleTarget);

            EditorGUILayout.Space();

            _gazeParameters.ColorOffReticleBackgroud = EditorGUILayout.ColorField("Color Off Background Reticle", _gazeParameters.ColorOffReticleBackgroud);
            _gazeParameters.ColorOffReticleTarget = EditorGUILayout.ColorField("Color Off Target Reticle", _gazeParameters.ColorOffReticleTarget);

            EditorGUILayout.Space();

            _gazeParameters.ColorSelectableReticleBackgroud = EditorGUILayout.ColorField("Color Selectable Background Reticle", _gazeParameters.ColorSelectableReticleBackgroud);
            _gazeParameters.ColorSelectableReticleTarget = EditorGUILayout.ColorField("Color Selectable Target Reticle", _gazeParameters.ColorSelectableReticleTarget);
        }
        #endregion 


        // EMPTY
        #region GETTERS_SETTERS

        #endregion
    }
}