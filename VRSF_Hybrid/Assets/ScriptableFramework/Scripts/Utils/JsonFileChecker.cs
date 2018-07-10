using ScriptableFramework.Util.Components;
using System.IO;
using UnityEngine;

namespace ScriptableFramework.Util
{
    /// <summary>
    /// Used in the ScriptableObjectSaveSystem and in the ScriptableObjectsLoadSystem to check that the JSON files are set correctly.
    /// </summary>
    public static class JsonFileChecker
    {
        /// <summary>
        /// Check if the JSON Files exists foreach ScriptableObject that are in the public list. If not, create a new JSON File.
        /// Check as well if we need to remove Unused JSON Files.
        /// </summary>
        public static void CheckJSONFiles(ScriptableObjectSaveAndLoadComponent comp)
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
        private static void FeedRuntimeSets(ScriptableObjectSaveAndLoadComponent comp)
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
        private static void CheckJSONFilesToRemove(ScriptableObjectSaveAndLoadComponent comp)
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