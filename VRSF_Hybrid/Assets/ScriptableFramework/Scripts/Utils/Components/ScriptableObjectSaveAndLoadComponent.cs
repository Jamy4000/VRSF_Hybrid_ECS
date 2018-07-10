using ScriptableFramework.RuntimeSet;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ScriptableFramework.Util.Components
{
    /// <summary>
    /// Contains variable to save and load Scriptable Object when Editor or app start and/or quit.
    /// </summary>
    public class ScriptableObjectSaveAndLoadComponent : MonoBehaviour
    {
        [Tooltip("The List of Scriptable Objects to save.")]
        public List<ScriptableObject> ScriptablesToSave;

        [Tooltip("The List of Scriptable Objects to load.")]
        public List<ScriptableObject> ScriptablesToLoad;

        [Tooltip("If you want to load the Parameters for the ScriptablesToSave when the app Start.")]
        [HideInInspector] public bool LoadScriptableFromJSONOnAppStart = true;
        
        [Tooltip("If you want to save the Parameters for the ScriptablesToSave when the App Stop.")]
        [HideInInspector] public bool SaveScriptableInJSONOnAppQuit = true;

        [Tooltip("If you want to delete the JSON Files that doesn't have a scriptable object linked to them in the list above.")]
        [HideInInspector] public bool RemoveUnusedJSONFiles = true;

        [HideInInspector] public ScriptableToSaveAndLoad ScriptableToSave;
        [HideInInspector] public ScriptableToSaveAndLoad ScriptableToLoad;

        /// <summary>
        /// The name of the scriptable file
        /// </summary>
        [HideInInspector]
        public string JsonPath
        {
            get
            {
                return Path.Combine(Application.streamingAssetsPath, "Saved_Scriptable_Objects");
            }
        }
    }
}