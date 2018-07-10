using ScriptableFramework.Util.Components;
using System.IO;
using Unity.Entities;
using UnityEngine;

namespace ScriptableFramework.Util.Systems
{
    public class ScriptableObjectsLoadSystem : ComponentSystem
    {
        struct Filter
        {
            public ScriptableObjectSaveAndLoadComponent ScriptableSaverLoaderComp;
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            foreach (var e in GetEntities<Filter>())
            {
                if (e.ScriptableSaverLoaderComp.LoadScriptableFromJSONOnAppStart)
                {
                    Load(e.ScriptableSaverLoaderComp);
                }
            }

            this.Enabled = false;
        }

        protected override void OnUpdate() {}

        /// <summary>
        /// Load the JsonFiles and overwrite the scriptable objects in the public list.
        /// </summary>
        private void Load(ScriptableObjectSaveAndLoadComponent comp)
        {
            JsonFileChecker.CheckJSONFiles(comp);

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
    }
}