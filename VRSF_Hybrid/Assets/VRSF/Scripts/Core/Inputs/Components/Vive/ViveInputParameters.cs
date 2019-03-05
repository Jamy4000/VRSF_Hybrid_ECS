﻿using ScriptableFramework.RuntimeSet;
using ScriptableFramework.Variables;
using VRSF.Controllers;

namespace VRSF.Inputs.Components.Vive
{
    /// <summary>
    /// Used to handle the Right and Left controllers Inputs in the Vive
    /// </summary>
    public struct ViveInputParameters
    {
        public EHand Hand;
        public VRInputsBoolean ClickBools;
        public VRInputsBoolean TouchBools;
        // TODO
        //public SteamVR_Controller.Device Controller;
        public Vector2Variable ThumbPosition;
    }
}