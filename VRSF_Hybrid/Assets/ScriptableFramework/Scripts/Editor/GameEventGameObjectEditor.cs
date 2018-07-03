using ScriptableFramework.Events;
using UnityEditor;
using UnityEngine;

namespace ScriptableFramework.UI.Editor
{
    [CustomEditor(typeof(GameEventGameObject))]
    [CanEditMultipleObjects]
    public class GameEventGameObjectEditor : GameEventEditorBase
    {
        private GameObject _source;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            var e = target as GameEventGameObject;

            _source = (GameObject) EditorGUILayout.ObjectField("Object to send", _source,typeof(GameObject),true);

            GUI.enabled = Application.isPlaying || _source != null;

            if (GUILayout.Button("Raise"))
            {
                e.Raise(_source);
            }
            DrawListeners();
        }
    }
} 