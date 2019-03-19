using ScriptableFramework.Util.Components;
using System.IO;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ScriptableFramework.Util.Systems
{
    public class ScriptableObjectsLoadSystem : ComponentSystem
    {
        struct Filter
        {
            public ScriptableObjectLoadComponent ScriptableLoaderComp;
        }

        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            foreach (var e in GetEntities<Filter>())
            {
                if (e.ScriptableLoaderComp.LoadScriptableFromJSONOnAppStart)
                {
                    Load(e.ScriptableLoaderComp);
                }
            }

            this.Enabled = false;
        }

        protected override void OnUpdate() {}

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();

            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        /// <summary>
        /// Load the JsonFiles and overwrite the scriptable objects in the public list.
        /// </summary>
        private void Load(ScriptableObjectLoadComponent comp)
        {
            for (int i = 0; i < comp.ScriptableToLoad.Items.Count; i++)
            {
                if (comp.ScriptableToLoad.Items[i] != null)
                {
                    string name = comp.ScriptableToLoad.Items[i].name + ".json";

                    try
                    {
                        string path = Path.Combine(comp.JsonPath, name);
                        string json = File.ReadAllText(path);
                        JsonUtility.FromJsonOverwrite(json, comp.ScriptableToLoad.Items[i]);
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
        /// Reactivate the System when switching to another Scene.
        /// </summary>
        /// <param name="oldScene">The previous scene before switching</param>
        private void OnSceneUnloaded(Scene oldScene)
        {
            this.Enabled = true;
        }
    }
}