using ScriptableFramework.Events;
using UnityEditor;
using UnityEngine;

namespace ScriptableFramework.UI.Editor
{
    [CustomEditor(typeof(GameEventBool))]
    [CanEditMultipleObjects]
    public class GameEventBoolEditor : GameEventEditorBase
    {
        private bool _source;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            var e = target as GameEventBool;

            _source = EditorGUILayout.Toggle("True/False", _source);

            GUI.enabled = Application.isPlaying;

            if (GUILayout.Button("Raise"))
            {
                e.Raise(_source);
            }
            DrawListeners();
        }
    }
} 