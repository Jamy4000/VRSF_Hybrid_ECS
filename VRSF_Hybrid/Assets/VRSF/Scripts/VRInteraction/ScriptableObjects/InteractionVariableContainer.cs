using ScriptableFramework.Events;
using ScriptableFramework.Util;
using ScriptableFramework.Variables;
using UnityEngine;

namespace VRSF.Interactions
{
	public class InteractionVariableContainer : ScriptableSingleton<InteractionVariableContainer>
    {
        #region PUBLIC_VARIABLES
        [Multiline(10)]
        public string DeveloperDescription = "";

        [Header("The RayVariable for the Controllers and Gaze.")]
        public RayVariable RightRay;
        public RayVariable LeftRay;
        public RayVariable GazeRay;

        [Header("The RaycastHitVariable for the Controllers and Gaze.")]
        public RaycastHitVariable RightHit;
        public RaycastHitVariable LeftHit;
        public RaycastHitVariable GazeHit;

        [Header("BoolVariable to verify if something is Hit")]
        public BoolVariable HasClickSomethingRight;
        public BoolVariable HasClickSomethingLeft;
        public BoolVariable HasClickSomethingGaze;

        [Header("The GameEventTransforms to raise when an object is hit")]
        public GameEventTransform RightObjectWasClicked;
        public GameEventTransform LeftObjectWasClicked;
        public GameEventTransform GazeObjectWasClicked;

        [Header("The GameEventTransforms to raise when an object is hit")]
        public GameEventTransform RightOverObject;
        public GameEventTransform LeftOverObject;
        public GameEventTransform GazeOverObject;

        [Header("BoolVariable to verify if something is Hit")]
        public BoolVariable IsOverSomethingRight;
        public BoolVariable IsOverSomethingLeft;
        public BoolVariable IsOverSomethingGaze;
        #endregion
    }
}