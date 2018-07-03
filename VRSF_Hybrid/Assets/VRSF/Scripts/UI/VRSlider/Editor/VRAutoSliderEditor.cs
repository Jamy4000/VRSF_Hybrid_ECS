#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VRSF.UI.Editor
{
    /// <summary>
    /// Add the public variable of VRAutoSlider to the Editor and add a MenuItem to add an autoFillSlider to the scene
    /// </summary>
    [CustomEditor(typeof(VRAutoFillSlider), true)]
    [CanEditMultipleObjects]
    public class VRAutoFillSliderEditor : UnityEditor.UI.SliderEditor
    {
        // EMPTY
        #region PUBLIC_VARIABLES
        #endregion PUBLIC_VARIABLES


        #region PRIVATE_VARIABLES
        private SerializedProperty m_OnBarFilled;
        private SerializedProperty m_OnBarReleased;

        private static GameObject vrSliderPrefab;

        private VRAutoFillSlider autoSlider;
        #endregion PRIVATE_VARIABLES


        #region MONOBEHAVIOUR_METHODS
        protected override void OnEnable()
        {
            base.OnEnable();

            autoSlider = (VRAutoFillSlider)target;

            m_OnBarFilled = serializedObject.FindProperty("OnBarFilled");
            m_OnBarReleased = serializedObject.FindProperty("OnBarReleased");
        }
        #endregion


        #region PUBLIC_METHODS
        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("VRSF Parameters", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            Undo.RecordObject(autoSlider.gameObject, "Add BoxCollider");

            if (autoSlider.gameObject.GetComponent<BoxCollider>() != null)
            {
                autoSlider.SetColliderAuto = EditorGUILayout.ToggleLeft("Set Box Collider Automatically", autoSlider.SetColliderAuto);
            }
            else
            {
                EditorGUILayout.LabelField("This option required a BoxCollider Component.", EditorStyles.miniLabel);
                autoSlider.SetColliderAuto = false;
                autoSlider.SetColliderAuto = EditorGUILayout.ToggleLeft("Set Box Collider Automatically", false);
                
                // Add a button to replace the collider by a BoxCollider2D
                if (GUILayout.Button("Add BoxCollider"))
                {
                    autoSlider.gameObject.AddComponent<BoxCollider>();
                    DestroyImmediate(autoSlider.GetComponent<Collider>());
                    autoSlider.SetColliderAuto = true;
                }
            }
            EditorGUILayout.Space();

            autoSlider.FillWithClick = EditorGUILayout.ToggleLeft("Fill With Click", autoSlider.FillWithClick);
            autoSlider.FillTime = EditorGUILayout.FloatField("Fill Time", autoSlider.FillTime);
            
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Basic Slider Parameters", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            base.OnInspectorGUI();

            EditorGUILayout.Space();

            serializedObject.Update();

            EditorGUILayout.PropertyField(m_OnBarFilled);
            EditorGUILayout.PropertyField(m_OnBarReleased);

            serializedObject.ApplyModifiedProperties();
        }
        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        /// <summary>
        /// Add a new VR Auto Filling Slider to the Scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("VR Framework/UI/Sliders/VR AutoFill Slider", priority = 0)]
        [MenuItem("GameObject/VR Framework/UI/Sliders/VR AutoFill Slider", priority = 0)]
        static void InstantiateVRAutoFillSlider(MenuCommand menuCommand)
        {
            vrSliderPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/VRSF/Prefabs/UI/UIElements/VRAutoFillSlider.prefab");

            // Create a custom game object
            GameObject newSlider = PrefabUtility.InstantiatePrefab(vrSliderPrefab) as GameObject;

            RectTransform rt = newSlider.GetComponent<RectTransform>();
            rt.localPosition = new Vector3(rt.rect.x, rt.rect.y, 0);
            rt.localScale = Vector3.one;

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(newSlider, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(newSlider, "Create " + newSlider.name);
            Selection.activeObject = newSlider;
        }
        #endregion PRIVATE_METHODS
    }
}
#endif