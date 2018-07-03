#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VRSF.Utils.Editor
{
    /// <summary>
    /// Script to add some Editor feature for the SetupVR GameObject.
    /// </summary>
	public static class SetupVREditor 
	{
        // EMPTY
        #region PUBLIC_VARIABLES

        #endregion


        #region PRIVATE_VARIABLES
        private static GameObject setupVRPrefab;
        #endregion 


        // EMPTY
        #region MONOBEHAVIOUR_METHODS
        #endregion

        // EMPTY
        #region PUBLIC_METHODS

        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Add the SetupVR Prefab to the scene.
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("GameObject/VR Framework/Add SetupVR to Scene", priority = 0)]
        [MenuItem("VR Framework/Add SetupVR to Scene", priority = 0)]
        static void InstantiateSetupVR(MenuCommand menuCommand)
        {
            if (GameObject.FindObjectOfType<SetupVR>() != null)
            {
                Debug.LogError("VRSF : SetupVR is already present in the scene.\n" +
                    "If multiple instance of this object are placed in the same scene, you will encounter conflict problems.");
                return;
            }

            setupVRPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/VRSF/Prefabs/SetupVR.prefab");

            // Create a custom game object
            GameObject setupVR = PrefabUtility.InstantiatePrefab(setupVRPrefab) as GameObject;

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(setupVR, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(setupVR, "Create " + setupVR.name);
            Selection.activeObject = setupVR;
        }
        #endregion

        // EMPTY
        #region GETTERS_SETTERS

        #endregion
    }
}
#endif