using System.Text;
using ScriptableFramework.Events;
using UnityEditor;

namespace ScriptableFramework.UI.Editor
{
    [CustomEditor(typeof(GameEventListenerGameObject))]
    public class GameEventListenerGameObjectEditor : UnityEditor.Editor
    {
       
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (target == null)
            {
                return;
            }
            var curTarget = (GameEventListenerGameObject)target;
            var sb = new StringBuilder();

            sb.AppendLine("Listeners:");
            if (curTarget.Event != null)
            {
                foreach (var listener in curTarget.Event.GetListeners())
                {
                    sb.AppendLine(listener.name);
                }
            }
            sb.AppendLine("Responses:");
            if (curTarget.Response != null)
            {
                for (int i = 0; i < curTarget.Response.GetPersistentEventCount(); i++)
                {
                    sb.AppendLine(string.Format("Target: {0}.{1}", curTarget.Response.GetPersistentTarget(i), curTarget.Response.GetPersistentMethodName(i)));
                }
            }
            EditorGUILayout.LabelField(sb.ToString());
        }
    }
}
