using UnityEditor;
using UnityEngine.UI;
using VRSF.MoveAround.Teleport.Components;

namespace VRSF.MoveAround.Teleport.Editor
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

            _lrtComp.UseLoadingSlider = EditorGUILayout.Toggle("Use Loading Slider", _lrtComp.UseLoadingSlider);

            if (_lrtComp.UseLoadingSlider)
            {
                _lrtComp.FillRect = (Image)EditorGUILayout.ObjectField("Slider Fill Rect", _lrtComp.FillRect, typeof(Image), true);
                _lrtComp.TeleportText = (Text)EditorGUILayout.ObjectField("Teleport Available Text", _lrtComp.TeleportText, typeof(Text), true);
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            _lrtComp._UseBoundaries = EditorGUILayout.ToggleLeft("Use Boundaries for Teleport", _lrtComp._UseBoundaries);

            if (_lrtComp._UseBoundaries)
            {
                _lrtComp._MinUserPosition = EditorGUILayout.Vector3Field("Minimum Position in Scene", _lrtComp._MinUserPosition);
                _lrtComp._MaxUserPosition = EditorGUILayout.Vector3Field("Maximum Position in Scene", _lrtComp._MaxUserPosition);
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("You only need to set the parameters for the LongRangeTeleportComponent and the ButtonActionChoserComponents to be able to use this feature.\n" +
                "The Slider will start loading when StartClicking, and the user will be teleported when releasing the button if the ray is hitting the Teleport layer and the timer is exceeding the TimerBeforeTeleport variable.", MessageType.Info);
        }
    }
}