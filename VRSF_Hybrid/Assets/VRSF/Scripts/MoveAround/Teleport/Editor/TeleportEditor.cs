using UnityEditor;
using VRSF.Utils.Editor;
using UnityEngine.UI;

namespace VRSF.MoveAround.Teleport.Editor
{
    /// <summary>
    /// Just add a message to the user to let him know where the parameters are set.
    /// </summary>
    [CustomEditor(typeof(TeleportFarAway))]
    public class TeleportFarAwayEditor : ButtonActionChoserEditor
    {
        private TeleportFarAway _script;

        public override void OnEnable()
        {
            base.OnEnable();

            _script = (TeleportFarAway)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("OPTIONAL : Loading Slider", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Those are used to display the loading slider on the controller when charging the teleport feature.", EditorStyles.miniLabel);

            _script.UseLoadingSlider = EditorGUILayout.Toggle("Use Loading Slider", _script.UseLoadingSlider);

            if (_script.UseLoadingSlider)
            {
                _script.FillRect = (Image)EditorGUILayout.ObjectField("Slider Fill Rect", _script.FillRect, typeof(Image), true);
                _script.TeleportText = (Text)EditorGUILayout.ObjectField("Teleport Available Text", _script.TeleportText, typeof(Text), true);
            }

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("The parameters of the Teleport Scripts are set via Scriptable Objects. To modify them, please open Window/VRSF/VR Teleport Parameters.", MessageType.Info);
        }
    }
}