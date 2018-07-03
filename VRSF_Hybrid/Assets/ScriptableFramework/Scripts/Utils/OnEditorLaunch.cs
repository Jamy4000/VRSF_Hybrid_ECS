#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace ScriptableFramework.Util
{
    /// <summary>
    /// Class used to call method when the Editor is launching.
    /// </summary>
    [InitializeOnLoad]
    public class OnEditorLaunch 
	{
        static OnEditorLaunch()
        {
            if (EditorApplication.timeSinceStartup < 5.0f) 
                EditorApplication.update += RunOnce; 
        }

        // EMPTY
        #region PUBLIC_VARIABLES

        #endregion


        #region PRIVATE_VARIABLES
        /// <summary>
        /// Method called one time, when the editor is launching.
        /// </summary>
        private static void RunOnce()
        {
            LoadScriptableObjects();
            EditorApplication.update -= RunOnce;
        }

        /// <summary>
        /// Load the Scriptable objects if the bool in 
        /// </summary>
        private static void LoadScriptableObjects()
        {
            try
            {
                ScriptableObjectsLoader ScriptableLoader = GameObject.FindObjectOfType<ScriptableObjectsLoader>();

                if (ScriptableLoader.LoadOnUnityEditorLaunch)
                {
                    ScriptableLoader.Load();
                }
            } catch
            { 
                // No ScritpableObjectsLoader is present in the scene
            }

        }
        #endregion


        // EMPTY
        #region PUBLIC_METHODS

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