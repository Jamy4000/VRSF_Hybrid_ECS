using ScriptableFramework.RuntimeSet;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ScriptableFramework.Util.Components
{
    /// <summary>
    /// Contains variable to save and load Scriptable Object when Editor or app start and/or quit.
    /// </summary>
    public class ScriptableObjectLoadComponent : MonoBehaviour
    {
        [Tooltip("The List of Scriptable Objects to load.")]
        public List<ScriptableObject> ScriptablesToLoad;

        [Tooltip("If you want to load the Parameters for the ScriptablesToSave when the app Start.")]
        [HideInInspector] public bool LoadScriptableFromJSONOnAppStart = true;
        
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