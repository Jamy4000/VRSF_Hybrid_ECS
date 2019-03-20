using UnityEditor;

namespace VRSF.Core.SetupVR
{
    public static class InputManagerCopier
    {
        private static SerializedProperty _currentAxisArray;

        public static bool SetInputManager(SerializedObject currentAxisObject, SerializedProperty vrsfAxisArray)
        {
            try
            {
                SerializedProperty currentAxisArray = currentAxisObject.FindProperty("m_Axes");
                currentAxisArray.ClearArray();
                currentAxisObject.ApplyModifiedProperties();

                _currentAxisArray = currentAxisArray;

                for (int i = 0; i < vrsfAxisArray.arraySize; i++)
                {
                    SerializedProperty vrsfAxisProperty = vrsfAxisArray.GetArrayElementAtIndex(i);

                    AddAxis(currentAxisObject, new InputAxis()
                    {
                        Name = GetChildProperty(vrsfAxisProperty, "m_Name").stringValue,
                        DescriptiveName = GetChildProperty(vrsfAxisProperty, "descriptiveName").stringValue,
                        DescriptiveNegativeName = GetChildProperty(vrsfAxisProperty, "descriptiveNegativeName").stringValue,
                        NegativeButton = GetChildProperty(vrsfAxisProperty, "negativeButton").stringValue,
                        PositiveButton = GetChildProperty(vrsfAxisProperty, "positiveButton").stringValue,
                        AltNegativeButton = GetChildProperty(vrsfAxisProperty, "altNegativeButton").stringValue,
                        AltPositiveButton = GetChildProperty(vrsfAxisProperty, "altPositiveButton").stringValue,

                        Gravity = GetChildProperty(vrsfAxisProperty, "gravity").floatValue,
                        Dead = GetChildProperty(vrsfAxisProperty, "dead").floatValue,
                        Sensitivity = GetChildProperty(vrsfAxisProperty, "sensitivity").floatValue,

                        Snap = GetChildProperty(vrsfAxisProperty, "snap").boolValue,
                        Invert = GetChildProperty(vrsfAxisProperty, "invert").boolValue,

                        Type = (AxisType)GetChildProperty(vrsfAxisProperty, "type").intValue,

                        Axis = GetChildProperty(vrsfAxisProperty, "axis").intValue,
                        JoyNum = GetChildProperty(vrsfAxisProperty, "joyNum").intValue,
                    });
                }
                return true;
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError("<b>[VRSF] :</b> An error as occured while copying the InputManager. Please check that there's an InputManager in the Resources/VRSF folder.\n" +
                    "If the error persist, please reaise an issue on Github with the following error :\n" + e.ToString());
                return false;
            }
        }

        private static void AddAxis(SerializedObject serializedObject, InputAxis axis)
        {
            _currentAxisArray.arraySize++;
            serializedObject.ApplyModifiedProperties();

            SerializedProperty axisProperty = _currentAxisArray.GetArrayElementAtIndex(_currentAxisArray.arraySize - 1);

            GetChildProperty(axisProperty, "m_Name").stringValue = axis.Name;
            GetChildProperty(axisProperty, "descriptiveName").stringValue = axis.DescriptiveName;
            GetChildProperty(axisProperty, "descriptiveNegativeName").stringValue = axis.DescriptiveNegativeName;
            GetChildProperty(axisProperty, "negativeButton").stringValue = axis.NegativeButton;
            GetChildProperty(axisProperty, "positiveButton").stringValue = axis.PositiveButton;
            GetChildProperty(axisProperty, "altNegativeButton").stringValue = axis.AltNegativeButton;
            GetChildProperty(axisProperty, "altPositiveButton").stringValue = axis.AltPositiveButton;
            GetChildProperty(axisProperty, "gravity").floatValue = axis.Gravity;
            GetChildProperty(axisProperty, "dead").floatValue = axis.Dead;
            GetChildProperty(axisProperty, "sensitivity").floatValue = axis.Sensitivity;
            GetChildProperty(axisProperty, "snap").boolValue = axis.Snap;
            GetChildProperty(axisProperty, "invert").boolValue = axis.Invert;
            GetChildProperty(axisProperty, "type").intValue = (int)axis.Type;
            GetChildProperty(axisProperty, "axis").intValue = axis.Axis;
            GetChildProperty(axisProperty, "joyNum").intValue = axis.JoyNum;

            serializedObject.ApplyModifiedProperties();
        }

        private static SerializedProperty GetChildProperty(SerializedProperty parent, string name)
        {
            SerializedProperty child = parent.Copy();
            child.Next(true);
            do
            {
                if (child.name == name) return child;
            }
            while (child.Next(false));
            return null;
        }

        public static bool InputArrayIsNotVRSF(SerializedProperty currentAxisArray, SerializedProperty vrsfAxisArray)
        {
            if (currentAxisArray.arraySize != vrsfAxisArray.arraySize)
            {
                return true;
            }
            else
            {
                for (int i = 0; i < currentAxisArray.arraySize; i++)
                {
                    var currentAxis = currentAxisArray.GetArrayElementAtIndex(i);
                    var currentName = currentAxis.FindPropertyRelative("m_Name").stringValue;
                    var currentAxisVal = currentAxis.FindPropertyRelative("axis").intValue;
                    var currentInputType = (InputType)currentAxis.FindPropertyRelative("type").intValue;

                    var vrsfAxis = vrsfAxisArray.GetArrayElementAtIndex(i);
                    var vrsfName = vrsfAxis.FindPropertyRelative("m_Name").stringValue;
                    var vrsfAxisVal = vrsfAxis.FindPropertyRelative("axis").intValue;
                    var vrsfInputType = (InputType)vrsfAxis.FindPropertyRelative("type").intValue;

                    if (vrsfName != currentName || vrsfAxisVal != currentAxisVal || vrsfInputType != currentInputType)
                        return true;
                }
            }
            return false;
        }
    }
}