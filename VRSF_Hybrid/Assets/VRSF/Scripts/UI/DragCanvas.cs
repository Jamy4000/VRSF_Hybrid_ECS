using ScriptableFramework.Variables;
using VRSF.Controllers;
using VRSF.Utils;
using UnityEngine;
using ScriptableFramework.Util;
using VRSF.Inputs;
using VRSF.Interactions;

namespace VRSF.UI
{
    /// <summary>
    /// Allow the user to drag a Canvas. Place this script on the root of the movable canvas.
    /// To use this feature, you'll need to create a 3D UI like the VRKeyboard or the UIExample prefabs.
    /// On this window, you need then to create a GameObject containing some colliders (Those are the place
    /// where the user can drag the window). This is the Border GameObject in the UIExample. 
    /// Finally, the object with the colliders needs a tag called UIBorder.
    /// </summary>
    public class DragCanvas : MonoBehaviour
    {
        #region PUBLIC_VARIABLES
        [Header("Speed factor when moving the Canvas.")]
        public float OffsetSpeed = 3.5f;
        #endregion


        #region PRIVATE_VARIABLES
        private InputVariableContainer _inputContainer;
        private InteractionVariableContainer _interactionContainer;

        private BoolVariable _leftTriggerDown;
        private BoolVariable _rightTriggerDown;

        private bool _draggingRight;
        private bool _draggingLeft;
        private bool _draggingGaze;

        private Vector3 _curDragPos;
        private Vector3 _lastDragPos;
        //private GameObject _draggedThing;
        private float _distance;
        
        private EHand _draggingHand;
        #endregion


        #region MONOBEHAVIOUR_METHODS
        private void Start()
        {
            _inputContainer = InputVariableContainer.Instance;
            _interactionContainer = InteractionVariableContainer.Instance;

            _leftTriggerDown = _inputContainer.LeftClickBoolean.Get("TriggerIsDown");
            _rightTriggerDown = _inputContainer.RightClickBoolean.Get("TriggerIsDown");
        }

        // Update is called once per frame
        void Update()
        {
            CheckDragging();
        }
        #endregion
        

        #region PUBLIC_METHODS
        /// <summary>
        /// Called from the GameEventTransformListener using UIObjectHit
        /// </summary>
        /// <param name="objectHit">The UI object that was hit</param>
        public void CheckObjectHit(Transform objectHit)
        {
            if (objectHit.tag == "UIBorder" && transform.FindDeepChild(objectHit) == objectHit)
            {
                if (_leftTriggerDown.Value)
                    StartDragging(_interactionContainer.LeftHit.Value, EHand.LEFT);
                else if (_rightTriggerDown.Value)
                    StartDragging(_interactionContainer.RightHit.Value, EHand.RIGHT);
                else
                    StartDragging(_interactionContainer.GazeHit.Value, EHand.GAZE);
            }
        }
        #endregion
        

        #region PRIVATE_METHODS
        /// <summary>
        /// Check if the user is already dragging a Canvas
        /// </summary>
        void CheckDragging()
        {
            if (_draggingLeft && !_leftTriggerDown.Value)
                _draggingLeft = false;

            if (_draggingRight && !_rightTriggerDown.Value)
                _draggingRight = false;

            if (_draggingGaze && !_inputContainer.GazeIsCliking.Value)
                _draggingGaze = false;
            
            if (_draggingLeft || _draggingRight || _draggingGaze)
                KeepDragging();
        }

        /// <summary>
        /// Set the references when the user start to drag a Canvas
        /// </summary>
        /// <param name="raycastHit">The raycastHit hitting the UI</param>
        /// <param name="hand">The hand with which the user is dragging the Canvas</param>
        void StartDragging(RaycastHit raycastHit, EHand hand)
        {
            if (!(_draggingLeft || _draggingRight || _draggingGaze))
            {
                _lastDragPos = raycastHit.point;
                _curDragPos = raycastHit.point;
            }

            _draggingHand = hand;

            if (_draggingHand == EHand.LEFT)
                _draggingLeft = true;
            else if (_draggingHand == EHand.RIGHT)
                _draggingRight = true;
            else
                _draggingGaze = true;

            _distance = Vector3.Distance(GetPointerRaycastPos(), raycastHit.point);

            //_draggedThing = raycastHit.collider.gameObject;
        }

        /// <summary>
        /// Move the canvas while the user is pressing the trigger
        /// </summary>
        void KeepDragging()
        {
            RayVariable rayVar;

            if (_draggingLeft)
                rayVar = _interactionContainer.LeftRay;
            else if (_draggingRight)
                rayVar = _interactionContainer.RightRay;
            else
                rayVar = _interactionContainer.GazeRay;

            _lastDragPos = _curDragPos;
            _curDragPos = rayVar.Value.GetPoint(_distance);
            var delta = _curDragPos - _lastDragPos;
            
            // Resolve a speed bug when the UI is at the root of the scene. If it's a child of another GameObject, we don't need to divide by the OffsetSpeed.
            if (transform.parent == null)
                transform.position += (delta / OffsetSpeed);
            else
                transform.position += delta;

            transform.rotation = Quaternion.LookRotation(transform.position - GetPointerRaycastPos());
        }

        /// <summary>
        /// Chose which Raycast pos to use depending on the Hand on which this script is attached to
        /// </summary>
        /// <returns>The Left, Right or Gaze pos from the PointerRayCast Script</returns>
        Vector3 GetPointerRaycastPos()
        {
            if (_draggingHand == EHand.LEFT)
                return VRSF_Components.LeftController.transform.position;
            else if (_draggingHand == EHand.RIGHT)
                return VRSF_Components.RightController.transform.position;
            else
                return VRSF_Components.VRCamera.transform.position;
        }
        #endregion
        

        // EMPTY
        #region GETTERS_SETTERS
        #endregion
    }
}