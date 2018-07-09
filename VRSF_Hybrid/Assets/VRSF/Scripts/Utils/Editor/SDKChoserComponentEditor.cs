#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VRSF.Utils.Components;

namespace VRSF.Utils.Editor
{
    /// <summary>
    /// Handle the Options in the Inspector for the class that extend ButtonActionChoser 
    /// </summary>
    [CustomEditor(typeof(SDKChoserComponent), true)]
    public class SDKChoserComponentEditor : UnityEditor.Editor
    {
        private SDKChoserComponent _component;

        private void OnEnable()
        {
            // We set the buttonActionChoser reference
            _component = (SDKChoserComponent)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            base.DrawDefaultInspector();

            EditorGUILayout.Space();

            DisplaySDKsToggles();

        }

        private void DisplaySDKsToggles()
        {
            GUILayoutOption[] options = { GUILayout.MaxWidth(100.0f), GUILayout.MinWidth(10.0f) };

            EditorGUILayout.LabelField("Chose which SDK is using this script", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            _component.UseOpenVR = EditorGUILayout.ToggleLeft("OpenVR", _component.UseOpenVR, options);
            _component.UseOVR = EditorGUILayout.ToggleLeft("OVR", _component.UseOVR, options);
            _component.UseSimulator = EditorGUILayout.ToggleLeft("Simulator", _component.UseSimulator, options);

            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif