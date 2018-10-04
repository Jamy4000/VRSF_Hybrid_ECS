﻿using ScriptableFramework.Variables;
using System.Collections.Generic;
using UnityEngine;
using VRSF.Utils.Systems.ButtonActionChoser;

namespace VRSF.Utils.Components.ButtonActionChoser
{
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity), typeof(BACGeneralVariablesComponents))]
    public class BACCalculationsVariablesComponents : MonoBehaviour
    {
        [HideInInspector] public bool ActionButtonIsReady = false;
        [HideInInspector] public bool ParametersAreInvalid = false;
        [HideInInspector] public bool CorrectSDK = true;
        [HideInInspector] public bool IsSetup = false;
        [HideInInspector] public bool CanBeUsed = true;

        // To keep track of the event that were raised, used for the features that use the Thumbstick
        [HideInInspector] public bool ClickActionBeyondThreshold;
        [HideInInspector] public bool TouchActionBeyondThreshold;
        [HideInInspector] public bool UntouchedEventWasRaised;
        [HideInInspector] public bool UnclickEventWasRaised;

        // For SDKs Specific ActionButton 
        [HideInInspector] public bool IsUsingOculusButton;
        [HideInInspector] public bool IsUsingPortableOVRButton;
        [HideInInspector] public bool IsUsingViveButton;
        [HideInInspector] public bool IsUsingWheelButton;

        // Thumb Parameters
        [HideInInspector] public Vector2Variable ThumbPos = null;

        // BoolVariable to check
        [HideInInspector] public BoolVariable IsTouching = null;
        [HideInInspector] public BoolVariable IsClicking = null;
    }
}