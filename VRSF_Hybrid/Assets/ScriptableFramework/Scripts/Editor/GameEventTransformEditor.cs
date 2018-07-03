using ScriptableFramework.Events;
using UnityEditor;
using UnityEngine;

namespace ScriptableFramework.UI.Editor
{
    [CustomEditor(typeof(GameEventTransform))]
    public class GameEventTransformEditor : GameEventEditorBase
    {
        private Transform _source;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            _source = (Transform) EditorGUILayout.ObjectField("Transform: ", _source, typeof(Transform), true);

            var e = target as GameEventTransform;
            GUI.enabled = Application.isPlaying || _source != null;
            if (GUILayout.Button("Raise"))
            {
                e.Raise(_source);
            }
            DrawListeners();
        }
    }
}