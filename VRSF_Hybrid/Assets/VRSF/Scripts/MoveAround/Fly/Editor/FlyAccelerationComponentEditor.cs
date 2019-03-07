using UnityEditor;
using UnityEngine;

namespace VRSF.MoveAround.Fly
{
    /// <summary>
    /// Just add a message to the user to let him know where the parameters are set.
    /// </summary>
    [CustomEditor(typeof(FlyAccelerationComponent))]
    public class FlyAccelerationComponentEditor : UnityEditor.Editor
    {
        #region PRIVATE_VARIABLES
        FlyAccelerationComponent _flyComp;
        #endregion


        #region MONOBEHAVIOUR_METHOD
        /// <summary>
        /// Used to get the Parameters Variable Instance
        /// </summary>
        private void OnEnable()
        {
            _flyComp = (FlyAccelerationComponent)target;
        }
        #endregion MONOBEHAVIOUR_METHOD


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            ShowFlyingParameters();
        }



        /// <summary>
        /// Display the Flying Parameters
        /// </summary>
        private void ShowFlyingParameters()
        {
            Undo.RecordObject(_flyComp, "Option Clicked");
            Undo.RecordObject(this, "Option Clicked");
            
            // Begin the Vertical for the Sliding Effect Parameters
            GUILayout.BeginVertical();
            
            _flyComp.AccelerationDecelerationEffect = EditorGUILayout.ToggleLeft("Use Acceleration and Deceleration", _flyComp.AccelerationDecelerationEffect);
            if (_flyComp.AccelerationDecelerationEffect)
            {
                EditorGUILayout.LabelField("The bigger the values are, the longer the effect will take place.", EditorStyles.miniBoldLabel);
                EditorGUILayout.Space();

                _flyComp.AccelerationEffectFactor = EditorGUILayout.FloatField("Acceleration Factor", _flyComp.AccelerationEffectFactor);
                _flyComp.DecelerationEffectFactor = EditorGUILayout.FloatField("Deceleration Factor", _flyComp.DecelerationEffectFactor);
            }

            Undo.FlushUndoRecordObjects();
        }
    }
}