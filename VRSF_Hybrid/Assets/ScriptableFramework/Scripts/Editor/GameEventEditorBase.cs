using ScriptableFramework.Events;
using UnityEditor;
using UnityEngine;

namespace ScriptableFramework.UI.Editor
{
    public class GameEventEditorBase : UnityEditor.Editor
    {
        /// Overwritten in a few classes
        protected Object Source;
        
        protected void DrawListeners()
        {
            var curTarget = (GameEventBase) target;

            EditorGUILayout.LabelField("Listeners:");

            foreach (var listener in curTarget.GetListeners())
            {
                EditorGUILayout.ObjectField("", listener, listener.GetType(), true);
            }
        }
    }
}
