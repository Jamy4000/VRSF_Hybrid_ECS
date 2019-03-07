using UnityEditor;
using UnityEngine.UI;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Display the parameters for the LongRangeTeleport Component
    /// </summary>
    [CustomEditor(typeof(LongRangeTeleportComponent))]
    public class LongRangeTeleportEditor : UnityEditor.Editor
    {
        private LongRangeTeleportComponent _lrtComp;

        private void OnEnable()
        {
            _lrtComp = (LongRangeTeleportComponent)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("OPTIONAL : Loading Slider", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Those are used to display the loading slider on the controller when charging the teleport feature.", EditorStyles.miniLabel);
            
            _lrtComp.FillRect = (Image)EditorGUILayout.ObjectField("Slider Fill Rect", _lrtComp.FillRect, typeof(Image), true);
            _lrtComp.TeleportText = (Text)EditorGUILayout.ObjectField("Teleport Available Text", _lrtComp.TeleportText, typeof(Text), true);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("You only need to set the parameters for the different component on this Entity.\n" +
                "The Slider will start loading when StartClicking, and the user will be teleported when releasing the button if the ray is hitting the Teleport NavMesh.", MessageType.Info);
        }
    }
}