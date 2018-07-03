using ScriptableFramework.Events;
using UnityEditor;
using UnityEngine;

namespace ScriptableFramework.UI.Editor
{
    [CustomEditor(typeof(GameEventString))]
    public class GameEventStringEditor : GameEventEditorBase
    {
        private string _source;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            _source = EditorGUILayout.TextField("Text to send", "");
            
            var e = target as GameEventString;

            GUI.enabled = Application.isPlaying || _source != null;

            if (GUILayout.Button("Raise"))
            {
                e.Raise(_source);
            }
            DrawListeners();
        }
    }
}