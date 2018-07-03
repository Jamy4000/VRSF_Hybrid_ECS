using System.IO;
using UnityEngine;
using System.Collections.Generic;
using ScriptableFramework.RuntimeSet;
using System.Threading;
using System.Collections;

namespace ScriptableFramework.Util
{
    /// <summary>
    /// Save and Load the Scriptable object from JSON Files, which are saved in the StreamingAsset folder
    /// </summary>
    public class ScriptableObjectsLoader : MonoBehaviour
	{
        #region PUBLIC_VARIABLES
        [Tooltip("The List of Scriptable Objects to save / load.")]
        public List<ScriptableObject> ScriptablesToSaveAndLoad;

        [Tooltip("If you want to load the Parameters for the ScriptablesToSave when the app Start.")]
        public bool LoadOnAppStart = true;

        [Tooltip("If you want to load the Parameters for the ScriptablesToSave when Unity Editor Start.")]
        public bool LoadOnUnityEditorLaunch = true;

        [Tooltip("If you want to save the Parameters for the ScriptablesToSave when the App Stop.")]
        public bool SaveOnAppQuit = true;

        [Tooltip("If you want to delete the JSON Files that doesn't have a scriptable object linked to them in the list above.")]
        public bool RemoveUnusedJSONFiles = true;

        [HideInInspector] public ScriptableToSave ScriptableToSave;
        #endregion


        #region PRIVATE_VARIABLES
        /// <summary>
        /// The name of the scriptable file
        /// </summary>
        private string JsonPath
        {
            get
            {
                return Path.Combine(Application.streamingAssetsPath, "Saved_Scriptable_Objects");
            }
        }
        #endregion


        #region MONOBEHAVIOUR_METHODS
        void Start()
        {
            if (LoadOnAppStart)
            {
                Load();
            }
        }
        
        void OnValidate()
        {
            CheckJSONFiles();

#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                StartCoroutine(RefreshAssets());
            }
#endif
        }

        private void OnApplicationQuit()
        {
            if (SaveOnAppQuit)
            {
                Save();
            }
        }
        #endregion


        #region PUBLIC_METHODS
        /// <summary>
        /// Check if the JSON Files exists foreach ScriptableObject that are in the public list. If not, create a new JSON File.
        /// Check as well if we need to remove Unused JSON Files.
        /// </summary>
        public void CheckJSONFiles()
        {
            // Empty the ScriptableToSave RuntimeSet
            ScriptableToSave.Items.Clear();

            // Feed it with the new values
            FeedRuntimeSets();
            
            // Created the Directory at the JsonPath if it doesn't exist
            if (!Directory.Exists(JsonPath))
            {
                Directory.CreateDirectory(JsonPath);
            }

            // Check if we need to remove unused json files
            if (RemoveUnusedJSONFiles)
            {
                CheckJSONFilesToRemove();
            }

            // Create all of the json files if they doen't exists.
            for (int i = 0; i < ScriptableToSave.Items.Count; i++)
            {
                if (ScriptableToSave.Items[i] != null)
                {
                    string name = ScriptableToSave.Items[i].name + ".json";
                    string path = Path.Combine(JsonPath, name);

                    if (!File.Exists(path))
                    {
                        File.Create(path);
                    }
                }
            }
        }

        /// <summary>
        /// Load the JsonFiles and overwrite the scriptable objects in the public list.
        /// </summary>
        public void Load()
        {
            CheckJSONFiles();

            for (int i = 0; i < ScriptableToSave.Items.Count; i++)
            {
                if (ScriptableToSave.Items[i] != null)
                {
                    string name = ScriptableToSave.Items[i].name + ".json";

                    try
                    {
                        string path = Path.Combine(JsonPath, name);
                        string json = File.ReadAllText(path);
                        JsonUtility.FromJsonOverwrite(json, ScriptableToSave.Items[i]);
                    }
                    catch
                    {
                        Debug.LogError("One of the json file didn't exist. Be sure that you saved this file before trying to load it.\n" +
                            "Problematic file : " + name);
                    }
                }
            }

            Debug.Log("Scriptable Files loaded correctly.");
        }

        /// <summary>
        /// Save the JsonFiles with the current status of the Scriptable Object in the public list.
        /// </summary>
        public void Save()
        {
            CheckJSONFiles();

            foreach (ScriptableObject so in ScriptableToSave.Items)
            {
                if (so != null)
                {
                    SaveAssetStatus(so);
                }
            }

            Debug.Log("Scriptable Files saved correctly.");
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Feed the runtime set ScriptableToSave with the Scriptable objects in ScriptablesToSaveAndLoad
        /// This avoid an error when trying to acces the object of the public list on App Quit.
        /// </summary>
        void FeedRuntimeSets()
        {
            if (ScriptablesToSaveAndLoad != null)
            {
                foreach (ScriptableObject so in ScriptablesToSaveAndLoad)
                {
                    if (!ScriptableToSave.Items.Contains(so))
                        ScriptableToSave.Add(so);
                }
            }
        }
        
        /// <summary>
        /// Save the Scriptable Object status in its JSON File.
        /// </summary>
        /// <param name="toSave">The Scriptable to save</param>
        void SaveAssetStatus(ScriptableObject toSave)
        {
            string fileName = toSave.name + ".json";
            string path = Path.Combine(JsonPath, fileName);
            File.WriteAllText(path, JsonUtility.ToJson(toSave));
        }

#if UNITY_EDITOR
        /// <summary>
        /// Method called to refresh the Assets seen in the editor after creating or deleting the new JSON Files
        /// We use a coroutine as the refresh must be done a little bit after creating the files, and not directly at the next frame.
        /// </summary>
        IEnumerator RefreshAssets()
        {
            yield return new WaitForSeconds(1.0f);
            UnityEditor.AssetDatabase.Refresh();
        }
#endif

        /// <summary>
        /// If the RemoveUnusedJSONFiles bool is at true, check all Json Files and compare them to the ScriptableToSave Items.
        /// If one files doesn't have a correpsonding ScriptableObject in the RuntimeSet, we delete it.
        /// </summary>
        void CheckJSONFilesToRemove()
        {
            string[] jSONfiles = Directory.GetFiles(JsonPath, "*.json");

            if (ScriptableToSave != null && jSONfiles.Length != ScriptableToSave.Items.Count)
            {
                foreach (string json in jSONfiles)
                {
                    bool jsonNotUsed = true;

                    foreach (ScriptableObject so in ScriptableToSave.Items)
                    {
                        if (so != null && json.Contains(so.name))
                        {
                            jsonNotUsed = false;
                            break;
                        }
                    }

                    if (jsonNotUsed)
                    {
                        File.Delete(json);
                    }
                }
            }
        }
        #endregion


        // EMPTY
        #region GETTERS_SETTERS

        #endregion
    }
}