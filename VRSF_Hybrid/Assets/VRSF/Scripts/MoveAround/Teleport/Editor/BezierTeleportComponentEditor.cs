using UnityEditor;
using VRSF.MoveAround.Teleport.Components;

namespace VRSF.MoveAround.Teleport.Editor
{
    /// <summary>
    /// Just add a message to the user to let him know where the parameters are set.
    /// </summary>
    [CustomEditor(typeof(BezierTeleportComponent))]
    public class BezierTeleportComponentEditor : UnityEditor.Editor
    {
        private BezierTeleportComponent bezierComp;

        private void OnEnable()
        {
            bezierComp = (BezierTeleportComponent)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            bezierComp._UseBoundaries = EditorGUILayout.ToggleLeft("Use Boundaries for Teleport", bezierComp._UseBoundaries);

            if (bezierComp._UseBoundaries)
            {
                bezierComp._MinUserPosition = EditorGUILayout.Vector3Field("Minimum Position in Scene", bezierComp._MinUserPosition);
                bezierComp._MaxUserPosition = EditorGUILayout.Vector3Field("Maximum Position in Scene", bezierComp._MaxUserPosition);
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("You only need to set the parameters for the BezierTeleportComponent and the ButtonActionChoserComponents to be able to use this feature.\n" +
                "The Arc will be display when the user Start Clicking, and the user will be teleported when releasing the button (if the ray is hitting the Teleport layer).", MessageType.Info);
        }
    }
}