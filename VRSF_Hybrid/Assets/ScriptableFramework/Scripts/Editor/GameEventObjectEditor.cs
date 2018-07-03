using ScriptableFramework.Events;
using UnityEditor;
using UnityEngine;

namespace ScriptableFramework.UI.Editor
{
    [CustomEditor(typeof(GameEventObject))]
    [CanEditMultipleObjects]
    public class GameEventObjectEditor : GameEventEditorBase
    {
        private  Object _source;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            var e = target as GameEventObject;
            _source = EditorGUILayout.ObjectField("Object to send", _source, typeof(Object), true);
            GUI.enabled = Application.isPlaying || _source != null;
            if (GUILayout.Button("Raise"))
            {
                e.Raise(_source);
            }
            DrawListeners();
        }
    }
} 