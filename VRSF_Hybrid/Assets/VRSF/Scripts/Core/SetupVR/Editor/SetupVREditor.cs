#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VRSF.Core.SetupVR
{
    /// <summary>
    /// Script to add some Editor feature for the SetupVR GameObject.
    /// </summary>
    [CustomEditor(typeof(SetupVRComponents), true)]
    public class SetupVREditor : UnityEditor.Editor
	{
        #region PRIVATE_VARIABLES
        private static GameObject _setupVRPrefab;

        private SerializedProperty _currentAxisArray;
        private SerializedProperty _vrsfAxisArray;
        private SerializedObject _currentInputObj;
        #endregion

        #region PUBLIC_METHODS
        public override void OnInspectorGUI()
        {
            var currentInputManager = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0];
            _currentInputObj = new SerializedObject(currentInputManager);
            _currentAxisArray = _currentInputObj.FindProperty("m_Axes");

            var vrsfInputManager = AssetDatabase.LoadAllAssetsAtPath("Assets/Resources/VRSF/InputManager.asset")[0];
            SerializedObject vrsfInputObj = new SerializedObject(vrsfInputManager);
            _vrsfAxisArray = vrsfInputObj.FindProperty("m_Axes");

            if (InputManagerCopier.InputArrayIsNotVRSF(_currentAxisArray, _vrsfAxisArray))
            {
                EditorGUILayout.HelpBox("The current InputManager is not set as the one required for VRSF. Click the button below to set them automatically.", MessageType.Warning);

                EditorGUILayout.Space();

                if (GUILayout.Button("Set InputManager"))
                {
                    if (InputManagerCopier.SetInputManager(_currentInputObj, _vrsfAxisArray))
                    {
                        _currentInputObj = null;
                        _currentAxisArray = null;
                        _vrsfAxisArray = null;
                    }
                }

                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }

            base.DrawDefaultInspector();
        }
        #endregion PUBLIC_METHODS

        #region PRIVATE_METHODS
        private void OnEnable()
        {
        }

        /// <summary>
        /// Add the SetupVR Prefab to the scene.
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("GameObject/VRSF/Add SetupVR to Scene", priority = 0)]
        [MenuItem("VRSF/Add SetupVR to Scene", priority = 0)]
        private static void InstantiateSetupVR(MenuCommand menuCommand)
        {
            if (GameObject.FindObjectOfType<SetupVRComponents>() != null)
            {
                Debug.LogError("VRSF : SetupVR is already present in the scene.\n" +
                    "If multiple instance of this object are placed in the same scene, you will encounter conflict problems.");
                return;
            }

            _setupVRPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/VRSF/Prefabs/Core/SetupVR.prefab");

            // Create a custom game object
            GameObject setupVR = PrefabUtility.InstantiatePrefab(_setupVRPrefab) as GameObject;

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(setupVR, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(setupVR, "Create " + setupVR.name);
            Selection.activeObject = setupVR;
        }
        #endregion

        // EMPTY
        #region GETTERS_SETTERS

        #endregion
    }
}
#endif