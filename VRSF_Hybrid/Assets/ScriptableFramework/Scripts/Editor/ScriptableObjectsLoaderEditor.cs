#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ScriptableFramework.Util.Editor
{
    /// <summary>
    /// Add a button to save and load the Scriptable Objects in the lists and to check the JSON Files.
    /// </summary>
    [CustomEditor(typeof(ScriptableObjectsLoader), true)]
    public class ScriptableObjectsLoaderEditor : UnityEditor.Editor
    {
        // EMPTY
        #region PUBLIC_VARIABLES

        #endregion


        // EMPTY
        #region PRIVATE_VARIABLES

        #endregion


        #region PUBLIC_METHODS
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            ScriptableObjectsLoader sol = (ScriptableObjectsLoader)target;

            if (GUILayout.Button("Check JSON Files"))
            {
                sol.CheckJSONFiles();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Save Scriptable Objects"))
            {
                sol.Save();
            }

            EditorGUILayout.Space();
            
            if (GUILayout.Button("Load Scriptable Objects"))
            {
                sol.Load();
            }
        }
        #endregion


        // EMPTY
        #region PRIVATE_METHODS

        #endregion


        // EMPTY
        #region GETTERS_SETTERS

        #endregion
    }
}
#endif