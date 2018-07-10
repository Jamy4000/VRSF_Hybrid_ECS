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
            public ScriptableObjectSaveComponent ScriptableSaverComp;
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            // To let the EntityManager know where are the Entities
            foreach (var e in GetEntities<Filter>()) { }
            this.Enabled = false;
        }

        protected override void OnUpdate() { }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();

            foreach (var e in GetEntities<Filter>())
            {
                if (e.ScriptableSaverComp.SaveScriptableInJSONOnAppQuit)
                {
                    Save(e.ScriptableSaverComp);
                }
            }
        }


        /// <summary>
        /// Save the JsonFiles with the current status of the Scriptable Object in the public list.
        /// </summary>
        public void Save(ScriptableObjectSaveComponent comp)
        {
            CheckJSONFiles(comp);

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
        void SaveAssetStatus(ScriptableObject toSave, ScriptableObjectSaveComponent comp)
        {
            string fileName = toSave.name + ".json";
            string path = Path.Combine(comp.JsonPath, fileName);
            File.WriteAllText(path, JsonUtility.ToJson(toSave));
        }


        /// <summary>
        /// Check if the JSON Files exists foreach ScriptableObject that are in the public list. If not, create a new JSON File.
        /// Check as well if we need to remove Unused JSON Files.
        /// </summary>
        public static void CheckJSONFiles(ScriptableObjectSaveComponent comp)
        {
            // Empty the ScriptableToSave RuntimeSet
            comp.ScriptableToSave.Items.Clear();

            // Feed it with the new values
            FeedRuntimeSets(comp);

            // Created the Directory at the JsonPath if it doesn't exist
            if (!Directory.Exists(comp.JsonPath))
            {
                Directory.CreateDirectory(comp.JsonPath);
            }

            // Check if we need to remove unused json files
            if (comp.RemoveUnusedJSONFiles)
            {
                CheckJSONFilesToRemove(comp);
            }

            // Create all of the json files if they doen't exists.
            for (int i = 0; i < comp.ScriptableToSave.Items.Count; i++)
            {
                if (comp.ScriptableToSave.Items[i] != null)
                {
                    string name = comp.ScriptableToSave.Items[i].name + ".json";
                    string path = Path.Combine(comp.JsonPath, name);

                    if (!File.Exists(path))
                    {
                        File.Create(path);
                    }
                }
            }
        }

        /// <summary>
        /// Feed the runtime set ScriptableToSave with the Scriptable objects in ScriptablesToSaveAndLoad
        /// This avoid an error when trying to acces the object of the public list on App Quit.
        /// </summary>
        private static void FeedRuntimeSets(ScriptableObjectSaveComponent comp)
        {
            if (comp.ScriptablesToSave != null)
            {
                foreach (ScriptableObject so in comp.ScriptablesToSave)
                {
                    if (!comp.ScriptableToSave.Items.Contains(so))
                        comp.ScriptableToSave.Add(so);
                }
            }
        }

        /// <summary>
        /// If the RemoveUnusedJSONFiles bool is at true, check all Json Files and compare them to the ScriptableToSave Items.
        /// If one files doesn't have a correpsonding ScriptableObject in the RuntimeSet, we delete it.
        /// </summary>
        private static void CheckJSONFilesToRemove(ScriptableObjectSaveComponent comp)
        {
            string[] jSONfiles = Directory.GetFiles(comp.JsonPath, "*.json");

            if (comp.ScriptableToSave != null && jSONfiles.Length != comp.ScriptableToSave.Items.Count)
            {
                foreach (string json in jSONfiles)
                {
                    bool jsonNotUsed = true;

                    foreach (ScriptableObject so in comp.ScriptableToSave.Items)
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
    }
}