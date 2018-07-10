using ScriptableFramework.Util.Components;
using System.IO;
using Unity.Entities;
using UnityEngine;

namespace ScriptableFramework.Util.Systems
{
    public class ScriptableObjectSaveSystem : ComponentSystem
    {
        struct Filter
        {
            public ScriptableObjectSaveAndLoadComponent ScriptableSaverLoaderComp;
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            this.Enabled = false;
        }

        protected override void OnUpdate() { }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();

            foreach (var e in GetEntities<Filter>())
            {
                if (e.ScriptableSaverLoaderComp.SaveScriptableInJSONOnAppQuit)
                {
                    Save(e.ScriptableSaverLoaderComp);
                }
            }
        }

        /// <summary>
        /// Save the JsonFiles with the current status of the Scriptable Object in the public list.
        /// </summary>
        public void Save(ScriptableObjectSaveAndLoadComponent comp)
        {
            JsonFileChecker.CheckJSONFiles(comp);

            foreach (ScriptableObject so in comp.ScriptableToSave.Items)
            {
                if (so != null)
                {
                    SaveAssetStatus(so, comp);
                }
            }

            Debug.Log("Scriptable Files saved correctly.");
        }

        /// <summary>
        /// Save the Scriptable Object status in its JSON File.
        /// </summary>
        /// <param name="toSave">The Scriptable to save</param>
        void SaveAssetStatus(ScriptableObject toSave, ScriptableObjectSaveAndLoadComponent comp)
        {
            string fileName = toSave.name + ".json";
            string path = Path.Combine(comp.JsonPath, fileName);
            File.WriteAllText(path, JsonUtility.ToJson(toSave));
        }
    }
}